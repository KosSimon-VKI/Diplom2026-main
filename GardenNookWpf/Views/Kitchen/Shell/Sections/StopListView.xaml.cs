using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GardenNookWpf.Views.Kitchen;
using GardenNookWpf.Views.Kitchen.Shell;
using GardenNookWpf.Views.Kitchen.Shell.Controllers;

namespace GardenNookWpf.Views.Kitchen.Shell.Sections
{
    public partial class StopListView : UserControl, IKitchenSectionView
    {
        private readonly StopListSectionController _controller;

        public StopListView(HttpClient httpClient, string userRole)
        {
            _controller = new StopListSectionController(httpClient, userRole);

            InitializeComponent();

            _controller.BusyStateChanged += Controller_BusyStateChanged;
        }

        public bool IsBusy => _controller.IsBusy;

        public async Task ActivateAsync()
        {
            SetStatus(string.Empty);
            var result = await _controller.ReloadAsync();
            if (!result.Success)
            {
                PositionsList.ItemsSource = null;
                PositionsScrollViewer.Visibility = Visibility.Collapsed;
                EmptyText.Visibility = Visibility.Visible;
                SetStatus(result.Message);
                return;
            }

            RenderStopList();
        }

        public void Deactivate()
        {
            _controller.Deactivate();
        }

        private async void AddPositionButton_Click(object sender, RoutedEventArgs e)
        {
            if (_controller.IsBusy)
            {
                return;
            }

            if (_controller.AllPositions.Count == 0)
            {
                SetStatus("Список позиций не загружен. Обновите окно стоп-листа.");
                return;
            }

            var addWindow = new AddStopListItemWindow(_controller.AllPositions.ToList());
            var owner = Window.GetWindow(this);
            if (owner != null)
            {
                addWindow.Owner = owner;
            }

            if (addWindow.ShowDialog() != true)
            {
                return;
            }

            SetStatus(string.Empty);
            var result = await _controller.AddPositionToStopListAsync(
                addWindow.SelectedItemType,
                addWindow.SelectedItemId,
                addWindow.SelectedRemainingPortions);

            if (!result.Success)
            {
                SetStatus(result.Message);
                return;
            }

            RenderStopList();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_controller.IsBusy)
            {
                return;
            }

            RenderStopList();
        }

        private async void RemovePositionButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button || button.Tag is not StopListPositionDisplayModel item)
            {
                return;
            }

            if (_controller.IsBusy)
            {
                return;
            }

            SetStatus(string.Empty);
            var result = await _controller.RemovePositionFromStopListAsync(item.ItemType, item.ItemId);
            if (!result.Success)
            {
                SetStatus(result.Message);
                return;
            }

            RenderStopList();
        }

        private void RenderStopList()
        {
            var filter = _controller.Filter(SearchTextBox.Text);
            PositionsList.ItemsSource = filter.VisibleItems;
            PositionsScrollViewer.Visibility = filter.VisibleItems.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
            EmptyText.Visibility = filter.VisibleItems.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            SetStatus(filter.StatusMessage);
        }

        private void SetStatus(string message)
        {
            StatusText.Text = message;
            StatusText.Visibility = string.IsNullOrWhiteSpace(message)
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        private void Controller_BusyStateChanged(bool isBusy)
        {
            RootGrid.IsEnabled = !isBusy;
        }
    }
}
