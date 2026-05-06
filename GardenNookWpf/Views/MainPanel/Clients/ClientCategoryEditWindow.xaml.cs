using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TransferModels.Clients;

namespace GardenNookWpf.Views.MainPanel.Clients
{
    public partial class ClientCategoryEditWindow : Window
    {
        public ClientCategoryEditWindow(ClientEditOptionsResponse options, ClientManagementDto client)
        {
            InitializeComponent();

            ClientNameText.Text = string.IsNullOrWhiteSpace(client.FullName)
                ? "Клиент без имени"
                : client.FullName;
            ClientPhoneText.Text = "Телефон: " + (string.IsNullOrWhiteSpace(client.PhoneNumber) ? "не указан" : client.PhoneNumber);

            CategoryComboBox.ItemsSource = BuildCategoryOptions(options?.Categories);
            CategoryComboBox.SelectedValue = client.ClientCategoryId;

            Loaded += (_, _) => CategoryComboBox.Focus();
        }

        public ClientCategoryUpdateRequest Request { get; private set; } = new ClientCategoryUpdateRequest();

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            HideValidation();

            var categoryId = CategoryComboBox.SelectedValue is int selectedCategoryId
                ? selectedCategoryId
                : (int?)null;

            if (!categoryId.HasValue || categoryId.Value <= 0)
            {
                ShowValidation("Выберите категорию клиента.");
                return;
            }

            Request = new ClientCategoryUpdateRequest
            {
                ClientCategoryId = categoryId
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

        private static List<CategoryOption> BuildCategoryOptions(IReadOnlyCollection<ClientCategoryOptionDto>? source)
        {
            var result = new List<CategoryOption>
            {
                new CategoryOption(null, "Выберите категорию")
            };

            result.AddRange((source ?? Array.Empty<ClientCategoryOptionDto>())
                .OrderBy(x => x.Name)
                .Select(x => new CategoryOption(x.Id, x.Name)));

            return result;
        }

        private sealed class CategoryOption
        {
            public CategoryOption(int? id, string name)
            {
                Id = id;
                Name = name;
            }

            public int? Id { get; }
            public string Name { get; }
        }
    }
}
