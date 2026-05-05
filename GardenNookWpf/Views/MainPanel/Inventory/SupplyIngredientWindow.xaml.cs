using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TransferModels.Inventory;

namespace GardenNookWpf.Views.MainPanel.Inventory
{
    public partial class SupplyIngredientWindow : Window
    {
        private readonly List<IngredientSupplyCard> _allIngredientCards;
        private readonly ObservableCollection<IngredientSupplyCard> _visibleIngredientCards = new ObservableCollection<IngredientSupplyCard>();
        private readonly ObservableCollection<SelectedIngredientSupplyCard> _selectedIngredientCards = new ObservableCollection<SelectedIngredientSupplyCard>();

        public SupplyIngredientWindow(IReadOnlyCollection<InventoryIngredientDto> ingredients)
        {
            InitializeComponent();

            _allIngredientCards = (ingredients ?? Array.Empty<InventoryIngredientDto>())
                .OrderBy(x => x.Name)
                .Select(x => new IngredientSupplyCard(x))
                .ToList();

            IngredientsList.ItemsSource = _visibleIngredientCards;
            SelectedIngredientsList.ItemsSource = _selectedIngredientCards;
            ApplyIngredientFilter();
            RefreshSelectedState();

            Loaded += (_, _) =>
            {
                IngredientSearchTextBox.Focus();
            };
        }

        public InventoryIngredientSupplyRequest Request { get; private set; } = new InventoryIngredientSupplyRequest();

        private void IngredientSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyIngredientFilter();
        }

        private void ApplyIngredientFilter()
        {
            if (IngredientsList == null)
            {
                return;
            }

            var query = (IngredientSearchTextBox?.Text ?? string.Empty).Trim();
            var filtered = string.IsNullOrWhiteSpace(query)
                ? _allIngredientCards
                : _allIngredientCards
                    .Where(x =>
                        x.Name.IndexOf(query, StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                        x.UnitName.IndexOf(query, StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                        x.CategoryName.IndexOf(query, StringComparison.CurrentCultureIgnoreCase) >= 0)
                    .ToList();

            _visibleIngredientCards.Clear();
            foreach (var item in filtered)
            {
                _visibleIngredientCards.Add(item);
            }

            EmptyIngredientsText.Visibility = _visibleIngredientCards.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            IngredientsScrollViewer.Visibility = _visibleIngredientCards.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        private void IngredientCard_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is IngredientSupplyCard card)
            {
                AddIngredient(card);
            }
        }

        private void AddIngredientButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is IngredientSupplyCard card)
            {
                AddIngredient(card);
                e.Handled = true;
            }
        }

        private void AddIngredient(IngredientSupplyCard card)
        {
            HideValidation();

            if (card.IsSelected)
            {
                return;
            }

            card.IsSelected = true;
            _selectedIngredientCards.Add(new SelectedIngredientSupplyCard(card));
            RefreshSelectedState();
        }

        private void RemoveSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is not SelectedIngredientSupplyCard selected)
            {
                return;
            }

            _selectedIngredientCards.Remove(selected);
            var source = _allIngredientCards.FirstOrDefault(x => x.Id == selected.Id);
            if (source != null)
            {
                source.IsSelected = false;
            }

            RefreshSelectedState();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            HideValidation();

            if (_selectedIngredientCards.Count == 0)
            {
                ShowValidation("Выберите сырье слева.");
                return;
            }

            var lines = new List<InventoryIngredientSupplyLineRequest>();
            foreach (var card in _selectedIngredientCards)
            {
                var text = (card.QuantityText ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(text))
                {
                    ShowValidation($"Введите количество для сырья \"{card.Name}\".");
                    return;
                }

                if (!TryReadPositiveDecimal(text, card.Name, out var quantity))
                {
                    return;
                }

                lines.Add(new InventoryIngredientSupplyLineRequest
                {
                    IngredientId = card.Id,
                    Quantity = quantity
                });
            }

            Request = new InventoryIngredientSupplyRequest
            {
                Lines = lines
            };

            DialogResult = true;
        }

        private bool TryReadPositiveDecimal(string value, string ingredientName, out decimal result)
        {
            var normalized = value.Trim().Replace(',', '.');
            if (!decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.InvariantCulture, out result))
            {
                ShowValidation($"Проверьте количество для сырья \"{ingredientName}\".");
                return false;
            }

            if (result <= 0)
            {
                ShowValidation($"Количество для сырья \"{ingredientName}\" должно быть больше нуля.");
                return false;
            }

            return true;
        }

        private void RefreshSelectedState()
        {
            EmptySelectedText.Visibility = _selectedIngredientCards.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            SelectedScrollViewer.Visibility = _selectedIngredientCards.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
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

        private static string FormatDecimal(decimal value)
        {
            return value.ToString("0.##", CultureInfo.InvariantCulture);
        }

        private sealed class IngredientSupplyCard : INotifyPropertyChanged
        {
            private bool _isSelected;

            public IngredientSupplyCard(InventoryIngredientDto source)
            {
                Id = source.Id;
                Name = source.Name ?? string.Empty;
                UnitName = source.UnitName ?? string.Empty;
                Stock = source.Stock;
                CategoryName = source.CategoryName ?? string.Empty;
            }

            public event PropertyChangedEventHandler? PropertyChanged;

            public int Id { get; }
            public string Name { get; }
            public string UnitName { get; }
            public decimal Stock { get; }
            public string CategoryName { get; }

            public bool IsSelected
            {
                get => _isSelected;
                set
                {
                    if (_isSelected == value)
                    {
                        return;
                    }

                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                    OnPropertyChanged(nameof(AddButtonText));
                    OnPropertyChanged(nameof(CanAdd));
                    OnPropertyChanged(nameof(CardBackground));
                    OnPropertyChanged(nameof(CardBorderBrush));
                }
            }

            public bool CanAdd => !IsSelected;
            public string AddButtonText => IsSelected ? "Выбрано" : "Добавить";
            public string CategoryDisplay => string.IsNullOrWhiteSpace(CategoryName) ? "Без категории" : CategoryName;
            public string StockDisplay => string.IsNullOrWhiteSpace(UnitName)
                ? "Остаток: " + FormatDecimal(Stock)
                : "Остаток: " + FormatDecimal(Stock) + " " + UnitName;
            public Brush CardBackground => IsSelected
                ? (Brush)Application.Current.Resources["ModalColorCardAltBrush"]
                : Brushes.White;
            public Brush CardBorderBrush => IsSelected
                ? (Brush)Application.Current.Resources["ModalColorPrimaryDarkBrush"]
                : new SolidColorBrush(Color.FromRgb(111, 104, 100));

            private void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private sealed class SelectedIngredientSupplyCard : INotifyPropertyChanged
        {
            private string _quantityText = string.Empty;

            public SelectedIngredientSupplyCard(IngredientSupplyCard source)
            {
                Id = source.Id;
                Name = source.Name;
                UnitName = source.UnitName;
            }

            public event PropertyChangedEventHandler? PropertyChanged;

            public int Id { get; }
            public string Name { get; }
            public string UnitName { get; }

            public string QuantityText
            {
                get => _quantityText;
                set
                {
                    if (_quantityText == value)
                    {
                        return;
                    }

                    _quantityText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QuantityText)));
                }
            }

            public string UnitDisplay => string.IsNullOrWhiteSpace(UnitName)
                ? "Единица не указана"
                : "Ед. изм.: " + UnitName;
        }
    }
}
