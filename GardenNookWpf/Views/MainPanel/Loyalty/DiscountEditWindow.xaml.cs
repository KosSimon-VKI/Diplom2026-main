using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using LoyaltyContracts = TransferModels.Loyalty;

namespace GardenNookWpf.Views.MainPanel.Loyalty
{
    public partial class DiscountEditWindow : Window
    {
        public DiscountEditWindow(LoyaltyContracts.DiscountManagementDto? existingDiscount = null)
        {
            InitializeComponent();

            TitleText.Text = existingDiscount == null ? "Добавить скидку" : "Редактировать скидку";
            NameTextBox.Text = existingDiscount?.Name ?? string.Empty;
            PercentTextBox.Text = existingDiscount == null
                ? string.Empty
                : existingDiscount.DiscountPercent.ToString("0.##", CultureInfo.CurrentCulture);

            Loaded += (_, _) =>
            {
                NameTextBox.Focus();
                NameTextBox.SelectAll();
            };
        }

        public LoyaltyContracts.DiscountUpsertRequest Request { get; private set; } = new LoyaltyContracts.DiscountUpsertRequest();

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            HideValidation();

            var name = NormalizeName(NameTextBox.Text);
            if (string.IsNullOrWhiteSpace(name))
            {
                ShowValidation("Введите название скидки.");
                return;
            }

            if (!TryParseDecimal(PercentTextBox.Text, out var percent))
            {
                ShowValidation("Введите корректный процент скидки.");
                return;
            }

            if (percent < 0m || percent > 100m)
            {
                ShowValidation("Процент скидки должен быть от 0 до 100.");
                return;
            }

            Request = new LoyaltyContracts.DiscountUpsertRequest
            {
                Name = name,
                DiscountPercent = Math.Round(percent, 2, MidpointRounding.AwayFromZero)
            };
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

        private void HideValidation()
        {
            ValidationText.Text = string.Empty;
            ValidationText.Visibility = Visibility.Collapsed;
        }

        private static bool TryParseDecimal(string? value, out decimal result)
        {
            var text = (value ?? string.Empty).Trim();
            if (decimal.TryParse(text, NumberStyles.Number, CultureInfo.CurrentCulture, out result))
            {
                return true;
            }

            return decimal.TryParse(text.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out result);
        }

        private static string NormalizeName(string? value)
        {
            return string.Join(" ", (value ?? string.Empty)
                .Trim()
                .Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
        }
    }
}
