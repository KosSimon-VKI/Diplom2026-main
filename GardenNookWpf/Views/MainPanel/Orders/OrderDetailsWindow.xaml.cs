using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GardenNookWpf.Views.Controls;
using GardenNookWpf.Views.MainPanel.TechnicalCards;
using TransferModels.Kitchen;

namespace GardenNookWpf.Views.MainPanel.Orders
{
    /// <summary>
    /// Логика взаимодействия для OrderDetailsWindow.xaml
    /// </summary>
    public partial class OrderDetailsWindow : Window
    {
        private const string KitchenApiBaseAddress = "https://localhost:7235/api/kitchen";
        private const string AdminRole = "Администратор";

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly HttpClient _httpClient;
        private readonly KitchenOrderCardViewModel _orderCard;
        private readonly ObservableCollection<KitchenOrderItemViewModel> _items;
        private readonly bool _isAdmin;
        private readonly bool _isReadOnly;
        private bool _isBusy;

        public event EventHandler? OrderUpdated;

        public OrderDetailsWindow(HttpClient httpClient, KitchenOrderCardViewModel orderCard, string userRole)
        {
            _httpClient = httpClient;
            _orderCard = orderCard;
            _items = new ObservableCollection<KitchenOrderItemViewModel>(orderCard.DisplayItems ?? new List<KitchenOrderItemViewModel>());
            _isAdmin = string.Equals(userRole?.Trim(), AdminRole, StringComparison.CurrentCulture);
            _isReadOnly = orderCard.IsReadOnly;
            if (_isReadOnly)
            {
                foreach (var item in _items)
                {
                    item.CompleteButtonVisibility = Visibility.Collapsed;
                }
            }

            InitializeComponent();

            ItemsList.ItemsSource = _items;
            BindHeader();
            BindRoleActions();
            RefreshEmptyState();
        }

        public OrderDetailsWindow(HttpClient httpClient, KitchenOrderCardViewModel orderCard)
            : this(httpClient, orderCard, string.Empty)
        {
        }

        private void BindHeader()
        {
            var createdAtText = _orderCard.CreatedAt.HasValue
                ? _orderCard.CreatedAt.Value.ToString("dd.MM.yyyy HH:mm", CultureInfo.CurrentCulture)
                : "не указано";

            OrderHeaderText.Text = $"Заказ №{_orderCard.OrderNumberText}";
            OrderMetaText.Text = string.IsNullOrWhiteSpace(_orderCard.StatusText)
                ? $"{_orderCard.OrderTypeText} | Создан: {createdAtText}"
                : $"{_orderCard.OrderTypeText} | Статус: {_orderCard.StatusText} | Создан: {createdAtText}";

            if (!string.IsNullOrWhiteSpace(_orderCard.PickupAtText))
            {
                PickupAtTextBlock.Text = _orderCard.PickupAtText;
                PickupAtTextBlock.Visibility = Visibility.Visible;
            }

            if (!string.IsNullOrWhiteSpace(_orderCard.OrderCommentText))
            {
                OrderCommentTextBlock.Text = _orderCard.OrderCommentText;
                OrderCommentTextBlock.Visibility = Visibility.Visible;
            }
        }

        private async void ShowTechnicalCard_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button || button.Tag is not KitchenOrderItemViewModel item)
            {
                return;
            }

            if (_isBusy)
            {
                return;
            }

