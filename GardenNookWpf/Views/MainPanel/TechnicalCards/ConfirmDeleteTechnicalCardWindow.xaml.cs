using System.Windows;
using System.Windows.Input;

namespace GardenNookWpf.Views.MainPanel.TechnicalCards
{
    public partial class ConfirmDeleteTechnicalCardWindow : Window
    {
        public ConfirmDeleteTechnicalCardWindow(string cardName)
        {
            InitializeComponent();
            CardNameText.Text = string.IsNullOrWhiteSpace(cardName) ? string.Empty : cardName;
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
