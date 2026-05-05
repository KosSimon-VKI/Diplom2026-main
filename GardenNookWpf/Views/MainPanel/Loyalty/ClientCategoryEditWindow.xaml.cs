using System.Windows;
using System.Windows.Input;
using LoyaltyContracts = TransferModels.Loyalty;

namespace GardenNookWpf.Views.MainPanel.Loyalty
{
    public partial class ClientCategoryEditWindow : Window
    {
        public ClientCategoryEditWindow(LoyaltyContracts.ClientCategoryManagementDto? existingCategory = null)
        {
            InitializeComponent();

            TitleText.Text = existingCategory == null ? "Добавить категорию клиента" : "Редактировать категорию клиента";
            NameTextBox.Text = existingCategory?.Name ?? string.Empty;

            Loaded += (_, _) =>
            {
                NameTextBox.Focus();
                NameTextBox.SelectAll();
            };
        }

        public LoyaltyContracts.ClientCategoryUpsertRequest Request { get; private set; } = new LoyaltyContracts.ClientCategoryUpsertRequest();

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            HideValidation();

            var name = NormalizeName(NameTextBox.Text);
            if (string.IsNullOrWhiteSpace(name))
            {
                ShowValidation("Введите название категории клиента.");
                return;
            }

            Request = new LoyaltyContracts.ClientCategoryUpsertRequest
            {
                Name = name
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

        private static string NormalizeName(string? value)
        {
            return string.Join(" ", (value ?? string.Empty)
                .Trim()
                .Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
        }
    }
}
