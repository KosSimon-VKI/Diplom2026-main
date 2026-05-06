using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using GardenNookWpf.Views.MainPanel.Orders;
using GardenNookWpf.Views.Shell;
using TransferModels.Menu;
using TransferModels.Orders;

namespace GardenNookWpf.Views.Shell.Sections.OrderHistory
{
    public partial class OrderHistoryView : UserControl, IMainSectionView
    {
        private const string ApiBaseAddress = "https://localhost:7235";
        private const string HistoryAddress = ApiBaseAddress + "/api/orders/history";
        private const string MenuAddress = ApiBaseAddress + "/api/menu";
        private const string AllStatusesText = "Все статусы";
        private static readonly TimeSpan RefreshInterval = TimeSpan.FromSeconds(10);

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly HttpClient _httpClient;
        private readonly ObservableCollection<OrderHistoryListItemViewModel> _visibleOrders = new ObservableCollection<OrderHistoryListItemViewModel>();
        private readonly List<OrderHistoryListItemViewModel> _allOrders = new List<OrderHistoryListItemViewModel>();
        private readonly List<string> _statuses = new List<string>();
        private readonly List<OrderHistoryPeriodOption> _periods = new List<OrderHistoryPeriodOption>
        {
            new OrderHistoryPeriodOption("today", "Сегодня"),
            new OrderHistoryPeriodOption("week", "Последняя неделя"),
            new OrderHistoryPeriodOption("month", "Последний месяц"),
            new OrderHistoryPeriodOption("threeMonths", "Последние 3 месяца")
        };
        private readonly DispatcherTimer _refreshTimer;
        private MenuResponse? _menu;
        private int? _clientFilterId;
        private string _clientFilterTitle = string.Empty;
        private bool _isLoadedOnce;
        private bool _isBusy;
        private bool _isOpeningDetails;

        public OrderHistoryView(HttpClient httpClient, string userRole)
        {
            _httpClient = httpClient;
            _refreshTimer = new DispatcherTimer
            {
                Interval = RefreshInterval
            };
            _refreshTimer.Tick += RefreshTimer_Tick;

            InitializeComponent();

            OrdersList.ItemsSource = _visibleOrders;
            StatusFilterComboBox.ItemsSource = _statuses;
            PeriodFilterComboBox.ItemsSource = _periods;
            SearchTextBox.Text = string.Empty;
            _statuses.Add(AllStatusesText);
            StatusFilterComboBox.SelectedIndex = 0;
            PeriodFilterComboBox.SelectedIndex = 0;
        }

        public bool IsBusy => _isBusy;

        public void SetClientFilter(int clientId, string clientName)
        {
            _clientFilterId = clientId > 0 ? clientId : null;
            _clientFilterTitle = clientName?.Trim() ?? string.Empty;
            _isLoadedOnce = false;
            UpdateClientFilterUi();
        }

        public void ClearClientFilter()
        {
            if (!_clientFilterId.HasValue && string.IsNullOrWhiteSpace(_clientFilterTitle))
            {
                UpdateClientFilterUi();
                return;
            }

            _clientFilterId = null;
            _clientFilterTitle = string.Empty;
            _isLoadedOnce = false;
            UpdateClientFilterUi();
        }

        public async Task ActivateAsync()
        {
            if (_isLoadedOnce)
            {
                ApplyFilters();
                _refreshTimer.Start();
                return;
            }

            await ReloadAsync();
            _refreshTimer.Start();
        }

        public void Deactivate()
        {
            _refreshTimer.Stop();
        }

        private async void RefreshTimer_Tick(object? sender, EventArgs e)
        {
            if (_isBusy || _isOpeningDetails)
            {
                return;
            }

            await ReloadAsync();
        }

        private void Filter_Changed(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private async void AllClientsButton_Click(object sender, RoutedEventArgs e)
        {
            ClearClientFilter();
            await ReloadAsync();
        }

        private async void PeriodFilter_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (!_isLoadedOnce || _isBusy)
            {
                return;
            }

            await ReloadAsync();
        }

        private async void OrdersList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isOpeningDetails || OrdersList.SelectedItem is not OrderHistoryListItemViewModel selected)
            {
                return;
            }