            await ShowTechnicalCardAsync(item);
        }

        private async void CompleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button || button.Tag is not KitchenOrderItemViewModel item)
            {
                return;
            }

            if (_isBusy)
            {
                return;
            }

            if (_isReadOnly)
            {
                return;
            }

            await CompleteItemAsync(item);
        }

        private async void CompleteOrder_Click(object sender, RoutedEventArgs e)
        {
            if (_isBusy)
            {
                return;
            }

            if (_isReadOnly)
            {
                return;
            }

            await CompleteOrderAsync();
        }

        private async void CancelOrder_Click(object sender, RoutedEventArgs e)
        {
            if (_isBusy || !_isAdmin || _isReadOnly)
            {
                return;
            }

            var confirmationWindow = new ConfirmCancelOrderWindow(_orderCard.OrderNumberText)
            {
                Owner = this
            };

            if (confirmationWindow.ShowDialog() != true)
            {
                return;
            }

            await CancelOrderAsync();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void HeaderClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private async Task ShowTechnicalCardAsync(KitchenOrderItemViewModel item)
        {
            try
            {
                SetBusy(true);
                var address = BuildTechnicalCardAddress(item);
                var response = await _httpClient.GetAsync(address);

                if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                {
                    MessageBox.Show("Нет доступа к тех. картам.", "Garden Nook", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!response.IsSuccessStatusCode)
                {
                    var message = await ReadErrorMessageAsync(response);
                    MessageBox.Show(
                        string.IsNullOrWhiteSpace(message) ? "Не удалось загрузить тех. карту." : message,
                        "Garden Nook",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                var json = await response.Content.ReadAsStringAsync();
                var technicalCard = JsonSerializer.Deserialize<KitchenTechnicalCardResponse>(json, JsonOptions);
                if (technicalCard == null)
                {
                    MessageBox.Show("Не удалось распознать данные тех. карты.", "Garden Nook", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var technicalCardWindow = new TechnicalCardWindow(technicalCard)
                {
                    Owner = this
                };
                technicalCardWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки тех. карты: {ex.Message}", "Garden Nook", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async Task CompleteItemAsync(KitchenOrderItemViewModel item)
        {
            try
            {
                SetBusy(true);

                var address = BuildCompleteItemAddress(item);
                var response = await _httpClient.PostAsync(address, content: null);

                if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                {
                    MessageBox.Show("Нет доступа к обновлению заказа.", "Garden Nook", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!response.IsSuccessStatusCode)
                {
                    var message = await ReadErrorMessageAsync(response);
                    MessageBox.Show(
                        string.IsNullOrWhiteSpace(message) ? "Не удалось пометить позицию готовой." : message,
                        "Garden Nook",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<KitchenCompleteOrderItemResponse>(json, JsonOptions);

                _items.Remove(item);
                RefreshEmptyState();
                OrderUpdated?.Invoke(this, EventArgs.Empty);

                if ((result != null && result.OrderCompleted) || _items.Count == 0)
                {
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления позиции: {ex.Message}", "Garden Nook", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async Task CompleteOrderAsync()
        {
            try
            {
                SetBusy(true);

                if (!_isAdmin)
                {
                    await CompleteVisibleItemsAsync();
                    return;
                }

                var response = await _httpClient.PostAsync(BuildCompleteOrderAddress(), content: null);

                if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                {
                    MessageBox.Show("Нет доступа к обновлению заказа.", "Garden Nook", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!response.IsSuccessStatusCode)
                {
                    var message = await ReadErrorMessageAsync(response);
                    MessageBox.Show(
                        string.IsNullOrWhiteSpace(message) ? "Не удалось завершить заказ." : message,
                        "Garden Nook",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                _items.Clear();
                RefreshEmptyState();
                OrderUpdated?.Invoke(this, EventArgs.Empty);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка завершения заказа: {ex.Message}", "Garden Nook", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async Task CompleteVisibleItemsAsync()
        {
            var itemsToComplete = _items.ToList();
            foreach (var item in itemsToComplete)
            {
                var response = await _httpClient.PostAsync(BuildCompleteItemAddress(item), content: null);

                if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                {
                    MessageBox.Show("Нет доступа к обновлению заказа.", "Garden Nook", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!response.IsSuccessStatusCode)
                {
                    var message = await ReadErrorMessageAsync(response);
                    MessageBox.Show(
                        string.IsNullOrWhiteSpace(message) ? "Не удалось пометить позиции готовыми." : message,
                        "Garden Nook",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                _items.Remove(item);
            }

            RefreshEmptyState();
            OrderUpdated?.Invoke(this, EventArgs.Empty);
            Close();
        }

        private async Task CancelOrderAsync()
        {
            try
            {
                SetBusy(true);

                var response = await _httpClient.PostAsync(BuildCancelOrderAddress(), content: null);

                if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                {
                    MessageBox.Show("Нет доступа к отмене заказа.", "Garden Nook", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!response.IsSuccessStatusCode)
                {
                    var message = await ReadErrorMessageAsync(response);
                    MessageBox.Show(
                        string.IsNullOrWhiteSpace(message) ? "Не удалось отменить заказ." : message,
                        "Garden Nook",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                _items.Clear();
                RefreshEmptyState();
                OrderUpdated?.Invoke(this, EventArgs.Empty);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка отмены заказа: {ex.Message}", "Garden Nook", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private string BuildTechnicalCardAddress(KitchenOrderItemViewModel item)
        {
            return $"{KitchenApiBaseAddress}/orders/{_orderCard.OrderId}/items/{Uri.EscapeDataString(item.ItemType)}/{item.ItemId}/technical-card";
        }

        private string BuildCompleteItemAddress(KitchenOrderItemViewModel item)
        {
            return $"{KitchenApiBaseAddress}/orders/{_orderCard.OrderId}/items/{Uri.EscapeDataString(item.ItemType)}/{item.ItemId}/complete";
        }

        private string BuildCompleteOrderAddress()
        {
            return $"{KitchenApiBaseAddress}/orders/{_orderCard.OrderId}/complete";
        }

        private string BuildCancelOrderAddress()
        {
            return $"{KitchenApiBaseAddress}/orders/{_orderCard.OrderId}/cancel";
        }

        private static async Task<string> ReadErrorMessageAsync(HttpResponseMessage response)
        {
            var payload = await response.Content.ReadAsStringAsync();
            return string.IsNullOrWhiteSpace(payload) ? response.ReasonPhrase ?? string.Empty : payload;
        }

        private void RefreshEmptyState()
        {
            var hasItems = _items.Count > 0;
            ItemsList.Visibility = hasItems ? Visibility.Visible : Visibility.Collapsed;
            EmptyItemsText.Visibility = hasItems ? Visibility.Collapsed : Visibility.Visible;
        }

        private void BindRoleActions()
        {
            if (_isReadOnly)
            {
                CancelOrderButton.Visibility = Visibility.Collapsed;
                CompleteOrderButton.Visibility = Visibility.Collapsed;
                return;
            }

            CancelOrderButton.Visibility = _isAdmin ? Visibility.Visible : Visibility.Collapsed;
            CompleteOrderButton.Content = _isAdmin ? "Завершить заказ" : "Завершить позиции";
        }

        private void SetBusy(bool isBusy)
        {
            _isBusy = isBusy;
            RootGrid.IsEnabled = !isBusy;
        }
    }
}
