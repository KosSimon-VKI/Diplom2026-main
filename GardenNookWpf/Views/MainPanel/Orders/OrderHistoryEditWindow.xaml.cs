using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TransferModels.Menu;
using TransferModels.Orders;

namespace GardenNookWpf.Views.MainPanel.Orders
{
    public partial class OrderHistoryEditWindow : Window
    {
        private const string DishType = "dish";
        private const string DrinkType = "drink";
        private const string ToppingType = "topping";
        private const string DishTypeText = "Блюдо";
        private const string DrinkTypeText = "Напиток";
        private const string ToppingTypeText = "Добавка";

        private readonly OrderHistoryDetailsDto _details;
        private readonly MenuResponse _menu;
        private readonly ObservableCollection<OrderLineViewModel> _items = new ObservableCollection<OrderLineViewModel>();
        private readonly ObservableCollection<MenuItemOption> _visibleMenuItems = new ObservableCollection<MenuItemOption>();
        private readonly List<MenuItemOption> _dishOptions;
        private readonly List<MenuItemOption> _drinkOptions;
        private readonly List<MenuItemOption> _toppingOptions;
        private readonly List<ModifierOption> _milkOptions;
        private readonly List<ModifierOption> _coffeeOptions;

        public OrderHistoryEditWindow(OrderHistoryDetailsDto details, MenuResponse menu)
        {
            _details = details;
            _menu = menu;
            _dishOptions = (menu.Dishes ?? new List<DishDto>())
                .Where(x => x.IsAvailable)
                .OrderBy(x => x.Name)
                .Select(x => new MenuItemOption(DishType, x.Id, x.Name ?? $"Блюдо #{x.Id}"))
                .ToList();
            _drinkOptions = (menu.Drinks ?? new List<DrinkDto>())
                .Where(x => x.IsAvailable)
                .OrderBy(x => x.Name)
                .Select(x => new MenuItemOption(DrinkType, x.Id, x.Name ?? $"Напиток #{x.Id}"))
                .ToList();
            _toppingOptions = (menu.Toppings ?? new List<ToppingDto>())
                .Where(x => x.IsAvailable)
                .OrderBy(x => x.Name)
                .Select(x => new MenuItemOption(ToppingType, x.Id, x.Name ?? $"Добавка #{x.Id}"))
                .ToList();
            _milkOptions = new List<ModifierOption> { new ModifierOption(null, "Молоко по умолчанию") }
                .Concat((menu.DrinkModifiers?.MilkOptions ?? new List<DrinkModifierOptionDto>())
                    .Select(x => new ModifierOption(x.Id, x.Name ?? $"Молоко #{x.Id}")))
                .ToList();
            _coffeeOptions = new List<ModifierOption> { new ModifierOption(null, "Кофе по умолчанию") }
                .Concat((menu.DrinkModifiers?.CoffeeOptions ?? new List<DrinkModifierOptionDto>())
                    .Select(x => new ModifierOption(x.Id, x.Name ?? $"Кофе #{x.Id}")))
                .ToList();

            InitializeComponent();

            ItemsGrid.ItemsSource = _items;
            MenuItemComboBox.ItemsSource = _visibleMenuItems;
            ItemTypeComboBox.ItemsSource = new[] { DishTypeText, DrinkTypeText, ToppingTypeText };
            ItemTypeComboBox.SelectedIndex = 0;
            MilkComboBox.ItemsSource = _milkOptions;
            CoffeeComboBox.ItemsSource = _coffeeOptions;

            BindOrderFields();
            LoadOrderItems();
            UpdateSelectedLineControls();
        }

        public OrderHistoryUpdateRequest Request { get; private set; } = new OrderHistoryUpdateRequest();

        private void BindOrderFields()
        {
            HeaderTitleText.Text = $"Редактировать заказ №{_details.OrderId}";
            OrderTypeComboBox.ItemsSource = _details.OrderTypes ?? new List<OrderHistoryOptionDto>();
            StatusComboBox.ItemsSource = _details.Statuses ?? new List<OrderHistoryOptionDto>();
            DiscountComboBox.ItemsSource = new List<DiscountOption> { new DiscountOption(null, "Без скидки", 0m) }
                .Concat((_details.Discounts ?? new List<OrderHistoryDiscountOptionDto>())
                    .Select(x => new DiscountOption(x.Id, $"{x.Name} - {x.DiscountPercent:0.##}%", x.DiscountPercent)))
                .ToList();

            OrderTypeComboBox.SelectedValue = _details.OrderTypeId;
            StatusComboBox.SelectedValue = _details.StatusId;
            DiscountComboBox.SelectedValue = _details.DiscountId;
            PickupAtTextBox.Text = _details.PickupAt.HasValue
                ? _details.PickupAt.Value.ToString("dd.MM.yyyy HH:mm", CultureInfo.CurrentCulture)
                : string.Empty;
            CommentTextBox.Text = _details.Comment ?? string.Empty;
        }

