using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using GardenNookWpf.Views.MainPanel.Clients;
using GardenNookWpf.Views.Shell;
using ClientContracts = TransferModels.Clients;

namespace GardenNookWpf.Views.Shell.Sections.Clients
{
    public partial class ClientsView : UserControl, IMainSectionView
    {
        private const string ApiBaseAddress = "https://localhost:7235";
        private const string ClientsAddress = ApiBaseAddress + "/api/clients";
        private const string EditOptionsAddress = ClientsAddress + "/edit-options";

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly HttpClient _httpClient;
        private readonly Func<int, string, Task> _openOrderHistoryAsync;
        private readonly ObservableCollection<ClientViewModel> _visibleClients = new ObservableCollection<ClientViewModel>();
        private readonly List<ClientViewModel> _allClients = new List<ClientViewModel>();
        private readonly DispatcherTimer _searchReloadTimer;
        private ClientContracts.ClientEditOptionsResponse _editOptions = new ClientContracts.ClientEditOptionsResponse();
        private bool _editOptionsLoaded;
        private bool _isLoadedOnce;
        private bool _isBusy;

        public ClientsView(HttpClient httpClient, string userRole, Func<int, string, Task> openOrderHistoryAsync)
        {
            _httpClient = httpClient;
            _openOrderHistoryAsync = openOrderHistoryAsync;

            InitializeComponent();

            ClientsList.ItemsSource = _visibleClients;
            _searchReloadTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(350)
            };
            _searchReloadTimer.Tick += SearchReloadTimer_Tick;
        }

        public bool IsBusy => _isBusy;

        public async Task ActivateAsync()
        {
            if (_isLoadedOnce)
            {
                RenderClients();
                return;
            }

            await ReloadAsync();
        }

        public void Deactivate()
        {
            _searchReloadTimer.Stop();
        }

        private async Task ReloadAsync()
        {
            try
            {
                SetBusy(true);
                SetStatus("Загрузка клиентов...", false);

                using var response = await _httpClient.GetAsync(BuildClientsAddress());
                if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                {
                    throw new InvalidOperationException("нет доступа к клиентам.");
                }

                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var clients = JsonSerializer.Deserialize<List<ClientContracts.ClientManagementDto>>(json, JsonOptions)
                    ?? new List<ClientContracts.ClientManagementDto>();

                _allClients.Clear();
                _allClients.AddRange(clients.Select(x => new ClientViewModel(x)));
                _isLoadedOnce = true;

                SetStatus(string.Empty, false);
                RenderClients();
            }
            catch (Exception ex)
            {
                _allClients.Clear();
                _visibleClients.Clear();
                EmptyText.Visibility = Visibility.Visible;
                ClientsScrollViewer.Visibility = Visibility.Collapsed;
                SetStatus("Не удалось загрузить клиентов: " + ex.Message, true);
            }
            finally
            {
                SetBusy(false);
            }
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
                SetStatus("Загрузка категорий клиентов...", false);

                using var response = await _httpClient.GetAsync(EditOptionsAddress);
                if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                {
                    throw new InvalidOperationException("нет доступа к категориям клиентов.");
                }

                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                _editOptions = JsonSerializer.Deserialize<ClientContracts.ClientEditOptionsResponse>(json, JsonOptions)
                    ?? new ClientContracts.ClientEditOptionsResponse();
                _editOptionsLoaded = true;
                SetStatus(string.Empty, false);

                return true;
            }
            catch (Exception ex)
            {
                SetStatus("Не удалось загрузить категории клиентов: " + ex.Message, true);
                return false;
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async void EditCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is not ClientViewModel client)
            {
                return;
            }

            if (!await EnsureEditOptionsLoadedAsync())
            {
                return;
            }

            var window = new ClientCategoryEditWindow(_editOptions, client.Source)
            {
                Owner = Window.GetWindow(this)
            };

            if (window.ShowDialog() != true)
            {
                return;
            }

