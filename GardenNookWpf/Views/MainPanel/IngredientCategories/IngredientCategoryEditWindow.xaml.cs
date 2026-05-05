using System;
using System.Windows;
using System.Windows.Input;

namespace GardenNookWpf.Views.MainPanel.IngredientCategories
{
    public partial class IngredientCategoryEditWindow : Window
    {
        public IngredientCategoryEditWindow(string title, string initialName)
        {
            InitializeComponent();

            TitleText.Text = title;
            NameTextBox.Text = initialName ?? string.Empty;

            Loaded += (_, _) =>
            {
                NameTextBox.Focus();
                NameTextBox.SelectAll();
            };
        }

        public string CategoryName { get; private set; } = string.Empty;

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            HideValidation();

            var name = NormalizeName(NameTextBox.Text);
            if (string.IsNullOrWhiteSpace(name))
            {
                ShowValidation("Введите название категории сырья.");
                return;
            }

            CategoryName = name;
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
