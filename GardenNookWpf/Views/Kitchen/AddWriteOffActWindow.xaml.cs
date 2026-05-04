using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TransferModels.Kitchen;

namespace GardenNookWpf.Views.Kitchen
{
    public partial class AddWriteOffActWindow : Window
    {
        private const string SemiFinishedKind = "semi-finished";
        private const string IngredientKind = "ingredient";
        private const decimal DecimalEpsilon = 0.000001m;

        private readonly List<KitchenWriteOffTypeDto> _writeOffTypes;
        private readonly List<SelectableItemModel> _semiFinishedItems;
        private readonly List<SelectableItemModel> _ingredientItems;
        private readonly List<SelectedWriteOffLineModel> _selectedLines = new List<SelectedWriteOffLineModel>();

        public KitchenCreateWriteOffActRequest Request { get; private set; } = new KitchenCreateWriteOffActRequest();

        public AddWriteOffActWindow(
            IReadOnlyCollection<KitchenWriteOffTypeDto> writeOffTypes,
            IReadOnlyCollection<KitchenWriteOffSemiFinishedOptionDto> semiFinishedOptions,
            IReadOnlyCollection<KitchenWriteOffIngredientOptionDto> ingredientOptions)
        {
            _writeOffTypes = (writeOffTypes ?? new List<KitchenWriteOffTypeDto>())
                .OrderBy(x => x.WriteOffTypeId)
                .ToList();

            _semiFinishedItems = (semiFinishedOptions ?? new List<KitchenWriteOffSemiFinishedOptionDto>())
                .Select(x => new SelectableItemModel
                {
                    Kind = SemiFinishedKind,
                    ItemId = x.SemiFinishedId,
                    Name = string.IsNullOrWhiteSpace(x.Name) ? $"Полуфабрикат #{x.SemiFinishedId}" : x.Name,
                    UnitName = ToShortUnitName(string.IsNullOrWhiteSpace(x.UnitName) ? "г" : x.UnitName),
                    AvailableQuantity = Math.Max(0m, x.AvailableStock),
                    IdDisplay = $"ID: {x.SemiFinishedId}"
                })
                .OrderBy(x => x.Name)
                .ThenBy(x => x.ItemId)
                .ToList();

            _ingredientItems = (ingredientOptions ?? new List<KitchenWriteOffIngredientOptionDto>())
                .Select(x => new SelectableItemModel
                {
                    Kind = IngredientKind,
                    ItemId = x.IngredientId,
                    Name = string.IsNullOrWhiteSpace(x.Name) ? $"Сырье #{x.IngredientId}" : x.Name,
                    UnitName = ToShortUnitName(x.UnitName),
                    AvailableQuantity = Math.Max(0m, x.AvailableStock),
                    IdDisplay = $"ID: {x.IngredientId}"
                })
                .OrderBy(x => x.Name)
                .ThenBy(x => x.ItemId)
                .ToList();

            InitializeComponent();

            DatePicker.SelectedDate = DateTime.Today;
            WriteOffTypeComboBox.ItemsSource = _writeOffTypes;
            WriteOffTypeComboBox.SelectedIndex = _writeOffTypes.Count > 0 ? 0 : -1;

            LineKindComboBox.ItemsSource = BuildKindOptions();
            LineKindComboBox.SelectedIndex = 0;

            QuantityTextBox.Text = "0";
            RefreshSelectedLines();
            ApplyFilter(string.Empty);
        }

