using System.Windows;
using System.Windows.Input;

namespace GardenNookWpf.Views.MainPanel.Staff
{
    public partial class ConfirmDeleteStaffWindow : Window
    {
        public ConfirmDeleteStaffWindow(string fullName, string login, string roleName)
        {
            InitializeComponent();

            StaffNameText.Text = string.IsNullOrWhiteSpace(fullName)
                ? "Без имени"
                : fullName;
            StaffDetailsText.Text = $"Логин: {login} | Роль: {roleName}";
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
