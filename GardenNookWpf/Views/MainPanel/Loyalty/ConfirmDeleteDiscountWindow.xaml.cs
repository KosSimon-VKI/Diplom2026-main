using System.Globalization;
using System.Windows;
using System.Windows.Input;

namespace GardenNookWpf.Views.MainPanel.Loyalty
{
    public partial class ConfirmDeleteDiscountWindow : Window
    {
        public ConfirmDeleteDiscountWindow(string discountName, decimal discountPercent, int ordersCount)
        {
            InitializeComponent();

            DiscountNameText.Text = string.IsNullOrWhiteSpace(discountName)
                ? "Без названия"
                : discountName;
            DiscountDetailsText.Text = "Скидка: " + discountPercent.ToString("0.##", CultureInfo.CurrentCulture) + "% | Заказов: " + ordersCount;
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
    }
}
