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
using GardenNookWpf.Views.MainPanel.Menu;
using GardenNookWpf.Views.Shell;
using TransferModels.Menu;

namespace GardenNookWpf.Views.Shell.Sections.Menu
{
    public partial class MenuManagementView : UserControl, IMainSectionView
    {
        private const string ApiBaseAddress = "https://localhost:7235";
        private const string ItemsAddress = ApiBaseAddress + "/api/menu/items";
        private const string EditOptionsAddress = ItemsAddress + "/edit-options";
        private const string DishType = "dishes";
        private const string DrinkType = "drinks";
        private const string ToppingType = "toppings";
        private const string AllType = "all";

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly HttpClient _httpClient;
        private readonly ObservableCollection<MenuItemViewModel> _visibleItems = new ObservableCollection<MenuItemViewModel>();
        private readonly List<MenuItemViewModel> _allItems = new List<MenuItemViewModel>();
        private readonly List<MenuItemCategoryOptionDto> _categories = new List<MenuItemCategoryOptionDto>();
        private readonly DispatcherTimer _filterReloadTimer;
        private MenuItemEditOptionsResponse _editOptions = new MenuItemEditOptionsResponse();
        private string _activeType = AllType;
        private int? _activeCategoryId;
        private string _availabilityFilter = AllType;
        private bool _isLoadedOnce;
        private bool _isBusy;
        private bool _editOptionsLoaded;

        public MenuManagementView(HttpClient httpClient, string userRole)
        {
            _httpClient = httpClient;
            InitializeComponent();

            MenuItemsList.ItemsSource = _visibleItems;
            _filterReloadTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(350)
            };
            _filterReloadTimer.Tick += FilterReloadTimer_Tick;
            HighlightTypeButtons();
        }

        public bool IsBusy => _isBusy;

        public async Task ActivateAsync()
        {
            if (_isLoadedOnce)
            {
                RenderItems();
                return;
            }

            await ReloadAsync();
        }

        public void Deactivate()
        {
        }

        private async Task ReloadAsync()
        {
            try
            {
                SetBusy(true);
                SetStatus("Загрузка меню...", false);

                if (!_editOptionsLoaded)
                {
                    await LoadEditOptionsAsync();
                }

                await LoadItemsAsync();

                _isLoadedOnce = true;
                SetStatus(string.Empty, false);
                RenderSubcategories();
                RenderItems();
            }
            catch (Exception ex)
            {
                _allItems.Clear();
                _visibleItems.Clear();
                EmptyText.Visibility = Visibility.Visible;
                SetStatus("Не удалось загрузить меню: " + ex.Message, true);
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
                throw new InvalidOperationException("нет доступа к управлению меню.");
            }

            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            _editOptions = JsonSerializer.Deserialize<MenuItemEditOptionsResponse>(json, JsonOptions)
                ?? new MenuItemEditOptionsResponse();

            _categories.Clear();
            _categories.AddRange(_editOptions.Categories ?? new List<MenuItemCategoryOptionDto>());
            _editOptionsLoaded = true;
        }

        private async Task LoadItemsAsync()
        {
            using var response = await _httpClient.GetAsync(BuildItemsAddress());
            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new InvalidOperationException("нет доступа к управлению меню.");
            }

            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<MenuItemManagementDto>>(json, JsonOptions)
                ?? new List<MenuItemManagementDto>();

