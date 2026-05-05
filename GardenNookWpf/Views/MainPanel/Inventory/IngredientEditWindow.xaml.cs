using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TransferModels.Inventory;

namespace GardenNookWpf.Views.MainPanel.Inventory
{
    public partial class IngredientEditWindow : Window
    {
        public IngredientEditWindow(InventoryEditOptionsResponse options, InventoryIngredientDto? ingredient)
        {
            InitializeComponent();

            TitleText.Text = ingredient == null ? "Добавить сырье" : "Редактировать сырье";
            SaveButton.Content = ingredient == null ? "Добавить" : "Сохранить";

            UnitComboBox.ItemsSource = BuildOptions(options?.UnitsOfMeasure);
            CategoryComboBox.ItemsSource = BuildOptions(options?.IngredientCategories);
            FillFields(ingredient);

            Loaded += (_, _) =>
            {
                NameTextBox.Focus();
                NameTextBox.SelectAll();
            };
        }

        public InventoryIngredientRequest Request { get; private set; } = new InventoryIngredientRequest();

        private void FillFields(InventoryIngredientDto? ingredient)
        {
            NameTextBox.Text = ingredient?.Name ?? string.Empty;
            StockTextBox.Text = FormatDecimal(ingredient?.Stock ?? 0m);
            CostTextBox.Text = FormatDecimal(ingredient?.CostRub ?? 0m);
            UnitComboBox.SelectedValue = ingredient?.UnitOfMeasureId;
            CategoryComboBox.SelectedValue = ingredient?.CategoryId;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            HideValidation();

            var name = NormalizeName(NameTextBox.Text);
            if (string.IsNullOrWhiteSpace(name))
            {
                ShowValidation("Введите название сырья.");
                return;
            }

            if (!TryReadDecimal(StockTextBox.Text, "остаток", out var stock) ||
                !TryReadDecimal(CostTextBox.Text, "себестоимость", out var cost))
            {
                return;
            }

            Request = new InventoryIngredientRequest
            {
                Name = name,
                Stock = stock,
                CostRub = cost,
                UnitOfMeasureId = UnitComboBox.SelectedValue as int?,
                CategoryId = CategoryComboBox.SelectedValue as int?
            };

            DialogResult = true;
        }

        private bool TryReadDecimal(string value, string fieldName, out decimal result)
        {
            var normalized = (value ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(normalized))
            {
                result = 0m;
                return true;
            }

            normalized = normalized.Replace(',', '.');
            if (!decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.InvariantCulture, out result))
            {
                ShowValidation($"Проверьте поле \"{fieldName}\".");
                return false;
            }

            if (result < 0)
            {
                ShowValidation($"Поле \"{fieldName}\" не может быть отрицательным.");
                return false;
            }

            return true;
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

        private static List<NullableOption> BuildOptions(IReadOnlyCollection<InventoryOptionDto>? source)
        {
            var result = new List<NullableOption>
            {
                new NullableOption(null, "Не выбрано")
            };

            result.AddRange((source ?? Array.Empty<InventoryOptionDto>())
                .OrderBy(x => x.Name)
                .Select(x => new NullableOption(x.Id, x.Name)));

            return result;
        }

        private static string NormalizeName(string? value)
        {
            return string.Join(" ", (value ?? string.Empty)
                .Trim()
                .Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
        }

        private static string FormatDecimal(decimal value)
        {
            return value.ToString("0.##", CultureInfo.InvariantCulture);
        }

        private sealed class NullableOption
        {
            public NullableOption(int? id, string name)
            {
                Id = id;
                Name = name;
            }

            public int? Id { get; }
            public string Name { get; }
        }
    }
}
