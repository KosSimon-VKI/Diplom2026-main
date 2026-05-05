using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GardenNookWpf.Views.Shell;
using TransferModels.Menu;
using TransferModels.Orders;

namespace GardenNookWpf.Views.Shell.Sections.Menu
{
    public partial class OrderItemView : UserControl, IMainSectionView
    {
        private const string ApiBaseAddress = "https://localhost:7235";
        private const string MenuAddress = ApiBaseAddress + "/api/menu";
        private const string OrdersAddress = ApiBaseAddress + "/api/orders";
        private const string PickupSlotsAddress = OrdersAddress + "/pickup-slots";
        private const string DiscountsAddress = OrdersAddress + "/discounts";
        private const string DishType = "dishes";
        private const string DrinkType = "drinks";
        private const string ToppingType = "toppings";
        private const int CoffeeDrinkCategoryId = 1;
        private const int DefaultTakeawayOrderTypeId = 2;
        private const string DefaultMilkOptionName = "КОРОВЬЕ МОЛОКО";
        private const string DefaultCoffeeOptionName = "Кофе в зернах ТАВ Galaxy";

        private static readonly HashSet<int> ModifierExcludedDrinkIds = new HashSet<int> { 5, 6, 43, 12 };
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly HttpClient _httpClient;
        private readonly ObservableCollection<MenuPositionViewModel> _visibleItems = new ObservableCollection<MenuPositionViewModel>();
        private readonly ObservableCollection<CartItemViewModel> _cartItems = new ObservableCollection<CartItemViewModel>();
        private readonly List<MenuPositionViewModel> _allItems = new List<MenuPositionViewModel>();
        private readonly List<ToppingDto> _allToppings = new List<ToppingDto>();
        private readonly ObservableCollection<DiscountOptionViewModel> _discounts = new ObservableCollection<DiscountOptionViewModel>();
        private readonly ObservableCollection<PickupSlotViewModel> _pickupSlots = new ObservableCollection<PickupSlotViewModel>();
        private DrinkModifierCatalogDto _drinkModifiers = new DrinkModifierCatalogDto();
        private string _activeType = "all";
        private string? _activeSubcategory;
        private int _takeawayOrderTypeId = DefaultTakeawayOrderTypeId;
        private bool _isLoadedOnce;
        private bool _isBusy;

        public OrderItemView(HttpClient httpClient, string userRole)
        {
            _httpClient = httpClient;
            InitializeComponent();

            MenuItemsList.ItemsSource = _visibleItems;
            CartItemsList.ItemsSource = _cartItems;
            DiscountComboBox.ItemsSource = _discounts;
            PickupSlotComboBox.ItemsSource = _pickupSlots;
            _discounts.Add(new DiscountOptionViewModel());
            DiscountComboBox.SelectedIndex = 0;
            UpdateTotals();
        }

        public bool IsBusy => _isBusy;

        public async Task ActivateAsync()
        {
            if (_isLoadedOnce)
            {
                ApplyFilters();
                return;
            }

            await ReloadAsync();
        }

        public void Deactivate()
        {
        }