            await SendCategoryAsync(client.Id, window.Request);
        }

        private async void OrderHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is not ClientViewModel client)
            {
                return;
            }

            await _openOrderHistoryAsync(client.Id, client.FullNameDisplay);
        }

        private async void DeleteClientButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is not ClientViewModel client)
            {
                return;
            }

            var window = new ConfirmDeleteClientWindow(client.FullName, client.PhoneNumber, client.ClientCategoryName, client.OrderCount)
            {
                Owner = Window.GetWindow(this)
            };

            if (window.ShowDialog() != true)
            {
                return;
            }

            await SendDeleteAsync(client);
        }

        private async Task SendCategoryAsync(int clientId, ClientContracts.ClientCategoryUpdateRequest requestBody)
        {
            try
            {
                SetBusy(true);
                SetStatus("Сохранение категории клиента...", false);

                using var request = new HttpRequestMessage(HttpMethod.Put, $"{ClientsAddress}/{clientId}/category")
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
                SetStatus("Категория клиента сохранена.", false);
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

        private async Task SendDeleteAsync(ClientViewModel client)
        {
            try
            {
                SetBusy(true);
                SetStatus("Удаление клиента...", false);

                using var response = await _httpClient.DeleteAsync($"{ClientsAddress}/{client.Id}");
                if (!response.IsSuccessStatusCode)
                {
                    SetStatus(await ReadApiMessageAsync(response), true);
                    return;
                }

                await ReloadAsync();
                SetStatus("Клиент удален.", false);
            }
            catch (Exception ex)
            {
                SetStatus("Не удалось удалить клиента: " + ex.Message, true);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isLoadedOnce)
            {
                return;
            }

            _searchReloadTimer.Stop();
            _searchReloadTimer.Start();
        }

        private async void SearchReloadTimer_Tick(object? sender, EventArgs e)
        {
            _searchReloadTimer.Stop();
            await ReloadAsync();
        }

        private string BuildClientsAddress()
        {
            var search = (SearchTextBox?.Text ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(search))
            {
                return ClientsAddress;
            }

            return ClientsAddress + "?search=" + Uri.EscapeDataString(search);
        }

        private void RenderClients()
        {
            _visibleClients.Clear();
            foreach (var client in _allClients.OrderBy(x => x.FullName).ThenBy(x => x.PhoneNumber))
            {
                _visibleClients.Add(client);
            }

            EmptyText.Visibility = _visibleClients.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            ClientsScrollViewer.Visibility = _visibleClients.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
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
            var message = ExtractApiMessage(content);
            if (!string.IsNullOrWhiteSpace(message))
            {
                return message;
            }

            return response.StatusCode switch
            {
                HttpStatusCode.Conflict => "Операция невозможна из-за связанных данных.",
                HttpStatusCode.NotFound => "Клиент не найден.",
                HttpStatusCode.BadRequest => "Проверьте данные клиента.",
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

        public sealed class ClientViewModel : NotifyBase
        {
            public ClientViewModel(ClientContracts.ClientManagementDto source)
            {
                Source = source;
            }

            public ClientContracts.ClientManagementDto Source { get; }
            public int Id => Source.Id;
            public string FullName => Source.FullName ?? string.Empty;
            public string PhoneNumber => Source.PhoneNumber ?? string.Empty;
            public string ClientCategoryName => Source.ClientCategoryName ?? string.Empty;
            public int OrderCount => Source.OrderCount;
            public string FullNameDisplay => string.IsNullOrWhiteSpace(FullName) ? "Клиент без имени" : FullName;
            public string PhoneDisplay => "Телефон: " + (string.IsNullOrWhiteSpace(PhoneNumber) ? "не указан" : PhoneNumber);
            public string CategoryDisplay => string.IsNullOrWhiteSpace(ClientCategoryName)
                ? "Категория не назначена"
                : "Категория: " + ClientCategoryName;
            public string OrdersDisplay => "Заказов: " + OrderCount;
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
