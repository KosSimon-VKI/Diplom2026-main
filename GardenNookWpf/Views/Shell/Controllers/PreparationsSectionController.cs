using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using TransferModels.Kitchen;

namespace GardenNookWpf.Views.Shell.Controllers
{
    public sealed class PreparationsSectionController : IMainSectionController
    {
        private const string KitchenApiBaseAddress = "https://localhost:7235/api/kitchen";
        private const string PreparationsAddress = KitchenApiBaseAddress + "/preparations";
        private const string PreparationTasksAddress = PreparationsAddress + "/tasks";
        private const string TechnicalCardsAddress = KitchenApiBaseAddress + "/technical-cards";
        private const string AdminRole = "Администратор";
        private const int WarningDays = 7;
        private const int CriticalDays = 14;

        private static readonly Brush DefaultPreparationBorderBrush = CreateFrozenBrush(Color.FromRgb(0x24, 0x24, 0x24));
        private static readonly Brush WarningPreparationBorderBrush = CreateFrozenBrush(Color.FromRgb(0xE1, 0xB5, 0x00));
        private static readonly Brush CriticalPreparationBorderBrush = CreateFrozenBrush(Color.FromRgb(0xC6, 0x28, 0x28));

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly HttpClient _httpClient;
        private readonly bool _isAdmin;
        private bool _isBusy;
        private List<KitchenSemiFinishedOptionDto> _semiFinishedOptions = new List<KitchenSemiFinishedOptionDto>();

        public PreparationsSectionController(HttpClient httpClient, string userRole)
        {
            _httpClient = httpClient;
            _isAdmin = string.Equals(userRole, AdminRole, StringComparison.CurrentCultureIgnoreCase);
        }

        public event Action<bool>? BusyStateChanged;

        public bool IsBusy => _isBusy;

        public IReadOnlyList<KitchenSemiFinishedOptionDto> SemiFinishedOptions => _semiFinishedOptions;

        public IReadOnlyList<PreparationTaskDisplayModel> TaskItems { get; private set; } = Array.Empty<PreparationTaskDisplayModel>();

        public IReadOnlyList<ExistingPreparationDisplayModel> ExistingItems { get; private set; } = Array.Empty<ExistingPreparationDisplayModel>();

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

                var response = await _httpClient.GetAsync(PreparationsAddress);

                if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                {
                    ClearBoard();
                    return (false, "Нет доступа к данным заготовок.");
                }

                if (!response.IsSuccessStatusCode)
                {
                    ClearBoard();
                    return (false, "Не удалось загрузить данные заготовок.");
                }

                var json = await response.Content.ReadAsStringAsync();
                var board = JsonSerializer.Deserialize<KitchenPreparationsBoardResponse>(json, JsonOptions)
                    ?? new KitchenPreparationsBoardResponse();

                _semiFinishedOptions = (board.SemiFinishedOptions ?? new List<KitchenSemiFinishedOptionDto>())
                    .OrderBy(x => x.Name)
                    .ThenBy(x => x.SemiFinishedId)
                    .ToList();

                TaskItems = (board.Tasks ?? new List<KitchenPreparationTaskDto>())
                    .Select(x =>
                    {
                        var isLinked = x.IsLinkedToSemiFinished || x.SemiFinishedId.HasValue;
                        var semiFinishedName = string.IsNullOrWhiteSpace(x.SemiFinishedName)
                            ? (x.SemiFinishedId.HasValue ? $"SemiFinished #{x.SemiFinishedId.Value}" : string.Empty)
                            : x.SemiFinishedName;

                        var taskText = string.IsNullOrWhiteSpace(x.TaskText)
                            ? (isLinked && !string.IsNullOrWhiteSpace(semiFinishedName)
                                ? semiFinishedName
                                : $"Задача #{x.TaskId}")
                            : x.TaskText;

                        return new PreparationTaskDisplayModel
                        {
                            TaskId = x.TaskId,
                            TechnicalCardId = x.TechnicalCardId,
                            IsLinkedToSemiFinished = isLinked,
                            CompleteButtonVisibility = _isAdmin
                                ? Visibility.Collapsed
                                : Visibility.Visible,
                            DeleteButtonVisibility = !_isAdmin && isLinked
                                ? Visibility.Visible
                                : Visibility.Collapsed,
                            TaskText = taskText,
                            SemiFinishedName = semiFinishedName,
                            SemiFinishedDisplay = isLinked
                                ? $"Полуфабрикат: {semiFinishedName}"
                                : string.Empty,
                            SemiFinishedVisibility = isLinked
                                ? Visibility.Visible
                                : Visibility.Collapsed,
                            CommentDisplay = string.IsNullOrWhiteSpace(x.Comment)
                                ? "Комментарий: —"
                                : $"Комментарий: {x.Comment}",
                            CreatedAtText = $"Создано: {x.CreatedAt.ToString("dd.MM.yyyy HH:mm", CultureInfo.CurrentCulture)}"
                        };
                    })
                    .ToList();

