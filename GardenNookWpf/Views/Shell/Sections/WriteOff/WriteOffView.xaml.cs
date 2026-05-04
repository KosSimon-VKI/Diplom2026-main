using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GardenNookWpf.Views.MainPanel.WriteOff;
using GardenNookWpf.Views.Shell;
using GardenNookWpf.Views.Shell.Controllers;

namespace GardenNookWpf.Views.Shell.Sections.WriteOff
{
    public partial class WriteOffView : UserControl, IMainSectionView
    {
        private readonly WriteOffSectionController _controller;

        public WriteOffView(HttpClient httpClient, string userRole)
        {
            _controller = new WriteOffSectionController(httpClient, userRole);

            InitializeComponent();

            _controller.BusyStateChanged += Controller_BusyStateChanged;
        }

        public bool IsBusy => _controller.IsBusy;

        public async Task ActivateAsync()
        {
            SetStatus(string.Empty);
            var result = await _controller.LoadBoardAsync();
            if (!result.Success)
            {
                HistoryList.ItemsSource = null;
                HistoryScrollViewer.Visibility = Visibility.Collapsed;
                EmptyText.Visibility = Visibility.Visible;
                SetStatus(result.Message);
                return;
            }

            RenderHistory();
        }

        public void Deactivate()
        {
            _controller.Deactivate();
        }

        private async void AddActButton_Click(object sender, RoutedEventArgs e)
        {
            if (_controller.IsBusy)
            {
                return;
            }

            if (_controller.WriteOffTypes.Count == 0)
            {
                SetStatus("Справочник типов списания пуст.");
                return;
            }

            var addWindow = new AddWriteOffActWindow(
                _controller.WriteOffTypes,
                _controller.SemiFinishedOptions,
                _controller.IngredientOptions)
            {
                Owner = Window.GetWindow(this)
            };

            if (addWindow.ShowDialog() != true)
            {
                return;
            }

            var result = await _controller.CreateActAsync(addWindow.Request);
            if (!result.Success)
            {
                SetStatus(result.Message);
                return;
            }

            SetStatus(string.Empty);
            RenderHistory();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_controller.IsBusy)
            {
                return;
            }

            RenderHistory();
        }

        private async void DeleteActButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button ||
                button.Tag is not WriteOffSectionController.WriteOffActDisplayModel act)
            {
                return;
            }

            if (_controller.IsBusy)
            {
                return;
            }

            var confirmationWindow = new ConfirmDeleteWriteOffActWindow(act.HeaderDisplay)
            {
                Owner = Window.GetWindow(this)
            };

            if (confirmationWindow.ShowDialog() != true)
            {
                return;
            }

            var result = await _controller.DeleteActAsync(act.ActId);
            if (!result.Success)
            {
                SetStatus(result.Message);
                return;
            }

            SetStatus(string.Empty);
            RenderHistory();
        }

        private void RenderHistory()
        {
            var filtered = _controller.Filter(SearchTextBox.Text);
            HistoryList.ItemsSource = filtered.VisibleItems;
            HistoryScrollViewer.Visibility = filtered.VisibleItems.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
            EmptyText.Visibility = filtered.VisibleItems.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            SetStatus(filtered.StatusMessage);
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
