using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TransferModels.Kitchen;

namespace GardenNookWpf.Views.Shell.Controllers
{
    public sealed class TechnicalCardsSectionController : IMainSectionController
    {
        private const string KitchenApiBaseAddress = "https://localhost:7235/api/kitchen";
        private const string TechnicalCardsAddress = KitchenApiBaseAddress + "/technical-cards";

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        private static readonly JsonSerializerOptions WriteJsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        private readonly HttpClient _httpClient;
        private readonly List<TechnicalCardListItemViewModel> _allCards = new List<TechnicalCardListItemViewModel>();
        private bool _isBusy;

        public TechnicalCardsSectionController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public event Action<bool>? BusyStateChanged;

        public bool IsBusy => _isBusy;

        public bool HasCards => _allCards.Count > 0;

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

                var response = await _httpClient.GetAsync(TechnicalCardsAddress);

                if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                {
                    _allCards.Clear();
                    return (false, "Нет доступа к техническим картам.");
                }

                if (!response.IsSuccessStatusCode)
                {
                    _allCards.Clear();
                    return (false, "Не удалось загрузить технические карты.");
                }

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<KitchenTechnicalCardsResponse>(json, JsonOptions)
                    ?? new KitchenTechnicalCardsResponse();

                var cards = data.TechnicalCards ?? new List<KitchenTechnicalCardListItemDto>();
                var displayCards = new List<TechnicalCardListItemViewModel>(cards.Count);

                foreach (var item in cards)
                {
                    var description = item.Description?.Trim() ?? string.Empty;
                    if (description.Length > 180)
                    {
                        description = description.Substring(0, 180) + "...";
                    }

                    displayCards.Add(new TechnicalCardListItemViewModel
                    {
                        TechnicalCardId = item.TechnicalCardId,
                        CardName = string.IsNullOrWhiteSpace(item.CardName)
                            ? $"Техническая карта #{item.TechnicalCardId}"
                            : item.CardName,
                        DescriptionPreview = string.IsNullOrWhiteSpace(description)
                            ? "Описание отсутствует."
                            : description
                    });
                }

