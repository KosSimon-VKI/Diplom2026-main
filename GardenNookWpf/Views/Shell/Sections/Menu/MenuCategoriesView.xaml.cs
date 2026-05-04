using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GardenNookWpf.Views.MainPanel.Menu;
using GardenNookWpf.Views.Shell;
using TransferModels.Menu;

namespace GardenNookWpf.Views.Shell.Sections.Menu
{
    public partial class MenuCategoriesView : UserControl, IMainSectionView
    {
        private const string ApiBaseAddress = "https://localhost:7235";
        private const string CategoriesAddress = ApiBaseAddress + "/api/menu/categories";
        private const string DishType = "dishes";
        private const string DrinkType = "drinks";
        private const string ToppingType = "toppings";

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly HttpClient _httpClient;
        private readonly ObservableCollection<MenuCategoryViewModel> _visibleCategories = new ObservableCollection<MenuCategoryViewModel>();
        private readonly List<MenuCategoryViewModel> _allCategories = new List<MenuCategoryViewModel>();
        private string _activeType = DishType;
        private bool _isLoadedOnce;
        private bool _isBusy;

        public MenuCategoriesView(HttpClient httpClient, string userRole)
        {
            _httpClient = httpClient;
            InitializeComponent();

            CategoriesCardsList.ItemsSource = _visibleCategories;
            UpdateActionButtons();
            HighlightTypeButtons();
        }

        public bool IsBusy => _isBusy;

        public async Task ActivateAsync()
        {
            if (_isLoadedOnce)
            {
                ApplyTypeFilter();
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
                SetStatus("Загрузка категорий меню...", false);

                using var response = await _httpClient.GetAsync(CategoriesAddress);
                if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                {
                    throw new InvalidOperationException("нет доступа к категориям меню.");
                }

                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var categories = JsonSerializer.Deserialize<List<MenuCategoryDto>>(json, JsonOptions) ?? new List<MenuCategoryDto>();

                _allCategories.Clear();
                _allCategories.AddRange(categories.Select(x => new MenuCategoryViewModel(x)));
                _isLoadedOnce = true;

                SetStatus(string.Empty, false);
                ApplyTypeFilter();
            }
            catch (Exception ex)
            {
                _allCategories.Clear();
                _visibleCategories.Clear();
                EmptyText.Visibility = Visibility.Visible;
                SetStatus("Не удалось загрузить категории меню: " + ex.Message, true);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void TypeButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string type)
            {
                _activeType = type;
                ApplyTypeFilter();
            }
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var name = ShowCategoryDialog("Добавить категорию", string.Empty);
            if (name == null)
            {
                return;
            }

            await SendCategoryRequestAsync(HttpMethod.Post, $"{CategoriesAddress}/{_activeType}", name);
        }

        private async void EditCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is not MenuCategoryViewModel category)
            {
                return;
            }

            var name = ShowCategoryDialog("Изменить категорию", category.Name);
            if (name == null)
            {
                return;
            }

            await SendCategoryRequestAsync(HttpMethod.Put, $"{CategoriesAddress}/{category.Type}/{category.Id}", name);
        }

        private async void DeleteCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is not MenuCategoryViewModel category)
            {
                return;
            }

            var window = new ConfirmDeleteMenuCategoryWindow(category.Name, category.ItemsCount)
            {
                Owner = Window.GetWindow(this)
            };
            if (window.ShowDialog() != true)
            {
                return;
            }

            await SendCategoryRequestAsync(HttpMethod.Delete, $"{CategoriesAddress}/{category.Type}/{category.Id}", null);
        }

        private async Task SendCategoryRequestAsync(HttpMethod method, string address, string? name)
        {
            try
            {
                SetBusy(true);
                SetStatus("Сохранение изменений...", false);

                using var request = new HttpRequestMessage(method, address);
                if (name != null)
                {
                    var body = JsonSerializer.Serialize(new MenuCategoryRequest { Name = name });
                    request.Content = new StringContent(body, Encoding.UTF8, "application/json");
                }

                using var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    var message = await ReadApiMessageAsync(response);
                    SetStatus(message, true);
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

        private void ApplyTypeFilter()
        {
            _visibleCategories.Clear();

            foreach (var category in _allCategories
                         .Where(x => x.Type == _activeType)
                         .OrderBy(x => x.Name))
            {
                _visibleCategories.Add(category);
            }

            EmptyText.Visibility = _visibleCategories.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            CategoriesScrollViewer.Visibility = _visibleCategories.Count == 0 ? Visibility.Collapsed : Visibility.Visible;

            HighlightTypeButtons();
            UpdateActionButtons();
        }

        private void HighlightTypeButtons()
        {
            foreach (var button in new[] { DishesTypeButton, DrinksTypeButton, ToppingsTypeButton })
            {
                var isActive = string.Equals(button.Tag as string, _activeType, StringComparison.OrdinalIgnoreCase);
                button.Background = isActive
                    ? (Brush)Application.Current.Resources["ModalColorPrimaryBrush"]
                    : Brushes.White;
                button.Foreground = isActive
                    ? Brushes.White
                    : (Brush)Application.Current.Resources["ModalColorTextBrush"];
            }
        }

        private void UpdateActionButtons()
        {
            if (AddButton != null)
            {
                AddButton.IsEnabled = !_isBusy;
            }
        }

        private void SetBusy(bool isBusy)
        {
            _isBusy = isBusy;
            RootGrid.IsEnabled = !isBusy;
            UpdateActionButtons();
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

        private string? ShowCategoryDialog(string title, string initialName)
        {
            var window = new MenuCategoryEditWindow(title, initialName)
            {
                Owner = Window.GetWindow(this)
            };

            if (window.ShowDialog() != true)
            {
                return null;
            }

            return window.CategoryName;
        }

        private static async Task<string> ReadApiMessageAsync(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            var message = ExtractApiMessage(content);
            if (!string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            return response.StatusCode switch
            {
                HttpStatusCode.Conflict => "Не удалось выполнить операцию: конфликт данных.",
                HttpStatusCode.NotFound => "Категория не найдена.",
                HttpStatusCode.BadRequest => "Проверьте данные категории.",
                _ => "Не удалось выполнить операцию."
            };
        }

        private static string ExtractApiMessage(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return string.Empty;
            }

            var trimmed = content.Trim();
            if (trimmed.StartsWith("\"") && trimmed.EndsWith("\""))
            {
                try
                {
                    return JsonSerializer.Deserialize<string>(trimmed) ?? string.Empty;
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

        public sealed class MenuCategoryViewModel : NotifyBase
        {
            public MenuCategoryViewModel(MenuCategoryDto dto)
            {
                Id = dto.Id;
                Name = dto.Name ?? string.Empty;
                Type = dto.Type ?? string.Empty;
                ItemsCount = dto.ItemsCount;
            }

            public int Id { get; }
            public string Name { get; }
            public string Type { get; }
            public int ItemsCount { get; }

            public string TypeDisplay => Type switch
            {
                DishType => "Блюда",
                DrinkType => "Напитки",
                ToppingType => "Добавки",
                _ => Type
            };
        }

        public abstract class NotifyBase : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler? PropertyChanged;

            protected void OnPropertyChanged(string? propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
