using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GardenNookWpf.Views.Kitchen.Shell.Sections;

namespace GardenNookWpf.Views.Kitchen.Shell
{
    public partial class KitchenShellWindow : Window
    {
        private const string AdminRole = "Администратор";
        private const string CookRole = "Повар";
        private const string BaristaRole = "Бариста";

        private readonly HttpClient _httpClient;
        private readonly string _userRole;
        private readonly KitchenSection? _requestedStartupSection;
        private readonly Dictionary<KitchenSection, IKitchenSectionView> _sections = new Dictionary<KitchenSection, IKitchenSectionView>();
        private readonly Dictionary<KitchenSection, Button> _navButtons;
        private IReadOnlyList<KitchenSection> _visibleSections = Array.Empty<KitchenSection>();
        private IKitchenSectionView? _activeSectionView;
        private KitchenSection? _activeSection;
        private bool _isNavigating;
        private bool _isMenuExpanded = true;

        public KitchenShellWindow(HttpClient httpClient, string userRole, KitchenSection? startupSection = null)
        {
            _httpClient = httpClient;
            _userRole = userRole?.Trim() ?? string.Empty;
            _requestedStartupSection = startupSection;

            InitializeComponent();

            _navButtons = new Dictionary<KitchenSection, Button>
            {
                [KitchenSection.Orders] = OrdersButton,
                [KitchenSection.TechnicalCards] = TechCardsButton,
                [KitchenSection.Preparations] = PreparationsButton,
                [KitchenSection.StopList] = StopListButton,
                [KitchenSection.WriteOff] = WriteOffButton,
                [KitchenSection.MenuItems] = MenuItemsButton,
                [KitchenSection.ProductCategories] = ProductCategoriesButton,
                [KitchenSection.Inventory] = InventoryButton,
                [KitchenSection.IngredientCategories] = IngredientCategoriesButton,
                [KitchenSection.OrderHistory] = OrderHistoryButton,
                [KitchenSection.Clients] = ClientsButton,
                [KitchenSection.Loyalty] = LoyaltyButton,
                [KitchenSection.Reports] = ReportsButton,
                [KitchenSection.Staff] = StaffButton
            };

            ConfigureRoleAccess();

            Loaded += KitchenShellWindow_Loaded;
            Closed += KitchenShellWindow_Closed;
        }

        public KitchenShellWindow(HttpClient httpClient, KitchenSection startupSection = KitchenSection.Orders)
            : this(httpClient, CookRole, startupSection)
        {
        }

        private async void KitchenShellWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (_visibleSections.Count == 0)
            {
                MessageBox.Show("Для роли не настроен доступ.", "Garden Nook", MessageBoxButton.OK, MessageBoxImage.Warning);
                Close();
                return;
            }

            var startupSection = ResolveStartupSection();
            await NavigateToSectionAsync(startupSection);
        }

        private void KitchenShellWindow_Closed(object? sender, EventArgs e)
        {
            foreach (var sectionView in _sections.Values)
            {
                sectionView.Deactivate();
            }
        }

        private async void OrdersButton_Click(object sender, RoutedEventArgs e)
        {
            await NavigateToSectionAsync(KitchenSection.Orders);
        }

        private async void TechCardsButton_Click(object sender, RoutedEventArgs e)
        {
            await NavigateToSectionAsync(KitchenSection.TechnicalCards);
        }

        private async void PreparationsButton_Click(object sender, RoutedEventArgs e)
        {
            await NavigateToSectionAsync(KitchenSection.Preparations);
        }

        private async void StopListButton_Click(object sender, RoutedEventArgs e)
        {
            await NavigateToSectionAsync(KitchenSection.StopList);
        }

        private async void WriteOffButton_Click(object sender, RoutedEventArgs e)
        {
            await NavigateToSectionAsync(KitchenSection.WriteOff);
        }

        private async void MenuItemsButton_Click(object sender, RoutedEventArgs e)
        {
            await NavigateToSectionAsync(KitchenSection.MenuItems);
        }

        private async void ProductCategoriesButton_Click(object sender, RoutedEventArgs e)
        {
            await NavigateToSectionAsync(KitchenSection.ProductCategories);
        }

        private async void InventoryButton_Click(object sender, RoutedEventArgs e)
        {
            await NavigateToSectionAsync(KitchenSection.Inventory);
        }

        private async void IngredientCategoriesButton_Click(object sender, RoutedEventArgs e)
        {
            await NavigateToSectionAsync(KitchenSection.IngredientCategories);
        }

