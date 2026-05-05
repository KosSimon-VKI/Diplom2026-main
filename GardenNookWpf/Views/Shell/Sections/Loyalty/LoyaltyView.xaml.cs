using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using GardenNookWpf.Views.MainPanel.Loyalty;
using GardenNookWpf.Views.Shell;
using LoyaltyContracts = TransferModels.Loyalty;

namespace GardenNookWpf.Views.Shell.Sections.Loyalty
{
    public partial class LoyaltyView : UserControl, IMainSectionView
    {
        private const string ApiBaseAddress = "https://localhost:7235";
        private const string LoyaltyAddress = ApiBaseAddress + "/api/loyalty";
        private const string DiscountsAddress = LoyaltyAddress + "/discounts";
        private const string CategoriesAddress = LoyaltyAddress + "/client-categories";

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly HttpClient _httpClient;
        private readonly ObservableCollection<DiscountViewModel> _discounts = new ObservableCollection<DiscountViewModel>();
        private readonly ObservableCollection<ClientCategoryViewModel> _categories = new ObservableCollection<ClientCategoryViewModel>();
        private bool _isLoadedOnce;
        private bool _isBusy;

        public LoyaltyView(HttpClient httpClient, string userRole)
        {
            _httpClient = httpClient;
            InitializeComponent();

            DiscountsList.ItemsSource = _discounts;
            CategoriesList.ItemsSource = _categories;
            UpdateActionButtons();
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
        }

        private async Task ReloadAsync()
        {
            try
            {
                SetBusy(true);
                SetStatus("Загрузка системы лояльности...", false);

                using var discountsResponse = await _httpClient.GetAsync(DiscountsAddress);
                EnsureAccess(discountsResponse, "нет доступа к скидкам.");
                discountsResponse.EnsureSuccessStatusCode();
                var discountsJson = await discountsResponse.Content.ReadAsStringAsync();
                var discounts = JsonSerializer.Deserialize<List<LoyaltyContracts.DiscountManagementDto>>(discountsJson, JsonOptions)
                    ?? new List<LoyaltyContracts.DiscountManagementDto>();

                using var categoriesResponse = await _httpClient.GetAsync(CategoriesAddress);
                EnsureAccess(categoriesResponse, "нет доступа к категориям клиентов.");
                categoriesResponse.EnsureSuccessStatusCode();
                var categoriesJson = await categoriesResponse.Content.ReadAsStringAsync();
                var categories = JsonSerializer.Deserialize<List<LoyaltyContracts.ClientCategoryManagementDto>>(categoriesJson, JsonOptions)
                    ?? new List<LoyaltyContracts.ClientCategoryManagementDto>();

                _discounts.Clear();
                foreach (var discount in discounts.OrderBy(x => x.Name).ThenBy(x => x.Id))
                {
                    _discounts.Add(new DiscountViewModel(discount));
                }

                _categories.Clear();
                foreach (var category in categories.OrderBy(x => x.Name).ThenBy(x => x.Id))
                {
                    _categories.Add(new ClientCategoryViewModel(category));
                }

                _isLoadedOnce = true;
                SetStatus(string.Empty, false);
                RenderEmptyStates();
            }
            catch (Exception ex)
            {
                _discounts.Clear();
                _categories.Clear();
                RenderEmptyStates();
                SetStatus("Не удалось загрузить систему лояльности: " + ex.Message, true);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async void AddDiscountButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new DiscountEditWindow(null)
            {
                Owner = Window.GetWindow(this)
            };

            if (window.ShowDialog() != true)
            {
                return;
            }

            await SendDiscountAsync(HttpMethod.Post, DiscountsAddress, window.Request, "Скидка добавлена.");
        }

        private async void EditDiscountButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is not DiscountViewModel discount)
            {
                return;
            }

            var window = new DiscountEditWindow(discount.Source)
            {
                Owner = Window.GetWindow(this)
            };

            if (window.ShowDialog() != true)
            {
                return;
            }

            await SendDiscountAsync(HttpMethod.Put, $"{DiscountsAddress}/{discount.Id}", window.Request, "Изменения сохранены.");
        }