        private void LoadOrderItems()
        {
            foreach (var dish in _details.Dishes ?? new List<OrderHistoryDishItemDto>())
            {
                _items.Add(new OrderLineViewModel
                {
                    ItemType = DishType,
                    ItemId = dish.DishId,
                    Name = dish.Name,
                    Quantity = dish.Quantity,
                    Toppings = (dish.Toppings ?? new List<OrderHistoryLinkedToppingDto>())
                        .Select(x => new LinkedToppingViewModel
                        {
                            ToppingId = x.ToppingId,
                            Name = x.Name,
                            Quantity = dish.Quantity > 0m ? x.Quantity / dish.Quantity : x.Quantity
                        })
                        .ToList()
                });
            }

            foreach (var drink in _details.Drinks ?? new List<OrderHistoryDrinkItemDto>())
            {
                _items.Add(new OrderLineViewModel
                {
                    ItemType = DrinkType,
                    ItemId = drink.DrinkId,
                    Name = drink.Name,
                    Quantity = drink.Quantity,
                    MilkIngredientId = drink.MilkIngredientId,
                    MilkIngredientName = drink.MilkIngredientName,
                    CoffeeIngredientId = drink.CoffeeIngredientId,
                    CoffeeIngredientName = drink.CoffeeIngredientName,
                    Toppings = (drink.Toppings ?? new List<OrderHistoryLinkedToppingDto>())
                        .Select(x => new LinkedToppingViewModel
                        {
                            ToppingId = x.ToppingId,
                            Name = x.Name,
                            Quantity = drink.Quantity > 0m ? x.Quantity / drink.Quantity : x.Quantity
                        })
                        .ToList()
                });
            }

            foreach (var topping in _details.Toppings ?? new List<OrderHistoryToppingItemDto>())
            {
                _items.Add(new OrderLineViewModel
                {
                    ItemType = ToppingType,
                    ItemId = topping.ToppingId,
                    Name = topping.Name,
                    Quantity = topping.Quantity
                });
            }
        }

        private void ItemTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyMenuSearch();
        }

