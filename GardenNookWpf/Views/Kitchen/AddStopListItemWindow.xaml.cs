using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GardenNookWpf.Views.Kitchen.Shell.Controllers;

namespace GardenNookWpf.Views.Kitchen
{
    public partial class AddStopListItemWindow : Window
    {
        private static readonly Brush AvailableStateBrush = CreateFrozenBrush(Color.FromRgb(0x2E, 0x7D, 0x32));
        private static readonly Brush LimitedStateBrush = CreateFrozenBrush(Color.FromRgb(0x8D, 0x6E, 0x63));
        private static readonly Brush InStopListStateBrush = CreateFrozenBrush(Color.FromRgb(0xB3, 0x31, 0x31));

        private readonly List<AddablePositionDisplayModel> _allPositions;

        public string SelectedItemType { get; private set; } = string.Empty;
        public int SelectedItemId { get; private set; }
        public decimal SelectedRemainingPortions { get; private set; }

        public AddStopListItemWindow(IReadOnlyCollection<StopListPositionDisplayModel> positions)
        {
            _allPositions = (positions ?? new List<StopListPositionDisplayModel>())
                .Select(x =>
                {
                    var hasManualLimit = x.ManualRemainingPortions.HasValue;
                    var manualLimit = x.ManualRemainingPortions.GetValueOrDefault();

                    var stateText = !x.IsAvailable
                        ? "Уже в стоп-листе"
                        : hasManualLimit
                            ? $"Лимит: {FormatPortions(manualLimit)}"
                            : "Доступно";

                    var stateBrush = !x.IsAvailable
                        ? InStopListStateBrush
                        : hasManualLimit
                            ? LimitedStateBrush
                            : AvailableStateBrush;

                    return new AddablePositionDisplayModel
                    {
                        ItemType = x.ItemType,
                        ItemId = x.ItemId,
                        Name = x.Name,
                        CategoryDisplay = x.CategoryDisplay,
                        CategoryVisibility = x.CategoryVisibility,
                        VolumeWeightDisplay = x.VolumeWeightDisplay,
                        VolumeWeightVisibility = x.VolumeWeightVisibility,
                        ItemTypeDisplay = x.ItemTypeDisplay,
                        IsAvailable = x.IsAvailable,
                        ManualRemainingPortions = x.ManualRemainingPortions,
                        StateText = stateText,
                        StateBrush = stateBrush,
                        ExistingLimitDisplay = hasManualLimit
                            ? $"Текущий ручной лимит: {FormatPortions(manualLimit)}"
                            : string.Empty,
                        ExistingLimitVisibility = hasManualLimit ? Visibility.Visible : Visibility.Collapsed
                    };
                })
                .OrderBy(x => x.ItemTypeDisplay)
                .ThenBy(x => x.Name)
                .ThenBy(x => x.ItemId)
                .ToList();

            InitializeComponent();
            ApplyFilter(string.Empty);
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            HideValidation();

            if (PositionsListBox.SelectedItem is not AddablePositionDisplayModel item)
            {
                ShowValidation("Выберите позицию.");
                return;
            }

            if (!item.IsAvailable)
            {
                ShowValidation("Позиция уже находится в стоп-листе.");
                return;
            }

            if (!TryParseRemainingPortions(RemainingPortionsTextBox.Text, out var remainingPortions))
            {
                return;
            }

            SelectedItemType = item.ItemType;
            SelectedItemId = item.ItemId;
            SelectedRemainingPortions = remainingPortions;
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

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter(SearchTextBox.Text);
        }

        private void PositionsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = PositionsListBox.SelectedItem as AddablePositionDisplayModel;
            AddButton.IsEnabled = selected != null && selected.IsAvailable;

            if (selected == null)
            {
                HideValidation();
                return;
            }

            if (selected.ManualRemainingPortions.HasValue && selected.ManualRemainingPortions.Value > 0m)
            {
                RemainingPortionsTextBox.Text = selected.ManualRemainingPortions.Value
                    .ToString("0.##", CultureInfo.InvariantCulture);
            }

            if (selected.IsAvailable)
            {
                HideValidation();
                return;
            }

            ShowValidation("Выбранная позиция уже в стоп-листе.");
        }

        private void ApplyFilter(string? searchText)
        {
            var query = (searchText ?? string.Empty).Trim();
            IEnumerable<AddablePositionDisplayModel> source = _allPositions;

            if (!string.IsNullOrWhiteSpace(query))
            {
                source = source.Where(x =>
                    x.Name.Contains(query, StringComparison.CurrentCultureIgnoreCase) ||
                    x.ItemTypeDisplay.Contains(query, StringComparison.CurrentCultureIgnoreCase) ||
                    x.CategoryDisplay.Contains(query, StringComparison.CurrentCultureIgnoreCase) ||
                    x.VolumeWeightDisplay.Contains(query, StringComparison.CurrentCultureIgnoreCase) ||
                    x.ItemId.ToString().Contains(query, StringComparison.CurrentCultureIgnoreCase));
            }

            var items = source.ToList();
            PositionsListBox.ItemsSource = items;
            PositionsListBox.SelectedIndex = items.Count > 0 ? 0 : -1;

            if (items.Count == 0)
            {
                AddButton.IsEnabled = false;
                ShowValidation("По вашему запросу ничего не найдено.");
                return;
            }

            var selected = PositionsListBox.SelectedItem as AddablePositionDisplayModel;
            AddButton.IsEnabled = selected != null && selected.IsAvailable;

            if (selected != null && !selected.IsAvailable)
            {
                ShowValidation("Выбранная позиция уже в стоп-листе.");
            }
            else
            {
                HideValidation();
            }
        }

        private bool TryParseRemainingPortions(string? rawText, out decimal remainingPortions)
        {
            remainingPortions = 0m;
            var text = (rawText ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(text))
            {
                ShowValidation("Укажите остаток порций.");
                return false;
            }

            if (!decimal.TryParse(text, NumberStyles.Number, CultureInfo.CurrentCulture, out var parsedValue) &&
                !decimal.TryParse(text.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out parsedValue))
            {
                ShowValidation("Остаток порций должен быть числом.");
                return false;
            }

            if (parsedValue < 0m)
            {
                ShowValidation("Остаток порций не может быть отрицательным.");
                return false;
            }

            remainingPortions = Math.Round(parsedValue, 2, MidpointRounding.AwayFromZero);
            return true;
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

        private static string FormatPortions(decimal value)
        {
            return value.ToString("0.##", CultureInfo.InvariantCulture) + " порц.";
        }

        private static Brush CreateFrozenBrush(Color color)
        {
            var brush = new SolidColorBrush(color);
            brush.Freeze();
            return brush;
        }

        private sealed class AddablePositionDisplayModel
        {
            public string ItemType { get; set; } = string.Empty;
            public int ItemId { get; set; }
            public string Name { get; set; } = string.Empty;
            public string CategoryDisplay { get; set; } = string.Empty;
            public Visibility CategoryVisibility { get; set; }
            public string VolumeWeightDisplay { get; set; } = string.Empty;
            public Visibility VolumeWeightVisibility { get; set; }
            public string ItemTypeDisplay { get; set; } = string.Empty;
            public bool IsAvailable { get; set; }
            public decimal? ManualRemainingPortions { get; set; }
            public string StateText { get; set; } = string.Empty;
            public Brush StateBrush { get; set; } = AvailableStateBrush;
            public string ExistingLimitDisplay { get; set; } = string.Empty;
            public Visibility ExistingLimitVisibility { get; set; } = Visibility.Collapsed;
        }
    }
}
