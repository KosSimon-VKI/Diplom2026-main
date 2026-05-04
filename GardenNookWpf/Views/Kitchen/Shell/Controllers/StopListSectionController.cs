using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TransferModels.Kitchen;

namespace GardenNookWpf.Views.Kitchen.Shell.Controllers
{
    public sealed class StopListSectionController : IKitchenSectionController
    {
        private const string KitchenApiBaseAddress = "https://localhost:7235/api/kitchen";
        private const string StopListPositionsAddress = KitchenApiBaseAddress + "/stop-list/positions";
        private const string StopListItemsAddress = KitchenApiBaseAddress + "/stop-list/items";
        private const string AdminRole = "Администратор";
        private const string CookRole = "Повар";
        private const string BaristaRole = "Бариста";
        private const string DishToppingCategoryToken = "к блюд";
        private const string DrinkToppingCategoryToken = "к напит";

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly HttpClient _httpClient;
        private readonly string _userRole;
        private readonly List<StopListPositionDisplayModel> _allPositions = new List<StopListPositionDisplayModel>();
        private bool _isBusy;

        public StopListSectionController(HttpClient httpClient, string userRole)
        {
            _httpClient = httpClient;
            _userRole = userRole ?? string.Empty;
        }

        public event Action<bool>? BusyStateChanged;

        public bool IsBusy => _isBusy;

        public IReadOnlyCollection<StopListPositionDisplayModel> AllPositions => _allPositions
            .Where(IsVisibleForRole)
            .ToList();

        public async Task ActivateAsync()
        {
            await ReloadAsync();
        }

        public void Deactivate()
        {
        }

        public async Task<(bool Success, string Message)> ReloadAsync()
        {
            try
            {
                SetBusy(true);

                var response = await _httpClient.GetAsync(StopListPositionsAddress);
                if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                {
                    _allPositions.Clear();
                    return (false, "Нет доступа к стоп-листу.");
                }

                if (!response.IsSuccessStatusCode)
                {
                    _allPositions.Clear();
                    var error = await ReadErrorMessageAsync(response);
                    if (error.Length > 200)
                    {
                        error = error.Substring(0, 200) + "...";
                    }

                    var message = string.IsNullOrWhiteSpace(error)
                        ? $"Не удалось загрузить стоп-лист. Код: {(int)response.StatusCode}."
                        : $"Не удалось загрузить стоп-лист ({(int)response.StatusCode}): {error}";

                    return (false, message);
                }

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<KitchenStopListPositionsResponse>(json, JsonOptions)
                    ?? new KitchenStopListPositionsResponse();

                var positions = data.Positions ?? new List<KitchenStopListPositionDto>();
                _allPositions.Clear();
                _allPositions.AddRange(positions.Select(MapToDisplayModel));

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                _allPositions.Clear();
                return (false, $"Ошибка загрузки: {ex.Message}");
            }
            finally
            {
                SetBusy(false);
            }
        }

        public StopListFilterResult Filter(string? queryText)
        {
            var baseItems = _allPositions
                .Where(IsVisibleForRole)
                .Where(x => !x.IsAvailable || x.ManualRemainingPortions.HasValue)
                .ToList();

            var query = (queryText ?? string.Empty).Trim();
            IEnumerable<StopListPositionDisplayModel> visibleSource = baseItems;

            if (!string.IsNullOrWhiteSpace(query))
            {
                visibleSource = baseItems.Where(x =>
                    x.Name.Contains(query, StringComparison.CurrentCultureIgnoreCase) ||
                    x.ItemTypeDisplay.Contains(query, StringComparison.CurrentCultureIgnoreCase) ||
                    x.Category.Contains(query, StringComparison.CurrentCultureIgnoreCase) ||
                    x.StateDisplay.Contains(query, StringComparison.CurrentCultureIgnoreCase) ||
                    x.ItemId.ToString().Contains(query, StringComparison.CurrentCultureIgnoreCase));
            }

            var visibleItems = visibleSource
                .OrderBy(x => x.IsAvailable ? 1 : 0)
                .ThenBy(x => x.ItemTypeDisplay)
                .ThenBy(x => x.Name)
                .ThenBy(x => x.ItemId)
                .ToList();

            var statusMessage = baseItems.Count == 0
                ? "Нет ограничений и позиций в стоп-листе."
                : visibleItems.Count == 0
                    ? "По вашему запросу ничего не найдено."
                    : string.Empty;

            return new StopListFilterResult
            {
                VisibleItems = visibleItems,
                StatusMessage = statusMessage
            };
        }

