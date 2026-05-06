using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GardenNookWpf.Views.Shell.Sections.Clients;
using GardenNookWpf.Views.Shell.Sections.Menu;
using GardenNookWpf.Views.Shell.Sections.Inventory;
using GardenNookWpf.Views.Shell.Sections.IngredientCategories;
using GardenNookWpf.Views.Shell.Sections.Loyalty;
using GardenNookWpf.Views.Shell.Sections.Orders;
using GardenNookWpf.Views.Shell.Sections.OrderHistory;
using GardenNookWpf.Views.Shell.Sections.Preparations;
using GardenNookWpf.Views.Shell.Sections.Reports;
using GardenNookWpf.Views.Shell.Sections.Shared;
using GardenNookWpf.Views.Shell.Sections.Staff;
using GardenNookWpf.Views.Shell.Sections.StopList;
using GardenNookWpf.Views.Shell.Sections.TechnicalCards;
using GardenNookWpf.Views.Shell.Sections.WriteOff;

namespace GardenNookWpf.Views.Shell
{
    public partial class MainShellWindow : Window
    {
        private const string AdminRole = "Администратор";
        private const string CookRole = "Повар";
        private const string BaristaRole = "Бариста";

        private readonly HttpClient _httpClient;
        private readonly string _userRole;
        private readonly MainSection? _requestedStartupSection;
        private readonly Dictionary<MainSection, IMainSectionView> _sections = new Dictionary<MainSection, IMainSectionView>();
        private readonly Dictionary<MainSection, Button> _navButtons;
        private IReadOnlyList<MainSection> _visibleSections = Array.Empty<MainSection>();
        private IMainSectionView? _activeSectionView;
        private MainSection? _activeSection;
        private bool _isNavigating;
        private bool _isMenuExpanded = true;

        public MainShellWindow(HttpClient httpClient, string userRole, MainSection? startupSection = null)
        {
            _httpClient = httpClient;
            _userRole = userRole?.Trim() ?? string.Empty;
            _requestedStartupSection = startupSection;

            InitializeComponent();

            _navButtons = new Dictionary<MainSection, Button>
            {
                [MainSection.Orders] = OrdersButton,
                [MainSection.TechnicalCards] = TechCardsButton,
                [MainSection.Preparations] = PreparationsButton,
                [MainSection.StopList] = StopListButton,
                [MainSection.WriteOff] = WriteOffButton,
                [MainSection.MenuItems] = MenuItemsButton,
                [MainSection.MenuManagement] = MenuManagementButton,
                [MainSection.ProductCategories] = ProductCategoriesButton,
                [MainSection.Inventory] = InventoryButton,
                [MainSection.IngredientCategories] = IngredientCategoriesButton,
                [MainSection.OrderHistory] = OrderHistoryButton,
                [MainSection.Clients] = ClientsButton,
                [MainSection.Loyalty] = LoyaltyButton,
                [MainSection.Reports] = ReportsButton,
                [MainSection.Staff] = StaffButton
            };

            ConfigureRoleAccess();

            Loaded += MainShellWindow_Loaded;
            Closed += MainShellWindow_Closed;
        }

        public MainShellWindow(HttpClient httpClient, MainSection startupSection = MainSection.Orders)
            : this(httpClient, CookRole, startupSection)
        {
        }

        private async void MainShellWindow_Loaded(object sender, RoutedEventArgs e)
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

        private void MainShellWindow_Closed(object? sender, EventArgs e)
        {
            foreach (var sectionView in _sections.Values)
            {
                sectionView.Deactivate();
            }
        }

        private async void OrdersButton_Click(object sender, RoutedEventArgs e)
        {
            await NavigateToSectionAsync(MainSection.Orders);
        }

        private async void TechCardsButton_Click(object sender, RoutedEventArgs e)
        {
            await NavigateToSectionAsync(MainSection.TechnicalCards);
        }

        private async void PreparationsButton_Click(object sender, RoutedEventArgs e)
        {
            await NavigateToSectionAsync(MainSection.Preparations);
        }

        private async void StopListButton_Click(object sender, RoutedEventArgs e)
        {
            await NavigateToSectionAsync(MainSection.StopList);
        }

        private async void WriteOffButton_Click(object sender, RoutedEventArgs e)
        {
            await NavigateToSectionAsync(MainSection.WriteOff);
        }

        private async void MenuItemsButton_Click(object sender, RoutedEventArgs e)
        {
            await NavigateToSectionAsync(MainSection.MenuItems);
        }

        private async void MenuManagementButton_Click(object sender, RoutedEventArgs e)
        {
            await NavigateToSectionAsync(MainSection.MenuManagement);
        }

        private async void ProductCategoriesButton_Click(object sender, RoutedEventArgs e)
        {
            await NavigateToSectionAsync(MainSection.ProductCategories);
        }

        private async void InventoryButton_Click(object sender, RoutedEventArgs e)
        {
            await NavigateToSectionAsync(MainSection.Inventory);
        }

