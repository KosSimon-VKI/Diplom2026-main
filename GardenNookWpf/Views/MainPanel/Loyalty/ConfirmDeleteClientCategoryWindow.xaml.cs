using System.Windows;
using System.Windows.Input;

namespace GardenNookWpf.Views.MainPanel.Loyalty
{
    public partial class ConfirmDeleteClientCategoryWindow : Window
    {
        public ConfirmDeleteClientCategoryWindow(string categoryName, int clientsCount)
        {
            InitializeComponent();

            CategoryNameText.Text = string.IsNullOrWhiteSpace(categoryName)
                ? "Без названия"
                : categoryName;
            ClientsCountText.Text = "Клиентов: " + clientsCount;
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
