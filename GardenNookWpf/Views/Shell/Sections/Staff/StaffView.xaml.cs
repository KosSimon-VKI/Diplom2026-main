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
using GardenNookWpf.Views.MainPanel.Staff;
using GardenNookWpf.Views.Shell;
using StaffContracts = TransferModels.Staff;

namespace GardenNookWpf.Views.Shell.Sections.Staff
{
    public partial class StaffView : UserControl, IMainSectionView
    {
        private const string ApiBaseAddress = "https://localhost:7235";
        private const string StaffAddress = ApiBaseAddress + "/api/staff";
        private const string EditOptionsAddress = StaffAddress + "/edit-options";

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly HttpClient _httpClient;
        private readonly ObservableCollection<StaffViewModel> _visibleStaff = new ObservableCollection<StaffViewModel>();
        private readonly List<StaffViewModel> _allStaff = new List<StaffViewModel>();
        private readonly DispatcherTimer _searchReloadTimer;
        private StaffContracts.StaffEditOptionsResponse _editOptions = new StaffContracts.StaffEditOptionsResponse();
        private bool _editOptionsLoaded;
        private bool _isLoadedOnce;
        private bool _isBusy;

        public StaffView(HttpClient httpClient, string userRole)
        {
            _httpClient = httpClient;
            InitializeComponent();

            StaffList.ItemsSource = _visibleStaff;
            _searchReloadTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(350)
            };
            _searchReloadTimer.Tick += SearchReloadTimer_Tick;
            UpdateActionButtons();
        }

        public bool IsBusy => _isBusy;

        public async Task ActivateAsync()
        {
            if (_isLoadedOnce)
            {
                RenderStaff();
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
                SetStatus("Загрузка персонала...", false);

                using var response = await _httpClient.GetAsync(BuildStaffAddress());
                if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                {
                    throw new InvalidOperationException("нет доступа к персоналу.");
                }

                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var staff = JsonSerializer.Deserialize<List<StaffContracts.StaffManagementDto>>(json, JsonOptions)
                    ?? new List<StaffContracts.StaffManagementDto>();

                _allStaff.Clear();
                _allStaff.AddRange(staff.Select(x => new StaffViewModel(x)));
                _isLoadedOnce = true;

                SetStatus(string.Empty, false);
                RenderStaff();
            }
            catch (Exception ex)
            {
                _allStaff.Clear();
                _visibleStaff.Clear();
                EmptyText.Visibility = Visibility.Visible;
                StaffScrollViewer.Visibility = Visibility.Collapsed;
                SetStatus("Не удалось загрузить персонал: " + ex.Message, true);
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
                SetStatus("Загрузка ролей персонала...", false);

                using var response = await _httpClient.GetAsync(EditOptionsAddress);
                if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                {
                    throw new InvalidOperationException("нет доступа к ролям персонала.");
                }

                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                _editOptions = JsonSerializer.Deserialize<StaffContracts.StaffEditOptionsResponse>(json, JsonOptions)
                    ?? new StaffContracts.StaffEditOptionsResponse();
                _editOptionsLoaded = true;
                SetStatus(string.Empty, false);

                return true;
            }
            catch (Exception ex)
            {
                SetStatus("Не удалось загрузить роли персонала: " + ex.Message, true);
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

            var window = new StaffEditWindow(_editOptions, null)
            {
                Owner = Window.GetWindow(this)
            };

            if (window.ShowDialog() != true)
            {
                return;
            }

            await SendUpsertAsync(HttpMethod.Post, StaffAddress, window.Request);
        }

        private async void EditStaffButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is not StaffViewModel staff)
            {
                return;
            }

            if (!await EnsureEditOptionsLoadedAsync())
            {
                return;
            }

            var window = new StaffEditWindow(_editOptions, staff.Source)
            {
                Owner = Window.GetWindow(this)
            };

            if (window.ShowDialog() != true)
            {
                return;
            }

            await SendUpsertAsync(HttpMethod.Put, $"{StaffAddress}/{staff.Id}", window.Request);
        }

        private async void DeleteStaffButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is not StaffViewModel staff)
            {
                return;
            }

            var window = new ConfirmDeleteStaffWindow(staff.FullName, staff.Login, staff.RoleName)
            {
                Owner = Window.GetWindow(this)
            };

            if (window.ShowDialog() != true)
            {
                return;
            }

            await SendDeleteAsync(staff);
        }

        private async Task SendUpsertAsync(HttpMethod method, string address, StaffContracts.StaffUpsertRequest requestBody)
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

        private async Task SendDeleteAsync(StaffViewModel staff)
        {
            try
            {
                SetBusy(true);
                SetStatus("Удаление сотрудника...", false);

                using var response = await _httpClient.DeleteAsync($"{StaffAddress}/{staff.Id}");
                if (!response.IsSuccessStatusCode)
                {
                    SetStatus(await ReadApiMessageAsync(response), true);
                    return;
                }

                await ReloadAsync();
                SetStatus("Сотрудник удален.", false);
            }
            catch (Exception ex)
            {
                SetStatus("Не удалось удалить сотрудника: " + ex.Message, true);
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

        private string BuildStaffAddress()
        {
            var search = (SearchTextBox?.Text ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(search))
            {
                return StaffAddress;
            }

            return StaffAddress + "?search=" + Uri.EscapeDataString(search);
        }

        private void RenderStaff()
        {
            _visibleStaff.Clear();
            foreach (var staff in _allStaff.OrderBy(x => x.FullName).ThenBy(x => x.Login))
            {
                _visibleStaff.Add(staff);
            }

            EmptyText.Visibility = _visibleStaff.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            StaffScrollViewer.Visibility = _visibleStaff.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
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
            if (AddButton != null)
            {
                AddButton.IsEnabled = !_isBusy;
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
                HttpStatusCode.NotFound => "Сотрудник не найден.",
                HttpStatusCode.BadRequest => "Проверьте данные сотрудника.",
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

        public sealed class StaffViewModel : NotifyBase
        {
            public StaffViewModel(StaffContracts.StaffManagementDto source)
            {
                Source = source;
            }

            public StaffContracts.StaffManagementDto Source { get; }
            public int Id => Source.Id;
            public string FullName => Source.FullName ?? string.Empty;
            public string Login => Source.Login ?? string.Empty;
            public string RoleName => Source.RoleName ?? string.Empty;
            public string LoginDisplay => "Логин: " + Login;
            public string RoleDisplay => string.IsNullOrWhiteSpace(RoleName)
                ? "Роль не назначена"
                : "Роль: " + RoleName;
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