        public async Task<(bool Success, string Message)> AddPositionToStopListAsync(string itemType, int itemId, decimal remainingPortions)
        {
            try
            {
                SetBusy(true);

                var payload = JsonSerializer.Serialize(new KitchenStopListItemRequest
                {
                    ItemType = itemType,
                    ItemId = itemId,
                    RemainingPortions = remainingPortions
                });

                var response = await _httpClient.PostAsync(
                    StopListItemsAddress,
                    new StringContent(payload, Encoding.UTF8, "application/json"));

                if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                {
                    return (false, "Нет доступа к обновлению стоп-листа.");
                }

                if (!response.IsSuccessStatusCode)
                {
                    var error = await ReadErrorMessageAsync(response);
                    return (false, string.IsNullOrWhiteSpace(error)
                        ? "Не удалось применить лимит/стоп для позиции."
                        : error);
                }

                return await ReloadAsync();
            }
            catch (Exception ex)
            {
                return (false, $"Ошибка обновления: {ex.Message}");
            }
            finally
            {
                SetBusy(false);
            }
        }

        public async Task<(bool Success, string Message)> RemovePositionFromStopListAsync(string itemType, int itemId)
        {
            try
            {
                SetBusy(true);

                var response = await _httpClient.DeleteAsync($"{StopListItemsAddress}/{Uri.EscapeDataString(itemType)}/{itemId}");
                if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                {
                    return (false, "Нет доступа к обновлению стоп-листа.");
                }

                if (!response.IsSuccessStatusCode)
                {
                    var error = await ReadErrorMessageAsync(response);
                    return (false, string.IsNullOrWhiteSpace(error)
                        ? "Не удалось снять лимит/убрать позицию из стоп-листа."
                        : error);
                }

                return await ReloadAsync();
            }
            catch (Exception ex)
            {
                return (false, $"Ошибка обновления: {ex.Message}");
            }
            finally
            {
                SetBusy(false);
            }
        }

        private static StopListPositionDisplayModel MapToDisplayModel(KitchenStopListPositionDto source)
        {
            var normalizedCategory = string.IsNullOrWhiteSpace(source.Category)
                ? string.Empty
                : source.Category.Trim();

            var manualRemaining = source.ManualRemainingPortions;
            var autoRemaining = source.AutoAvailablePortions;
            var effectiveRemaining = source.EffectiveRemainingPortions;

            return new StopListPositionDisplayModel
            {
                ItemType = source.ItemType ?? string.Empty,
                ItemId = source.ItemId,
                Name = string.IsNullOrWhiteSpace(source.Name)
                    ? $"Позиция #{source.ItemId}"
                    : source.Name,
                Category = normalizedCategory,
                IsAvailable = source.IsAvailable,
                ManualRemainingPortions = manualRemaining,
                AutoAvailablePortions = autoRemaining,
                EffectiveRemainingPortions = effectiveRemaining,
                ItemTypeDisplay = ToItemTypeDisplay(source.ItemType),
                StateDisplay = BuildStateDisplay(source.IsAvailable, manualRemaining),
                CategoryDisplay = string.IsNullOrWhiteSpace(normalizedCategory)
                    ? string.Empty
                    : $"Категория: {normalizedCategory}",
                CategoryVisibility = string.IsNullOrWhiteSpace(normalizedCategory)
                    ? System.Windows.Visibility.Collapsed
                    : System.Windows.Visibility.Visible,
                VolumeWeightDisplay = string.IsNullOrWhiteSpace(source.VolumeWeight)
                    ? string.Empty
                    : $"Вес/объем: {source.VolumeWeight}",
                VolumeWeightVisibility = string.IsNullOrWhiteSpace(source.VolumeWeight)
                    ? System.Windows.Visibility.Collapsed
                    : System.Windows.Visibility.Visible,
                ManualRemainingDisplay = manualRemaining.HasValue
                    ? $"Ручной остаток: {FormatPortions(manualRemaining.Value)}"
                    : string.Empty,
                ManualRemainingVisibility = manualRemaining.HasValue
                    ? System.Windows.Visibility.Visible
                    : System.Windows.Visibility.Collapsed,
                AutoAvailableDisplay = autoRemaining.HasValue
                    ? $"Авто-доступно: {FormatPortions(autoRemaining.Value)}"
                    : string.Empty,
                AutoAvailableVisibility = autoRemaining.HasValue
                    ? System.Windows.Visibility.Visible
                    : System.Windows.Visibility.Collapsed,
                EffectiveRemainingDisplay = effectiveRemaining.HasValue
                    ? $"Эффективный остаток: {FormatPortions(effectiveRemaining.Value)}"
                    : string.Empty,
                EffectiveRemainingVisibility = effectiveRemaining.HasValue
                    ? System.Windows.Visibility.Visible
                    : System.Windows.Visibility.Collapsed
            };
        }

