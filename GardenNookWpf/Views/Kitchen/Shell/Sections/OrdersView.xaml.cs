using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GardenNookWpf.Views.Controls;
using GardenNookWpf.Views.Kitchen;
using GardenNookWpf.Views.Kitchen.Shell;
using GardenNookWpf.Views.Kitchen.Shell.Controllers;

namespace GardenNookWpf.Views.Kitchen.Shell.Sections
{
    public partial class OrdersView : UserControl, IKitchenSectionView
    {
        private readonly HttpClient _httpClient;
        private readonly string _userRole;
        private readonly OrdersSectionController _controller;

        public OrdersView(HttpClient httpClient, string userRole)
        {
            _httpClient = httpClient;
            _userRole = userRole?.Trim() ?? string.Empty;
            _controller = new OrdersSectionController(httpClient, _userRole);

            InitializeComponent();

            _controller.StateChanged += Controller_StateChanged;
            _controller.BusyStateChanged += Controller_BusyStateChanged;
            OrdersDisplayControl.OrderCardClicked += OrdersDisplayControl_OrderCardClicked;
        }

        public bool IsBusy => _controller.IsBusy;

        public Task ActivateAsync()
        {
            return _controller.ActivateAsync();
        }

        public void Deactivate()
        {
            _controller.Deactivate();
        }

        private async void OrdersDisplayControl_OrderCardClicked(object? sender, KitchenOrderCardViewModel card)
        {
            var detailsWindow = new OrderDetailsWindow(_httpClient, _controller.GetCardForDetails(card), _userRole);
            var owner = Window.GetWindow(this);
            if (owner != null)
            {
                detailsWindow.Owner = owner;
            }

            detailsWindow.OrderUpdated += OrderDetailsWindow_OrderUpdated;
            detailsWindow.ShowDialog();
            detailsWindow.OrderUpdated -= OrderDetailsWindow_OrderUpdated;

            await _controller.ReloadAsync();
        }

        private async void OrderDetailsWindow_OrderUpdated(object? sender, EventArgs e)
        {
            await _controller.ReloadAsync();
        }

        private void Controller_StateChanged(OrdersSectionController.OrdersDisplayState state)
        {
            if (state.IsMessageOnly)
            {
                OrdersDisplayControl.ShowMessage(state.MessageText);
                return;
            }

            OrdersDisplayControl.ShowOrders(
                state.PickupCards,
                state.NoPickupCards,
                state.MessageText);
        }

        private void Controller_BusyStateChanged(bool isBusy)
        {
            RootGrid.IsEnabled = !isBusy;
        }
    }
}
