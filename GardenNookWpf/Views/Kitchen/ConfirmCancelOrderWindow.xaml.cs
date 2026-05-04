using System.Windows;
using System.Windows.Input;

namespace GardenNookWpf.Views.Kitchen
{
    public partial class ConfirmCancelOrderWindow : Window
    {
        public ConfirmCancelOrderWindow(string orderNumberText)
        {
            InitializeComponent();

            MessageTextBlock.Text = string.IsNullOrWhiteSpace(orderNumberText)
                ? "Вы уверены, что хотите отменить заказ?"
                : $"Вы уверены, что хотите отменить заказ №{orderNumberText}?";
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
