using System.Windows;
using System.Windows.Input;

namespace GardenNookWpf.Views.MainPanel.WriteOff
{
    public partial class ConfirmDeleteWriteOffActWindow : Window
    {
        public ConfirmDeleteWriteOffActWindow(string actTitle)
        {
            InitializeComponent();
            ActTitleText.Text = string.IsNullOrWhiteSpace(actTitle) ? string.Empty : actTitle;
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void HeaderClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
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
