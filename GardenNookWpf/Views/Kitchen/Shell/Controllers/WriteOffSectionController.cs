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
    public sealed class WriteOffSectionController : IKitchenSectionController
    {
        private const string KitchenApiBaseAddress = "https://localhost:7235/api/kitchen";
        private const string WriteOffBoardAddress = KitchenApiBaseAddress + "/write-off/board";
        private const string WriteOffActsAddress = KitchenApiBaseAddress + "/write-off/acts";
        private const string AdminRole = "Администратор";

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly HttpClient _httpClient;
        private readonly bool _isAdmin;
        private bool _isBusy;
        private List<KitchenWriteOffTypeDto> _writeOffTypes = new List<KitchenWriteOffTypeDto>();
        private List<KitchenWriteOffSemiFinishedOptionDto> _semiFinishedOptions = new List<KitchenWriteOffSemiFinishedOptionDto>();
        private List<KitchenWriteOffIngredientOptionDto> _ingredientOptions = new List<KitchenWriteOffIngredientOptionDto>();

        public WriteOffSectionController(HttpClient httpClient, string userRole)
        {
            _httpClient = httpClient;
            _isAdmin = string.Equals(userRole, AdminRole, StringComparison.CurrentCultureIgnoreCase);
        }

        public event Action<bool>? BusyStateChanged;

        public bool IsBusy => _isBusy;

        public IReadOnlyList<KitchenWriteOffTypeDto> WriteOffTypes => _writeOffTypes;

        public IReadOnlyList<KitchenWriteOffSemiFinishedOptionDto> SemiFinishedOptions => _semiFinishedOptions;

        public IReadOnlyList<KitchenWriteOffIngredientOptionDto> IngredientOptions => _ingredientOptions;

        public IReadOnlyList<WriteOffActDisplayModel> HistoryItems { get; private set; } = Array.Empty<WriteOffActDisplayModel>();

        public async Task ActivateAsync()
        {
            await LoadBoardAsync();
        }

        public void Deactivate()
        {
        }

        public async Task<(bool Success, string Message)> LoadBoardAsync()
        {
            try
            {
                SetBusy(true);

                var response = await _httpClient.GetAsync(WriteOffBoardAddress);
                if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                {
                    ClearBoard();
                    return (false, "Нет доступа к актам списания.");
                }

                if (!response.IsSuccessStatusCode)
                {
                    ClearBoard();
                    var error = await ReadErrorMessageAsync(response);
                    return (false, string.IsNullOrWhiteSpace(error)
                        ? "Не удалось загрузить акты списания."
                        : error);
                }

                var json = await response.Content.ReadAsStringAsync();
                var board = JsonSerializer.Deserialize<KitchenWriteOffBoardResponse>(json, JsonOptions)
                    ?? new KitchenWriteOffBoardResponse();

                _writeOffTypes = (board.WriteOffTypes ?? new List<KitchenWriteOffTypeDto>())
                    .OrderBy(x => x.WriteOffTypeId)
                    .ToList();

                _semiFinishedOptions = (board.SemiFinishedOptions ?? new List<KitchenWriteOffSemiFinishedOptionDto>())
                    .OrderBy(x => x.Name)
                    .ThenBy(x => x.SemiFinishedId)
                    .ToList();

                _ingredientOptions = (board.IngredientOptions ?? new List<KitchenWriteOffIngredientOptionDto>())
                    .OrderBy(x => x.Name)
                    .ThenBy(x => x.IngredientId)
                    .ToList();

                HistoryItems = (board.Acts ?? new List<KitchenWriteOffActDto>())
                    .OrderByDescending(x => x.Date)
                    .ThenByDescending(x => x.ActId)
                    .Select(MapToDisplayModel)
                    .ToList();

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                ClearBoard();
                return (false, $"Ошибка загрузки: {ex.Message}");
            }
            finally
            {
                SetBusy(false);
            }
        }

        public async Task<(bool Success, string Message)> CreateActAsync(KitchenCreateWriteOffActRequest request)
        {
            try
            {
                SetBusy(true);

                var payload = JsonSerializer.Serialize(request ?? new KitchenCreateWriteOffActRequest());
                using var content = new StringContent(payload, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(WriteOffActsAddress, content);

                if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                {
                    return (false, "Нет доступа к созданию акта списания.");
                }

                if (!response.IsSuccessStatusCode)
                {
                    var error = await ReadErrorMessageAsync(response);
                    return (false, string.IsNullOrWhiteSpace(error)
                        ? "Не удалось создать акт списания."
                        : error);
                }

                return await LoadBoardAsync();
            }
            catch (Exception ex)
            {
                return (false, $"Ошибка создания акта: {ex.Message}");
            }
            finally
            {
                SetBusy(false);
            }
        }

        public async Task<(bool Success, string Message)> DeleteActAsync(int actId)
        {
            try
            {
                SetBusy(true);

                var response = await _httpClient.DeleteAsync($"{WriteOffActsAddress}/{actId}");

                if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                {
                    return (false, "Нет доступа к удалению акта списания.");
                }

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return (false, "Акт списания не найден.");
                }

                if (!response.IsSuccessStatusCode)
                {
                    var error = await ReadErrorMessageAsync(response);
                    return (false, string.IsNullOrWhiteSpace(error)
                        ? "Не удалось удалить акт списания."
                        : error);
                }

                return await LoadBoardAsync();
            }
            catch (Exception ex)
            {
                return (false, $"Ошибка удаления акта: {ex.Message}");
            }
            finally
            {
                SetBusy(false);
            }
        }


        public WriteOffFilterResult Filter(string? queryText)
        {
            var query = (queryText ?? string.Empty).Trim();
            IEnumerable<WriteOffActDisplayModel> source = HistoryItems;

            if (!string.IsNullOrWhiteSpace(query))
            {
                source = HistoryItems.Where(x =>
                    x.HeaderDisplay.Contains(query, StringComparison.CurrentCultureIgnoreCase) ||
                    x.CommentDisplay.Contains(query, StringComparison.CurrentCultureIgnoreCase) ||
                    x.StaffDisplay.Contains(query, StringComparison.CurrentCultureIgnoreCase) ||
                    x.LinesSearchText.Contains(query, StringComparison.CurrentCultureIgnoreCase));
            }

            var visibleItems = source
                .OrderByDescending(x => x.Date)
                .ThenByDescending(x => x.ActId)
                .ToList();

            var statusMessage = HistoryItems.Count == 0
                ? "История списаний пока пуста."
                : visibleItems.Count == 0
                    ? "По вашему запросу ничего не найдено."
                    : string.Empty;

            return new WriteOffFilterResult
            {
                VisibleItems = visibleItems,
                StatusMessage = statusMessage
            };
        }

        private void ClearBoard()
        {
            _writeOffTypes.Clear();
            _semiFinishedOptions.Clear();
            _ingredientOptions.Clear();
            HistoryItems = Array.Empty<WriteOffActDisplayModel>();
        }

        private WriteOffActDisplayModel MapToDisplayModel(KitchenWriteOffActDto source)
        {
            var ingredientLines = (source.IngredientLines ?? new List<KitchenWriteOffActLineDto>())
                .Select(x => MapLine("Сырье", x))
                .ToList();

            var semiFinishedLines = (source.SemiFinishedLines ?? new List<KitchenWriteOffActLineDto>())
                .Select(x => MapLine("ПФ", x))
                .ToList();

            var allLines = ingredientLines.Concat(semiFinishedLines).ToList();
            if (allLines.Count == 0)
            {
                allLines.Add(new WriteOffActLineDisplayModel
                {
                    Display = "Состав акта пуст"
                });
            }

            var commentDisplay = string.IsNullOrWhiteSpace(source.Comment)
                ? "Комментарий: —"
                : $"Комментарий: {source.Comment.Trim()}";

            var staffDisplay = !string.IsNullOrWhiteSpace(source.StaffFullName)
                ? $"Сотрудник: {source.StaffFullName.Trim()}"
                : "Сотрудник: —";

            return new WriteOffActDisplayModel
            {
                ActId = source.ActId,
                Date = source.Date,
                HeaderDisplay = $"Акт #{source.ActId} от {source.Date.ToString("dd.MM.yyyy", CultureInfo.CurrentCulture)}",
                CommentDisplay = commentDisplay,
                StaffDisplay = staffDisplay,
                Lines = allLines,
                LinesSearchText = string.Join(" ", allLines.Select(x => x.Display)),
                DeleteButtonVisibility = _isAdmin
                    ? System.Windows.Visibility.Visible
                    : System.Windows.Visibility.Collapsed
            };
        }

        private static WriteOffActLineDisplayModel MapLine(string kindDisplay, KitchenWriteOffActLineDto source)
        {
            var itemName = string.IsNullOrWhiteSpace(source.ItemName)
                ? $"Позиция #{source.ItemId}"
                : source.ItemName.Trim();

            var unitName = string.IsNullOrWhiteSpace(source.UnitName)
                ? string.Empty
                : source.UnitName.Trim();

            var quantityText = decimal.Round(source.Quantity, 2, MidpointRounding.AwayFromZero)
                .ToString("0.##", CultureInfo.CurrentCulture);
            var quantityDisplay = string.IsNullOrWhiteSpace(unitName)
                ? quantityText
                : $"{quantityText} {unitName}";

            var typeName = string.IsNullOrWhiteSpace(source.WriteOffTypeName)
                ? "тип не указан"
                : source.WriteOffTypeName.Trim();

            return new WriteOffActLineDisplayModel
            {
                Display = $"{kindDisplay}: {itemName} - {quantityDisplay} ({typeName})"
            };
        }

        private static async Task<string> ReadErrorMessageAsync(HttpResponseMessage response)
        {
            var payload = await response.Content.ReadAsStringAsync();
            return string.IsNullOrWhiteSpace(payload)
                ? response.ReasonPhrase ?? string.Empty
                : payload;
        }

        private void SetBusy(bool isBusy)
        {
            _isBusy = isBusy;
            BusyStateChanged?.Invoke(isBusy);
        }

        public sealed class WriteOffFilterResult
        {
            public IReadOnlyList<WriteOffActDisplayModel> VisibleItems { get; set; } = Array.Empty<WriteOffActDisplayModel>();
            public string StatusMessage { get; set; } = string.Empty;
        }

        public sealed class WriteOffActDisplayModel
        {
            public int ActId { get; set; }
            public DateTime Date { get; set; }
            public string HeaderDisplay { get; set; } = string.Empty;
            public string CommentDisplay { get; set; } = string.Empty;
            public string StaffDisplay { get; set; } = string.Empty;
            public string LinesSearchText { get; set; } = string.Empty;
            public System.Windows.Visibility DeleteButtonVisibility { get; set; }
            public IReadOnlyList<WriteOffActLineDisplayModel> Lines { get; set; } = Array.Empty<WriteOffActLineDisplayModel>();
        }

        public sealed class WriteOffActLineDisplayModel
        {
            public string Display { get; set; } = string.Empty;
        }
    }
}
