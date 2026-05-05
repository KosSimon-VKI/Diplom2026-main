using System.Windows;
using System.Windows.Input;

namespace GardenNookWpf.Views.MainPanel.Menu
{
    public partial class ConfirmDeleteMenuItemWindow : Window
    {
        public ConfirmDeleteMenuItemWindow(string itemName, string itemType)
        {
            InitializeComponent();

            ItemNameText.Text = string.IsNullOrWhiteSpace(itemName)
                ? "Без названия"
                : itemName;
            ItemTypeText.Text = string.IsNullOrWhiteSpace(itemType)
                ? string.Empty
                : "Тип: " + itemType;
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
