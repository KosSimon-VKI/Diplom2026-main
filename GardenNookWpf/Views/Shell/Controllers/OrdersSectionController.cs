using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using GardenNookWpf.Views.Controls;
using TransferModels.Kitchen;

namespace GardenNookWpf.Views.Shell.Controllers
{
    public sealed class OrdersSectionController : IMainSectionController
    {
        private const string KitchenApiBaseAddress = "https://localhost:7235/api/kitchen";
        private const string KitchenOrdersAddress = KitchenApiBaseAddress + "/orders";
        private const int OverdueOrderMinutes = 15;
        private const string AdminRole = "Администратор";
        private const string BaristaRole = "Бариста";
        private const string ToppingCategoryDishTokenRu = "к блюд";
        private const string ToppingCategoryDrinkTokenRu = "к напит";
        private const string ToppingCategoryInactiveTokenRu = "неактив";

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly HttpClient _httpClient;
        private readonly OrderItemsVisibilityMode _itemsVisibilityMode;
        private readonly DispatcherTimer _ordersRefreshTimer;
        private readonly DispatcherTimer _orderElapsedTimer;
        private bool _isLoadingOrders;
        private List<KitchenOrderCardViewModel> _orderCards = new List<KitchenOrderCardViewModel>();
        private IReadOnlyList<KitchenOrderCardViewModel> _pickupCards = Array.Empty<KitchenOrderCardViewModel>();
        private IReadOnlyList<KitchenOrderCardViewModel> _noPickupCards = Array.Empty<KitchenOrderCardViewModel>();

        public OrdersSectionController(HttpClient httpClient, string userRole)
        {
            _httpClient = httpClient;
            _itemsVisibilityMode = ResolveItemsVisibilityMode(userRole);

            _ordersRefreshTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(7)
            };
            _ordersRefreshTimer.Tick += OrdersRefreshTimer_Tick;

            _orderElapsedTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _orderElapsedTimer.Tick += OrderElapsedTimer_Tick;
        }

        public event Action<bool>? BusyStateChanged;
        public event Action<OrdersDisplayState>? StateChanged;

        public bool IsBusy => _isLoadingOrders;

        public async Task ActivateAsync()
        {
            await LoadOrdersAsync();
            _ordersRefreshTimer.Start();
            _orderElapsedTimer.Start();
        }

        public void Deactivate()
        {
            _ordersRefreshTimer.Stop();
            _orderElapsedTimer.Stop();
        }

        public async Task ReloadAsync()
        {
            await LoadOrdersAsync();
        }

        public KitchenOrderCardViewModel GetCardForDetails(KitchenOrderCardViewModel card)
        {
            var sourceCard = _orderCards.FirstOrDefault(c => c.OrderId == card.OrderId) ?? card;
            return CloneCardForDetails(sourceCard);
        }

        private async void OrdersRefreshTimer_Tick(object? sender, EventArgs e)
        {
            await LoadOrdersAsync();
        }

        private void OrderElapsedTimer_Tick(object? sender, EventArgs e)
        {
            UpdateOrderHeaders(DateTime.Now);
            PublishOrdersState();
        }