        private void AddLine_Click(object sender, RoutedEventArgs e)
        {
            HideValidation();

            if (ItemsListBox.SelectedItem is not SelectableItemModel selectedItem)
            {
                ShowValidation("Выберите позицию для списания.");
                return;
            }

            if (WriteOffTypeComboBox.SelectedItem is not KitchenWriteOffTypeDto selectedType)
            {
                ShowValidation("Выберите тип списания.");
                return;
            }

            if (!TryParseQuantity(QuantityTextBox.Text, out var quantity))
            {
                return;
            }

            var alreadySelected = _selectedLines
                .Where(x => x.Kind == selectedItem.Kind && x.ItemId == selectedItem.ItemId)
                .Sum(x => x.Quantity);

            if (alreadySelected + quantity > selectedItem.AvailableQuantity + DecimalEpsilon)
            {
                ShowValidation("Недостаточно остатка с учетом уже добавленных позиций.");
                return;
            }

            _selectedLines.Add(new SelectedWriteOffLineModel
            {
                Kind = selectedItem.Kind,
                KindDisplay = selectedItem.Kind == IngredientKind ? "Сырье" : "ПФ",
                ItemId = selectedItem.ItemId,
                ItemName = selectedItem.Name,
                Quantity = quantity,
                UnitName = selectedItem.UnitName,
                WriteOffTypeId = selectedType.WriteOffTypeId,
                WriteOffTypeName = selectedType.Name
            });

            RefreshSelectedLines();
            QuantityTextBox.Text = "0";
        }

        private void RemoveLine_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is SelectedWriteOffLineModel line)
            {
                _selectedLines.Remove(line);
                RefreshSelectedLines();
            }
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            HideValidation();

            if (_selectedLines.Count == 0)
            {
                ShowValidation("Добавьте хотя бы одну позицию акта.");
                return;
            }

            var selectedDate = DatePicker.SelectedDate ?? DateTime.Today;

            Request = new KitchenCreateWriteOffActRequest
            {
                Date = selectedDate.Date,
                Comment = (CommentTextBox.Text ?? string.Empty).Trim(),
                IngredientLines = _selectedLines
                    .Where(x => x.Kind == IngredientKind)
                    .Select(x => new KitchenCreateIngredientWriteOffLineRequest
                    {
                        IngredientId = x.ItemId,
                        Quantity = x.Quantity,
                        WriteOffTypeId = x.WriteOffTypeId
                    })
                    .ToList(),
                SemiFinishedLines = _selectedLines
                    .Where(x => x.Kind == SemiFinishedKind)
                    .Select(x => new KitchenCreateSemiFinishedWriteOffLineRequest
                    {
                        SemiFinishedId = x.ItemId,
                        Quantity = x.Quantity,
                        WriteOffTypeId = x.WriteOffTypeId
                    })
                    .ToList()
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

        private void LineKindComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilter(SearchTextBox.Text);
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter(SearchTextBox.Text);
        }

