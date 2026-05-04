using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TransferModels.Menu;

namespace GardenNookWpf.Views.MainPanel.Menu
{
    public partial class MenuItemEditWindow : Window
    {
        private const string DishType = "dishes";
        private const string DrinkType = "drinks";
        private const string ToppingType = "toppings";

        private readonly MenuItemEditOptionsResponse _options;
        private readonly MenuItemManagementDto? _item;

        public MenuItemEditWindow(MenuItemEditOptionsResponse options, MenuItemManagementDto? item, string defaultType)
        {
            _options = options ?? new MenuItemEditOptionsResponse();
            _item = item;

            InitializeComponent();
            BindTypeOptions();
            BindSharedOptions();

            ItemType = string.IsNullOrWhiteSpace(item?.Type)
                ? NormalizeType(defaultType) ?? DishType
                : NormalizeType(item.Type) ?? DishType;

            TitleText.Text = item == null ? "Добавить позицию меню" : "Изменить позицию меню";
            TypeComboBox.SelectedValue = ItemType;
            TypeComboBox.IsEnabled = item == null;

            FillFields(item);
            RefreshCategoryOptions();
            UpdateQuantityVisibility();

            Loaded += (_, _) =>
            {
                NameTextBox.Focus();
                NameTextBox.SelectAll();
            };
        }

        public string ItemType { get; private set; }

        public MenuItemUpsertRequest Request { get; private set; } = new MenuItemUpsertRequest();

        private void BindTypeOptions()
        {
            TypeComboBox.ItemsSource = new List<TypeOption>
            {
                new TypeOption(DishType, "Блюдо"),
                new TypeOption(DrinkType, "Напиток"),
                new TypeOption(ToppingType, "Добавка")
            };
        }

        private void BindSharedOptions()
        {
            UnitComboBox.ItemsSource = BuildNullableOptions(_options.UnitsOfMeasure);
            TechnicalCardComboBox.ItemsSource = BuildNullableOptions(_options.TechnicalCards);
        }

        private void FillFields(MenuItemManagementDto? item)
        {
            NameTextBox.Text = item?.Name ?? string.Empty;
            PriceTextBox.Text = FormatDecimal(item?.PriceRub ?? 0m);
            QuantityTextBox.Text = item?.Quantity.HasValue == true ? FormatDecimal(item.Quantity.Value) : string.Empty;
            CaloriesTextBox.Text = FormatDecimal(item?.CaloriesKcal ?? 0m);
            KilojoulesTextBox.Text = FormatDecimal(item?.Kilojoules ?? 0m);
            ProteinsTextBox.Text = FormatDecimal(item?.ProteinsG ?? 0m);
            FatsTextBox.Text = FormatDecimal(item?.FatsG ?? 0m);
            CarbsTextBox.Text = FormatDecimal(item?.CarbsG ?? 0m);
            ImageUrlTextBox.Text = item?.ImageUrl ?? string.Empty;
            IsAvailableCheckBox.IsChecked = item?.IsAvailable ?? true;
            UnitComboBox.SelectedValue = item?.UnitOfMeasureId;
            TechnicalCardComboBox.SelectedValue = item?.TechnicalCardId;
        }

        private void TypeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (TypeComboBox.SelectedValue is string type)
            {
                ItemType = type;
                RefreshCategoryOptions();
                UpdateQuantityVisibility();
            }
        }

        private void RefreshCategoryOptions()
        {
            if (CategoryComboBox == null)
            {
                return;
            }

            CategoryComboBox.ItemsSource = BuildNullableOptions(
                (_options.Categories ?? new List<MenuItemCategoryOptionDto>())
                    .Where(x => x.Type == ItemType)
                    .Select(x => new MenuItemOptionDto { Id = x.Id, Name = x.Name })
                    .ToList());

            CategoryComboBox.SelectedValue = _item != null && _item.Type == ItemType
                ? _item.CategoryId
                : null;
        }

        private void UpdateQuantityVisibility()
        {
            QuantityPanel.Visibility = ItemType == DishType ? Visibility.Collapsed : Visibility.Visible;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            HideValidation();

            var name = NormalizeName(NameTextBox.Text);
            if (string.IsNullOrWhiteSpace(name))
            {
                ShowValidation("Введите название позиции меню.");
                return;
            }

            if (!TryReadDecimal(PriceTextBox.Text, "цену", out var price))
            {
                return;
            }

            decimal? quantity = null;
            if (ItemType != DishType && !string.IsNullOrWhiteSpace(QuantityTextBox.Text))
            {
                if (!TryReadDecimal(QuantityTextBox.Text, "количество", out var parsedQuantity))
                {
                    return;
                }

                quantity = parsedQuantity;
            }

            if (!TryReadDecimal(CaloriesTextBox.Text, "калории", out var calories) ||
                !TryReadDecimal(KilojoulesTextBox.Text, "килоджоули", out var kilojoules) ||
                !TryReadDecimal(ProteinsTextBox.Text, "белки", out var proteins) ||
                !TryReadDecimal(FatsTextBox.Text, "жиры", out var fats) ||
                !TryReadDecimal(CarbsTextBox.Text, "углеводы", out var carbs))
            {
                return;
            }

            Request = new MenuItemUpsertRequest
            {
                Name = name,
                CategoryId = CategoryComboBox.SelectedValue as int?,
                UnitOfMeasureId = UnitComboBox.SelectedValue as int?,
                Quantity = quantity,
                PriceRub = price,
                TechnicalCardId = TechnicalCardComboBox.SelectedValue as int?,
                CaloriesKcal = calories,
                Kilojoules = kilojoules,
                ProteinsG = proteins,
                FatsG = fats,
                CarbsG = carbs,
                ImageUrl = ImageUrlTextBox.Text?.Trim() ?? string.Empty,
                IsAvailable = IsAvailableCheckBox.IsChecked == true
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

        private static List<NullableOption> BuildNullableOptions(IReadOnlyCollection<MenuItemOptionDto>? source)
        {
            var result = new List<NullableOption>
            {
                new NullableOption(null, "Не выбрано")
            };

            result.AddRange((source ?? Array.Empty<MenuItemOptionDto>())
                .OrderBy(x => x.Name)
                .Select(x => new NullableOption(x.Id, x.Name)));

            return result;
        }

        private static string? NormalizeType(string? type)
        {
            return (type ?? string.Empty).Trim().ToLowerInvariant() switch
            {
                DishType => DishType,
                DrinkType => DrinkType,
                ToppingType => ToppingType,
                _ => null
            };
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

        private sealed class TypeOption
        {
            public TypeOption(string type, string name)
            {
                Type = type;
                Name = name;
            }

            public string Type { get; }
            public string Name { get; }
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