        private async void IngredientCategoriesButton_Click(object sender, RoutedEventArgs e)
        {
            await NavigateToSectionAsync(MainSection.IngredientCategories);
        }

        private async void OrderHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (GetOrCreateSection(MainSection.OrderHistory) is OrderHistoryView orderHistoryView)
            {
                orderHistoryView.ClearClientFilter();
            }

            await NavigateToSectionAsync(MainSection.OrderHistory);
        }

        private async void ClientsButton_Click(object sender, RoutedEventArgs e)
        {
            await NavigateToSectionAsync(MainSection.Clients);
        }

        private async void LoyaltyButton_Click(object sender, RoutedEventArgs e)
        {
            await NavigateToSectionAsync(MainSection.Loyalty);
        }

        private async void ReportsButton_Click(object sender, RoutedEventArgs e)
        {
            await NavigateToSectionAsync(MainSection.Reports);
        }

        private async void StaffButton_Click(object sender, RoutedEventArgs e)
        {
            await NavigateToSectionAsync(MainSection.Staff);
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

        private async Task NavigateToSectionAsync(MainSection section)
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

        private IMainSectionView GetOrCreateSection(MainSection section)
        {
            if (_sections.TryGetValue(section, out var existing))
            {
                return existing;
            }

            IMainSectionView created = section switch
            {
                MainSection.Orders => new OrdersView(_httpClient, _userRole),
                MainSection.TechnicalCards => new TechnicalCardsView(_httpClient, _userRole),
                MainSection.Preparations => new PreparationsView(_httpClient, _userRole),
                MainSection.StopList => new StopListView(_httpClient, _userRole),
                MainSection.WriteOff => new WriteOffView(_httpClient, _userRole),
                MainSection.MenuItems => new OrderItemView(_httpClient, _userRole),
                MainSection.MenuManagement => new MenuManagementView(_httpClient, _userRole),
                MainSection.ProductCategories => new MenuCategoriesView(_httpClient, _userRole),
                MainSection.Inventory => new InventoryView(_httpClient, _userRole),
                MainSection.IngredientCategories => new IngredientCategoriesView(_httpClient, _userRole),
                MainSection.OrderHistory => new OrderHistoryView(_httpClient, _userRole),
                MainSection.Clients => new ClientsView(_httpClient, _userRole, OpenClientOrderHistoryAsync),
                MainSection.Loyalty => new LoyaltyView(_httpClient, _userRole),
                MainSection.Reports => new ReportsView(_httpClient, _userRole),
                MainSection.Staff => new StaffView(_httpClient, _userRole),
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

        private async Task OpenClientOrderHistoryAsync(int clientId, string clientName)
        {
            if (GetOrCreateSection(MainSection.OrderHistory) is OrderHistoryView orderHistoryView)
            {
                orderHistoryView.SetClientFilter(clientId, clientName);
            }

            await NavigateToSectionAsync(MainSection.OrderHistory);
        }

        private MainSection ResolveStartupSection()
        {
            if (_requestedStartupSection.HasValue && _visibleSections.Contains(_requestedStartupSection.Value))
            {
                return _requestedStartupSection.Value;
            }

            return _visibleSections.First();
        }

        private static IReadOnlyList<MainSection> ResolveVisibleSections(string role)
        {
            return role switch
            {
                CookRole => new[]
                {
                    MainSection.Orders,
                    MainSection.TechnicalCards,
                    MainSection.Preparations,
                    MainSection.StopList,
                    MainSection.WriteOff
                },
                BaristaRole => new[]
                {
                    MainSection.MenuItems,
                    MainSection.Orders,
                    MainSection.TechnicalCards,
                    MainSection.Preparations,
                    MainSection.StopList
                },
                AdminRole => new[]
                {
                    MainSection.Orders,
                    MainSection.TechnicalCards,
                    MainSection.Preparations,
                    MainSection.StopList,
                    MainSection.WriteOff,
                    MainSection.MenuItems,
                    MainSection.MenuManagement,
                    MainSection.ProductCategories,
                    MainSection.Inventory,
                    MainSection.IngredientCategories,
                    MainSection.OrderHistory,
                    MainSection.Clients,
                    MainSection.Loyalty,
                    MainSection.Reports,
                    MainSection.Staff
                },
                _ => Array.Empty<MainSection>()
            };
        }

        private static string GetSectionTitle(MainSection section)
        {
            return section switch
            {
                MainSection.MenuItems => "Меню",
                MainSection.ProductCategories => "Категории товаров",
                MainSection.Inventory => "Склад сырья и полуфабрикатов",
                MainSection.IngredientCategories => "Категории сырья",
                MainSection.OrderHistory => "История заказов",
                MainSection.Clients => "Клиенты",
                MainSection.Loyalty => "Программа лояльности",
                MainSection.Reports => "Аналитические отчеты",
                MainSection.Staff => "Персонал",
                _ => "Раздел"
            };
        }

        private void ChangeButtonBackground(MainSection section)
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