        private void ItemsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshSelectedItemInfo();
        }

        private void ApplyFilter(string? searchText)
        {
            var selectedKind = LineKindComboBox.SelectedValue as string;
            var baseItems = selectedKind == IngredientKind
                ? _ingredientItems
                : _semiFinishedItems;

            var query = (searchText ?? string.Empty).Trim();
            IEnumerable<SelectableItemModel> filtered = baseItems;

            if (!string.IsNullOrWhiteSpace(query))
            {
                filtered = baseItems.Where(x =>
                    x.Name.Contains(query, StringComparison.CurrentCultureIgnoreCase) ||
                    x.IdDisplay.Contains(query, StringComparison.CurrentCultureIgnoreCase) ||
                    x.AvailableDisplay.Contains(query, StringComparison.CurrentCultureIgnoreCase));
            }

            var items = filtered.ToList();
            ItemsListBox.ItemsSource = items;
            ItemsListBox.SelectedIndex = items.Count > 0 ? 0 : -1;

            if (items.Count == 0)
            {
                ShowValidation("По вашему запросу ничего не найдено.");
            }
            else
            {
                HideValidation();
            }

            RefreshSelectedItemInfo();
        }

        private void RefreshSelectedItemInfo()
        {
            if (ItemsListBox.SelectedItem is not SelectableItemModel selectedItem)
            {
                QuantityLabel.Text = "Количество";
                AvailableText.Text = "Доступный остаток: -";
                return;
            }

            var unitName = string.IsNullOrWhiteSpace(selectedItem.UnitName)
                ? string.Empty
                : $" ({selectedItem.UnitName})";
            QuantityLabel.Text = "Количество" + unitName;

            var alreadySelected = _selectedLines
                .Where(x => x.Kind == selectedItem.Kind && x.ItemId == selectedItem.ItemId)
                .Sum(x => x.Quantity);
            var availableLeft = Math.Max(0m, selectedItem.AvailableQuantity - alreadySelected);
            AvailableText.Text = "Доступный остаток: " + FormatQuantity(availableLeft, selectedItem.UnitName);
        }

        private void RefreshSelectedLines()
        {
            SelectedLinesListBox.ItemsSource = null;
            SelectedLinesListBox.ItemsSource = _selectedLines;
            RefreshSelectedItemInfo();
        }

        private bool TryParseQuantity(string? rawText, out decimal quantity)
        {
            quantity = 0m;
            var text = (rawText ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(text))
            {
                ShowValidation("Укажите количество списания.");
                return false;
            }

            if (!decimal.TryParse(text, NumberStyles.Number, CultureInfo.CurrentCulture, out var parsed) &&
                !decimal.TryParse(text.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out parsed))
            {
                ShowValidation("Количество должно быть числом.");
                return false;
            }

            if (parsed <= 0m)
            {
                ShowValidation("Количество должно быть больше нуля.");
                return false;
            }

            quantity = Math.Round(parsed, 2, MidpointRounding.AwayFromZero);
            return true;
        }

        private static List<KindOptionModel> BuildKindOptions()
        {
            return new List<KindOptionModel>
            {
                new KindOptionModel
                {
                    Kind = SemiFinishedKind,
                    DisplayName = "Полуфабрикат"
                },
                new KindOptionModel
                {
                    Kind = IngredientKind,
                    DisplayName = "Сырье"
                }
            };
        }

        private static string FormatQuantity(decimal value, string unitName)
        {
            var quantityText = decimal.Round(value, 2, MidpointRounding.AwayFromZero)
                .ToString("0.##", CultureInfo.CurrentCulture);
            return string.IsNullOrWhiteSpace(unitName)
                ? quantityText
                : $"{quantityText} {unitName}";
        }

        private static string ToShortUnitName(string? unitName)
        {
            var normalized = (unitName ?? string.Empty).Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(normalized))
            {
                return string.Empty;
            }

            return normalized switch
            {
                "килограмм" or "килограмма" or "килограммы" or "кг." => "кг",
                "грамм" or "грамма" or "граммы" or "гр" or "гр." or "г." => "г",
                "литр" or "литра" or "литры" or "л." => "л",
                "миллилитр" or "миллилитра" or "миллилитры" or "мл." => "мл",
                "штука" or "штуки" or "штук" or "шт." => "шт",
                _ => (unitName ?? string.Empty).Trim()
            };
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

        private sealed class KindOptionModel
        {
            public string Kind { get; set; } = string.Empty;
            public string DisplayName { get; set; } = string.Empty;
        }

        private sealed class SelectableItemModel
        {
            public string Kind { get; set; } = string.Empty;
            public int ItemId { get; set; }
            public string Name { get; set; } = string.Empty;
            public string UnitName { get; set; } = string.Empty;
            public decimal AvailableQuantity { get; set; }
            public string IdDisplay { get; set; } = string.Empty;

            public string AvailableDisplay => "Остаток: " + FormatQuantity(AvailableQuantity, UnitName);
        }

        private sealed class SelectedWriteOffLineModel
        {
            public string Kind { get; set; } = string.Empty;
            public string KindDisplay { get; set; } = string.Empty;
            public int ItemId { get; set; }
            public string ItemName { get; set; } = string.Empty;
            public decimal Quantity { get; set; }
            public string UnitName { get; set; } = string.Empty;
            public int WriteOffTypeId { get; set; }
            public string WriteOffTypeName { get; set; } = string.Empty;

            public string Display => $"{KindDisplay}: {ItemName} - {FormatQuantity(Quantity, UnitName)} ({WriteOffTypeName})";
        }
    }
}
