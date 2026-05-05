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
using GardenNookWpf.Views.MainPanel.IngredientCategories;
using GardenNookWpf.Views.Shell;
using TransferModels.Inventory;

namespace GardenNookWpf.Views.Shell.Sections.IngredientCategories
{
    public partial class IngredientCategoriesView : UserControl, IMainSectionView
    {
        private const string ApiBaseAddress = "https://localhost:7235";
        private const string CategoriesAddress = ApiBaseAddress + "/api/ingredient-categories";

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly HttpClient _httpClient;
        private readonly ObservableCollection<IngredientCategoryViewModel> _visibleCategories = new ObservableCollection<IngredientCategoryViewModel>();
        private readonly List<IngredientCategoryViewModel> _allCategories = new List<IngredientCategoryViewModel>();
        private bool _isLoadedOnce;
        private bool _isBusy;

        public IngredientCategoriesView(HttpClient httpClient, string userRole)
        {
            _httpClient = httpClient;
            InitializeComponent();

            CategoriesCardsList.ItemsSource = _visibleCategories;
            UpdateActionButtons();
        }

        public bool IsBusy => _isBusy;

        public async Task ActivateAsync()
        {
            if (_isLoadedOnce)
            {
                ApplyFilter();
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
                SetStatus("Загрузка категорий сырья...", false);

                using var response = await _httpClient.GetAsync(CategoriesAddress);
                if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                {
                    throw new InvalidOperationException("нет доступа к категориям сырья.");
                }

                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var categories = JsonSerializer.Deserialize<List<IngredientCategoryDto>>(json, JsonOptions) ?? new List<IngredientCategoryDto>();

                _allCategories.Clear();
                _allCategories.AddRange(categories.Select(x => new IngredientCategoryViewModel(x)));
                _isLoadedOnce = true;

                SetStatus(string.Empty, false);
                ApplyFilter();
            }
            catch (Exception ex)
            {
                _allCategories.Clear();
                _visibleCategories.Clear();
                EmptyText.Visibility = Visibility.Visible;
                SetStatus("Не удалось загрузить категории сырья: " + ex.Message, true);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var name = ShowCategoryDialog("Добавить категорию сырья", string.Empty);
            if (name == null)
            {
                return;
            }

            await SendCategoryRequestAsync(HttpMethod.Post, CategoriesAddress, name, "Изменения сохранены.");
        }

        private async void EditCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is not IngredientCategoryViewModel category)
            {
                return;
            }

            var name = ShowCategoryDialog("Изменить категорию сырья", category.Name);
            if (name == null)
            {
                return;
            }

            await SendCategoryRequestAsync(HttpMethod.Put, $"{CategoriesAddress}/{category.Id}", name, "Изменения сохранены.");
        }

        private async void DeleteCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is not IngredientCategoryViewModel category)
            {
                return;
            }

            var window = new ConfirmDeleteIngredientCategoryWindow(category.Name, category.ItemsCount)
            {
                Owner = Window.GetWindow(this)
            };
            if (window.ShowDialog() != true)
            {
                return;
            }

            await SendCategoryRequestAsync(HttpMethod.Delete, $"{CategoriesAddress}/{category.Id}", null, "Категория сырья удалена.");
        }

        private async Task SendCategoryRequestAsync(HttpMethod method, string address, string? name, string successMessage)
        {
            try
            {
                SetBusy(true);
                SetStatus("Сохранение изменений...", false);

                using var request = new HttpRequestMessage(method, address);
                if (name != null)
                {
                    var body = JsonSerializer.Serialize(new IngredientCategoryRequest { Name = name });
                    request.Content = new StringContent(body, Encoding.UTF8, "application/json");
                }

                using var response = await _httpClient.SendAsync(request);
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
                SetStatus("Не удалось сохранить изменения: " + ex.Message, true);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void ApplyFilter()
        {
            _visibleCategories.Clear();

            foreach (var category in _allCategories.OrderBy(x => x.Name))
            {
                _visibleCategories.Add(category);
            }

            EmptyText.Visibility = _visibleCategories.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            CategoriesScrollViewer.Visibility = _visibleCategories.Count == 0 ? Visibility.Collapsed : Visibility.Visible;

            UpdateActionButtons();
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
            var window = new IngredientCategoryEditWindow(title, initialName)
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
                HttpStatusCode.NotFound => "Категория сырья не найдена.",
                HttpStatusCode.BadRequest => "Проверьте данные категории сырья.",
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

        public sealed class IngredientCategoryViewModel : NotifyBase
        {
            public IngredientCategoryViewModel(IngredientCategoryDto dto)
            {
                Id = dto.Id;
                Name = dto.Name ?? string.Empty;
                ItemsCount = dto.ItemsCount;
            }

            public int Id { get; }
            public string Name { get; }
            public int ItemsCount { get; }
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
