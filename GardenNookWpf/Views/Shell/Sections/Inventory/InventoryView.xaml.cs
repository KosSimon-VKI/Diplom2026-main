using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using GardenNookWpf.Views.MainPanel.Inventory;
using GardenNookWpf.Views.Shell;
using TransferModels.Inventory;

namespace GardenNookWpf.Views.Shell.Sections.Inventory
{
    public partial class InventoryView : UserControl, IMainSectionView
    {
        private const string ApiBaseAddress = "https://localhost:7235";
        private const string InventoryAddress = ApiBaseAddress + "/api/inventory";
        private const string IngredientsAddress = InventoryAddress + "/ingredients";
        private const string IngredientSupplyAddress = IngredientsAddress + "/supply";
        private const string SemiFinishedAddress = InventoryAddress + "/semi-finished";
        private const string EditOptionsAddress = InventoryAddress + "/edit-options";

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly HttpClient _httpClient;
        private readonly ObservableCollection<IngredientViewModel> _ingredients = new ObservableCollection<IngredientViewModel>();
        private readonly ObservableCollection<SemiFinishedViewModel> _semiFinished = new ObservableCollection<SemiFinishedViewModel>();
        private readonly DispatcherTimer _searchTimer;
        private InventoryEditOptionsResponse _editOptions = new InventoryEditOptionsResponse();
        private bool _isLoadedOnce;
        private bool _editOptionsLoaded;
        private bool _isBusy;

        public InventoryView(HttpClient httpClient, string userRole)
        {
            _httpClient = httpClient;
            InitializeComponent();

            IngredientsList.ItemsSource = _ingredients;
            SemiFinishedList.ItemsSource = _semiFinished;

            _searchTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(350)
            };
            _searchTimer.Tick += SearchTimer_Tick;

            UpdateAddButtonText();
        }

        public bool IsBusy => _isBusy;

        public async Task ActivateAsync()
        {
            if (_isLoadedOnce)
            {
                RenderEmptyStates();
                return;
            }

            await ReloadAsync();
        }

        public void Deactivate()
        {
            _searchTimer.Stop();
        }