        private async Task ReloadAsync()
        {
            try
            {
                SetBusy(true);
                SetStatus("Загрузка меню...");

                await LoadMenuAsync();
                await LoadDiscountsAsync();
                await LoadPickupSlotsAsync();

                _isLoadedOnce = true;
                SetStatus(string.Empty);
                ApplyFilters();
                UpdateCartItemNumbers();
                UpdateTotals();
            }
            catch (Exception ex)
            {
                _allItems.Clear();
                _visibleItems.Clear();
                MenuEmptyText.Visibility = Visibility.Visible;
                SetStatus("Не удалось загрузить меню: " + ex.Message);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async Task LoadMenuAsync()
        {
            using var response = await _httpClient.GetAsync(MenuAddress);
            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new InvalidOperationException("нет доступа к меню.");
            }

            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var menu = JsonSerializer.Deserialize<MenuResponse>(json, JsonOptions) ?? new MenuResponse();

            _allItems.Clear();
            _allToppings.Clear();
            _allToppings.AddRange(menu.Toppings ?? new List<ToppingDto>());
            _drinkModifiers = menu.DrinkModifiers ?? new DrinkModifierCatalogDto();

            foreach (var dish in menu.Dishes ?? new List<DishDto>())
            {
                _allItems.Add(MenuPositionViewModel.FromDish(dish));
            }

            foreach (var drink in menu.Drinks ?? new List<DrinkDto>())
            {
                _allItems.Add(MenuPositionViewModel.FromDrink(drink));
            }

            foreach (var topping in menu.Toppings ?? new List<ToppingDto>())
            {
                _allItems.Add(MenuPositionViewModel.FromTopping(topping));
            }

            DishesCategoryButton.Content = $"Блюда ({_allItems.Count(x => x.ItemType == DishType)})";
            DrinksCategoryButton.Content = $"Напитки ({_allItems.Count(x => x.ItemType == DrinkType)})";
            ToppingsCategoryButton.Content = $"Добавки ({_allItems.Count(x => x.ItemType == ToppingType)})";
            AllCategoryButton.Content = $"Все ({_allItems.Count})";
        }

        private async Task LoadDiscountsAsync()
        {
            _discounts.Clear();
            _discounts.Add(new DiscountOptionViewModel());

            using var response = await _httpClient.GetAsync(DiscountsAddress);
            if (!response.IsSuccessStatusCode)
            {
                DiscountComboBox.SelectedIndex = 0;
                return;
            }

            var json = await response.Content.ReadAsStringAsync();
            var discounts = JsonSerializer.Deserialize<List<DiscountDto>>(json, JsonOptions) ?? new List<DiscountDto>();
            foreach (var discount in discounts.OrderBy(x => x.Id))
            {
                _discounts.Add(new DiscountOptionViewModel(discount));
            }

            DiscountComboBox.SelectedIndex = 0;
        }

        private async Task LoadPickupSlotsAsync()
        {
            _pickupSlots.Clear();
            PickupSlotNoteText.Visibility = Visibility.Collapsed;

            using var response = await _httpClient.GetAsync(PickupSlotsAddress);
            if (!response.IsSuccessStatusCode)
            {
                PickupSlotNoteText.Text = "Не удалось загрузить слоты самовывоза. Можно оформить без времени.";
                PickupSlotNoteText.Visibility = Visibility.Visible;
                return;
            }

            var json = await response.Content.ReadAsStringAsync();
            var payload = JsonSerializer.Deserialize<PickupSlotsResponse>(json, JsonOptions) ?? new PickupSlotsResponse();
            _takeawayOrderTypeId = payload.TakeawayOrderTypeId > 0
                ? payload.TakeawayOrderTypeId
                : DefaultTakeawayOrderTypeId;

            foreach (var slot in payload.Slots ?? new List<PickupSlotDto>())
            {
                if (!string.IsNullOrWhiteSpace(slot.Value) && !string.IsNullOrWhiteSpace(slot.Label))
                {
                    _pickupSlots.Add(new PickupSlotViewModel(slot.Value, slot.Label));
                }
            }

            if (_pickupSlots.Count > 0)
            {
                PickupSlotComboBox.SelectedIndex = 0;
            }
            else
            {
                PickupSlotNoteText.Text = "На сегодня нет доступных слотов. Можно оформить без времени.";
                PickupSlotNoteText.Visibility = Visibility.Visible;
            }
        }

        private void AddMenuItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button || button.Tag is not MenuPositionViewModel item || !item.IsAvailable)
            {
                return;
            }

            if (item.ItemType == ToppingType)
            {
                _cartItems.Add(CartItemViewModel.FromStandaloneTopping(item));
                AfterCartChanged();
                return;
            }

            DrinkModifierSelection? modifierSelection = null;
            if (item.ItemType == DrinkType && RequiresDrinkModifiers(item))
            {
                modifierSelection = ShowDrinkModifierDialog();
                if (modifierSelection == null)
                {
                    return;
                }
            }

            var toppingCategory = item.ItemType == DishType ? "К блюдам" : "К напиткам";
            var selectedToppings = ShowToppingsDialog(toppingCategory);
            if (selectedToppings == null)
            {
                return;
            }

            _cartItems.Add(CartItemViewModel.FromMenuPosition(item, selectedToppings, modifierSelection));
            AfterCartChanged();
        }

        private bool RequiresDrinkModifiers(MenuPositionViewModel item)
        {
            return item.CategoryId == CoffeeDrinkCategoryId && !ModifierExcludedDrinkIds.Contains(item.ItemId);
        }

