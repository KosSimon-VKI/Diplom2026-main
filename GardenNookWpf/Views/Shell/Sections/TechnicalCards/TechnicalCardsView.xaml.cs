using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GardenNookWpf.Views.MainPanel.TechnicalCards;
using GardenNookWpf.Views.Shell;
using GardenNookWpf.Views.Shell.Controllers;
using TransferModels.Kitchen;

namespace GardenNookWpf.Views.Shell.Sections.TechnicalCards
{
    public partial class TechnicalCardsView : UserControl, IMainSectionView
    {
        private const string AdminRole = "Администратор";
        private const string DefaultEmptyMessage = "Технические карты не найдены.";
        private const string SearchEmptyMessage = "По вашему запросу ничего не найдено.";

        private readonly TechnicalCardsSectionController _controller;
        private readonly bool _isAdmin;

        public TechnicalCardsView(HttpClient httpClient, string userRole)
        {
            _controller = new TechnicalCardsSectionController(httpClient);
            _isAdmin = string.Equals(userRole, AdminRole, StringComparison.CurrentCultureIgnoreCase);

            InitializeComponent();

            EmptyText.Text = DefaultEmptyMessage;
            AddTechnicalCardButton.Visibility = _isAdmin ? Visibility.Visible : Visibility.Collapsed;
            _controller.BusyStateChanged += Controller_BusyStateChanged;
        }

        public bool IsBusy => _controller.IsBusy;

        public async Task ActivateAsync()
        {
            SetStatus(string.Empty);
            var result = await _controller.ReloadAsync();
            if (!result.Success)
            {
                ClearCards();
                SetStatus(result.Message);
                return;
            }

            ApplySearchFilter();
            SetStatus(_controller.HasCards ? string.Empty : DefaultEmptyMessage);
        }

        public void Deactivate()
        {
            _controller.Deactivate();
        }

        private async void TechnicalCard_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button ||
                button.Tag is not TechnicalCardsSectionController.TechnicalCardListItemViewModel card)
            {
                return;
            }

            if (_controller.IsBusy)
            {
                return;
            }

            SetStatus(string.Empty);
            var result = await _controller.LoadTechnicalCardAsync(card.TechnicalCardId);
            if (!result.Success || result.Card == null)
            {
                SetStatus(result.Message);
                return;
            }

            var detailsWindow = new TechnicalCardWindow(result.Card, _isAdmin);
            var owner = Window.GetWindow(this);
            if (owner != null)
            {
                detailsWindow.Owner = owner;
            }

            detailsWindow.ShowDialog();

            if (!_isAdmin)
            {
                return;
            }

            if (detailsWindow.EditRequested)
            {
                await OpenEditWindowAsync(card.TechnicalCardId);
                return;
            }

            if (detailsWindow.DeleteRequested)
            {
                await DeleteTechnicalCardAsync(card);
            }
        }

        private async void AddTechnicalCard_Click(object sender, RoutedEventArgs e)
        {
            if (_controller.IsBusy)
            {
                return;
            }

            await OpenEditWindowAsync(null);
        }

        private async Task OpenEditWindowAsync(int? technicalCardId)
        {
            SetStatus(string.Empty);

            var optionsResult = await _controller.LoadEditOptionsAsync();
            if (!optionsResult.Success || optionsResult.Options == null)
            {
                SetStatus(optionsResult.Message);
                return;
            }

            KitchenTechnicalCardEditResponse? editCard = null;
            if (technicalCardId.HasValue)
            {
                var cardResult = await _controller.LoadTechnicalCardForEditAsync(technicalCardId.Value);
                if (!cardResult.Success || cardResult.Card == null)
                {
                    SetStatus(cardResult.Message);
                    return;
                }

                editCard = cardResult.Card;
            }

            var editWindow = new TechnicalCardEditWindow(optionsResult.Options, editCard);
            var owner = Window.GetWindow(this);
            if (owner != null)
            {
                editWindow.Owner = owner;
            }

            if (editWindow.ShowDialog() != true || editWindow.Request == null)
            {
                return;
            }

            var saveResult = technicalCardId.HasValue
                ? await _controller.UpdateTechnicalCardAsync(technicalCardId.Value, editWindow.Request)
                : await _controller.CreateTechnicalCardAsync(editWindow.Request);

            if (!saveResult.Success)
            {
                SetStatus(saveResult.Message);
                return;
            }

            var reloadResult = await _controller.ReloadAsync();
            if (!reloadResult.Success)
            {
                ClearCards();
                SetStatus(reloadResult.Message);
                return;
            }

            ApplySearchFilter();
            SetStatus("Техкарта сохранена.");
        }

        private async Task DeleteTechnicalCardAsync(TechnicalCardsSectionController.TechnicalCardListItemViewModel card)
        {
            var confirmationWindow = new ConfirmDeleteTechnicalCardWindow(card.CardName);
            var owner = Window.GetWindow(this);
            if (owner != null)
            {
                confirmationWindow.Owner = owner;
            }

            if (confirmationWindow.ShowDialog() != true)
            {
                return;
            }

            SetStatus(string.Empty);
            var deleteResult = await _controller.DeleteTechnicalCardAsync(card.TechnicalCardId);
            if (!deleteResult.Success)
            {
                SetStatus(deleteResult.Message);
                return;
            }

            var reloadResult = await _controller.ReloadAsync();
            if (!reloadResult.Success)
            {
                ClearCards();
                SetStatus(reloadResult.Message);
                return;
            }

            ApplySearchFilter();
            SetStatus("Техкарта удалена.");
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_controller.IsBusy)
            {
                return;
            }

            ApplySearchFilter();
        }

        private void ShowEmptyState(bool isEmpty)
        {
            CardsScrollViewer.Visibility = isEmpty ? Visibility.Collapsed : Visibility.Visible;
            EmptyText.Visibility = isEmpty ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ClearCards()
        {
            CardsList.ItemsSource = null;
            EmptyText.Text = DefaultEmptyMessage;
            ShowEmptyState(true);
        }

        private void ApplySearchFilter()
        {
            var visibleCards = _controller.FilterCards(SearchTextBox.Text);
            CardsList.ItemsSource = visibleCards;

            if (!_controller.HasCards)
            {
                EmptyText.Text = DefaultEmptyMessage;
                ShowEmptyState(true);
                return;
            }

            if (visibleCards.Count == 0)
            {
                EmptyText.Text = SearchEmptyMessage;
                ShowEmptyState(true);
                return;
            }

            EmptyText.Text = DefaultEmptyMessage;
            ShowEmptyState(false);
        }

        private void SetStatus(string message)
        {
            StatusText.Text = message;
            StatusBorder.Visibility = string.IsNullOrWhiteSpace(message) ? Visibility.Collapsed : Visibility.Visible;
        }

        private void Controller_BusyStateChanged(bool isBusy)
        {
            RootGrid.IsEnabled = !isBusy;
        }
    }
}