        private async Task ReloadAsync()
        {
            try
            {
                SetBusy(true);
                SetStatus("Загрузка сырья и полуфабрикатов...", false);

                if (!_editOptionsLoaded)
                {
                    await LoadEditOptionsAsync();
                }

                if (IsIngredientsTabActive)
                {
                    await LoadIngredientsAsync();
                }
                else
                {
                    await LoadSemiFinishedAsync();
                }

                _isLoadedOnce = true;
                SetStatus(string.Empty, false);
                RenderEmptyStates();
            }
            catch (Exception ex)
            {
                SetStatus("Не удалось загрузить данные: " + ex.Message, true);
                if (IsIngredientsTabActive)
                {
                    _ingredients.Clear();
                }
                else
                {
                    _semiFinished.Clear();
                }

                RenderEmptyStates();
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async Task LoadEditOptionsAsync()
        {
            using var response = await _httpClient.GetAsync(EditOptionsAddress);
            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new InvalidOperationException("нет доступа к справочникам сырья.");
            }

            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            _editOptions = JsonSerializer.Deserialize<InventoryEditOptionsResponse>(json, JsonOptions)
                ?? new InventoryEditOptionsResponse();
            _editOptionsLoaded = true;
        }

        private async Task<bool> EnsureEditOptionsLoadedAsync()
        {
            if (_editOptionsLoaded)
            {
                return true;
            }

            try
            {
                SetBusy(true);
                SetStatus("Загрузка справочников...", false);
                await LoadEditOptionsAsync();
                SetStatus(string.Empty, false);
                return true;
            }
            catch (Exception ex)
            {
                SetStatus("Не удалось загрузить справочники: " + ex.Message, true);
                return false;
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async Task LoadIngredientsAsync()
        {
            using var response = await _httpClient.GetAsync(BuildAddress(IngredientsAddress));
            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new InvalidOperationException("нет доступа к сырью.");
            }

            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<InventoryIngredientDto>>(json, JsonOptions)
                ?? new List<InventoryIngredientDto>();

            _ingredients.Clear();
            foreach (var item in items.Select(x => new IngredientViewModel(x)))
            {
                _ingredients.Add(item);
            }
        }

        private async Task LoadSemiFinishedAsync()
        {
            using var response = await _httpClient.GetAsync(BuildAddress(SemiFinishedAddress));
            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new InvalidOperationException("нет доступа к полуфабрикатам.");
            }

            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<InventorySemiFinishedDto>>(json, JsonOptions)
                ?? new List<InventorySemiFinishedDto>();

            _semiFinished.Clear();
            foreach (var item in items.Select(x => new SemiFinishedViewModel(x)))
            {
                _semiFinished.Add(item);
            }
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (!await EnsureEditOptionsLoadedAsync())
            {
                return;
            }

            if (IsIngredientsTabActive)
            {
                var window = new IngredientEditWindow(_editOptions, null)
                {
                    Owner = Window.GetWindow(this)
                };

                if (window.ShowDialog() == true)
                {
                    await SendUpsertAsync(HttpMethod.Post, IngredientsAddress, window.Request);
                }
            }
            else
            {
                var window = new SemiFinishedEditWindow(_editOptions, null)
                {
                    Owner = Window.GetWindow(this)
                };

                if (window.ShowDialog() == true)
                {
                    await SendUpsertAsync(HttpMethod.Post, SemiFinishedAddress, window.Request);
                }
            }
        }

        private async void SupplyButton_Click(object sender, RoutedEventArgs e)
        {
            var ingredients = await LoadIngredientOptionsForSupplyAsync();
            if (ingredients.Count == 0)
            {
                SetStatus("Сначала добавьте сырье.", true);
                return;
            }

            var window = new SupplyIngredientWindow(ingredients)
            {
                Owner = Window.GetWindow(this)
            };

            if (window.ShowDialog() == true)
            {
                await SendSupplyAsync(window.Request);
            }
        }

        private async void EditIngredientButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is not IngredientViewModel item ||
                !await EnsureEditOptionsLoadedAsync())
            {
                return;
            }

            var window = new IngredientEditWindow(_editOptions, item.Source)
            {
                Owner = Window.GetWindow(this)
            };

