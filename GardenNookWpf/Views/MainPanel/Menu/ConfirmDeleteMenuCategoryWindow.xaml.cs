using System.Windows;
using System.Windows.Input;

namespace GardenNookWpf.Views.MainPanel.Menu
{
    public partial class ConfirmDeleteMenuCategoryWindow : Window
    {
        public ConfirmDeleteMenuCategoryWindow(string categoryName, int itemsCount)
        {
            InitializeComponent();

            CategoryNameText.Text = string.IsNullOrWhiteSpace(categoryName)
                ? string.Empty
                : categoryName;
            ItemsCountText.Text = $"Связанных позиций: {itemsCount}";
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
