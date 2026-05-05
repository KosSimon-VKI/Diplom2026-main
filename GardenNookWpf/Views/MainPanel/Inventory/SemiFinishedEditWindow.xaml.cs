using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TransferModels.Inventory;

namespace GardenNookWpf.Views.MainPanel.Inventory
{
    public partial class SemiFinishedEditWindow : Window
    {
        private List<NullableOption> _technicalCardOptions = new List<NullableOption>();

        public SemiFinishedEditWindow(InventoryEditOptionsResponse options, InventorySemiFinishedDto? semiFinished)
        {
            InitializeComponent();

            TitleText.Text = semiFinished == null ? "Добавить полуфабрикат" : "Редактировать полуфабрикат";
            SaveButton.Content = semiFinished == null ? "Добавить" : "Сохранить";

            UnitComboBox.ItemsSource = BuildOptions(options?.UnitsOfMeasure);
            CategoryComboBox.ItemsSource = BuildOptions(options?.SemiFinishedCategories);
            _technicalCardOptions = BuildOptions(options?.TechnicalCards);
            ApplyTechnicalCardFilter(null);
            FillFields(semiFinished);

            Loaded += (_, _) =>
            {
                NameTextBox.Focus();
                NameTextBox.SelectAll();
            };
        }

        public InventorySemiFinishedRequest Request { get; private set; } = new InventorySemiFinishedRequest();

        private void FillFields(InventorySemiFinishedDto? item)
        {
            NameTextBox.Text = item?.Name ?? string.Empty;
            CostTextBox.Text = FormatDecimal(item?.CostRub ?? 0m);
            UnitComboBox.SelectedValue = item?.UnitOfMeasureId;
            CategoryComboBox.SelectedValue = item?.CategoryId;
            TechnicalCardComboBox.SelectedValue = item?.TechnicalCardId;
            CaloriesTextBox.Text = FormatDecimal(item?.CaloriesKcal ?? 0m);
            ProteinsTextBox.Text = FormatDecimal(item?.ProteinsG ?? 0m);
            FatsTextBox.Text = FormatDecimal(item?.FatsG ?? 0m);
            CarbsTextBox.Text = FormatDecimal(item?.CarbsG ?? 0m);
            KilojoulesTextBox.Text = FormatDecimal(item?.Kilojoules ?? 0m);
        }

        private void TechnicalCardSearchTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var selectedValue = TechnicalCardComboBox.SelectedValue as int?;
            ApplyTechnicalCardFilter(selectedValue);
        }

        private void ApplyTechnicalCardFilter(int? selectedValue)
        {
            if (TechnicalCardComboBox == null)
            {
                return;
            }

            var query = (TechnicalCardSearchTextBox?.Text ?? string.Empty).Trim();
            var filtered = string.IsNullOrWhiteSpace(query)
                ? _technicalCardOptions
                : _technicalCardOptions
                    .Where(x => !x.Id.HasValue || x.Name.IndexOf(query, StringComparison.CurrentCultureIgnoreCase) >= 0)
                    .ToList();

            TechnicalCardComboBox.ItemsSource = filtered;
            if (selectedValue.HasValue && filtered.Any(x => x.Id == selectedValue.Value))
            {
                TechnicalCardComboBox.SelectedValue = selectedValue.Value;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            HideValidation();

            var name = NormalizeName(NameTextBox.Text);
            if (string.IsNullOrWhiteSpace(name))
            {
                ShowValidation("Введите название полуфабриката.");
                return;
            }

            if (!TryReadDecimal(CostTextBox.Text, "себестоимость", out var cost) ||
                !TryReadDecimal(CaloriesTextBox.Text, "калории", out var calories) ||
                !TryReadDecimal(ProteinsTextBox.Text, "белки", out var proteins) ||
                !TryReadDecimal(FatsTextBox.Text, "жиры", out var fats) ||
                !TryReadDecimal(CarbsTextBox.Text, "углеводы", out var carbs) ||
                !TryReadDecimal(KilojoulesTextBox.Text, "килоджоули", out var kilojoules))
            {
                return;
            }

            Request = new InventorySemiFinishedRequest
            {
                Name = name,
                CostRub = cost,
                UnitOfMeasureId = UnitComboBox.SelectedValue as int?,
                CategoryId = CategoryComboBox.SelectedValue as int?,
                TechnicalCardId = TechnicalCardComboBox.SelectedValue as int?,
                CaloriesKcal = calories,
                ProteinsG = proteins,
                FatsG = fats,
                CarbsG = carbs,
                Kilojoules = kilojoules
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
