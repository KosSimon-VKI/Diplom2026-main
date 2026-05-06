using System.Windows;
using System.Windows.Input;

namespace GardenNookWpf.Views.MainPanel.Clients
{
    public partial class ConfirmDeleteClientWindow : Window
    {
        public ConfirmDeleteClientWindow(string fullName, string phoneNumber, string categoryName, int ordersCount)
        {
            InitializeComponent();

            ClientNameText.Text = string.IsNullOrWhiteSpace(fullName)
                ? "Клиент без имени"
                : fullName;
            ClientDetailsText.Text = $"Телефон: {BuildValue(phoneNumber)} | Категория: {BuildValue(categoryName)} | Заказов: {ordersCount}";
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
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

        private static string BuildValue(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "не указано" : value;
        }
    }
}
