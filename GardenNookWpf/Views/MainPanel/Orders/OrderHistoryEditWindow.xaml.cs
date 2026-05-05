using System;
using System.Net.Http;
using System.Windows;
using System.Windows.Input;
using GardenNookWpf.Views.Shell.Sections.Menu;
using TransferModels.Menu;
using TransferModels.Orders;

namespace GardenNookWpf.Views.MainPanel.Orders
{
    public partial class OrderHistoryEditWindow : Window
    {
        private readonly OrderItemView _editor;

        public OrderHistoryEditWindow(HttpClient httpClient, OrderHistoryDetailsDto details, MenuResponse? menu = null)
        {
            InitializeComponent();

            HeaderTitleText.Text = $"Редактировать заказ №{details.OrderId}";
            _editor = new OrderItemView(httpClient, "Администратор", details);
            _editor.EditSaved += Editor_EditSaved;
            EditorHost.Content = _editor;
            Loaded += OrderHistoryEditWindow_Loaded;
        }

        public OrderHistoryUpdateRequest Request { get; private set; } = new OrderHistoryUpdateRequest();

        private async void OrderHistoryEditWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OrderHistoryEditWindow_Loaded;
            await _editor.ActivateAsync();
        }

        private void Editor_EditSaved(object? sender, EventArgs e)
        {
            if (_editor.EditRequest == null)
            {
                return;
            }

            Request = _editor.EditRequest;
            DialogResult = true;
            Close();
        }

        private void HeaderClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}