            if (window.ShowDialog() == true)
            {
                await SendUpsertAsync(HttpMethod.Put, $"{IngredientsAddress}/{item.Id}", window.Request);
            }
        }

        private async void EditSemiFinishedButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is not SemiFinishedViewModel item ||
                !await EnsureEditOptionsLoadedAsync())
            {
                return;
            }

            var window = new SemiFinishedEditWindow(_editOptions, item.Source)
            {
                Owner = Window.GetWindow(this)
            };

            if (window.ShowDialog() == true)
            {
                await SendUpsertAsync(HttpMethod.Put, $"{SemiFinishedAddress}/{item.Id}", window.Request);
            }
        }

        private async void DeleteIngredientButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is not IngredientViewModel item)
            {
                return;
            }

            var window = new ConfirmDeleteIngredientWindow(item.Name, item.DetailsDisplay)
            {
                Owner = Window.GetWindow(this)
            };

            if (window.ShowDialog() == true)
            {
                await SendDeleteAsync($"{IngredientsAddress}/{item.Id}", "Сырье удалено.");
            }
        }

        private async void DeleteSemiFinishedButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is not SemiFinishedViewModel item)
            {
                return;
            }

            var window = new ConfirmDeleteSemiFinishedWindow(item.Name, item.DetailsDisplay)
            {
                Owner = Window.GetWindow(this)
            };

            if (window.ShowDialog() == true)
            {
                await SendDeleteAsync($"{SemiFinishedAddress}/{item.Id}", "Полуфабрикат удален.");
            }
        }

        private async Task SendUpsertAsync<TRequest>(HttpMethod method, string address, TRequest requestBody)
        {
            try
            {
                SetBusy(true);
                SetStatus("Сохранение изменений...", false);

                using var request = new HttpRequestMessage(method, address)
                {
                    Content = JsonContent.Create(requestBody)
                };

                using var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    SetStatus(await ReadApiMessageAsync(response), true);
                    return;
                }

                await ReloadAsync();
                SetStatus("Изменения сохранены.", false);
            }
            catch (Exception ex)
            {
                SetStatus("Не удалось сохранить изменения: " + ex.Message, true);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async Task SendDeleteAsync(string address, string successMessage)
        {
            try
            {
                SetBusy(true);
                SetStatus("Удаление позиции...", false);

                using var response = await _httpClient.DeleteAsync(address);
                if (!response.IsSuccessStatusCode)
                {
                    SetStatus(await ReadApiMessageAsync(response), true);
                    return;
                }

                await ReloadAsync();
                SetStatus(successMessage, false);
            }
            catch (Exception ex)
            {
                SetStatus("Не удалось удалить позицию: " + ex.Message, true);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async Task SendSupplyAsync(InventoryIngredientSupplyRequest requestBody)
        {
            try
            {
                SetBusy(true);
                SetStatus("Сохранение поставки...", false);

                using var response = await _httpClient.PostAsJsonAsync(IngredientSupplyAddress, requestBody);
                if (!response.IsSuccessStatusCode)
                {
                    SetStatus(await ReadApiMessageAsync(response), true);
                    return;
                }

                await LoadIngredientsAsync();
                RenderEmptyStates();
                SetStatus("Поставка добавлена.", false);
            }
            catch (Exception ex)
            {
                SetStatus("Не удалось сохранить поставку: " + ex.Message, true);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async Task<List<InventoryIngredientDto>> LoadIngredientOptionsForSupplyAsync()
        {
            try
            {
                SetBusy(true);
                SetStatus("Загрузка сырья...", false);

                using var response = await _httpClient.GetAsync(IngredientsAddress);
                if (!response.IsSuccessStatusCode)
                {
                    SetStatus(await ReadApiMessageAsync(response), true);
                    return new List<InventoryIngredientDto>();
                }

                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<InventoryIngredientDto>>(json, JsonOptions)
                    ?? new List<InventoryIngredientDto>();
            }
            catch (Exception ex)
            {
                SetStatus("Не удалось загрузить сырье: " + ex.Message, true);
                return new List<InventoryIngredientDto>();
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async void InventoryTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source != InventoryTabs)
            {
                return;
            }

            UpdateAddButtonText();
            if (_isLoadedOnce)
            {
                await ReloadAsync();
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isLoadedOnce)
            {
                return;
            }

            _searchTimer.Stop();
            _searchTimer.Start();
        }

        private async void SearchTimer_Tick(object? sender, EventArgs e)
        {
            _searchTimer.Stop();
            await ReloadAsync();
        }

        private string BuildAddress(string baseAddress)
        {
            var search = (SearchTextBox?.Text ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(search))
            {
                return baseAddress;
            }

            return baseAddress + "?search=" + Uri.EscapeDataString(search);
        }

        private void RenderEmptyStates()
        {
            EmptyIngredientsText.Visibility = _ingredients.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            IngredientsScrollViewer.Visibility = _ingredients.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
            EmptySemiFinishedText.Visibility = _semiFinished.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            SemiFinishedScrollViewer.Visibility = _semiFinished.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        private void UpdateAddButtonText()
        {
            if (AddButton != null)
            {
                AddButton.Content = IsIngredientsTabActive ? "Добавить сырье" : "Добавить полуфабрикат";
                AddButton.Width = IsIngredientsTabActive ? 210 : 270;
            }

            if (SupplyButton != null)
            {
                SupplyButton.Visibility = IsIngredientsTabActive ? Visibility.Visible : Visibility.Collapsed;
            }

        }

        private void SetBusy(bool isBusy)
        {
            _isBusy = isBusy;
            RootGrid.IsEnabled = !isBusy;
        }

        private void SetStatus(string message, bool isError)
        {
            StatusText.Text = message;
            StatusText.Foreground = isError
                ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF742C27"))
                : (Brush)Application.Current.Resources["ModalColorPrimaryDarkBrush"];
            StatusText.Visibility = string.IsNullOrWhiteSpace(message)
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        private bool IsIngredientsTabActive => InventoryTabs?.SelectedIndex != 1;

        private static async Task<string> ReadApiMessageAsync(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            if (!string.IsNullOrWhiteSpace(content))
            {
                var trimmed = content.Trim();
                if (trimmed.StartsWith("\"") && trimmed.EndsWith("\""))
                {
                    try
                    {
                        return JsonSerializer.Deserialize<string>(trimmed) ?? trimmed.Trim('"');
                    }
                    catch
                    {
                        return trimmed.Trim('"');
                    }
                }

                try
                {
                    using var document = JsonDocument.Parse(trimmed);
                    if (document.RootElement.TryGetProperty("title", out var title))
                    {
                        return title.GetString() ?? string.Empty;
                    }

                    if (document.RootElement.TryGetProperty("detail", out var detail))
                    {
                        return detail.GetString() ?? string.Empty;
                    }
                }
                catch
                {
                }

                return trimmed;
            }

            return response.StatusCode switch
            {
                HttpStatusCode.Conflict => "Операция невозможна из-за связанных данных.",
                HttpStatusCode.NotFound => "Позиция не найдена.",
                HttpStatusCode.BadRequest => "Проверьте данные позиции.",
                _ => "Не удалось выполнить операцию."
            };
        }

        private static string FormatDecimal(decimal value)
        {
            return value.ToString("0.##", CultureInfo.InvariantCulture);
        }

        private sealed class IngredientViewModel
        {
            public IngredientViewModel(InventoryIngredientDto source)
            {
                Source = source;
            }

            public InventoryIngredientDto Source { get; }
            public int Id => Source.Id;
            public string Name => Source.Name ?? string.Empty;
            public string UnitName => Source.UnitName ?? string.Empty;
            public string CategoryName => Source.CategoryName ?? string.Empty;
            public string Subtitle => string.IsNullOrWhiteSpace(CategoryName) ? "Без категории" : CategoryName;
            public string CostDisplay => FormatDecimal(Source.CostRub) + " ₽";
            public string DetailsDisplay
            {
                get
                {
                    var unit = string.IsNullOrWhiteSpace(UnitName) ? string.Empty : " " + UnitName;
                    var parts = new List<string>
                    {
                        "Остаток: " + FormatDecimal(Source.Stock) + unit
                    };

                    if (!string.IsNullOrWhiteSpace(UnitName))
                    {
                        parts.Add("Ед. изм.: " + UnitName);
                    }

                    return string.Join(Environment.NewLine, parts);
                }
            }
        }

        private sealed class SemiFinishedViewModel
        {
            public SemiFinishedViewModel(InventorySemiFinishedDto source)
            {
                Source = source;
            }

            public InventorySemiFinishedDto Source { get; }
            public int Id => Source.Id;
            public string Name => Source.Name ?? string.Empty;
            public string UnitName => Source.UnitName ?? string.Empty;
            public string CategoryName => Source.CategoryName ?? string.Empty;
            public string TechnicalCardName => Source.TechnicalCardName ?? string.Empty;
            public string Subtitle => string.IsNullOrWhiteSpace(CategoryName) ? "Без категории" : CategoryName;
            public string CostDisplay => FormatDecimal(Source.CostRub) + " ₽";
            public string DetailsDisplay
            {
                get
                {
                    var parts = new List<string>();
                    if (!string.IsNullOrWhiteSpace(UnitName))
                    {
                        parts.Add("Ед. изм.: " + UnitName);
                    }

                    if (!string.IsNullOrWhiteSpace(TechnicalCardName))
                    {
                        parts.Add("Техкарта: " + TechnicalCardName);
                    }

                    parts.Add($"Ккал: {FormatDecimal(Source.CaloriesKcal)} | Б: {FormatDecimal(Source.ProteinsG)} | Ж: {FormatDecimal(Source.FatsG)} | У: {FormatDecimal(Source.CarbsG)} | кДж: {FormatDecimal(Source.Kilojoules)}");
                    return string.Join(Environment.NewLine, parts);
                }
            }
        }
    }
}