        private static string ToItemTypeDisplay(string? itemType)
        {
            var normalized = itemType?.Trim().ToLowerInvariant() ?? string.Empty;
            return normalized switch
            {
                KitchenItemTypes.Dish => "Тип: Блюдо",
                KitchenItemTypes.Drink => "Тип: Напиток",
                KitchenItemTypes.Topping => "Тип: Добавка/сироп",
                _ => "Тип: Неизвестно"
            };
        }

        private bool IsVisibleForRole(StopListPositionDisplayModel item)
        {
            if (string.Equals(_userRole, AdminRole, StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }

            var itemType = item.ItemType.Trim().ToLowerInvariant();
            if (string.Equals(_userRole, CookRole, StringComparison.CurrentCultureIgnoreCase))
            {
                return itemType == KitchenItemTypes.Dish ||
                       (itemType == KitchenItemTypes.Topping && CategoryContains(item.Category, DishToppingCategoryToken));
            }

            if (string.Equals(_userRole, BaristaRole, StringComparison.CurrentCultureIgnoreCase))
            {
                return itemType == KitchenItemTypes.Drink ||
                       (itemType == KitchenItemTypes.Topping && CategoryContains(item.Category, DrinkToppingCategoryToken));
            }

            return false;
        }

        private static bool CategoryContains(string category, string token)
        {
            return (category ?? string.Empty)
                .Trim()
                .ToLowerInvariant()
                .Contains(token);
        }

        private static string BuildStateDisplay(bool isAvailable, decimal? manualRemainingPortions)
        {
            if (!isAvailable)
            {
                return "Статус: в стоп-листе";
            }

            if (manualRemainingPortions.HasValue)
            {
                return "Статус: ограничено по лимиту порций";
            }

            return "Статус: доступно";
        }

        private static async Task<string> ReadErrorMessageAsync(HttpResponseMessage response)
        {
            var payload = await response.Content.ReadAsStringAsync();
            return string.IsNullOrWhiteSpace(payload)
                ? response.ReasonPhrase ?? string.Empty
                : payload;
        }

        private static string FormatPortions(decimal value)
        {
            return value.ToString("0.##", CultureInfo.InvariantCulture) + " порц.";
        }

        private void SetBusy(bool isBusy)
        {
            _isBusy = isBusy;
            BusyStateChanged?.Invoke(isBusy);
        }

        public sealed class StopListFilterResult
        {
            public IReadOnlyList<StopListPositionDisplayModel> VisibleItems { get; set; } = Array.Empty<StopListPositionDisplayModel>();
            public string StatusMessage { get; set; } = string.Empty;
        }
    }
}