        private async void DeleteDiscountButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is not DiscountViewModel discount)
            {
                return;
            }

            var window = new ConfirmDeleteDiscountWindow(discount.Name, discount.DiscountPercent, discount.OrdersCount)
            {
                Owner = Window.GetWindow(this)
            };

            if (window.ShowDialog() != true)
            {
                return;
            }

            await SendDeleteAsync($"{DiscountsAddress}/{discount.Id}", "Скидка удалена.", "Не удалось удалить скидку: ");
        }

        private async void AddCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new ClientCategoryEditWindow(null)
            {
                Owner = Window.GetWindow(this)
            };

            if (window.ShowDialog() != true)
            {
                return;
            }

            await SendCategoryAsync(HttpMethod.Post, CategoriesAddress, window.Request, "Категория клиента добавлена.");
        }

        private async void EditCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is not ClientCategoryViewModel category)
            {
                return;
            }

            var window = new ClientCategoryEditWindow(category.Source)
            {
                Owner = Window.GetWindow(this)
            };

            if (window.ShowDialog() != true)
            {
                return;
            }

            await SendCategoryAsync(HttpMethod.Put, $"{CategoriesAddress}/{category.Id}", window.Request, "Изменения сохранены.");
        }

        private async void DeleteCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is not ClientCategoryViewModel category)
            {
                return;
            }

            var window = new ConfirmDeleteClientCategoryWindow(category.Name, category.ClientsCount)
            {
                Owner = Window.GetWindow(this)
            };

            if (window.ShowDialog() != true)
            {
                return;
            }

            await SendDeleteAsync($"{CategoriesAddress}/{category.Id}", "Категория клиента удалена.", "Не удалось удалить категорию клиента: ");
        }

        private async Task SendDiscountAsync(HttpMethod method, string address, LoyaltyContracts.DiscountUpsertRequest requestBody, string successMessage)
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
                SetStatus(successMessage, false);
            }
            catch (Exception ex)
            {
                SetStatus("Не удалось сохранить скидку: " + ex.Message, true);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async Task SendCategoryAsync(HttpMethod method, string address, LoyaltyContracts.ClientCategoryUpsertRequest requestBody, string successMessage)
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
                SetStatus(successMessage, false);
            }
            catch (Exception ex)
            {
                SetStatus("Не удалось сохранить категорию клиента: " + ex.Message, true);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async Task SendDeleteAsync(string address, string successMessage, string errorPrefix)
        {
            try
            {
                SetBusy(true);
                SetStatus("Удаление записи...", false);

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
                SetStatus(errorPrefix + ex.Message, true);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void RenderEmptyStates()
        {
            EmptyDiscountsText.Visibility = _discounts.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            DiscountsScrollViewer.Visibility = _discounts.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
            EmptyCategoriesText.Visibility = _categories.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            CategoriesScrollViewer.Visibility = _categories.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
            UpdateActionButtons();
        }

        private void SetBusy(bool isBusy)
        {
            _isBusy = isBusy;
            RootGrid.IsEnabled = !isBusy;
            UpdateActionButtons();
        }

        private void UpdateActionButtons()
        {
            if (AddDiscountButton != null)
            {
                AddDiscountButton.IsEnabled = !_isBusy;
            }

            if (AddCategoryButton != null)
            {
                AddCategoryButton.IsEnabled = !_isBusy;
            }
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

        private static void EnsureAccess(HttpResponseMessage response, string forbiddenMessage)
        {
            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new InvalidOperationException(forbiddenMessage);
            }
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
                HttpStatusCode.Conflict => "Операция невозможна из-за связанных данных.",
                HttpStatusCode.NotFound => "Запись не найдена.",
                HttpStatusCode.BadRequest => "Проверьте данные.",
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

        public sealed class DiscountViewModel : NotifyBase
        {
            public DiscountViewModel(LoyaltyContracts.DiscountManagementDto source)
            {
                Source = source;
            }

            public LoyaltyContracts.DiscountManagementDto Source { get; }
            public int Id => Source.Id;
            public string Name => Source.Name ?? string.Empty;
            public decimal DiscountPercent => Source.DiscountPercent;
            public int OrdersCount => Source.OrdersCount;
            public string PercentDisplay => "Скидка: " + DiscountPercent.ToString("0.##", CultureInfo.CurrentCulture) + "%";
            public string OrdersDisplay => "Заказов: " + OrdersCount;
        }

        public sealed class ClientCategoryViewModel : NotifyBase
        {
            public ClientCategoryViewModel(LoyaltyContracts.ClientCategoryManagementDto source)
            {
                Source = source;
            }

            public LoyaltyContracts.ClientCategoryManagementDto Source { get; }
            public int Id => Source.Id;
            public string Name => Source.Name ?? string.Empty;
            public int ClientsCount => Source.ClientsCount;
            public string ClientsDisplay => "Клиентов: " + ClientsCount;
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