        private async Task LoadOrdersAsync()
        {
            if (_isLoadingOrders)
            {
                return;
            }

            try
            {
                SetBusy(true);
                var response = await _httpClient.GetAsync(KitchenOrdersAddress);

                if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                {
                    PublishMessageState("Нет доступа к заказам");
                    return;
                }

                if (!response.IsSuccessStatusCode)
                {
                    PublishMessageState("Не удалось загрузить заказы");
                    return;
                }

                var responseJson = await response.Content.ReadAsStringAsync();

                var data = JsonSerializer.Deserialize<KitchenOrdersResponse>(responseJson, JsonOptions)
                    ?? new KitchenOrdersResponse();

                var orders = data.Orders ?? new List<KitchenOrderDto>();
                var pickupOrders = orders
                    .Where(o => o.PickupAt.HasValue)
                    .ToList();
                var noPickupOrders = orders
                    .Where(o => !o.PickupAt.HasValue)
                    .ToList();

                var pickupCards = BuildOrderCards(pickupOrders, _itemsVisibilityMode);
                var noPickupCards = BuildOrderCards(noPickupOrders, _itemsVisibilityMode);

                _pickupCards = pickupCards;
                _noPickupCards = noPickupCards;
                _orderCards = pickupCards
                    .Concat(noPickupCards)
                    .ToList();
                UpdateOrderHeaders(DateTime.Now);
                PublishOrdersState();
            }
            catch (Exception ex)
            {
                PublishMessageState($"Ошибка загрузки заказов: {ex.Message}");
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void PublishOrdersState()
        {
            StateChanged?.Invoke(new OrdersDisplayState
            {
                IsMessageOnly = false,
                MessageText = "Нет активных заказов",
                PickupCards = _pickupCards,
                NoPickupCards = _noPickupCards
            });
        }

        private void PublishMessageState(string message)
        {
            _pickupCards = Array.Empty<KitchenOrderCardViewModel>();
            _noPickupCards = Array.Empty<KitchenOrderCardViewModel>();
            _orderCards = new List<KitchenOrderCardViewModel>();

            StateChanged?.Invoke(new OrdersDisplayState
            {
                IsMessageOnly = true,
                MessageText = message,
                PickupCards = _pickupCards,
                NoPickupCards = _noPickupCards
            });
        }

        private void UpdateOrderHeaders(DateTime now)
        {
            foreach (var card in _orderCards)
            {
                card.ElapsedText = BuildElapsedText(card.CreatedAt, now);
                card.IsOverdue = IsOrderOverdue(card.CreatedAt, now);
            }
        }

        private static string BuildElapsedText(DateTime? createdAt, DateTime now)
        {
            if (createdAt == null)
            {
                return "00:00";
            }

            var elapsed = now - createdAt.Value;
            if (elapsed < TimeSpan.Zero)
            {
                elapsed = TimeSpan.Zero;
            }

            var totalMinutes = (int)elapsed.TotalMinutes;
            return $"{totalMinutes:00}:{elapsed.Seconds:00}";
        }

        private static bool IsOrderOverdue(DateTime? createdAt, DateTime now)
        {
            if (createdAt == null)
            {
                return false;
            }

            return now - createdAt.Value >= TimeSpan.FromMinutes(OverdueOrderMinutes);
        }

        private static List<KitchenOrderCardViewModel> BuildOrderCards(IEnumerable<KitchenOrderDto> orders, OrderItemsVisibilityMode mode)
        {
            var cards = new List<KitchenOrderCardViewModel>();

            foreach (var order in orders)
            {
                var displayItems = new List<KitchenOrderItemViewModel>();
                var positionNumber = 1;

                if (mode is OrderItemsVisibilityMode.All or OrderItemsVisibilityMode.Kitchen)
                {
                    foreach (var dish in order.Dishes ?? new List<KitchenOrderDishDto>())
                    {
                        var toppingsLine = BuildDishToppingsLine(dish.Toppings);

                        displayItems.Add(new KitchenOrderItemViewModel
                        {
                            ItemId = dish.ItemId,
                            ItemType = KitchenItemTypes.Dish,
                            ItemTypeText = "Блюдо",
                            NameLine = $"{positionNumber}) {dish.Name} x{FormatQuantity(dish.Quantity)}",
                            ToppingsLine = toppingsLine,
                            ToppingsVisibility = string.IsNullOrWhiteSpace(toppingsLine) ? Visibility.Collapsed : Visibility.Visible
                        });

                        positionNumber++;
                    }
                }

                if (mode is OrderItemsVisibilityMode.All or OrderItemsVisibilityMode.Bar)
                {
                    foreach (var drink in order.Drinks ?? new List<KitchenOrderDrinkDto>())
                    {
                        var toppingsLine = BuildDrinkToppingsLine(drink.Toppings);

                        displayItems.Add(new KitchenOrderItemViewModel
                        {
                            ItemId = drink.ItemId,
                            ItemType = KitchenItemTypes.Drink,
                            ItemTypeText = "Напиток",
                            NameLine = $"{positionNumber}) {drink.Name} x{FormatQuantity(drink.Quantity)}",
                            ToppingsLine = toppingsLine,
                            ToppingsVisibility = string.IsNullOrWhiteSpace(toppingsLine) ? Visibility.Collapsed : Visibility.Visible
                        });

                        positionNumber++;
                    }
                }

                foreach (var topping in (order.Toppings ?? new List<KitchenOrderStandaloneToppingDto>())
                    .Where(topping => ShouldShowStandaloneTopping(topping, mode)))
                {
                    displayItems.Add(new KitchenOrderItemViewModel
                    {
                        ItemId = topping.ItemId,
                        ItemType = KitchenItemTypes.Topping,
                        ItemTypeText = "Добавка",
                        NameLine = $"{positionNumber}) {topping.Name} x{FormatQuantity(topping.Quantity)}",
                        ToppingsLine = string.Empty,
                        ToppingsVisibility = Visibility.Collapsed
                    });

                    positionNumber++;
                }

                if (displayItems.Count == 0)
                {
                    continue;
                }

                var pickupAtText = BuildPickupAtText(order.PickupAt);

                cards.Add(new KitchenOrderCardViewModel
                {
                    OrderId = order.OrderId,
                    CreatedAt = order.CreatedAt,
                    OrderNumberText = order.OrderId.ToString(CultureInfo.CurrentCulture),
                    ElapsedText = BuildElapsedText(order.CreatedAt, DateTime.Now),
                    IsOverdue = IsOrderOverdue(order.CreatedAt, DateTime.Now),
                    OrderTypeText = BuildOrderTypeText(order.OrderType),
                    PickupAtText = pickupAtText,
                    PickupAtVisibility = string.IsNullOrWhiteSpace(pickupAtText) ? Visibility.Collapsed : Visibility.Visible,
                    OrderCommentText = order.Comment ?? string.Empty,
                    OrderCommentVisibility = string.IsNullOrWhiteSpace(order.Comment) ? Visibility.Collapsed : Visibility.Visible,
                    DisplayItems = displayItems
                });
            }

            return cards;
        }

        private static KitchenOrderCardViewModel CloneCardForDetails(KitchenOrderCardViewModel source)
        {
            var clonedItems = source.DisplayItems
                .Select(i => new KitchenOrderItemViewModel
                {
                    ItemId = i.ItemId,
                    ItemType = i.ItemType,
                    ItemTypeText = i.ItemTypeText,
                    NameLine = i.NameLine,
                    ToppingsLine = i.ToppingsLine,
                    ToppingsVisibility = i.ToppingsVisibility
                })
                .ToList();

            return new KitchenOrderCardViewModel
            {
                OrderId = source.OrderId,
                CreatedAt = source.CreatedAt,
                OrderNumberText = source.OrderNumberText,
                OrderTypeText = source.OrderTypeText,
                PickupAtText = source.PickupAtText,
                PickupAtVisibility = source.PickupAtVisibility,
                OrderCommentText = source.OrderCommentText,
                OrderCommentVisibility = source.OrderCommentVisibility,
                ElapsedText = source.ElapsedText,
                IsOverdue = source.IsOverdue,
                DisplayItems = clonedItems
            };
        }

        private static string BuildDishToppingsLine(IEnumerable<KitchenOrderDishToppingDto>? toppings)
        {
            if (toppings == null)
            {
                return string.Empty;
            }

            var toppingList = toppings.ToList();
            if (toppingList.Count == 0)
            {
                return string.Empty;
            }

            return string.Join(Environment.NewLine, toppingList.Select(t => $"+ {t.Name} x{FormatQuantity(t.Quantity)}"));
        }

        private static string BuildDrinkToppingsLine(IEnumerable<KitchenOrderDrinkToppingDto>? toppings)
        {
            if (toppings == null)
            {
                return string.Empty;
            }

            var toppingList = toppings.ToList();
            if (toppingList.Count == 0)
            {
                return string.Empty;
            }

            return string.Join(Environment.NewLine, toppingList.Select(t => $"+ {t.Name} x{FormatQuantity(t.Quantity)}"));
        }

        private static string BuildOrderTypeText(string? orderType)
        {
            if (string.IsNullOrWhiteSpace(orderType))
            {
                return "Не указан тип";
            }

            return orderType.Trim();
        }

        private static string BuildPickupAtText(DateTime? pickupAt)
        {
            if (pickupAt == null)
            {
                return string.Empty;
            }

            return $"Самовывоз: {pickupAt.Value.ToString("dd.MM.yyyy HH:mm", CultureInfo.CurrentCulture)}";
        }

        private static string FormatQuantity(decimal quantity)
        {
            if (quantity == decimal.Truncate(quantity))
            {
                return quantity.ToString("0", CultureInfo.CurrentCulture);
            }

            return quantity.ToString("0.##", CultureInfo.CurrentCulture);
        }

        private void SetBusy(bool isBusy)
        {
            _isLoadingOrders = isBusy;
            BusyStateChanged?.Invoke(isBusy);
        }

        private static OrderItemsVisibilityMode ResolveItemsVisibilityMode(string userRole)
        {
            var role = userRole?.Trim() ?? string.Empty;
            return role switch
            {
                AdminRole => OrderItemsVisibilityMode.All,
                BaristaRole => OrderItemsVisibilityMode.Bar,
                _ => OrderItemsVisibilityMode.Kitchen
            };
        }

        private static bool ShouldShowStandaloneTopping(KitchenOrderStandaloneToppingDto topping, OrderItemsVisibilityMode mode)
        {
            var category = NormalizeCategory(topping.CategoryName);
            if (category.Contains(ToppingCategoryInactiveTokenRu))
            {
                return false;
            }

            return mode switch
            {
                OrderItemsVisibilityMode.All => true,
                OrderItemsVisibilityMode.Kitchen => category.Contains(ToppingCategoryDishTokenRu),
                OrderItemsVisibilityMode.Bar => category.Contains(ToppingCategoryDrinkTokenRu),
                _ => false
            };
        }

        private static string NormalizeCategory(string? categoryName)
        {
            return (categoryName ?? string.Empty).Trim().ToLowerInvariant();
        }

        private enum OrderItemsVisibilityMode
        {
            Kitchen,
            Bar,
            All
        }

        public sealed class OrdersDisplayState
        {
            public bool IsMessageOnly { get; set; }
            public string MessageText { get; set; } = string.Empty;
            public IReadOnlyList<KitchenOrderCardViewModel> PickupCards { get; set; } = Array.Empty<KitchenOrderCardViewModel>();
            public IReadOnlyList<KitchenOrderCardViewModel> NoPickupCards { get; set; } = Array.Empty<KitchenOrderCardViewModel>();
        }
    }
}