        private List<CartToppingViewModel>? ShowToppingsDialog(string toppingCategory)
        {
            var available = _allToppings
                .Where(x =>
                    string.Equals(x.Category?.Trim(), toppingCategory, StringComparison.CurrentCultureIgnoreCase) ||
                    string.Equals(x.Category?.Trim(), "Общие", StringComparison.CurrentCultureIgnoreCase))
                .OrderByDescending(x => x.IsAvailable)
                .ThenBy(x => x.Name)
                .Select(x => new ToppingSelectionViewModel(x))
                .ToList();

            var (window, bodyHost) = CreateMenuDialog("Добавки", 640, 620);
            bodyHost.Content = BuildToppingsDialogContent(available, window);

            if (window.ShowDialog() != true)
            {
                return null;
            }

            return available
                .Where(x => x.Quantity > 0 && x.IsAvailable)
                .Select(x => new CartToppingViewModel
                {
                    ToppingId = x.Id,
                    Name = x.Name,
                    Price = x.Price,
                    Calories = x.Calories,
                    Quantity = x.Quantity
                })
                .ToList();

            static Grid BuildToppingsDialogContent(List<ToppingSelectionViewModel> toppings, Window ownerWindow)
            {
                var root = new Grid { Margin = new Thickness(16) };
                root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                var list = new ItemsControl { ItemsSource = toppings };
                list.ItemTemplate = BuildToppingSelectionTemplate();
                var scroll = new ScrollViewer
                {
                    Content = list,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                    Padding = new Thickness(0, 0, 18, 0)
                };
                Grid.SetRow(scroll, 0);
                root.Children.Add(scroll);

                var footer = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Margin = new Thickness(0, 14, 18, 0)
                };
                var cancel = new Button
                {
                    Content = "Отмена",
                    Width = 130,
                    Height = 42,
                    Margin = new Thickness(0, 0, 10, 0),
                    Style = (Style)Application.Current.Resources["ModalSecondaryButton"]
                };
                cancel.Click += (_, _) => ownerWindow.DialogResult = false;
                var confirm = new Button
                {
                    Content = "Добавить",
                    Width = 140,
                    Height = 42,
                    Style = (Style)Application.Current.Resources["ModalPrimaryButton"]
                };
                confirm.Click += (_, _) => ownerWindow.DialogResult = true;
                footer.Children.Add(cancel);
                footer.Children.Add(confirm);
                Grid.SetRow(footer, 1);
                root.Children.Add(footer);

                return root;
            }
        }

        private static DataTemplate BuildToppingSelectionTemplate()
        {
            const string template = @"
<DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
  <Border BorderBrush=""#FF91A56E"" BorderThickness=""0,0,0,2"" Padding=""0,10"" Opacity=""{Binding Opacity}"">
    <Grid>
      <Grid.ColumnDefinitions>
        <ColumnDefinition Width=""*""/>
        <ColumnDefinition Width=""Auto""/>
      </Grid.ColumnDefinitions>
      <StackPanel>
        <TextBlock Text=""{Binding Name}"" FontSize=""20"" FontWeight=""Bold"" TextWrapping=""Wrap""/>
        <TextBlock Text=""{Binding PriceDisplay}"" FontSize=""17"" Margin=""0,3,0,0""/>
        <TextBlock Text=""Недоступно"" FontSize=""15"" FontWeight=""Bold"" Foreground=""#FF742C27"" Visibility=""{Binding UnavailableVisibility}""/>
      </StackPanel>
      <StackPanel Grid.Column=""1"" Orientation=""Horizontal"" VerticalAlignment=""Center"">
        <Button Content=""−"" Width=""36"" Height=""36"" FontSize=""20"" FontWeight=""Bold"" Padding=""0"" Command=""{Binding DecreaseCommand}"" IsEnabled=""{Binding IsAvailable}""/>
        <TextBlock Text=""{Binding Quantity}"" FontSize=""20"" FontWeight=""Bold"" Width=""38"" TextAlignment=""Center"" VerticalAlignment=""Center""/>
        <Button Content=""+"" Width=""36"" Height=""36"" FontSize=""20"" FontWeight=""Bold"" Padding=""0"" Command=""{Binding IncreaseCommand}"" IsEnabled=""{Binding IsAvailable}""/>
      </StackPanel>
    </Grid>
  </Border>
</DataTemplate>";
            return (DataTemplate)System.Windows.Markup.XamlReader.Parse(template);
        }

        private DrinkModifierSelection? ShowDrinkModifierDialog()
        {
            var milkOptions = _drinkModifiers.MilkOptions ?? new List<DrinkModifierOptionDto>();
            var coffeeOptions = _drinkModifiers.CoffeeOptions ?? new List<DrinkModifierOptionDto>();
            if (milkOptions.Count == 0 && coffeeOptions.Count == 0)
            {
                return null;
            }

            var (window, bodyHost) = CreateMenuDialog("Модификаторы напитка", 580, 390);

            var milkCombo = new ComboBox
            {
                ItemsSource = milkOptions,
                DisplayMemberPath = "Name",
                SelectedValuePath = "Id",
                Height = 38,
                FontSize = 17
            };
            milkCombo.SelectedItem = ResolveDefaultModifier(milkOptions, DefaultMilkOptionName);

            var coffeeCombo = new ComboBox
            {
                ItemsSource = coffeeOptions,
                DisplayMemberPath = "Name",
                SelectedValuePath = "Id",
                Height = 38,
                FontSize = 17
            };
            coffeeCombo.SelectedItem = ResolveDefaultModifier(coffeeOptions, DefaultCoffeeOptionName);

            var root = new Grid { Margin = new Thickness(18) };
            root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            AddDialogLabel(root, "Молоко", 0);
            Grid.SetRow(milkCombo, 1);
            root.Children.Add(milkCombo);
            AddDialogLabel(root, "Кофе", 2);
            Grid.SetRow(coffeeCombo, 3);
            root.Children.Add(coffeeCombo);

            var footer = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            var cancel = new Button
            {
                Content = "Отмена",
                Width = 130,
                Height = 42,
                Margin = new Thickness(0, 0, 10, 0),
                Style = (Style)Application.Current.Resources["ModalSecondaryButton"]
            };
            cancel.Click += (_, _) => window.DialogResult = false;
            var next = new Button
            {
                Content = "Далее",
                Width = 130,
                Height = 42,
                Style = (Style)Application.Current.Resources["ModalPrimaryButton"]
            };
            next.Click += (_, _) => window.DialogResult = true;
            footer.Children.Add(cancel);
            footer.Children.Add(next);
            Grid.SetRow(footer, 5);
            root.Children.Add(footer);

            bodyHost.Content = root;
            if (window.ShowDialog() != true)
            {
                return null;
            }

            var milk = milkCombo.SelectedItem as DrinkModifierOptionDto;
            var coffee = coffeeCombo.SelectedItem as DrinkModifierOptionDto;
            return new DrinkModifierSelection(
                milk?.Id,
                milk?.Name,
                coffee?.Id,
                coffee?.Name);
        }

        private static void AddDialogLabel(Grid root, string text, int row)
        {
            var label = new TextBlock
            {
                Text = text,
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Foreground = (Brush)Application.Current.Resources["ModalColorTextBrush"],
                Margin = new Thickness(0, row == 0 ? 0 : 12, 0, 6)
            };
            Grid.SetRow(label, row);
            root.Children.Add(label);
        }

        private (Window Window, ContentControl BodyHost) CreateMenuDialog(string title, double width, double height)
        {
            var owner = Window.GetWindow(this);
            var window = new Window
            {
                Owner = owner,
                Width = width,
                Height = height,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ResizeMode = ResizeMode.NoResize,
                WindowStyle = WindowStyle.None,
                AllowsTransparency = true,
                Background = Brushes.Transparent,
                ShowInTaskbar = false
            };

            var shell = new Border
            {
                Background = (Brush)Application.Current.Resources["ModalColorWhiteBrush"],
                BorderBrush = (Brush)Application.Current.Resources["ModalColorBorderBrush"],
                BorderThickness = new Thickness(2),
                CornerRadius = new CornerRadius(20),
                SnapsToDevicePixels = true
            };

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            var header = new Border
            {
                Background = (Brush)Application.Current.Resources["ModalColorPrimaryBrush"],
                BorderBrush = (Brush)Application.Current.Resources["ModalColorBorderBrush"],
                BorderThickness = new Thickness(0, 0, 0, 2),
                CornerRadius = new CornerRadius(18, 18, 0, 0),
                Padding = new Thickness(18, 12, 12, 12)
            };

            var headerGrid = new Grid();
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            headerGrid.Children.Add(new TextBlock
            {
                Text = title,
                FontSize = 26,
                FontWeight = FontWeights.Bold,
                Foreground = (Brush)Application.Current.Resources["ModalColorTextBrush"],
                VerticalAlignment = VerticalAlignment.Center
            });

            var closeButton = new Button
            {
                Content = "×",
                Style = (Style)Application.Current.Resources["ModalCloseButton"]
            };
            closeButton.Click += (_, _) => window.DialogResult = false;
            Grid.SetColumn(closeButton, 1);
            headerGrid.Children.Add(closeButton);

            header.Child = headerGrid;
            grid.Children.Add(header);

            var bodyHost = new ContentControl
            {
                Margin = new Thickness(0)
            };
            Grid.SetRow(bodyHost, 1);
            grid.Children.Add(bodyHost);

            shell.Child = grid;
            window.Content = shell;
            return (window, bodyHost);
        }

        private static DrinkModifierOptionDto? ResolveDefaultModifier(
            IReadOnlyList<DrinkModifierOptionDto> options,
            string preferredName)
        {
            if (options.Count == 0)
            {
                return null;
            }

            var normalized = NormalizeModifierName(preferredName);
            return options.FirstOrDefault(x => NormalizeModifierName(x.Name) == normalized) ?? options[0];
        }

        private static string NormalizeModifierName(string? value)
        {
            return string.Join(" ", (value ?? string.Empty)
                    .Trim()
                    .Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries))
                .ToUpperInvariant();
        }

        private void MainCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string type)
            {
                _activeType = type;
                _activeSubcategory = null;
                ApplyFilters();
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void PriceSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (PriceValueText != null)
            {
                PriceValueText.Text = $"0 - {(int)e.NewValue} ₽";
                ApplyFilters();
            }
        }

        private void ApplyFilters()
        {
            if (PriceSlider == null)
            {
                return;
            }

            var query = (SearchTextBox.Text ?? string.Empty).Trim();
            var maxPrice = (decimal)PriceSlider.Value;

            var filtered = _allItems
                .Where(x => _activeType == "all" || x.ItemType == _activeType)
                .Where(x => string.IsNullOrWhiteSpace(_activeSubcategory) || string.Equals(x.Category, _activeSubcategory, StringComparison.CurrentCultureIgnoreCase))
                .Where(x => x.Price <= maxPrice)
                .Where(x => string.IsNullOrWhiteSpace(query) ||
                            x.Name.Contains(query, StringComparison.CurrentCultureIgnoreCase) ||
                            x.Category.Contains(query, StringComparison.CurrentCultureIgnoreCase))
                .ToList();

            _visibleItems.Clear();
            foreach (var item in filtered)
            {
                _visibleItems.Add(item);
            }

            MenuScrollViewer.Visibility = _visibleItems.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
            MenuEmptyText.Text = _allItems.Count == 0 ? "Меню пусто" : "По вашему запросу ничего не найдено.";
            MenuEmptyText.Visibility = _visibleItems.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            RenderSubcategories();
            HighlightCategoryButtons();
        }

        private void RenderSubcategories()
        {
            SubcategoriesList.ItemsSource = null;
            SubcategoriesTitle.Visibility = Visibility.Collapsed;

            if (_activeType == "all")
            {
                return;
            }

            var categories = _allItems
                .Where(x => x.ItemType == _activeType)
                .Select(x => x.Category)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.CurrentCultureIgnoreCase)
                .OrderBy(x => x)
                .Select(x => new SubcategoryViewModel(x, string.Equals(x, _activeSubcategory, StringComparison.CurrentCultureIgnoreCase)))
                .ToList();

            if (categories.Count == 0)
            {
                return;
            }

            SubcategoriesTitle.Visibility = Visibility.Visible;
            SubcategoriesList.ItemsSource = categories;
        }

        private void SubcategoryButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.DataContext is SubcategoryViewModel subcategory)
            {
                _activeSubcategory = string.Equals(_activeSubcategory, subcategory.Name, StringComparison.CurrentCultureIgnoreCase)
                    ? null
                    : subcategory.Name;
                ApplyFilters();
            }
        }

        private void HighlightCategoryButtons()
        {
            foreach (var button in new[] { AllCategoryButton, DishesCategoryButton, DrinksCategoryButton, ToppingsCategoryButton })
            {
                var isActive = string.Equals(button.Tag as string, _activeType, StringComparison.OrdinalIgnoreCase);
                button.Background = isActive
                    ? (Brush)Application.Current.Resources["ModalColorPrimaryBrush"]
                    : Brushes.White;
                button.Foreground = isActive ? Brushes.White : (Brush)Application.Current.Resources["ModalColorTextBrush"];
            }
        }

        private void DecreaseCartItemButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is CartItemViewModel item)
            {
                item.Quantity--;
                if (item.Quantity <= 0)
                {
                    _cartItems.Remove(item);
                }

                AfterCartChanged();
            }
        }

        private void IncreaseCartItemButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is CartItemViewModel item)
            {
                item.Quantity++;
                AfterCartChanged();
            }
        }

        private void RemoveCartItemButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is CartItemViewModel item)
            {
                _cartItems.Remove(item);
                AfterCartChanged();
            }
        }

        private void DiscountComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateTotals();
        }

        private void OrderTypeRadio_Checked(object sender, RoutedEventArgs e)
        {
            if (PickupSlotPanel == null)
            {
                return;
            }

            PickupSlotPanel.Visibility = TakeawayOrderTypeRadio.IsChecked == true
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private async void SubmitOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isBusy)
            {
                return;
            }

            ClearOrderMessage();

            if (_cartItems.Count == 0)
            {
                ShowOrderMessage("Корзина пуста.", true);
                return;
            }

            try
            {
                SetBusy(true);
                SubmitOrderButton.IsEnabled = false;

                var orderTypeId = TakeawayOrderTypeRadio.IsChecked == true
                    ? _takeawayOrderTypeId
                    : 1;
                var request = BuildOrderRequest(orderTypeId);
                var payload = JsonSerializer.Serialize(request);
                using var content = new StringContent(payload, Encoding.UTF8, "application/json");
                using var response = await _httpClient.PostAsync(OrdersAddress, content);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    ShowOrderMessage(string.IsNullOrWhiteSpace(error)
                        ? $"Не удалось оформить заказ. Код: {(int)response.StatusCode}."
                        : error,
                        true);
                    return;
                }

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<OrderResponse>(json, JsonOptions);
                _cartItems.Clear();
                OrderCommentTextBox.Text = string.Empty;
                AfterCartChanged();
                ShowOrderMessage(result == null
                    ? "Заказ оформлен."
                    : $"Заказ №{result.OrderId} оформлен. Статус: {result.Status}. Итого: {result.TotalPrice:0.##} ₽.",
                    false);

                await LoadMenuAsync();
                ApplyFilters();
            }
            catch (Exception ex)
            {
                ShowOrderMessage("Ошибка оформления: " + ex.Message, true);
            }
            finally
            {
                SubmitOrderButton.IsEnabled = true;
                SetBusy(false);
            }
        }

        private OrderRequest BuildOrderRequest(int orderTypeId)
        {
            var request = new OrderRequest
            {
                OrderTypeId = orderTypeId,
                DiscountId = (DiscountComboBox.SelectedItem as DiscountOptionViewModel)?.Id,
                Comment = string.IsNullOrWhiteSpace(OrderCommentTextBox.Text) ? null : OrderCommentTextBox.Text.Trim(),
                PickupAt = ResolveSelectedPickupAt(orderTypeId),
                Dishes = new List<OrderDishItemRequest>(),
                Drinks = new List<OrderDrinkItemRequest>(),
                Toppings = new List<OrderToppingItemRequest>()
            };

            foreach (var item in _cartItems)
            {
                if (item.ItemType == DishType)
                {
                    request.Dishes.Add(new OrderDishItemRequest
                    {
                        DishId = item.ItemId,
                        Quantity = item.Quantity,
                        Toppings = item.Toppings
                            .Select(x => new OrderItemToppingRequest
                            {
                                ToppingId = x.ToppingId,
                                Quantity = x.Quantity
                            })
                            .ToList()
                    });
                }
                else if (item.ItemType == DrinkType)
                {
                    request.Drinks.Add(new OrderDrinkItemRequest
                    {
                        DrinkId = item.ItemId,
                        Quantity = item.Quantity,
                        MilkIngredientId = item.MilkIngredientId,
                        CoffeeIngredientId = item.CoffeeIngredientId,
                        Toppings = item.Toppings
                            .Select(x => new OrderItemToppingRequest
                            {
                                ToppingId = x.ToppingId,
                                Quantity = x.Quantity
                            })
                            .ToList()
                    });
                }
                else if (item.ItemType == ToppingType)
                {
                    request.Toppings.Add(new OrderToppingItemRequest
                    {
                        ToppingId = item.ItemId,
                        Quantity = item.Quantity
                    });
                }
            }

            return request;
        }

        private DateTime? ResolveSelectedPickupAt(int orderTypeId)
        {
            if (orderTypeId != _takeawayOrderTypeId ||
                PickupSlotComboBox.SelectedItem is not PickupSlotViewModel slot ||
                string.IsNullOrWhiteSpace(slot.Value))
            {
                return null;
            }

            return DateTime.TryParse(slot.Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var parsed)
                ? parsed
                : null;
        }

        private void AfterCartChanged()
        {
            UpdateCartItemNumbers();
            SyncMenuQuantities();
            UpdateTotals();
        }

        private void UpdateCartItemNumbers()
        {
            for (var i = 0; i < _cartItems.Count; i++)
            {
                _cartItems[i].Number = i + 1;
            }
        }

        private void SyncMenuQuantities()
        {
            foreach (var item in _allItems)
            {
                item.QuantityInCart = _cartItems
                    .Where(x => x.ItemType == item.ItemType && x.ItemId == item.ItemId)
                    .Sum(x => x.Quantity);
            }
        }

        private void UpdateTotals()
        {
            if (CartCaloriesText == null)
            {
                return;
            }

            var total = _cartItems.Sum(x => x.TotalPrice);
            var calories = _cartItems.Sum(x => x.TotalCalories);
            var discount = DiscountComboBox?.SelectedItem as DiscountOptionViewModel;
            var discountPercent = discount?.DiscountPercent ?? 0m;
            var discountedTotal = Round2(total * (1m - discountPercent / 100m));

            CartCaloriesText.Text = $"Ккал: {calories:0.##}";
            CartTotalBeforeDiscountText.Text = discountPercent > 0
                ? $"До скидки: {total:0.##} ₽, скидка: {discountPercent:0.##}%"
                : string.Empty;
            CartTotalText.Text = $"Итого: {discountedTotal:0.##} ₽";
        }

        private void SetBusy(bool isBusy)
        {
            _isBusy = isBusy;
            RootGrid.IsEnabled = !isBusy;
        }

        private void SetStatus(string message)
        {
            StatusText.Text = message;
            StatusText.Visibility = string.IsNullOrWhiteSpace(message)
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        private void ShowOrderMessage(string message, bool isError)
        {
            OrderMessageText.Text = message;
            OrderMessageText.Foreground = isError
                ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF742C27"))
                : (Brush)Application.Current.Resources["ModalColorPrimaryDarkBrush"];
            OrderMessageText.Visibility = Visibility.Visible;
        }

        private void ClearOrderMessage()
        {
            OrderMessageText.Text = string.Empty;
            OrderMessageText.Visibility = Visibility.Collapsed;
        }

        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject dependencyObject)
            where T : DependencyObject
        {
            if (dependencyObject == null)
            {
                yield break;
            }

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(dependencyObject); i++)
            {
                var child = VisualTreeHelper.GetChild(dependencyObject, i);
                if (child is T typed)
                {
                    yield return typed;
                }

                foreach (var descendant in FindVisualChildren<T>(child))
                {
                    yield return descendant;
                }
            }
        }

        private static decimal Round2(decimal value)
            => Math.Round(value, 2, MidpointRounding.AwayFromZero);

        private sealed record DrinkModifierSelection(
            int? MilkIngredientId,
            string? MilkIngredientName,
            int? CoffeeIngredientId,
            string? CoffeeIngredientName);

        private sealed class PickupSlotViewModel
        {
            public PickupSlotViewModel(string value, string label)
            {
                Value = value;
                Label = label;
            }

            public string Value { get; }
            public string Label { get; }
        }

        private sealed class DiscountOptionViewModel
        {
            public DiscountOptionViewModel()
            {
                DisplayName = "Без скидки";
            }

            public DiscountOptionViewModel(DiscountDto dto)
            {
                Id = dto.Id;
                Name = dto.Name;
                DiscountPercent = dto.DiscountPercent;
                DisplayName = $"{dto.Name} - {dto.DiscountPercent:0.##}%";
            }

            public int? Id { get; }
            public string Name { get; } = string.Empty;
            public decimal DiscountPercent { get; }
            public string DisplayName { get; }
        }

        private sealed class SubcategoryViewModel
        {
            public SubcategoryViewModel(string name, bool isActive)
            {
                Name = name;
                IsActive = isActive;
            }

            public string Name { get; }
            public bool IsActive { get; }
            public Brush Background => IsActive
                ? (Brush)Application.Current.Resources["ModalColorPrimaryBrush"]
                : Brushes.White;
            public Brush Foreground => IsActive
                ? Brushes.White
                : (Brush)Application.Current.Resources["ModalColorTextBrush"];
            public Brush BorderBrush => IsActive
                ? (Brush)Application.Current.Resources["ModalColorPrimaryDarkBrush"]
                : new SolidColorBrush(Color.FromRgb(183, 183, 183));
        }

        private sealed class MenuPositionViewModel : NotifyBase
        {
            private int _quantityInCart;

            public string ItemType { get; private init; } = string.Empty;
            public int ItemId { get; private init; }
            public string Name { get; private init; } = string.Empty;
            public string Category { get; private init; } = string.Empty;
            public int? CategoryId { get; private init; }
            public string SizeLabel { get; private init; } = string.Empty;
            public string Ingredients { get; private init; } = string.Empty;
            public decimal Price { get; private init; }
            public int Calories { get; private init; }
            public int Proteins { get; private init; }
            public int Fats { get; private init; }
            public int Carbs { get; private init; }
            public bool IsAvailable { get; private init; }
            public ImageSource? ImageSource { get; private init; }

            public int QuantityInCart
            {
                get => _quantityInCart;
                set
                {
                    if (SetField(ref _quantityInCart, value))
                    {
                        OnPropertyChanged(nameof(QuantityDisplay));
                        OnPropertyChanged(nameof(QuantityVisibility));
                    }
                }
            }

            public string PriceDisplay => $"{Price:0.##} ₽";
            public string DetailsDisplay => ItemType == ToppingType
                ? string.Empty
                : $"Ккал: {Calories} | Б: {Proteins} | Ж: {Fats} | У: {Carbs} (на порцию)";
            public string IngredientsDisplay => string.IsNullOrWhiteSpace(Ingredients)
                ? string.Empty
                : "Состав: " + Ingredients;
            public string QuantityDisplay => QuantityInCart.ToString(CultureInfo.InvariantCulture);
            public Brush CardBackground => IsAvailable ? Brushes.White : new SolidColorBrush(Color.FromRgb(240, 240, 240));
            public Brush CardBorderBrush => IsAvailable
                ? new SolidColorBrush(Color.FromRgb(111, 104, 100))
                : new SolidColorBrush(Color.FromRgb(184, 184, 184));
            public Visibility ImageVisibility => ImageSource == null ? Visibility.Collapsed : Visibility.Visible;
            public Visibility IngredientsVisibility => string.IsNullOrWhiteSpace(IngredientsDisplay) ? Visibility.Collapsed : Visibility.Visible;
            public Visibility QuantityVisibility => QuantityInCart > 0 ? Visibility.Visible : Visibility.Collapsed;
            public Visibility UnavailableVisibility => IsAvailable ? Visibility.Collapsed : Visibility.Visible;

            public static MenuPositionViewModel FromDish(DishDto dto)
            {
                return new MenuPositionViewModel
                {
                    ItemType = DishType,
                    ItemId = dto.Id,
                    Name = dto.Name ?? string.Empty,
                    Category = dto.Category ?? string.Empty,
                    SizeLabel = dto.WeightLabel ?? string.Empty,
                    Ingredients = dto.Ingredients ?? string.Empty,
                    Price = dto.Price,
                    Calories = dto.Calories,
                    Proteins = dto.Proteins,
                    Fats = dto.Fats,
                    Carbs = dto.Carbs,
                    IsAvailable = dto.IsAvailable,
                    ImageSource = LoadImage(dto.ImageUrl)
                };
            }

            public static MenuPositionViewModel FromDrink(DrinkDto dto)
            {
                return new MenuPositionViewModel
                {
                    ItemType = DrinkType,
                    ItemId = dto.Id,
                    Name = dto.Name ?? string.Empty,
                    Category = dto.Category ?? string.Empty,
                    CategoryId = dto.CategoryId,
                    SizeLabel = dto.VolumeLabel ?? string.Empty,
                    Ingredients = dto.Ingredients ?? string.Empty,
                    Price = dto.Price,
                    Calories = dto.Calories,
                    Proteins = dto.Proteins,
                    Fats = dto.Fats,
                    Carbs = dto.Carbs,
                    IsAvailable = dto.IsAvailable,
                    ImageSource = LoadImage(dto.ImageUrl)
                };
            }

            public static MenuPositionViewModel FromTopping(ToppingDto dto)
            {
                return new MenuPositionViewModel
                {
                    ItemType = ToppingType,
                    ItemId = dto.Id,
                    Name = dto.Name ?? string.Empty,
                    Category = dto.Category ?? string.Empty,
                    Price = dto.Price,
                    Calories = dto.Calories,
                    IsAvailable = dto.IsAvailable
                };
            }

            private static ImageSource? LoadImage(string? imageUrl)
            {
                var path = ResolveImagePath(imageUrl);
                if (path == null)
                {
                    return null;
                }

                try
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.UriSource = new Uri(path, UriKind.Absolute);
                    image.EndInit();
                    image.Freeze();
                    return image;
                }
                catch
                {
                    return null;
                }
            }

            private static string? ResolveImagePath(string? imageUrl)
            {
                var normalized = (imageUrl ?? string.Empty)
                    .Trim()
                    .Replace('\\', '/')
                    .TrimStart('/');

                if (string.IsNullOrWhiteSpace(normalized))
                {
                    normalized = "Images/placeholder.png";
                }

                if (normalized.StartsWith("Images/", StringComparison.OrdinalIgnoreCase))
                {
                    normalized = normalized.Substring("Images/".Length);
                }

                var baseDirectory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
                for (var current = baseDirectory; current != null; current = current.Parent)
                {
                    var candidate = Path.Combine(current.FullName, "GardenNookWeb", "wwwroot", "Images", normalized);
                    if (File.Exists(candidate))
                    {
                        return candidate;
                    }
                }

                return null;
            }
        }

        private sealed class CartItemViewModel : NotifyBase
        {
            private int _number;
            private int _quantity = 1;

            public Guid Id { get; } = Guid.NewGuid();
            public string ItemType { get; private init; } = string.Empty;
            public int ItemId { get; private init; }
            public string Name { get; private init; } = string.Empty;
            public decimal Price { get; private init; }
            public int Calories { get; private init; }
            public List<CartToppingViewModel> Toppings { get; private init; } = new List<CartToppingViewModel>();
            public int? MilkIngredientId { get; private init; }
            public string? MilkIngredientName { get; private init; }
            public int? CoffeeIngredientId { get; private init; }
            public string? CoffeeIngredientName { get; private init; }

            public int Number
            {
                get => _number;
                set
                {
                    if (SetField(ref _number, value))
                    {
                        OnPropertyChanged(nameof(NumberDisplay));
                    }
                }
            }

            public int Quantity
            {
                get => _quantity;
                set
                {
                    if (SetField(ref _quantity, value))
                    {
                        OnPropertyChanged(nameof(TotalPrice));
                        OnPropertyChanged(nameof(TotalCalories));
                    }
                }
            }

            public string NumberDisplay => Number + ")";
            public string PriceDisplay => $"{Price:0.##} ₽";
            public decimal TotalPrice => (Price + Toppings.Sum(x => x.Price * x.Quantity)) * Quantity;
            public decimal TotalCalories => (Calories + Toppings.Sum(x => x.Calories * x.Quantity)) * Quantity;
            public string DetailsDisplay
            {
                get
                {
                    var lines = new List<string>();
                    if (ItemType == DrinkType)
                    {
                        if (!string.IsNullOrWhiteSpace(MilkIngredientName))
                        {
                            lines.Add("Молоко: " + MilkIngredientName);
                        }

                        if (!string.IsNullOrWhiteSpace(CoffeeIngredientName))
                        {
                            lines.Add("Кофе: " + CoffeeIngredientName);
                        }
                    }

                    lines.AddRange(Toppings.Select(x => $"+ {x.Name} ×{x.Quantity}"));
                    return string.Join(Environment.NewLine, lines);
                }
            }

            public Visibility DetailsVisibility => string.IsNullOrWhiteSpace(DetailsDisplay)
                ? Visibility.Collapsed
                : Visibility.Visible;

            public static CartItemViewModel FromStandaloneTopping(MenuPositionViewModel item)
            {
                return new CartItemViewModel
                {
                    ItemType = ToppingType,
                    ItemId = item.ItemId,
                    Name = item.Name,
                    Price = item.Price,
                    Calories = item.Calories
                };
            }

            public static CartItemViewModel FromMenuPosition(
                MenuPositionViewModel item,
                List<CartToppingViewModel> toppings,
                DrinkModifierSelection? modifiers)
            {
                return new CartItemViewModel
                {
                    ItemType = item.ItemType,
                    ItemId = item.ItemId,
                    Name = item.Name,
                    Price = item.Price,
                    Calories = item.Calories,
                    Toppings = toppings,
                    MilkIngredientId = modifiers?.MilkIngredientId,
                    MilkIngredientName = modifiers?.MilkIngredientName,
                    CoffeeIngredientId = modifiers?.CoffeeIngredientId,
                    CoffeeIngredientName = modifiers?.CoffeeIngredientName
                };
            }
        }

        private sealed class CartToppingViewModel
        {
            public int ToppingId { get; set; }
            public string Name { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public int Calories { get; set; }
            public int Quantity { get; set; }
        }

        private sealed class ToppingSelectionViewModel : NotifyBase
        {
            private int _quantity;

            public ToppingSelectionViewModel(ToppingDto source)
            {
                Id = source.Id;
                Name = source.Name ?? string.Empty;
                Price = source.Price;
                Calories = source.Calories;
                IsAvailable = source.IsAvailable;
                IncreaseCommand = new RelayCommand(() => Quantity++, () => IsAvailable);
                DecreaseCommand = new RelayCommand(() => Quantity = Math.Max(0, Quantity - 1), () => IsAvailable);
            }

            public int Id { get; }
            public string Name { get; }
            public decimal Price { get; }
            public int Calories { get; }
            public bool IsAvailable { get; }
            public RelayCommand IncreaseCommand { get; }
            public RelayCommand DecreaseCommand { get; }
            public string PriceDisplay => $"{Price:0.##} ₽";
            public double Opacity => IsAvailable ? 1d : 0.65d;
            public Visibility UnavailableVisibility => IsAvailable ? Visibility.Collapsed : Visibility.Visible;

            public int Quantity
            {
                get => _quantity;
                set => SetField(ref _quantity, value);
            }
        }

        private abstract class NotifyBase : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler? PropertyChanged;

            protected bool SetField<T>(ref T field, T value, string? propertyName = null)
            {
                if (EqualityComparer<T>.Default.Equals(field, value))
                {
                    return false;
                }

                field = value;
                OnPropertyChanged(propertyName);
                return true;
            }

            protected void OnPropertyChanged(string? propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private sealed class RelayCommand : System.Windows.Input.ICommand
        {
            private readonly Action _execute;
            private readonly Func<bool> _canExecute;

            public RelayCommand(Action execute, Func<bool> canExecute)
            {
                _execute = execute;
                _canExecute = canExecute;
            }

            public event EventHandler? CanExecuteChanged
            {
                add { }
                remove { }
            }

            public bool CanExecute(object? parameter) => _canExecute();

            public void Execute(object? parameter) => _execute();
        }
    }
}