                ExistingItems = (board.ExistingPreparations ?? new List<KitchenPreparationListItemDto>())
                    .Select(x => new ExistingPreparationDisplayModel
                    {
                        PreparationId = x.PreparationId,
                        PreparationName = string.IsNullOrWhiteSpace(x.PreparationName)
                            ? $"Preparation #{x.PreparationId}"
                            : x.PreparationName,
                        StockText = $"Масса: {decimal.Round(x.StockGrams, 2, MidpointRounding.AwayFromZero).ToString("0.##", CultureInfo.CurrentCulture)} г",
                        ProductionDateText = x.ProductionDate.HasValue
                            ? $"Дата изготовления: {x.ProductionDate.Value.ToString("dd.MM.yyyy", CultureInfo.CurrentCulture)}"
                            : "Дата изготовления: —",
                        BorderBrush = ResolvePreparationBorderBrush(x.ProductionDate)
                    })
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

        public async Task<(bool Success, string Message)> CreatePreparationTaskAsync(string taskText, int? semiFinishedId, string? comment)
        {
            try
            {
                SetBusy(true);

                var payload = JsonSerializer.Serialize(new KitchenCreatePreparationTaskRequest
                {
                    TaskText = taskText,
                    SemiFinishedId = semiFinishedId,
                    Comment = comment ?? string.Empty
                });

                using var content = new StringContent(payload, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(PreparationTasksAddress, content);

                if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                {
                    return (false, "Нет доступа к добавлению задач.");
                }

                if (!response.IsSuccessStatusCode)
                {
                    var error = await ReadErrorMessageAsync(response);
                    return (false, string.IsNullOrWhiteSpace(error)
                        ? "Не удалось добавить задачу."
                        : error);
                }

                return await LoadBoardAsync();
            }
            catch (Exception ex)
            {
                return (false, $"Ошибка добавления: {ex.Message}");
            }
            finally
            {
                SetBusy(false);
            }
        }

        public async Task<(bool Success, string Message)> CompletePreparationTaskAsync(int taskId, decimal stockGrams, DateTime productionDate)
        {
            try
            {
                SetBusy(true);

                var payload = JsonSerializer.Serialize(new KitchenCompletePreparationTaskRequest
                {
                    StockGrams = stockGrams,
                    ProductionDate = productionDate
                });

                using var content = new StringContent(payload, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{PreparationTasksAddress}/{taskId}/complete", content);

                if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                {
                    return (false, "Нет доступа к обновлению задач.");
                }

                if (!response.IsSuccessStatusCode)
                {
                    var error = await ReadErrorMessageAsync(response);
                    return (false, string.IsNullOrWhiteSpace(error)
                        ? "Не удалось завершить задачу."
                        : error);
                }

                return await LoadBoardAsync();
            }
            catch (Exception ex)
            {
                return (false, $"Ошибка завершения: {ex.Message}");
            }
            finally
            {
                SetBusy(false);
            }
        }

        public async Task<(bool Success, string Message)> CompleteTodoTaskAsync(int taskId)
        {
            try
            {
                SetBusy(true);

                using var content = new StringContent("{}", Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{PreparationTasksAddress}/{taskId}/complete", content);

                if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                {
                    return (false, "Нет доступа к обновлению задач.");
                }

                if (!response.IsSuccessStatusCode)
                {
                    var error = await ReadErrorMessageAsync(response);
                    return (false, string.IsNullOrWhiteSpace(error)
                        ? "Не удалось завершить задачу."
                        : error);
                }

                return await LoadBoardAsync();
            }
            catch (Exception ex)
            {
                return (false, $"Ошибка завершения: {ex.Message}");
            }
            finally
            {
                SetBusy(false);
            }
        }

        public async Task<(bool Success, string Message)> DeletePreparationTaskAsync(int taskId)
        {
            try
            {
                SetBusy(true);

                var response = await _httpClient.DeleteAsync($"{PreparationTasksAddress}/{taskId}");

                if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                {
                    return (false, "Нет доступа к удалению задач.");
                }

                if (!response.IsSuccessStatusCode)
                {
                    var error = await ReadErrorMessageAsync(response);
                    return (false, string.IsNullOrWhiteSpace(error)
                        ? "Не удалось удалить задачу."
                        : error);
                }

                return await LoadBoardAsync();
            }
            catch (Exception ex)
            {
                return (false, $"Ошибка удаления: {ex.Message}");
            }
            finally
            {
                SetBusy(false);
            }
        }

        public async Task<(bool Success, string Message)> DeleteExistingPreparationAsync(int preparationId)
        {
            try
            {
                SetBusy(true);

                var response = await _httpClient.DeleteAsync($"{PreparationsAddress}/{preparationId}");

                if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                {
                    return (false, "Нет доступа к удалению заготовок.");
                }

                if (!response.IsSuccessStatusCode)
                {
                    var error = await ReadErrorMessageAsync(response);
                    return (false, string.IsNullOrWhiteSpace(error)
                        ? "Не удалось удалить заготовку."
                        : error);
                }

                return await LoadBoardAsync();
            }
            catch (Exception ex)
            {
                return (false, $"Ошибка удаления: {ex.Message}");
            }
            finally
            {
                SetBusy(false);
            }
        }

        public async Task<(bool Success, string Message, KitchenTechnicalCardResponse? Card)> LoadTechnicalCardAsync(int technicalCardId, string fallbackName)
        {
            try
            {
                SetBusy(true);

                var response = await _httpClient.GetAsync($"{TechnicalCardsAddress}/{technicalCardId}");

                if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                {
                    return (false, "Нет доступа к тех. карте.", null);
                }

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return (false, "Тех. карта не найдена.", null);
                }

                if (!response.IsSuccessStatusCode)
                {
                    return (false, "Не удалось загрузить тех. карту.", null);
                }

                var json = await response.Content.ReadAsStringAsync();
                var technicalCard = JsonSerializer.Deserialize<KitchenTechnicalCardResponse>(json, JsonOptions);
                if (technicalCard == null)
                {
                    return (false, "Не удалось обработать данные тех. карты.", null);
                }

                if (string.IsNullOrWhiteSpace(technicalCard.CardName))
                {
                    technicalCard.CardName = fallbackName;
                }

                return (true, string.Empty, technicalCard);
            }
            catch (Exception ex)
            {
                return (false, $"Ошибка загрузки тех. карты: {ex.Message}", null);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void ClearBoard()
        {
            _semiFinishedOptions.Clear();
            TaskItems = Array.Empty<PreparationTaskDisplayModel>();
            ExistingItems = Array.Empty<ExistingPreparationDisplayModel>();
        }

        private static Brush ResolvePreparationBorderBrush(DateTime? productionDate)
        {
            if (!productionDate.HasValue)
            {
                return DefaultPreparationBorderBrush;
            }

            var ageDays = (DateTime.Today - productionDate.Value.Date).TotalDays;
            if (ageDays > CriticalDays)
            {
                return CriticalPreparationBorderBrush;
            }

            if (ageDays > WarningDays)
            {
                return WarningPreparationBorderBrush;
            }

            return DefaultPreparationBorderBrush;
        }

        private static Brush CreateFrozenBrush(Color color)
        {
            var brush = new SolidColorBrush(color);
            brush.Freeze();
            return brush;
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

        public sealed class PreparationTaskDisplayModel
        {
            public int TaskId { get; set; }
            public int? TechnicalCardId { get; set; }
            public bool IsLinkedToSemiFinished { get; set; }
            public Visibility CompleteButtonVisibility { get; set; }
            public Visibility DeleteButtonVisibility { get; set; }
            public string TaskText { get; set; } = string.Empty;
            public string SemiFinishedName { get; set; } = string.Empty;
            public string SemiFinishedDisplay { get; set; } = string.Empty;
            public Visibility SemiFinishedVisibility { get; set; }
            public string CommentDisplay { get; set; } = string.Empty;
            public string CreatedAtText { get; set; } = string.Empty;
        }

        public sealed class ExistingPreparationDisplayModel
        {
            public int PreparationId { get; set; }
            public string PreparationName { get; set; } = string.Empty;
            public string StockText { get; set; } = string.Empty;
            public string ProductionDateText { get; set; } = string.Empty;
            public Brush BorderBrush { get; set; } = DefaultPreparationBorderBrush;
        }
    }
}