        private async void OrderHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            await NavigateToSectionAsync(KitchenSection.OrderHistory);
        }

        private async void ClientsButton_Click(object sender, RoutedEventArgs e)
        {
            await NavigateToSectionAsync(KitchenSection.Clients);
        }

        private async void LoyaltyButton_Click(object sender, RoutedEventArgs e)
        {
            await NavigateToSectionAsync(KitchenSection.Loyalty);
        }

        private async void ReportsButton_Click(object sender, RoutedEventArgs e)
        {
            await NavigateToSectionAsync(KitchenSection.Reports);
        }

        private async void StaffButton_Click(object sender, RoutedEventArgs e)
        {
            await NavigateToSectionAsync(KitchenSection.Staff);
        }

        private void MenuToggleButton_Click(object sender, RoutedEventArgs e)
        {
            _isMenuExpanded = !_isMenuExpanded;
            NavigationColumn.Width = new GridLength(_isMenuExpanded ? 460 : 96);
            NavigationButtonsPanel.Visibility = _isMenuExpanded ? Visibility.Visible : Visibility.Collapsed;
            MenuTitleText.Visibility = _isMenuExpanded ? Visibility.Visible : Visibility.Collapsed;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (_activeSectionView?.IsBusy == true)
            {
                return;
            }

            var authorizationWindow = new AuthorizationWindow();
            authorizationWindow.Show();
            Close();
        }

        private async Task NavigateToSectionAsync(KitchenSection section)
        {
            if (_isNavigating)
            {
                return;
            }

            if (!_visibleSections.Contains(section))
            {
                return;
            }

            if (_activeSectionView?.IsBusy == true)
            {
                return;
            }

            try
            {
                _isNavigating = true;

                if (_activeSectionView != null && _activeSection != section)
                {
                    _activeSectionView.Deactivate();
                }

                var targetView = GetOrCreateSection(section);
                _activeSectionView = targetView;
                _activeSection = section;
                SectionHost.Content = targetView;
                ChangeButtonBackground(section);

                await targetView.ActivateAsync();
            }
            finally
            {
                _isNavigating = false;
            }
        }

        private IKitchenSectionView GetOrCreateSection(KitchenSection section)
        {
            if (_sections.TryGetValue(section, out var existing))
            {
                return existing;
            }

            IKitchenSectionView created = section switch
            {
                KitchenSection.Orders => new OrdersView(_httpClient, _userRole),
                KitchenSection.TechnicalCards => new TechnicalCardsView(_httpClient, _userRole),
                KitchenSection.Preparations => new PreparationsView(_httpClient, _userRole),
                KitchenSection.StopList => new StopListView(_httpClient, _userRole),
                KitchenSection.WriteOff => new WriteOffView(_httpClient, _userRole),
                KitchenSection.MenuItems => new MenuItemsView(_httpClient, _userRole),
                _ => new PlaceholderSectionView(GetSectionTitle(section), _userRole)
            };

            _sections[section] = created;
            return created;
        }

        private void ConfigureRoleAccess()
        {
            _visibleSections = ResolveVisibleSections(_userRole);
            var visibleSet = _visibleSections.ToHashSet();

            foreach (var pair in _navButtons)
            {
                pair.Value.Visibility = visibleSet.Contains(pair.Key)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        private KitchenSection ResolveStartupSection()
        {
            if (_requestedStartupSection.HasValue && _visibleSections.Contains(_requestedStartupSection.Value))
            {
                return _requestedStartupSection.Value;
            }

            return _visibleSections.First();
        }

        private static IReadOnlyList<KitchenSection> ResolveVisibleSections(string role)
        {
            return role switch
            {
                CookRole => new[]
                {
                    KitchenSection.Orders,
                    KitchenSection.TechnicalCards,
                    KitchenSection.Preparations,
                    KitchenSection.StopList,
                    KitchenSection.WriteOff
                },
                BaristaRole => new[]
                {
                    KitchenSection.MenuItems,
                    KitchenSection.Orders,
                    KitchenSection.TechnicalCards,
                    KitchenSection.Preparations,
                    KitchenSection.StopList
                },
                AdminRole => new[]
                {
                    KitchenSection.Orders,
                    KitchenSection.TechnicalCards,
                    KitchenSection.Preparations,
                    KitchenSection.StopList,
                    KitchenSection.WriteOff,
                    KitchenSection.MenuItems,
                    KitchenSection.ProductCategories,
                    KitchenSection.Inventory,
                    KitchenSection.IngredientCategories,
                    KitchenSection.OrderHistory,
                    KitchenSection.Clients,
                    KitchenSection.Loyalty,
                    KitchenSection.Reports,
                    KitchenSection.Staff
                },
                _ => Array.Empty<KitchenSection>()
            };
        }

        private static string GetSectionTitle(KitchenSection section)
        {
            return section switch
            {
                KitchenSection.MenuItems => "Меню",
                KitchenSection.ProductCategories => "Категории товаров",
                KitchenSection.Inventory => "Склад сырья и полуфабрикатов",
                KitchenSection.IngredientCategories => "Категории сырья",
                KitchenSection.OrderHistory => "История заказов",
                KitchenSection.Clients => "Клиенты",
                KitchenSection.Loyalty => "Программа лояльности",
                KitchenSection.Reports => "Аналитические отчеты",
                KitchenSection.Staff => "Персонал",
                _ => "Раздел"
            };
        }

        private void ChangeButtonBackground(KitchenSection section)
        {
            Color selected = (Color)ColorConverter.ConvertFromString("#FF606E52");
            Color white = Colors.White;
            Color black = Colors.Black;

            foreach (var button in _navButtons.Values)
            {
                button.Background = new SolidColorBrush(white);
                button.Foreground = new SolidColorBrush(black);
            }

            if (_navButtons.TryGetValue(section, out var selectedButton))
            {
                selectedButton.Background = new SolidColorBrush(selected);
                selectedButton.Foreground = new SolidColorBrush(white);
            }
        }
    }
}