            _allItems.Clear();
            _allItems.AddRange(items.Select(x => new MenuItemViewModel(x)));
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
                RenderSubcategories();
                SetStatus(string.Empty, false);
                return true;
            }
            catch (Exception ex)
            {
                SetStatus("Не удалось загрузить справочники меню: " + ex.Message, true);
                return false;
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (!await EnsureEditOptionsLoadedAsync())
            {
                return;
            }

            var defaultType = _activeType == AllType ? DishType : _activeType;
            var window = new MenuItemEditWindow(_editOptions, null, defaultType)
            {
                Owner = Window.GetWindow(this)
            };

            if (window.ShowDialog() != true)
            {
                return;
            }

            await SendUpsertAsync(HttpMethod.Post, $"{ItemsAddress}/{window.ItemType}", window.Request);
        }

        private async void EditItemButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is not MenuItemViewModel item)
            {
                return;
            }

            if (!await EnsureEditOptionsLoadedAsync())
            {
                return;
            }

            var window = new MenuItemEditWindow(_editOptions, item.Source, item.Type)
            {
                Owner = Window.GetWindow(this)
            };

            if (window.ShowDialog() != true)
            {
                return;
            }

            await SendUpsertAsync(HttpMethod.Put, $"{ItemsAddress}/{item.Type}/{item.Id}", window.Request);
        }

        private async void DeleteItemButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is not MenuItemViewModel item)
            {
                return;
            }

            var window = new ConfirmDeleteMenuItemWindow(item.Name, item.TypeDisplay)
            {
                Owner = Window.GetWindow(this)
            };

            if (window.ShowDialog() != true)
            {
                return;
            }

            await SendDeleteAsync(item);
        }

        private async Task SendUpsertAsync(HttpMethod method, string address, MenuItemUpsertRequest requestBody)
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

        private async Task SendDeleteAsync(MenuItemViewModel item)
        {
            try
            {
                SetBusy(true);
                SetStatus("Удаление позиции меню...", false);

                using var response = await _httpClient.DeleteAsync($"{ItemsAddress}/{item.Type}/{item.Id}");
                if (!response.IsSuccessStatusCode)
                {
                    SetStatus(await ReadApiMessageAsync(response), true);
                    return;
                }

                await ReloadAsync();
                SetStatus("Позиция меню удалена.", false);
            }
            catch (Exception ex)
            {
                SetStatus("Не удалось удалить позицию меню: " + ex.Message, true);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async void TypeButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is string type)
            {
                _activeType = type;
                _activeCategoryId = null;
                RenderSubcategories();
                await ReloadAsync();
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ScheduleFilterReload();
        }

        private async void AvailabilityFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AvailabilityFilterComboBox.SelectedItem is ComboBoxItem item && item.Tag is string tag)
            {
                _availabilityFilter = tag;
                await ReloadAsync();
            }
        }

        private string BuildItemsAddress()
        {
            var parameters = new List<string>
            {
                "skip=0",
                "take=100"
            };

            if (_activeType != AllType)
            {
                parameters.Add("type=" + Uri.EscapeDataString(_activeType));
            }

            if (_activeCategoryId.HasValue)
            {
                parameters.Add("categoryId=" + _activeCategoryId.Value.ToString(CultureInfo.InvariantCulture));
            }

            if (_availabilityFilter != AllType)
            {
                parameters.Add("availability=" + Uri.EscapeDataString(_availabilityFilter));
            }

            var search = (SearchTextBox?.Text ?? string.Empty).Trim();
            if (!string.IsNullOrWhiteSpace(search))
            {
                parameters.Add("search=" + Uri.EscapeDataString(search));
            }

            return ItemsAddress + "?" + string.Join("&", parameters);
        }

        private void ScheduleFilterReload()
        {
            if (!_isLoadedOnce || _filterReloadTimer == null)
            {
                return;
            }

            _filterReloadTimer.Stop();
            _filterReloadTimer.Start();
        }

        private async void FilterReloadTimer_Tick(object? sender, EventArgs e)
        {
            _filterReloadTimer.Stop();
            await ReloadAsync();
        }

        private void RenderSubcategories()
        {
            if (SubcategoriesList == null)
            {
                return;
            }

            SubcategoriesList.ItemsSource = null;
            SubcategoriesTitle.Visibility = Visibility.Collapsed;

            if (_activeType == AllType)
            {
                _activeCategoryId = null;
                return;
            }

            var categories = _categories.Count > 0
                ? _categories
                : _allItems
                    .Where(x => x.CategoryId.HasValue && !string.IsNullOrWhiteSpace(x.CategoryName))
                    .GroupBy(x => new { x.Type, x.CategoryId, x.CategoryName })
                    .Select(x => new MenuItemCategoryOptionDto
                    {
                        Type = x.Key.Type,
                        Id = x.Key.CategoryId.GetValueOrDefault(),
                        Name = x.Key.CategoryName
                    })
                    .ToList();

            var options = categories
                .Where(x => x.Type == _activeType)
                .OrderBy(x => x.Name)
                .Select(x => new CategoryFilterOption(
                    x.Id,
                    x.Name,
                    x.Id == _activeCategoryId))
                .ToList();

            if (_activeCategoryId.HasValue && options.All(x => x.Id != _activeCategoryId.Value))
            {
                _activeCategoryId = null;
            }

            if (options.Count == 0)
            {
                return;
            }

            SubcategoriesTitle.Visibility = Visibility.Visible;
            SubcategoriesList.ItemsSource = options;
        }

        private async void SubcategoryButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.DataContext is not CategoryFilterOption option)
            {
                return;
            }

            _activeCategoryId = _activeCategoryId == option.Id
                ? null
                : option.Id;
            RenderSubcategories();
            await ReloadAsync();
        }

        private void RenderItems()
        {
            if (MenuItemsList == null)
            {
                return;
            }

            _visibleItems.Clear();
            foreach (var item in _allItems
                         .OrderBy(x => GetTypeOrder(x.Type))
                         .ThenBy(x => x.Name))
            {
                _visibleItems.Add(item);
            }

            EmptyText.Visibility = _visibleItems.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            ItemsScrollViewer.Visibility = _visibleItems.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
            RenderSubcategories();
            HighlightTypeButtons();
        }

        private void HighlightTypeButtons()
        {
            foreach (var button in new[] { AllTypeButton, DishesTypeButton, DrinksTypeButton, ToppingsTypeButton })
            {
                if (button == null)
                {
                    continue;
                }

                var isActive = string.Equals(button.Tag as string, _activeType, StringComparison.OrdinalIgnoreCase);
                button.Background = isActive
                    ? (Brush)Application.Current.Resources["ModalColorPrimaryBrush"]
                    : Brushes.White;
                button.Foreground = isActive
                    ? Brushes.White
                    : (Brush)Application.Current.Resources["ModalColorTextBrush"];
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

                return trimmed;
            }

            return response.StatusCode switch
            {
                HttpStatusCode.Conflict => "Операция невозможна из-за связанных данных.",
                HttpStatusCode.NotFound => "Позиция меню не найдена.",
                HttpStatusCode.BadRequest => "Проверьте данные позиции меню.",
                _ => "Не удалось выполнить операцию."
            };
        }

        private static int GetTypeOrder(string type)
        {
            return type switch
            {
                DishType => 0,
                DrinkType => 1,
                ToppingType => 2,
                _ => 3
            };
        }

        private sealed class CategoryFilterOption
        {
            public CategoryFilterOption(int id, string name, bool isActive)
            {
                Id = id;
                Name = name;
                IsActive = isActive;
            }

            public int Id { get; }
            public string Name { get; }
            public bool IsActive { get; }
            public Brush Background => IsActive
                ? (Brush)Application.Current.Resources["ModalColorPrimaryBrush"]
                : Brushes.White;
            public Brush Foreground => IsActive
                ? Brushes.White
                : (Brush)Application.Current.Resources["ModalColorTextBrush"];
            public Brush BorderBrush => IsActive
                ? (Brush)Application.Current.Resources["ModalColorPrimaryDarkBrush"]
                : new SolidColorBrush(Color.FromRgb(183, 183, 183));
        }

        private sealed class MenuItemViewModel
        {
            public MenuItemViewModel(MenuItemManagementDto source)
            {
                Source = source;
            }

            public MenuItemManagementDto Source { get; }
            public string Type => Source.Type ?? string.Empty;
            public int Id => Source.Id;
            public string Name => Source.Name ?? string.Empty;
            public int? CategoryId => Source.CategoryId;
            public string CategoryName => Source.CategoryName ?? string.Empty;
            public string TechnicalCardName => Source.TechnicalCardName ?? string.Empty;
            public bool IsAvailable => Source.IsAvailable;

            public string TypeDisplay => Type switch
            {
                DishType => "Блюдо",
                DrinkType => "Напиток",
                ToppingType => "Добавка",
                _ => Type
            };

            public string Subtitle => string.IsNullOrWhiteSpace(CategoryName)
                ? TypeDisplay
                : $"{TypeDisplay} · {CategoryName}";

            public string PriceDisplay => $"{Source.PriceRub:0.##} ₽";

            public string DetailsDisplay
            {
                get
                {
                    var parts = new List<string>();
                    if (Type != DishType && Source.Quantity.HasValue)
                    {
                        parts.Add($"Количество: {Source.Quantity.Value:0.##} {Source.UnitName}");
                    }
                    else if (!string.IsNullOrWhiteSpace(Source.UnitName))
                    {
                        parts.Add("Ед. изм.: " + Source.UnitName);
                    }

                    parts.Add($"Ккал: {Source.CaloriesKcal:0.##} | Б: {Source.ProteinsG:0.##} | Ж: {Source.FatsG:0.##} | У: {Source.CarbsG:0.##}");
                    if (!string.IsNullOrWhiteSpace(TechnicalCardName))
                    {
                        parts.Add("Техкарта: " + TechnicalCardName);
                    }

                    if (!string.IsNullOrWhiteSpace(Source.ImageUrl))
                    {
                        parts.Add("Изображение: " + Source.ImageUrl);
                    }

                    return string.Join(Environment.NewLine, parts);
                }
            }

            public string AvailabilityDisplay => IsAvailable ? "Доступно" : "Недоступно";

            public Brush AvailabilityBrush => IsAvailable
                ? (Brush)Application.Current.Resources["ModalColorPrimaryDarkBrush"]
                : new SolidColorBrush(Color.FromRgb(116, 44, 39));

            public Brush CardBackground => IsAvailable ? Brushes.White : new SolidColorBrush(Color.FromRgb(240, 240, 240));

            public Brush CardBorderBrush => IsAvailable
                ? new SolidColorBrush(Color.FromRgb(111, 104, 100))
                : new SolidColorBrush(Color.FromRgb(184, 184, 184));
        }
    }
}
