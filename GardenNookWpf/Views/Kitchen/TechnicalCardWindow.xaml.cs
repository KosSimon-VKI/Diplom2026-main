using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TransferModels.Kitchen;

namespace GardenNookWpf.Views.Kitchen
{
    /// <summary>
    /// Р›РѕРіРёРєР° РІР·Р°РёРјРѕРґРµР№СЃС‚РІРёСЏ РґР»СЏ TechnicalCardWindow.xaml
    /// </summary>
    public partial class TechnicalCardWindow : Window
    {
        public TechnicalCardWindow(KitchenTechnicalCardResponse card, bool isAdmin = false)
        {
            InitializeComponent();
            EditButton.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
            DeleteButton.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
            BindCard(card);
        }

        public bool EditRequested { get; private set; }

        public bool DeleteRequested { get; private set; }

        private void BindCard(KitchenTechnicalCardResponse card)
        {
            CardNameText.Text = string.IsNullOrWhiteSpace(card.CardName) ? "Техническая карта" : card.CardName;

            if (!string.IsNullOrWhiteSpace(card.Description))
            {
                CardDescriptionText.Text = card.Description;
                CardDescriptionText.Visibility = Visibility.Visible;
            }

            var components = (card.Components ?? new List<KitchenTechnicalCardComponentDto>())
                .Select(c => new TechnicalCardComponentDisplayModel
                {
                    Name = c.Name,
                    WeightText = decimal.Round(c.Weight, 2, System.MidpointRounding.AwayFromZero)
                        .ToString("0.00", CultureInfo.CurrentCulture),
                    Unit = c.Unit
                })
                .ToList();

            ComponentsList.ItemsSource = components;
            ComponentsList.Visibility = components.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            EmptyComponentsText.Visibility = components.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            EditRequested = true;
            Close();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            DeleteRequested = true;
            Close();
        }

        private void HeaderClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private sealed class TechnicalCardComponentDisplayModel
        {
            public string Name { get; set; } = string.Empty;
            public string WeightText { get; set; } = string.Empty;
            public string Unit { get; set; } = string.Empty;
        }
    }
}

