using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;

namespace GardenNookWpf.Views.MainPanel.Preparations
{
    public partial class CompletePreparationTaskWindow : Window
    {
        public decimal StockGrams { get; private set; }
        public DateTime ProductionDate { get; private set; }

        public CompletePreparationTaskWindow(string semiFinishedName)
        {
            InitializeComponent();
            SemiFinishedNameText.Text = semiFinishedName;
            ProductionDatePicker.SelectedDate = DateTime.Today;
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            var rawStock = StockTextBox.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(rawStock))
            {
                ShowValidation("Введите массу.");
                return;
            }

            if (!decimal.TryParse(rawStock, NumberStyles.Number, CultureInfo.CurrentCulture, out var stock) &&
                !decimal.TryParse(rawStock, NumberStyles.Number, CultureInfo.InvariantCulture, out stock))
            {
                ShowValidation("Масса введена некорректно.");
                return;
            }

            if (stock <= 0m)
            {
                ShowValidation("Масса должна быть больше нуля.");
                return;
            }

            StockGrams = decimal.Round(stock, 2, MidpointRounding.AwayFromZero);
            ProductionDate = (ProductionDatePicker.SelectedDate ?? DateTime.Today).Date;

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

        private void ShowValidation(string message)
        {
            ValidationText.Text = message;
            ValidationText.Visibility = Visibility.Visible;
        }
    }
}