            await ShowDetailsModalAsync(selected.OrderId);
        }

        private async Task ShowDetailsModalAsync(int orderId)
        {
            try
            {
                _isOpeningDetails = true;
                var details = await GetDetailsAsync(orderId);
                if (details == null)
                {
                    return;
                }

                var detailsWindow = new OrderHistoryDetailsWindow(details)
                {
                    Owner = Window.GetWindow(this)
                };

                var shouldEdit = detailsWindow.ShowDialog() == true;
                if (!shouldEdit)
                {
                    return;
                }

                var menu = await GetMenuAsync();
                if (menu == null)
                {
                    return;
                }

                var editWindow = new OrderHistoryEditWindow(_httpClient, details, menu)
                {
                    Owner = Window.GetWindow(this)
                };

                if (editWindow.ShowDialog() == true)
                {
                    await SaveOrderAsync(orderId, editWindow.Request);
                }
            }
            finally
            {
                OrdersList.SelectedItem = null;
                _isOpeningDetails = false;
            }
        }

        private async Task ReloadAsync()
        {
            try
            {
                SetBusy(true);
                SetStatus("Загрузка истории заказов...", false);

                using var response = await _httpClient.GetAsync(BuildHistoryAddress());
                if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                {
                    SetStatus("Нет доступа к истории заказов.", true);
                    return;
                }

                if (!response.IsSuccessStatusCode)
                {
                    SetStatus(await ReadErrorMessageAsync(response, "Не удалось загрузить историю заказов."), true);
                    return;
                }

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<OrderHistoryResponse>(json, JsonOptions) ?? new OrderHistoryResponse();

                _allOrders.Clear();
                _allOrders.AddRange((data.Orders ?? new List<OrderHistoryListItemDto>())
                    .Select(x => new OrderHistoryListItemViewModel(x)));

                RebuildStatusFilter();
                ApplyFilters();
                _isLoadedOnce = true;
                SetStatus(_allOrders.Count == 0 ? "История заказов пока пуста." : string.Empty, false);
            }
            catch (Exception ex)
            {
                SetStatus("Ошибка загрузки истории заказов: " + ex.Message, true);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async Task<OrderHistoryDetailsDto?> GetDetailsAsync(int orderId)
        {
            try
            {
                SetBusy(true);
                using var response = await _httpClient.GetAsync($"{HistoryAddress}/{orderId}");
                if (!response.IsSuccessStatusCode)
                {
                    SetStatus(await ReadErrorMessageAsync(response, "Не удалось загрузить заказ."), true);
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<OrderHistoryDetailsDto>(json, JsonOptions);
            }
            catch (Exception ex)
            {
                SetStatus("Ошибка загрузки заказа: " + ex.Message, true);
                return null;
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async Task<MenuResponse?> GetMenuAsync()
        {
            if (_menu != null)
            {
                return _menu;
            }

            try
            {
                SetBusy(true);
                using var response = await _httpClient.GetAsync(MenuAddress);
                if (!response.IsSuccessStatusCode)
                {
                    SetStatus(await ReadErrorMessageAsync(response, "Не удалось загрузить меню."), true);
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                _menu = JsonSerializer.Deserialize<MenuResponse>(json, JsonOptions) ?? new MenuResponse();
                return _menu;
            }
            catch (Exception ex)
            {
                SetStatus("Ошибка загрузки меню: " + ex.Message, true);
                return null;
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async Task SaveOrderAsync(int orderId, OrderHistoryUpdateRequest request)
        {
            try
            {
                SetBusy(true);
                SetStatus("Сохранение заказа...", false);

                var json = JsonSerializer.Serialize(request);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");
                using var response = await _httpClient.PutAsync($"{HistoryAddress}/{orderId}", content);

                if (!response.IsSuccessStatusCode)
                {
                    SetStatus(await ReadErrorMessageAsync(response, "Не удалось сохранить заказ."), true);
                    return;
                }

                SetStatus("Изменения сохранены.", false);
                await ReloadAsync();
            }
            catch (Exception ex)
            {
                SetStatus("Ошибка сохранения заказа: " + ex.Message, true);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void ApplyFilters()
        {
            var query = (SearchTextBox?.Text ?? string.Empty).Trim().ToLowerInvariant();
            var selectedStatus = StatusFilterComboBox?.SelectedItem as string;

            var filtered = _allOrders.Where(order =>
            {
                var matchesQuery = string.IsNullOrWhiteSpace(query) ||
                    order.OrderId.ToString(CultureInfo.InvariantCulture).Contains(query) ||
                    order.ClientDisplay.ToLowerInvariant().Contains(query) ||
                    order.CompositionSummary.ToLowerInvariant().Contains(query);
                var matchesStatus = string.IsNullOrWhiteSpace(selectedStatus) ||
                    selectedStatus == AllStatusesText ||
                    string.Equals(order.Status, selectedStatus, StringComparison.CurrentCultureIgnoreCase);
                return matchesQuery && matchesStatus;
            }).ToList();

            _visibleOrders.Clear();
            foreach (var order in filtered)
            {
                _visibleOrders.Add(order);
            }
        }

        private void RebuildStatusFilter()
        {
            var selected = StatusFilterComboBox.SelectedItem as string;
            _statuses.Clear();
            _statuses.Add(AllStatusesText);
            foreach (var status in _allOrders
                .Select(x => x.Status)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.CurrentCultureIgnoreCase)
                .OrderBy(x => x))
            {
                _statuses.Add(status);
            }

            StatusFilterComboBox.Items.Refresh();
            StatusFilterComboBox.SelectedItem = _statuses.Contains(selected ?? string.Empty)
                ? selected
                : AllStatusesText;
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
                ? Brushes.Firebrick
                : (Brush)Application.Current.Resources["ModalColorPrimaryDarkBrush"];
            StatusText.Visibility = string.IsNullOrWhiteSpace(message) ? Visibility.Collapsed : Visibility.Visible;
        }

        private static async Task<string> ReadErrorMessageAsync(HttpResponseMessage response, string fallback)
        {
            var text = await response.Content.ReadAsStringAsync();
            return string.IsNullOrWhiteSpace(text) ? fallback : text;
        }

        private string BuildHistoryAddress()
        {
            var period = (PeriodFilterComboBox?.SelectedItem as OrderHistoryPeriodOption)?.Token ?? "today";
            var address = $"{HistoryAddress}?period={Uri.EscapeDataString(period)}";
            if (_clientFilterId.HasValue)
            {
                address += "&clientId=" + _clientFilterId.Value.ToString(CultureInfo.InvariantCulture);
            }

            return address;
        }

        private void UpdateClientFilterUi()
        {
            if (TitleText == null || AllClientsButton == null)
            {
                return;
            }

            var hasClientFilter = _clientFilterId.HasValue;
            TitleText.Text = hasClientFilter && !string.IsNullOrWhiteSpace(_clientFilterTitle)
                ? "ИСТОРИЯ ЗАКАЗОВ: " + _clientFilterTitle
                : "ИСТОРИЯ ЗАКАЗОВ";
            AllClientsButton.Visibility = hasClientFilter ? Visibility.Visible : Visibility.Collapsed;
        }

        private static string FormatDate(DateTime? value)
        {
            return value.HasValue ? value.Value.ToString("dd.MM.yyyy HH:mm", CultureInfo.CurrentCulture) : "-";
        }

        private static string BuildClientDisplay(string name, string phone)
        {
            if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(phone))
            {
                return $"{name} ({phone})";
            }

            return string.IsNullOrWhiteSpace(name) ? phone : name;
        }

        private sealed class OrderHistoryPeriodOption
        {
            public OrderHistoryPeriodOption(string token, string title)
            {
                Token = token;
                Title = title;
            }

            public string Token { get; }
            public string Title { get; }
        }

        private sealed class OrderHistoryListItemViewModel
        {
            private readonly OrderHistoryListItemDto _source;

            public OrderHistoryListItemViewModel(OrderHistoryListItemDto source)
            {
                _source = source;
            }

            public int OrderId => _source.OrderId;
            public DateTime? CreatedAt => _source.CreatedAt;
            public string Title => $"Заказ №{_source.OrderId}";
            public string CreatedAtDisplay => FormatDate(_source.CreatedAt);
            public string ClientDisplay => BuildClientDisplay(_source.ClientName, _source.ClientPhone);
            public string Status => _source.Status;
            public string OrderType => _source.OrderType;
            public string MetaDisplay => $"Дата: {CreatedAtDisplay}\nКлиент: {ClientDisplay}\nТип: {OrderType}";
            public string TotalDisplay => $"Сумма: {_source.TotalPrice:0.##} ₽ · Калории: {_source.TotalCalories:0.##}";
            public string CompositionSummary => _source.CompositionSummary;
            public Brush StatusBorderBrush => ResolveStatusBrush(Status, "#FF91A56E", "#FFB97800", "#FF2F8D56", "#FFB63F35", "#FF707070");
            public Brush StatusBackgroundBrush => ResolveStatusBrush(Status, "#FFFCFFFA", "#FFFFF9EC", "#FFF2FFF6", "#FFFFF4F3", "#FFFAFAFA");
            public Brush StatusAccentBrush => ResolveStatusBrush(Status, "#FF1E5928", "#FF8C5D00", "#FF165F2A", "#FF7A2722", "#FF525252");
            public Brush StatusBadgeBrush => ResolveStatusBrush(Status, "#FFF3FFF3", "#FFFFF2D6", "#FFE9FBEF", "#FFFFEAE8", "#FFF2F2F2");

            private static Brush ResolveStatusBrush(string status, string normal, string progress, string ready, string cancelled, string unknown)
            {
                var normalized = (status ?? string.Empty).Trim().ToLowerInvariant();
                var color = normal;
                if (normalized.Contains("отмен") || normalized.Contains("cancel"))
                {
                    color = cancelled;
                }
                else if (normalized.Contains("готов") || normalized.Contains("ready") || normalized.Contains("выдан") || normalized.Contains("done"))
                {
                    color = ready;
                }
                else if (normalized.Contains("процесс") || normalized.Contains("process") || normalized.Contains("готовит") || normalized.Contains("создан") || normalized.Contains("new"))
                {
                    color = progress;
                }
                else if (string.IsNullOrWhiteSpace(normalized))
                {
                    color = unknown;
                }

                return new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
            }
        }
    }
}
