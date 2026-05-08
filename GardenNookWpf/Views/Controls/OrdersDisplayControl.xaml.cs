using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GardenNookWpf.Views.Controls
{
    /// <summary>
    /// Interaction logic for OrdersDisplayControl.xaml
    /// </summary>
    public partial class OrdersDisplayControl : UserControl
    {
        public event EventHandler<KitchenOrderCardViewModel>? OrderCardClicked;
        public event EventHandler<KitchenOrdersStatusFilter>? StatusFilterChanged;
        private bool _isUpdatingStatusFilter;

        public OrdersDisplayControl()
        {
            InitializeComponent();
            SetStatusFilter(SelectedStatusFilter);
        }

        public KitchenOrdersStatusFilter SelectedStatusFilter { get; private set; } = KitchenOrdersStatusFilter.Active;

        public void SetStatusFilter(KitchenOrdersStatusFilter statusFilter)
        {
            _isUpdatingStatusFilter = true;
            SelectedStatusFilter = statusFilter;
            ActiveOrdersToggle.IsChecked = statusFilter == KitchenOrdersStatusFilter.Active;
            ReadyOrdersToggle.IsChecked = statusFilter == KitchenOrdersStatusFilter.Ready;
            _isUpdatingStatusFilter = false;
        }

        public void ShowOrders(
            IReadOnlyCollection<KitchenOrderCardViewModel> pickupCards,
            IReadOnlyCollection<KitchenOrderCardViewModel> noPickupCards,
            string emptyText)
        {
            var allCards = new List<KitchenOrderCardViewModel>(pickupCards.Count + noPickupCards.Count);
            allCards.AddRange(pickupCards);
            allCards.AddRange(noPickupCards);

            OrdersItemsControl.ItemsSource = allCards;
            var hasOrders = allCards.Count > 0;
            OrdersScrollViewer.Visibility = hasOrders ? Visibility.Visible : Visibility.Collapsed;

            EmptyOrdersText.Text = emptyText;
            EmptyOrdersText.Visibility = hasOrders ? Visibility.Collapsed : Visibility.Visible;
        }

        public void ShowMessage(string message)
        {
            OrdersItemsControl.ItemsSource = null;
            OrdersScrollViewer.Visibility = Visibility.Collapsed;
            EmptyOrdersText.Text = message;
            EmptyOrdersText.Visibility = Visibility.Visible;
        }

        private void OrderCardBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is not FrameworkElement element)
            {
                return;
            }

            if (element.DataContext is not KitchenOrderCardViewModel card)
            {
                return;
            }

            OrderCardClicked?.Invoke(this, card);
        }

        private void ActiveOrdersToggle_Checked(object sender, RoutedEventArgs e)
        {
            ChangeStatusFilter(KitchenOrdersStatusFilter.Active);
        }

        private void ReadyOrdersToggle_Checked(object sender, RoutedEventArgs e)
        {
            ChangeStatusFilter(KitchenOrdersStatusFilter.Ready);
        }

        private void ChangeStatusFilter(KitchenOrdersStatusFilter statusFilter)
        {
            if (_isUpdatingStatusFilter)
            {
                return;
            }

            if (SelectedStatusFilter == statusFilter)
            {
                SetStatusFilter(statusFilter);
                return;
            }

            SetStatusFilter(statusFilter);
            StatusFilterChanged?.Invoke(this, statusFilter);
        }
    }
}