        private void MenuSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyMenuSearch();
        }

        private void ApplyMenuSearch()
        {
            if (_visibleMenuItems == null || MenuSearchTextBox == null)
            {
                return;
            }

            var selectedType = ItemTypeComboBox?.SelectedItem as string;
            var source = selectedType switch
            {
                DishTypeText => _dishOptions,
                DrinkTypeText => _drinkOptions,
                ToppingTypeText => _toppingOptions,
                _ => _dishOptions
            };
            var query = (MenuSearchTextBox.Text ?? string.Empty).Trim().ToLowerInvariant();
            var filtered = source
                .Where(x => string.IsNullOrWhiteSpace(query) || x.Name.ToLowerInvariant().Contains(query))
                .ToList();

            _visibleMenuItems.Clear();
            foreach (var item in filtered)
            {
                _visibleMenuItems.Add(item);
            }

            MenuItemComboBox.SelectedIndex = _visibleMenuItems.Count > 0 ? 0 : -1;
        }

        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            HideValidation();
            if (MenuItemComboBox.SelectedItem is not MenuItemOption selected)
            {
                ShowValidation("Выберите позицию меню.");
                return;
            }

            if (!TryParseQuantity(QuantityTextBox.Text, out var quantity) || quantity <= 0m)
            {
                ShowValidation("Количество должно быть больше нуля.");
                return;
            }

            _items.Add(new OrderLineViewModel
            {
                ItemType = selected.ItemType,
                ItemId = selected.Id,
                Name = selected.Name,
                Quantity = quantity
            });
        }

        private void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            if (ItemsGrid.SelectedItem is OrderLineViewModel selected)
            {
                _items.Remove(selected);
            }
        }

        private void EditToppings_Click(object sender, RoutedEventArgs e)
        {
            if (ItemsGrid.SelectedItem is not OrderLineViewModel selected || selected.ItemType == ToppingType)
            {
                ShowValidation("Выберите блюдо или напиток.");
                return;
            }

            var categoryToken = selected.ItemType == DishType ? "к блюд" : "к напит";
            var available = (_menu.Toppings ?? new List<ToppingDto>())
                .Where(x =>
                    x.IsAvailable &&
                    (ContainsToken(x.Category, categoryToken) ||
                     string.Equals((x.Category ?? string.Empty).Trim(), "Общие", StringComparison.CurrentCultureIgnoreCase)))
                .OrderBy(x => x.Name)
                .Select(x =>
                {
                    var existing = selected.Toppings.FirstOrDefault(t => t.ToppingId == x.Id);
                    return new LinkedToppingViewModel
                    {
                        ToppingId = x.Id,
                        Name = x.Name ?? $"Добавка #{x.Id}",
                        Quantity = existing?.Quantity ?? 0m
                    };
                })
                .ToList();

            var editor = new ToppingsEditorWindow(available)
            {
                Owner = this
            };

            if (editor.ShowDialog() == true)
            {
                selected.Toppings = editor.SelectedToppings;
                selected.RefreshDisplay();
            }
        }

        private void ItemsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSelectedLineControls();
        }

        private void Modifier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ItemsGrid.SelectedItem is not OrderLineViewModel selected || selected.ItemType != DrinkType)
            {
                return;
            }

            if (MilkComboBox.SelectedItem is ModifierOption milk)
            {
                selected.MilkIngredientId = milk.Id;
                selected.MilkIngredientName = milk.Id.HasValue ? milk.Name : string.Empty;
            }

            if (CoffeeComboBox.SelectedItem is ModifierOption coffee)
            {
                selected.CoffeeIngredientId = coffee.Id;
                selected.CoffeeIngredientName = coffee.Id.HasValue ? coffee.Name : string.Empty;
            }

            selected.RefreshDisplay();
        }

        private void UpdateSelectedLineControls()
        {
            var selected = ItemsGrid.SelectedItem as OrderLineViewModel;
            var isDrink = selected?.ItemType == DrinkType;
            MilkComboBox.IsEnabled = isDrink;
            CoffeeComboBox.IsEnabled = isDrink;
            MilkComboBox.SelectedValue = isDrink ? selected!.MilkIngredientId : null;
            CoffeeComboBox.SelectedValue = isDrink ? selected!.CoffeeIngredientId : null;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            HideValidation();

            if (OrderTypeComboBox.SelectedValue is not int orderTypeId || orderTypeId <= 0)
            {
                ShowValidation("Выберите тип заказа.");
                return;
            }

            if (_items.Count == 0)
            {
                ShowValidation("Состав заказа не может быть пустым.");
                return;
            }

            foreach (var line in _items)
            {
                if (!TryParseQuantity(line.QuantityText, out var parsed) || parsed <= 0m)
                {
                    ShowValidation("Количество каждой позиции должно быть больше нуля.");
                    return;
                }

                line.Quantity = parsed;
            }

            DateTime? pickupAt = null;
            var rawPickupAt = (PickupAtTextBox.Text ?? string.Empty).Trim();
            var parsedPickupAt = DateTime.MinValue;
            if (!string.IsNullOrWhiteSpace(rawPickupAt) &&
                !DateTime.TryParse(rawPickupAt, CultureInfo.CurrentCulture, DateTimeStyles.None, out parsedPickupAt))
            {
                ShowValidation("Укажите время самовывоза в корректном формате.");
                return;
            }
            else if (!string.IsNullOrWhiteSpace(rawPickupAt))
            {
                pickupAt = parsedPickupAt;
            }

            Request = new OrderHistoryUpdateRequest
            {
                OrderTypeId = orderTypeId,
                StatusId = StatusComboBox.SelectedValue as int?,
                DiscountId = DiscountComboBox.SelectedValue as int?,
                Comment = (CommentTextBox.Text ?? string.Empty).Trim(),
                PickupAt = pickupAt,
                Dishes = _items
                    .Where(x => x.ItemType == DishType)
                    .Select(x => new OrderDishItemRequest
                    {
                        DishId = x.ItemId,
                        Quantity = x.Quantity,
                        Toppings = x.Toppings
                            .Where(t => t.Quantity > 0m)
                            .Select(t => new OrderItemToppingRequest
                            {
                                ToppingId = t.ToppingId,
                                Quantity = t.Quantity
                            })
                            .ToList()
                    })
                    .ToList(),
                Drinks = _items
                    .Where(x => x.ItemType == DrinkType)
                    .Select(x => new OrderDrinkItemRequest
                    {
                        DrinkId = x.ItemId,
                        Quantity = x.Quantity,
                        MilkIngredientId = x.MilkIngredientId,
                        CoffeeIngredientId = x.CoffeeIngredientId,
                        Toppings = x.Toppings
                            .Where(t => t.Quantity > 0m)
                            .Select(t => new OrderItemToppingRequest
                            {
                                ToppingId = t.ToppingId,
                                Quantity = t.Quantity
                            })
                            .ToList()
                    })
                    .ToList(),
                Toppings = _items
                    .Where(x => x.ItemType == ToppingType)
                    .Select(x => new OrderToppingItemRequest
                    {
                        ToppingId = x.ItemId,
                        Quantity = (int)Math.Round(x.Quantity, MidpointRounding.AwayFromZero)
                    })
                    .ToList()
            };

            DialogResult = true;
            Close();
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

        private static bool TryParseQuantity(string? value, out decimal quantity)
        {
            var normalized = (value ?? string.Empty).Trim();
            if (decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.CurrentCulture, out quantity))
            {
                return true;
            }

            normalized = normalized.Replace(',', '.');
            return decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.InvariantCulture, out quantity);
        }

        private static bool ContainsToken(string? value, string token)
        {
            return (value ?? string.Empty).Trim().ToLowerInvariant().Contains(token);
        }

        private sealed record MenuItemOption(string ItemType, int Id, string Name);
        private sealed record ModifierOption(int? Id, string Name);
        private sealed record DiscountOption(int? Id, string DisplayName, decimal DiscountPercent);

        private sealed class OrderLineViewModel : INotifyPropertyChanged
        {
            private decimal _quantity;
            private string _quantityText = "1";
            private List<LinkedToppingViewModel> _toppings = new List<LinkedToppingViewModel>();

            public event PropertyChangedEventHandler? PropertyChanged;

            public string ItemType { get; set; } = string.Empty;
            public int ItemId { get; set; }
            public string Name { get; set; } = string.Empty;
            public int? MilkIngredientId { get; set; }
            public string MilkIngredientName { get; set; } = string.Empty;
            public int? CoffeeIngredientId { get; set; }
            public string CoffeeIngredientName { get; set; } = string.Empty;

            public decimal Quantity
            {
                get => _quantity;
                set
                {
                    _quantity = value;
                    _quantityText = FormatQuantity(value);
                    OnPropertyChanged(nameof(QuantityText));
                }
            }

            public string QuantityText
            {
                get => _quantityText;
                set => _quantityText = value;
            }

            public List<LinkedToppingViewModel> Toppings
            {
                get => _toppings;
                set
                {
                    _toppings = value ?? new List<LinkedToppingViewModel>();
                    RefreshDisplay();
                }
            }

            public string TypeDisplay => ItemType switch
            {
                DishType => DishTypeText,
                DrinkType => DrinkTypeText,
                ToppingType => ToppingTypeText,
                _ => "Позиция"
            };

            public string ToppingsDisplay => Toppings.Count == 0
                ? string.Empty
                : string.Join(", ", Toppings.Where(x => x.Quantity > 0m).Select(x => $"{x.Name} x{FormatQuantity(x.Quantity)}"));

            public string ModifiersDisplay
            {
                get
                {
                    var parts = new List<string>();
                    if (!string.IsNullOrWhiteSpace(MilkIngredientName))
                    {
                        parts.Add("Молоко: " + MilkIngredientName);
                    }

                    if (!string.IsNullOrWhiteSpace(CoffeeIngredientName))
                    {
                        parts.Add("Кофе: " + CoffeeIngredientName);
                    }

                    return string.Join("; ", parts);
                }
            }

            public void RefreshDisplay()
            {
                OnPropertyChanged(nameof(ToppingsDisplay));
                OnPropertyChanged(nameof(ModifiersDisplay));
            }

            private void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private sealed class LinkedToppingViewModel : INotifyPropertyChanged
        {
            private decimal _quantity;
            private string _quantityText = "0";

            public event PropertyChangedEventHandler? PropertyChanged;

            public int ToppingId { get; set; }
            public string Name { get; set; } = string.Empty;

            public decimal Quantity
            {
                get => _quantity;
                set
                {
                    _quantity = value;
                    _quantityText = FormatQuantity(value);
                    OnPropertyChanged(nameof(QuantityText));
                }
            }

            public string QuantityText
            {
                get => _quantityText;
                set => _quantityText = value;
            }

            private void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private sealed class ToppingsEditorWindow : Window
        {
            private readonly List<LinkedToppingViewModel> _toppings;
            private readonly TextBlock _validationText;

            public ToppingsEditorWindow(List<LinkedToppingViewModel> toppings)
            {
                _toppings = toppings;
                Title = "Добавки";
                Width = 620;
                Height = 620;
                WindowStartupLocation = WindowStartupLocation.CenterOwner;
                ResizeMode = ResizeMode.NoResize;
                WindowStyle = WindowStyle.None;
                AllowsTransparency = true;
                Background = System.Windows.Media.Brushes.Transparent;

                var shell = new Border
                {
                    Style = (Style)Application.Current.Resources["ModalShellBorder"]
                };
                var root = new Grid();
                root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                var header = new Border
                {
                    Style = (Style)Application.Current.Resources["ModalHeader"]
                };
                header.MouseLeftButtonDown += (_, e) =>
                {
                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        DragMove();
                    }
                };
                header.Child = new TextBlock
                {
                    Text = "Добавки",
                    Style = (Style)Application.Current.Resources["ModalHeaderTitle"]
                };
                root.Children.Add(header);

                var grid = new DataGrid
                {
                    ItemsSource = _toppings,
                    AutoGenerateColumns = false,
                    CanUserAddRows = false,
                    FontSize = 17,
                    Margin = new Thickness(18)
                };
                grid.Columns.Add(new DataGridTextColumn { Header = "Добавка", Binding = new System.Windows.Data.Binding("Name"), IsReadOnly = true, Width = new DataGridLength(1, DataGridLengthUnitType.Star) });
                grid.Columns.Add(new DataGridTextColumn { Header = "Кол-во на позицию", Binding = new System.Windows.Data.Binding("QuantityText") { UpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged }, Width = 170 });
                Grid.SetRow(grid, 1);
                root.Children.Add(grid);

                var footer = new Grid { Margin = new Thickness(18, 0, 18, 18) };
                footer.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                footer.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                _validationText = new TextBlock
                {
                    Foreground = System.Windows.Media.Brushes.Firebrick,
                    FontWeight = FontWeights.SemiBold,
                    Visibility = Visibility.Collapsed,
                    VerticalAlignment = VerticalAlignment.Center
                };
                footer.Children.Add(_validationText);
                var buttons = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Right
                };
                var cancel = new Button
                {
                    Content = "Отмена",
                    Width = 130,
                    Style = (Style)Application.Current.Resources["ModalSecondaryButton"],
                    Margin = new Thickness(0, 0, 10, 0)
                };
                cancel.Click += (_, _) => DialogResult = false;
                var save = new Button
                {
                    Content = "Готово",
                    Width = 130,
                    Style = (Style)Application.Current.Resources["ModalPrimaryButton"]
                };
                save.Click += Save_Click;
                buttons.Children.Add(cancel);
                buttons.Children.Add(save);
                Grid.SetColumn(buttons, 1);
                footer.Children.Add(buttons);
                Grid.SetRow(footer, 2);
                root.Children.Add(footer);

                shell.Child = root;
                Content = shell;
            }

            public List<LinkedToppingViewModel> SelectedToppings { get; private set; } = new List<LinkedToppingViewModel>();

            private void Save_Click(object sender, RoutedEventArgs e)
            {
                foreach (var topping in _toppings)
                {
                    if (!TryParseQuantity(topping.QuantityText, out var quantity) || quantity < 0m)
                    {
                        _validationText.Text = "Количество добавки не может быть отрицательным.";
                        _validationText.Visibility = Visibility.Visible;
                        return;
                    }

                    topping.Quantity = quantity;
                }

                SelectedToppings = _toppings.Where(x => x.Quantity > 0m).ToList();
                DialogResult = true;
            }
        }

        private static string FormatQuantity(decimal value)
        {
            return value == decimal.Truncate(value)
                ? value.ToString("0", CultureInfo.CurrentCulture)
                : value.ToString("0.##", CultureInfo.CurrentCulture);
        }
    }
}
