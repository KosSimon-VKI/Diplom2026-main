using System;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GardenNookApi.Entities;
using GardenNookApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TransferModels.Kitchen;
using TransferModels.Orders;

namespace GardenNookApi.Controllers
{
    [ApiController]
    [Route("api/orders")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IPreparationStockService _stockService;
        private readonly IPickupSchedulingService _pickupSchedulingService;

        // --- Правила категорий по кол-ву заказов ---
        // Под себя легко поменяешь.
        private const int RegularFromOrdersCount = 5;  // >= 5 => "Постоянный"

        // Id категорий из твоего seed:
        private const int ClientCategoryNewId = 1;       // "Новый"
        private const int ClientCategoryRegularId = 2;   // "Постоянный"
        private const int ClientCategorySpecialId = 3;   // "Особый"
        private const int ClientCategoryNoneId = 4;      // "Без категории"

        // Id скидок из твоего seed:
        private const int DiscountNewId = 1;       // 15%
        private const int DiscountRegularId = 2;   // 7%
        private const int DiscountSpecialId = 3;   // 20%

        private const string OrderStatusCreatedName = "В процессе";
        private const string OrderStatusCreatedToken = "процесс";
        private const string InactiveCategoryName = "Неактивные";
        private const int MilkCategoryId = 10;
        private const int CoffeeCategoryId = 2;
        private const string PreferredMilkModifierName = "КОРОВЬЕ МОЛОКО";
        private const string PreferredCoffeeModifierName = "Кофе в зернах ТАВ Galaxy";
        private const string WpfGuestClientFullName = "Гость WPF";
        private const string WpfGuestClientPhone = "79990000000";
        private const string WpfGuestClientPassword = "wpf-guest";
        private const decimal DecimalEpsilon = 0.000001m;
        private static readonly int[] MilkModifierIngredientIds = [106, 107, 108, 110, 113, 115, 118];
        private static readonly int[] CoffeeModifierIngredientIds = [6, 8];
        private static readonly int[] ModifierExcludedDrinkIds = [5, 6, 43, 12];

        public OrdersController(
            AppDbContext db,
            IPreparationStockService stockService,
            IPickupSchedulingService pickupSchedulingService)
        {
            _db = db;
            _stockService = stockService;
            _pickupSchedulingService = pickupSchedulingService;
        }

        [HttpGet("pickup-slots")]
        [AllowAnonymous]
        public ActionResult<PickupSlotsResponse> GetPickupSlots()
        {
            return Ok(_pickupSchedulingService.BuildSlotsResponse());
        }

        [HttpGet("discounts")]
        public async Task<ActionResult<List<DiscountDto>>> GetDiscounts()
        {
            var discounts = await _db.Discounts
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(x => new DiscountDto
                {
                    Id = x.Id,
                    Name = x.Name ?? $"Скидка #{x.Id}",
                    DiscountPercent = x.DiscountPercent ?? 0m
                })
                .ToListAsync();

            return Ok(discounts);
        }

        [HttpPost]
        public async Task<ActionResult<OrderResponse>> Create([FromBody] OrderRequest request)
        {
            // ---- базовая валидация ----
            if (request == null)
                return BadRequest("Пустое тело запроса");

            request.Dishes ??= [];
            request.Drinks ??= [];
            request.Toppings ??= [];

            if (request.OrderTypeId <= 0)
                return BadRequest("OrderTypeId обязателен");

            var hasAny = request.Dishes.Count > 0 || request.Drinks.Count > 0 || request.Toppings.Count > 0;
            if (!hasAny)
                return BadRequest("Корзина пуста");

            if (request.Dishes.Any(x => x.Quantity <= 0))
                return BadRequest("Quantity у блюда должен быть > 0");

            if (request.Drinks.Any(x => x.Quantity <= 0))
                return BadRequest("Quantity у напитка должен быть > 0");

            if (request.Toppings.Any(x => x.Quantity <= 0))
                return BadRequest("Quantity у добавки должен быть > 0");

            var isClientOrder = User.IsInRole("Client");
            var isWpfGuestOrder = !isClientOrder;
            Client client;

            if (isClientOrder)
            {
                // ---- ClientId из cookie/claims ----
                var clientIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(clientIdStr, out var clientId))
                    return Unauthorized("Некорректный ClientId в cookie");

                // ---- проверяем клиента ----
                var loadedClient = await _db.Clients.FirstOrDefaultAsync(x => x.Id == clientId);
                if (loadedClient == null)
                    return Unauthorized("Клиент не найден");

                client = loadedClient;
            }
            else
            {
                client = await ResolveWpfGuestClientAsync();
            }

            var discountResolution = await ResolveDiscountForOrderAsync(client, request.DiscountId);
            if (!discountResolution.Success)
                return BadRequest(discountResolution.Message);

            // ---- проверяем тип заказа ----
            var orderTypeExists = await _db.OrderTypes.AnyAsync(x => x.Id == request.OrderTypeId);
            if (!orderTypeExists)
                return BadRequest("Некорректный тип заказа");

            if (request.OrderTypeId != _pickupSchedulingService.TakeawayOrderTypeId)
            {
                request.PickupAt = null;
            }
            else if (request.PickupAt.HasValue)
            {
                var pickupAt = NormalizePickupAt(request.PickupAt.Value);
                if (!_pickupSchedulingService.IsPickupAtAllowed(pickupAt))
                {
                    return BadRequest("Выбранное время самовывоза недоступно. Выберите слот из списка.");
                }

                request.PickupAt = pickupAt;
            }

            // ---- соберём списки ID, чтобы разом подгрузить справочники ----
            var dishIds = request.Dishes.Select(x => x.DishId).Distinct().ToList();
            var drinkIds = request.Drinks.Select(x => x.DrinkId).Distinct().ToList();

            var toppingIdsFromDishes = request.Dishes
                .SelectMany(x => x.Toppings ?? Enumerable.Empty<OrderItemToppingRequest>())
                .Select(x => x.ToppingId);

            var toppingIdsFromDrinks = request.Drinks
                .SelectMany(x => x.Toppings ?? Enumerable.Empty<OrderItemToppingRequest>())
                .Select(x => x.ToppingId);

            var toppingIdsStandalone = request.Toppings.Select(x => x.ToppingId);

            var toppingIds = toppingIdsFromDishes
                .Concat(toppingIdsFromDrinks)
                .Concat(toppingIdsStandalone)
                .Distinct()
                .ToList();
            var portionLimitRequirements = BuildPortionLimitRequirements(request);

            await ApplyDefaultDrinkModifiersAsync(request.Drinks);

            var drinkModifierValidationError = await ValidateDrinkModifiersAsync(request.Drinks);
            if (drinkModifierValidationError != null)
                return BadRequest(drinkModifierValidationError);

            // ---- загрузим цены/ккал ----
            var dishes = await _db.Dishes
                .Where(x => dishIds.Contains(x.Id))
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.PriceRub,
                    x.CaloriesKcal,
                    CategoryName = x.Category != null ? x.Category.Name : null
                })
                .ToDictionaryAsync(x => x.Id, x => x);

            var drinks = await _db.Drinks
                .Where(x => drinkIds.Contains(x.Id))
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.PriceRub,
                    x.CaloriesKcal,
                    CategoryName = x.Category != null ? x.Category.Name : null
                })
                .ToDictionaryAsync(x => x.Id, x => x);

            var toppings = await _db.ToppingsAndSyrups
                .Where(x => toppingIds.Contains(x.Id))
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.PriceRub,
                    x.CaloriesKcal,
                    CategoryName = x.Category != null ? x.Category.Name : null
                })
                .ToDictionaryAsync(x => x.Id, x => x);

            // ---- проверим, что все ID реально существуют ----
            if (dishIds.Any() && dishes.Count != dishIds.Count)
                return BadRequest("В заказе есть несуществующие блюда");

            if (drinkIds.Any() && drinks.Count != drinkIds.Count)
                return BadRequest("В заказе есть несуществующие напитки");

            if (toppingIds.Any() && toppings.Count != toppingIds.Count)
                return BadRequest("В заказе есть несуществующие добавки");

            var hasInactiveCategoryItem =
                dishIds.Any(id => dishes.TryGetValue(id, out var dish) && IsInactiveCategory(dish.CategoryName))
                || drinkIds.Any(id => drinks.TryGetValue(id, out var drink) && IsInactiveCategory(drink.CategoryName))
                || toppingIds.Any(id => toppings.TryGetValue(id, out var topping) && IsInactiveCategory(topping.CategoryName));

            if (hasInactiveCategoryItem)
            {
                return Conflict(new OrderStockConflictResponse
                {
                    ErrorCode = "INACTIVE_CATEGORY",
                    Message = "В заказе есть позиции из категории \"Неактивные\". Обновите меню.",
                    Items = []
                });
            }

            var dishNamesById = dishes.ToDictionary(
                x => x.Key,
                x => string.IsNullOrWhiteSpace(x.Value.Name) ? $"Блюдо #{x.Key}" : x.Value.Name!);
            var drinkNamesById = drinks.ToDictionary(
                x => x.Key,
                x => string.IsNullOrWhiteSpace(x.Value.Name) ? $"Напиток #{x.Key}" : x.Value.Name!);
            var toppingNamesById = toppings.ToDictionary(
                x => x.Key,
                x => string.IsNullOrWhiteSpace(x.Value.Name) ? $"Добавка #{x.Key}" : x.Value.Name!);

            // ---- транзакция ----
            await using var tx = await _db.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            try
            {
                // 1) категория/скидка ДО расчёта итоговой цены
                // (считаем скидку по текущей категории; после заказа мы обновим OrderCount/категорию)
                var discountId = discountResolution.DiscountId;
                var discountPercent = discountResolution.DiscountPercent;
                var createdStatus = await ResolveCreatedStatusAsync();
                if (createdStatus == null)
                    return StatusCode(500, "Не найден статус заказа для создания");

                // 2) создаём Order
                var order = new Order
                {
                    CreatedAt = DateTime.Now,
                    ClientId = client.Id,
                    StatusId = createdStatus.Value.Id,
                    OrderTypeId = request.OrderTypeId,
                    Comment = string.IsNullOrWhiteSpace(request.Comment) ? null : request.Comment,
                    PickupAt = request.PickupAt,
                    DiscountId = discountId,
                    TotalCalories = 0m,
                    TotalPrice = 0m
                };

                _db.Orders.Add(order);
                await _db.SaveChangesAsync(); // чтобы получить order.Id

                decimal totalPriceBeforeDiscount = 0m;
                decimal totalCalories = 0m;

                // 3) Блюда + их добавки
                foreach (var dishReq in request.Dishes)
                {
                    var dish = dishes[dishReq.DishId];

                    var dishBasePrice = (dish.PriceRub ?? 0m) * dishReq.Quantity;
                    var dishBaseCalories = (dish.CaloriesKcal ?? 0m) * dishReq.Quantity;

                    // создаём OrderDishItem
                    var odi = new OrderDishItem
                    {
                        OrderId = order.Id,
                        DishId = dishReq.DishId,
                        Quantity = dishReq.Quantity,
                        FinalPrice = 0m
                    };

                    _db.OrderDishItems.Add(odi);
                    await _db.SaveChangesAsync(); // нужен odi.Id для DishToppings

                    decimal toppingsTotalPrice = 0m;
                    decimal toppingsTotalCalories = 0m;

                    foreach (var topReq in dishReq.Toppings ?? Enumerable.Empty<OrderItemToppingRequest>())
                    {
                        if (topReq.Quantity <= 0)
                            continue;

                        var top = toppings[topReq.ToppingId];

                        // В UI topping.quantity — "на 1 блюдо", а итог умножается на item.quantity
                        var totalToppingQty = topReq.Quantity * dishReq.Quantity;

                        var topPrice = (top.PriceRub ?? 0m) * totalToppingQty;
                        var topCalories = (top.CaloriesKcal ?? 0m) * totalToppingQty;

                        toppingsTotalPrice += topPrice;
                        toppingsTotalCalories += topCalories;

                        var dt = new DishTopping
                        {
                            ToppingId = topReq.ToppingId,
                            OrderDishItemId = odi.Id,
                            Quantity = totalToppingQty,
                            FinalPrice = topPrice
                        };

                        _db.DishToppings.Add(dt);
                    }

                    odi.FinalPrice = dishBasePrice + toppingsTotalPrice;

                    totalPriceBeforeDiscount += odi.FinalPrice ?? 0m;
                    totalCalories += dishBaseCalories + toppingsTotalCalories;

                    _db.OrderDishItems.Update(odi);
                    await _db.SaveChangesAsync();
                }

                // 4) Напитки + их добавки
                foreach (var drinkReq in request.Drinks)
                {
                    var drink = drinks[drinkReq.DrinkId];

                    var drinkBasePrice = (drink.PriceRub ?? 0m) * drinkReq.Quantity;
                    var drinkBaseCalories = (drink.CaloriesKcal ?? 0m) * drinkReq.Quantity;

                    var odi = new OrderDrinkItem
                    {
                        OrderId = order.Id,
                        DrinkId = drinkReq.DrinkId,
                        Quantity = drinkReq.Quantity,
                        FinalPrice = 0m
                    };

                    _db.OrderDrinkItems.Add(odi);
                    await _db.SaveChangesAsync(); // нужен odi.Id для DrinkToppings

                    _db.OrderDrinkItemModifiers.Add(new OrderDrinkItemModifier
                    {
                        OrderDrinkItemId = odi.Id,
                        MilkIngredientId = drinkReq.MilkIngredientId,
                        CoffeeIngredientId = drinkReq.CoffeeIngredientId
                    });

                    decimal toppingsTotalPrice = 0m;
                    decimal toppingsTotalCalories = 0m;

                    foreach (var topReq in drinkReq.Toppings ?? Enumerable.Empty<OrderItemToppingRequest>())
                    {
                        if (topReq.Quantity <= 0)
                            continue;

                        var top = toppings[topReq.ToppingId];

                        var totalToppingQty = topReq.Quantity * drinkReq.Quantity;

                        var topPrice = (top.PriceRub ?? 0m) * totalToppingQty;
                        var topCalories = (top.CaloriesKcal ?? 0m) * totalToppingQty;

                        toppingsTotalPrice += topPrice;
                        toppingsTotalCalories += topCalories;

                        var dt = new DrinkTopping
                        {
                            ToppingId = topReq.ToppingId,
                            OrderDrinkItemId = odi.Id,
                            Quantity = totalToppingQty,
                            FinalPrice = topPrice
                        };

                        _db.DrinkToppings.Add(dt);
                    }

                    odi.FinalPrice = drinkBasePrice + toppingsTotalPrice;

                    totalPriceBeforeDiscount += odi.FinalPrice ?? 0m;
                    totalCalories += drinkBaseCalories + toppingsTotalCalories;

                    _db.OrderDrinkItems.Update(odi);
                    await _db.SaveChangesAsync();
                }

                // 5) Отдельные добавки (OrderToppingItems)
                foreach (var topReq in request.Toppings)
                {
                    var top = toppings[topReq.ToppingId];

                    var linePrice = (top.PriceRub ?? 0m) * topReq.Quantity;
                    var lineCalories = (top.CaloriesKcal ?? 0m) * topReq.Quantity;

                    var oti = new OrderToppingItem
                    {
                        OrderId = order.Id,
                        ToppingId = topReq.ToppingId,
                        Quantity = topReq.Quantity,
                        TotalPrice = linePrice
                    };

                    _db.OrderToppingItems.Add(oti);

                    totalPriceBeforeDiscount += linePrice;
                    totalCalories += lineCalories;
                }

                // 6) применяем скидку
                var totalAfterDiscount = ApplyDiscount(totalPriceBeforeDiscount, discountPercent);

                order.TotalPrice = Round2(totalAfterDiscount);
                order.TotalCalories = Round2(totalCalories);

                _db.Orders.Update(order);
                await _db.SaveChangesAsync();

                StockConsumptionResult consumeResult;
                try
                {
                    consumeResult = await _stockService.TryConsumeForOrderAsync(order.Id);
                }
                catch (InvalidOperationException ex)
                {
                    await tx.RollbackAsync();
                    return BadRequest(ex.Message);
                }

                if (!consumeResult.IsSuccess)
                {
                    await tx.RollbackAsync();
                    return Conflict(new OrderStockConflictResponse
                    {
                        Message = "Недостаточно заготовок для оформления заказа. Обновите меню.",
                        Items = consumeResult.Items
                            .Select(x => new StockConflictItem
                            {
                                SemiFinishedId = x.SemiFinishedId,
                                SemiFinishedName = x.SemiFinishedName,
                                Required = x.Required,
                                Available = x.Available
                            })
                            .ToList()
                    });
                }

                var portionLimitsResult = await TryConsumePortionLimitsAsync(
                    portionLimitRequirements,
                    dishNamesById,
                    drinkNamesById,
                    toppingNamesById);
                if (!portionLimitsResult.IsSuccess)
                {
                    await tx.RollbackAsync();
                    return Conflict(new OrderStockConflictResponse
                    {
                        ErrorCode = "PORTION_LIMIT_CONFLICT",
                        Message = "Недостаточно лимита порций для некоторых позиций. Обновите меню.",
                        Items = portionLimitsResult.Items
                    });
                }

                await _db.SaveChangesAsync();

                // 7) обновляем OrderCount/категорию реального клиента (служебного гостя WPF не трогаем)
                if (!isWpfGuestOrder)
                {
                    await UpdateClientCategoryAfterOrderAsync(client);
                }

                await _stockService.RefreshMenuAvailabilityAsync();

                await tx.CommitAsync();

                return Ok(new OrderResponse
                {
                    OrderId = order.Id,
                    Status = createdStatus.Value.Name,
                    TotalPrice = order.TotalPrice ?? 0m,
                    TotalCalories = order.TotalCalories ?? 0m,
                    DiscountId = order.DiscountId,
                    DiscountPercent = discountPercent
                });
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        private async Task<Client> ResolveWpfGuestClientAsync()
        {
            var existing = await _db.Clients
                .FirstOrDefaultAsync(x => x.PhoneNumber == WpfGuestClientPhone);

            if (existing != null)
            {
                return existing;
            }

            var guest = new Client
            {
                FullName = WpfGuestClientFullName,
                PhoneNumber = WpfGuestClientPhone,
                Password = WpfGuestClientPassword,
                ClientCategoryId = ClientCategoryNoneId,
                OrderCount = 0
            };

            _db.Clients.Add(guest);
            await _db.SaveChangesAsync();
            return guest;
        }

        private async Task<DiscountResolution> ResolveDiscountForOrderAsync(Client client, int? requestedDiscountId)
        {
            if (requestedDiscountId.HasValue)
            {
                var requested = await _db.Discounts
                    .AsNoTracking()
                    .Where(x => x.Id == requestedDiscountId.Value)
                    .Select(x => new
                    {
                        x.Id,
                        Percent = x.DiscountPercent ?? 0m
                    })
                    .FirstOrDefaultAsync();

                if (requested == null)
                {
                    return DiscountResolution.Fail("Выбранная скидка не найдена.");
                }

                return DiscountResolution.Ok(requested.Id, requested.Percent);
            }

            var (discountId, discountPercent) = await ResolveDiscountByClientAsync(client);
            return DiscountResolution.Ok(discountId, discountPercent);
        }

        private async Task<(int? DiscountId, decimal DiscountPercent)> ResolveDiscountByClientAsync(Client client)
        {
            // если категория null — считаем "Без категории" (без скидки)
            var catId = client.ClientCategoryId ?? ClientCategoryNoneId;

            int? discountId = null;

            if (catId == ClientCategoryNewId) discountId = DiscountNewId;
            else if (catId == ClientCategoryRegularId) discountId = DiscountRegularId;
            else if (catId == ClientCategorySpecialId) discountId = DiscountSpecialId;

            if (discountId == null)
                return (null, 0m);

            var percent = await _db.Discounts
                .Where(x => x.Id == discountId.Value)
                .Select(x => x.DiscountPercent ?? 0m)
                .FirstOrDefaultAsync();

            return (discountId, percent);
        }

        private async Task UpdateClientCategoryAfterOrderAsync(Client client)
        {
            var currentCount = client.OrderCount ?? 0;
            currentCount += 1;
            client.OrderCount = currentCount;

            //  Если админ уже поставил "Особый" — НЕ трогаем категорию автоматически
            if (client.ClientCategoryId == ClientCategorySpecialId)
            {
                _db.Clients.Update(client);
                await _db.SaveChangesAsync();
                return;
            }

            //  Автоматом только "Новый" -> "Постоянный"
            int newCategoryId;

            if (currentCount >= RegularFromOrdersCount)
                newCategoryId = ClientCategoryRegularId;
            else
                newCategoryId = ClientCategoryNewId;

            client.ClientCategoryId = newCategoryId;

            _db.Clients.Update(client);
            await _db.SaveChangesAsync();
        }

        private static decimal ApplyDiscount(decimal total, decimal discountPercent)
        {
            if (discountPercent <= 0m) return total;
            var coef = 1m - (discountPercent / 100m);
            return total * coef;
        }

        private async Task<(int Id, string Name)?> ResolveCreatedStatusAsync()
        {
            var status = await _db.OrderStatuses
                .AsNoTracking()
                .Where(s => s.Name != null && EF.Functions.Like(s.Name.ToLower(), $"%{OrderStatusCreatedToken}%"))
                .OrderBy(s => s.Id)
                .Select(s => new { s.Id, s.Name })
                .FirstOrDefaultAsync();

            if (status != null)
                return (status.Id, status.Name ?? OrderStatusCreatedName);

            var fallback = await _db.OrderStatuses
                .AsNoTracking()
                .Where(s => s.Name == OrderStatusCreatedName)
                .Select(s => new { s.Id, s.Name })
                .FirstOrDefaultAsync();

            if (fallback != null)
                return (fallback.Id, fallback.Name ?? OrderStatusCreatedName);

            return null;
        }

        private static bool IsInactiveCategory(string? categoryName)
        {
            return !string.IsNullOrWhiteSpace(categoryName)
                && categoryName.Trim().Equals(InactiveCategoryName, StringComparison.OrdinalIgnoreCase);
        }

        private static Dictionary<MenuItemLimitKey, decimal> BuildPortionLimitRequirements(OrderRequest request)
        {
            var requirements = new Dictionary<MenuItemLimitKey, decimal>();

            foreach (var dish in request.Dishes ?? [])
            {
                AddPortionRequirement(requirements, KitchenItemTypes.Dish, dish.DishId, dish.Quantity);

                foreach (var topping in dish.Toppings ?? [])
                {
                    var totalToppingQuantity = dish.Quantity * topping.Quantity;
                    AddPortionRequirement(requirements, KitchenItemTypes.Topping, topping.ToppingId, totalToppingQuantity);
                }
            }

            foreach (var drink in request.Drinks ?? [])
            {
                AddPortionRequirement(requirements, KitchenItemTypes.Drink, drink.DrinkId, drink.Quantity);

                foreach (var topping in drink.Toppings ?? [])
                {
                    var totalToppingQuantity = drink.Quantity * topping.Quantity;
                    AddPortionRequirement(requirements, KitchenItemTypes.Topping, topping.ToppingId, totalToppingQuantity);
                }
            }

            foreach (var topping in request.Toppings ?? [])
            {
                AddPortionRequirement(requirements, KitchenItemTypes.Topping, topping.ToppingId, topping.Quantity);
            }

            return requirements;
        }

        private static void AddPortionRequirement(
            IDictionary<MenuItemLimitKey, decimal> target,
            string itemType,
            int itemId,
            decimal quantity)
        {
            if (itemId <= 0 || quantity <= 0m)
            {
                return;
            }

            var key = new MenuItemLimitKey(itemType.Trim().ToLowerInvariant(), itemId);
            if (target.TryGetValue(key, out var current))
            {
                target[key] = current + quantity;
            }
            else
            {
                target[key] = quantity;
            }
        }

        private async Task<PortionLimitConsumeResult> TryConsumePortionLimitsAsync(
            IReadOnlyDictionary<MenuItemLimitKey, decimal> requirementsByItem,
            IReadOnlyDictionary<int, string> dishNamesById,
            IReadOnlyDictionary<int, string> drinkNamesById,
            IReadOnlyDictionary<int, string> toppingNamesById)
        {
            if (requirementsByItem.Count == 0)
            {
                return PortionLimitConsumeResult.Success();
            }

            var itemTypes = requirementsByItem.Keys
                .Select(x => x.ItemType)
                .Distinct()
                .ToList();
            var itemIds = requirementsByItem.Keys
                .Select(x => x.ItemId)
                .Distinct()
                .ToList();

            var limitRows = await _db.MenuItemPortionLimits
                .Where(x =>
                    itemTypes.Contains(x.ItemType) &&
                    itemIds.Contains(x.ItemId))
                .ToListAsync();

            var limitsByKey = limitRows.ToDictionary(
                x => new MenuItemLimitKey(x.ItemType.Trim().ToLowerInvariant(), x.ItemId),
                x => x);

            var conflicts = new List<StockConflictItem>();

            foreach (var pair in requirementsByItem
                .OrderBy(x => x.Key.ItemType)
                .ThenBy(x => x.Key.ItemId))
            {
                if (!limitsByKey.TryGetValue(pair.Key, out var limitRow))
                {
                    continue;
                }

                var available = Math.Max(0m, limitRow.RemainingPortions);
                var required = pair.Value;

                if (available + DecimalEpsilon >= required)
                {
                    continue;
                }

                conflicts.Add(new StockConflictItem
                {
                    SemiFinishedId = pair.Key.ItemId,
                    SemiFinishedName = ResolveMenuItemName(pair.Key, dishNamesById, drinkNamesById, toppingNamesById),
                    Required = Round2(required),
                    Available = Round2(available)
                });
            }

            if (conflicts.Count > 0)
            {
                return PortionLimitConsumeResult.Fail(conflicts);
            }

            var now = DateTime.Now;
            var depletedDishIds = new HashSet<int>();
            var depletedDrinkIds = new HashSet<int>();
            var depletedToppingIds = new HashSet<int>();

            foreach (var pair in requirementsByItem)
            {
                if (!limitsByKey.TryGetValue(pair.Key, out var limitRow))
                {
                    continue;
                }

                var current = Math.Max(0m, limitRow.RemainingPortions);
                var remaining = current - pair.Value;
                var roundedRemaining = Round2(Math.Max(0m, remaining));

                if (remaining <= DecimalEpsilon || roundedRemaining <= 0m)
                {
                    limitRow.RemainingPortions = 0m;

                    if (pair.Key.ItemType == KitchenItemTypes.Dish)
                    {
                        depletedDishIds.Add(pair.Key.ItemId);
                    }
                    else if (pair.Key.ItemType == KitchenItemTypes.Drink)
                    {
                        depletedDrinkIds.Add(pair.Key.ItemId);
                    }
                    else if (pair.Key.ItemType == KitchenItemTypes.Topping)
                    {
                        depletedToppingIds.Add(pair.Key.ItemId);
                    }
                }
                else
                {
                    limitRow.RemainingPortions = roundedRemaining;
                }

                limitRow.UpdatedAt = now;
            }

            if (depletedDishIds.Count > 0)
            {
                var dishes = await _db.Dishes
                    .Where(x => depletedDishIds.Contains(x.Id))
                    .ToListAsync();

                foreach (var dish in dishes)
                {
                    dish.IsAvailable = false;
                }
            }

            if (depletedDrinkIds.Count > 0)
            {
                var drinks = await _db.Drinks
                    .Where(x => depletedDrinkIds.Contains(x.Id))
                    .ToListAsync();

                foreach (var drink in drinks)
                {
                    drink.IsAvailable = false;
                }
            }

            if (depletedToppingIds.Count > 0)
            {
                var toppings = await _db.ToppingsAndSyrups
                    .Where(x => depletedToppingIds.Contains(x.Id))
                    .ToListAsync();

                foreach (var topping in toppings)
                {
                    topping.IsAvailable = false;
                }
            }

            return PortionLimitConsumeResult.Success();
        }

        private static string ResolveMenuItemName(
            MenuItemLimitKey key,
            IReadOnlyDictionary<int, string> dishNamesById,
            IReadOnlyDictionary<int, string> drinkNamesById,
            IReadOnlyDictionary<int, string> toppingNamesById)
        {
            if (key.ItemType == KitchenItemTypes.Dish)
            {
                var name = dishNamesById.TryGetValue(key.ItemId, out var dishName)
                    ? dishName
                    : $"Блюдо #{key.ItemId}";
                return $"Блюдо: {name}";
            }

            if (key.ItemType == KitchenItemTypes.Drink)
            {
                var name = drinkNamesById.TryGetValue(key.ItemId, out var drinkName)
                    ? drinkName
                    : $"Напиток #{key.ItemId}";
                return $"Напиток: {name}";
            }

            var resolvedToppingName = toppingNamesById.TryGetValue(key.ItemId, out var toppingName)
                ? toppingName
                : $"Добавка #{key.ItemId}";
            return $"Добавка: {resolvedToppingName}";
        }

        private async Task ApplyDefaultDrinkModifiersAsync(IReadOnlyCollection<OrderDrinkItemRequest> drinks)
        {
            if (drinks.Count == 0)
            {
                return;
            }

            var targetDrinks = drinks
                .Where(x => !ModifierExcludedDrinkIds.Contains(x.DrinkId))
                .ToList();
            if (targetDrinks.Count == 0)
            {
                return;
            }

            var needsMilkDefault = targetDrinks.Any(x => !x.MilkIngredientId.HasValue);
            var needsCoffeeDefault = targetDrinks.Any(x => !x.CoffeeIngredientId.HasValue);
            if (!needsMilkDefault && !needsCoffeeDefault)
            {
                return;
            }

            var idsToLoad = new HashSet<int>();
            if (needsMilkDefault)
            {
                foreach (var id in MilkModifierIngredientIds)
                {
                    idsToLoad.Add(id);
                }
            }

            if (needsCoffeeDefault)
            {
                foreach (var id in CoffeeModifierIngredientIds)
                {
                    idsToLoad.Add(id);
                }
            }

            if (idsToLoad.Count == 0)
            {
                return;
            }

            var modifierInfoById = await _db.Ingredients
                .AsNoTracking()
                .Where(i => idsToLoad.Contains(i.Id))
                .Select(i => new
                {
                    i.Id,
                    i.Name,
                    i.CategoryId
                })
                .ToDictionaryAsync(
                    i => i.Id,
                    i => new ModifierInfo(i.Name, i.CategoryId));

            var defaultMilkModifierId = needsMilkDefault
                ? ResolveDefaultModifierId(
                    MilkModifierIngredientIds,
                    MilkCategoryId,
                    PreferredMilkModifierName,
                    modifierInfoById)
                : null;

            var defaultCoffeeModifierId = needsCoffeeDefault
                ? ResolveDefaultModifierId(
                    CoffeeModifierIngredientIds,
                    CoffeeCategoryId,
                    PreferredCoffeeModifierName,
                    modifierInfoById)
                : null;

            foreach (var drink in targetDrinks)
            {
                if (!drink.MilkIngredientId.HasValue && defaultMilkModifierId.HasValue)
                {
                    drink.MilkIngredientId = defaultMilkModifierId.Value;
                }

                if (!drink.CoffeeIngredientId.HasValue && defaultCoffeeModifierId.HasValue)
                {
                    drink.CoffeeIngredientId = defaultCoffeeModifierId.Value;
                }
            }
        }

        private static int? ResolveDefaultModifierId(
            IReadOnlyList<int> orderedIds,
            int categoryId,
            string preferredName,
            IReadOnlyDictionary<int, ModifierInfo> modifierInfoById)
        {
            var normalizedPreferredName = NormalizeModifierName(preferredName);
            int? firstAvailableId = null;

            foreach (var id in orderedIds)
            {
                if (!modifierInfoById.TryGetValue(id, out var modifierInfo) || modifierInfo.CategoryId != categoryId)
                {
                    continue;
                }

                firstAvailableId ??= id;

                if (NormalizeModifierName(modifierInfo.Name) == normalizedPreferredName)
                {
                    return id;
                }
            }

            return firstAvailableId;
        }

        private static string NormalizeModifierName(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            var normalizedSpaces = string.Join(" ", value.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
            return normalizedSpaces.ToUpperInvariant();
        }

        private async Task<string?> ValidateDrinkModifiersAsync(IReadOnlyCollection<OrderDrinkItemRequest> drinks)
        {
            var allModifierIds = new HashSet<int>();

            foreach (var drink in drinks)
            {
                if (drink.MilkIngredientId.HasValue)
                {
                    var milkIngredientId = drink.MilkIngredientId.Value;
                    if (!MilkModifierIngredientIds.Contains(milkIngredientId))
                    {
                        return $"MilkIngredientId={milkIngredientId} не входит в список разрешённых модификаторов.";
                    }

                    allModifierIds.Add(milkIngredientId);
                }

                if (drink.CoffeeIngredientId.HasValue)
                {
                    var coffeeIngredientId = drink.CoffeeIngredientId.Value;
                    if (!CoffeeModifierIngredientIds.Contains(coffeeIngredientId))
                    {
                        return $"CoffeeIngredientId={coffeeIngredientId} не входит в список разрешённых модификаторов.";
                    }

                    allModifierIds.Add(coffeeIngredientId);
                }
            }

            if (allModifierIds.Count == 0)
            {
                return null;
            }

            var ingredientCategories = await _db.Ingredients
                .Where(i => allModifierIds.Contains(i.Id))
                .Select(i => new { i.Id, i.CategoryId })
                .ToDictionaryAsync(i => i.Id, i => i.CategoryId);

            foreach (var modifierId in allModifierIds)
            {
                if (!ingredientCategories.ContainsKey(modifierId))
                {
                    return $"Ингредиент-модификатор Id={modifierId} не найден.";
                }
            }

            foreach (var drink in drinks)
            {
                if (drink.MilkIngredientId.HasValue)
                {
                    var milkIngredientId = drink.MilkIngredientId.Value;
                    if (!ingredientCategories.TryGetValue(milkIngredientId, out var milkCategoryId) || milkCategoryId != MilkCategoryId)
                    {
                        return $"Ингредиент Id={milkIngredientId} не относится к категории молока.";
                    }
                }

                if (drink.CoffeeIngredientId.HasValue)
                {
                    var coffeeIngredientId = drink.CoffeeIngredientId.Value;
                    if (!ingredientCategories.TryGetValue(coffeeIngredientId, out var coffeeCategoryId) || coffeeCategoryId != CoffeeCategoryId)
                    {
                        return $"Ингредиент Id={coffeeIngredientId} не относится к категории кофе.";
                    }
                }
            }

            return null;
        }

        private static DateTime NormalizePickupAt(DateTime value)
        {
            var local = value.Kind == DateTimeKind.Utc
                ? value.ToLocalTime()
                : value;

            return new DateTime(
                local.Year,
                local.Month,
                local.Day,
                local.Hour,
                local.Minute,
                local.Second,
                DateTimeKind.Unspecified);
        }

        private readonly record struct MenuItemLimitKey(string ItemType, int ItemId);

        private sealed class PortionLimitConsumeResult
        {
            public bool IsSuccess { get; private set; }
            public List<StockConflictItem> Items { get; private set; } = new List<StockConflictItem>();

            public static PortionLimitConsumeResult Success()
            {
                return new PortionLimitConsumeResult
                {
                    IsSuccess = true
                };
            }

            public static PortionLimitConsumeResult Fail(List<StockConflictItem> items)
            {
                return new PortionLimitConsumeResult
                {
                    IsSuccess = false,
                    Items = items ?? new List<StockConflictItem>()
                };
            }
        }

        private sealed class DiscountResolution
        {
            public bool Success { get; private set; }
            public string Message { get; private set; } = string.Empty;
            public int? DiscountId { get; private set; }
            public decimal DiscountPercent { get; private set; }

            public static DiscountResolution Ok(int? discountId, decimal discountPercent)
            {
                return new DiscountResolution
                {
                    Success = true,
                    DiscountId = discountId,
                    DiscountPercent = discountPercent
                };
            }

            public static DiscountResolution Fail(string message)
            {
                return new DiscountResolution
                {
                    Success = false,
                    Message = message
                };
            }
        }

        private sealed record ModifierInfo(string? Name, int? CategoryId);

        private static decimal Round2(decimal value)
            => Math.Round(value, 2, MidpointRounding.AwayFromZero);
    }
}