                _allCards.Clear();
                _allCards.AddRange(displayCards);
                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                _allCards.Clear();
                return (false, $"Ошибка загрузки: {ex.Message}");
            }
            finally
            {
                SetBusy(false);
            }
        }

        public IReadOnlyList<TechnicalCardListItemViewModel> FilterCards(string? searchQuery)
        {
            var query = searchQuery?.Trim() ?? string.Empty;
            IEnumerable<TechnicalCardListItemViewModel> filteredCards = _allCards;

            if (!string.IsNullOrWhiteSpace(query))
            {
                filteredCards = _allCards.Where(card =>
                    card.CardName.Contains(query, StringComparison.CurrentCultureIgnoreCase));
            }

            return filteredCards.ToList();
        }

        public async Task<(bool Success, string Message, KitchenTechnicalCardResponse? Card)> LoadTechnicalCardAsync(int technicalCardId)
        {
            try
            {
                SetBusy(true);

                var response = await _httpClient.GetAsync($"{TechnicalCardsAddress}/{technicalCardId}");

                if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                {
                    return (false, "Нет доступа к деталям технической карты.", null);
                }

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return (false, "Техническая карта не найдена.", null);
                }

                if (!response.IsSuccessStatusCode)
                {
                    return (false, "Не удалось загрузить детали технической карты.", null);
                }

                var json = await response.Content.ReadAsStringAsync();
                var technicalCard = JsonSerializer.Deserialize<KitchenTechnicalCardResponse>(json, JsonOptions);
                if (technicalCard == null)
                {
                    return (false, "Не удалось обработать ответ сервера.", null);
                }

                return (true, string.Empty, technicalCard);
            }
            catch (Exception ex)
            {
                return (false, $"Ошибка загрузки деталей: {ex.Message}", null);
            }
            finally
            {
                SetBusy(false);
            }
        }

        public async Task<(bool Success, string Message, KitchenTechnicalCardEditOptionsResponse? Options)> LoadEditOptionsAsync()
        {
            try
            {
                SetBusy(true);

                var response = await _httpClient.GetAsync($"{TechnicalCardsAddress}/edit-options");
                if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                {
                    return (false, "Нет доступа к управлению техкартами.", null);
                }

                if (!response.IsSuccessStatusCode)
                {
                    return (false, await ReadErrorMessageAsync(response, "Не удалось загрузить справочники для формы."), null);
                }

                var json = await response.Content.ReadAsStringAsync();
                var options = JsonSerializer.Deserialize<KitchenTechnicalCardEditOptionsResponse>(json, JsonOptions);
                return options == null
                    ? (false, "Не удалось обработать справочники формы.", null)
                    : (true, string.Empty, options);
            }
            catch (Exception ex)
            {
                return (false, $"Ошибка загрузки справочников: {ex.Message}", null);
            }
            finally
            {
                SetBusy(false);
            }
        }

        public async Task<(bool Success, string Message, KitchenTechnicalCardEditResponse? Card)> LoadTechnicalCardForEditAsync(int technicalCardId)
        {
            try
            {
                SetBusy(true);

                var response = await _httpClient.GetAsync($"{TechnicalCardsAddress}/{technicalCardId}/edit");
                if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                {
                    return (false, "Нет доступа к редактированию техкарты.", null);
                }

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return (false, "Техкарта не найдена.", null);
                }

                if (!response.IsSuccessStatusCode)
                {
                    return (false, await ReadErrorMessageAsync(response, "Не удалось загрузить техкарту для редактирования."), null);
                }

                var json = await response.Content.ReadAsStringAsync();
                var card = JsonSerializer.Deserialize<KitchenTechnicalCardEditResponse>(json, JsonOptions);
                return card == null
                    ? (false, "Не удалось обработать данные техкарты.", null)
                    : (true, string.Empty, card);
            }
            catch (Exception ex)
            {
                return (false, $"Ошибка загрузки техкарты: {ex.Message}", null);
            }
            finally
            {
                SetBusy(false);
            }
        }

        public async Task<(bool Success, string Message)> CreateTechnicalCardAsync(KitchenTechnicalCardUpsertRequest request)
        {
            try
            {
                SetBusy(true);

                var response = await _httpClient.PostAsync(
                    TechnicalCardsAddress,
                    CreateJsonContent(request));

                if (!response.IsSuccessStatusCode)
                {
                    return (false, await ReadErrorMessageAsync(response, "Не удалось добавить техкарту."));
                }

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, $"Ошибка добавления техкарты: {ex.Message}");
            }
            finally
            {
                SetBusy(false);
            }
        }

        public async Task<(bool Success, string Message)> UpdateTechnicalCardAsync(int technicalCardId, KitchenTechnicalCardUpsertRequest request)
        {
            try
            {
                SetBusy(true);

                var response = await _httpClient.PutAsync(
                    $"{TechnicalCardsAddress}/{technicalCardId}",
                    CreateJsonContent(request));

                if (!response.IsSuccessStatusCode)
                {
                    return (false, await ReadErrorMessageAsync(response, "Не удалось обновить техкарту."));
                }

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, $"Ошибка обновления техкарты: {ex.Message}");
            }
            finally
            {
                SetBusy(false);
            }
        }

        public async Task<(bool Success, string Message)> DeleteTechnicalCardAsync(int technicalCardId)
        {
            try
            {
                SetBusy(true);

                var response = await _httpClient.DeleteAsync($"{TechnicalCardsAddress}/{technicalCardId}");
                if (!response.IsSuccessStatusCode)
                {
                    return (false, await ReadErrorMessageAsync(response, "Не удалось удалить техкарту."));
                }

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, $"Ошибка удаления техкарты: {ex.Message}");
            }
            finally
            {
                SetBusy(false);
            }
        }

        private static StringContent CreateJsonContent<T>(T value)
        {
            var json = JsonSerializer.Serialize(value, WriteJsonOptions);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        private static async Task<string> ReadErrorMessageAsync(HttpResponseMessage response, string fallback)
        {
            var text = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(text))
            {
                return fallback;
            }

            return text.Trim().Trim('"');
        }

        private void SetBusy(bool isBusy)
        {
            _isBusy = isBusy;
            BusyStateChanged?.Invoke(isBusy);
        }

        public sealed class TechnicalCardListItemViewModel
        {
            public int TechnicalCardId { get; set; }
            public string CardName { get; set; } = string.Empty;
            public string DescriptionPreview { get; set; } = string.Empty;
        }
    }
}
