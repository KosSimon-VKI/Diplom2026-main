using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TransferModels.Kitchen;

namespace GardenNookWpf.Views.MainPanel.TechnicalCards
{
    public partial class TechnicalCardEditWindow : Window
    {
        private readonly KitchenTechnicalCardEditOptionsResponse _options;

        public TechnicalCardEditWindow(
            KitchenTechnicalCardEditOptionsResponse options,
            KitchenTechnicalCardEditResponse? existingCard = null)
        {
            _options = options;

            CompositionLines = new ObservableCollection<CompositionLineViewModel>();
            UnitOptions = new ObservableCollection<KitchenTechnicalCardUnitDto>(options.Units ?? new List<KitchenTechnicalCardUnitDto>());

            InitializeComponent();
            DataContext = this;

            HeaderTitle.Text = existingCard == null ? "Добавление техкарты" : "Редактирование техкарты";
            SaveButton.Content = existingCard == null ? "Добавить" : "Сохранить";

            CompositionKindComboBox.ItemsSource = new[]
            {
                new CompositionKindOption("Ингредиент", KitchenTechnicalCardCompositionKinds.Ingredient),
                new CompositionKindOption("Полуфабрикат", KitchenTechnicalCardCompositionKinds.SemiFinished)
            };
            CompositionKindComboBox.SelectedIndex = 0;

            BindExistingCard(existingCard);
        }

        public ObservableCollection<CompositionLineViewModel> CompositionLines { get; }

        public ObservableCollection<KitchenTechnicalCardUnitDto> UnitOptions { get; }

        public KitchenTechnicalCardUpsertRequest? Request { get; private set; }

        private void BindExistingCard(KitchenTechnicalCardEditResponse? existingCard)
        {
            if (existingCard == null)
            {
                return;
            }

            CardNameTextBox.Text = existingCard.CardName;
            DescriptionTextBox.Text = existingCard.Description;

            foreach (var line in existingCard.IngredientLines ?? new List<KitchenTechnicalCardCompositionLineDto>())
            {
                var reference = _options.Ingredients.FirstOrDefault(x => x.Id == line.ItemId);
                CompositionLines.Add(CreateLineViewModel(KitchenTechnicalCardCompositionKinds.Ingredient, line, reference));
            }

            foreach (var line in existingCard.SemiFinishedLines ?? new List<KitchenTechnicalCardCompositionLineDto>())
            {
                var reference = _options.SemiFinisheds.FirstOrDefault(x => x.Id == line.ItemId);
                CompositionLines.Add(CreateLineViewModel(KitchenTechnicalCardCompositionKinds.SemiFinished, line, reference));
            }
        }

        private CompositionLineViewModel CreateLineViewModel(
            string kind,
            KitchenTechnicalCardCompositionLineDto line,
            KitchenTechnicalCardReferenceDto? reference)
        {
            return new CompositionLineViewModel
            {
                Kind = kind,
                KindDisplay = kind == KitchenTechnicalCardCompositionKinds.Ingredient ? "Ингр." : "П/ф",
                ItemId = line.ItemId,
                ItemName = reference?.Name ?? $"#{line.ItemId}",
                UnitOfMeasureId = line.UnitOfMeasureId ?? reference?.UnitOfMeasureId,
                GrossWeight = line.GrossWeight,
                ColdLossPercent = line.ColdLossPercent,
                NetWeight = line.NetWeight,
                HotLossPercent = line.HotLossPercent,
                OutputWeight = line.OutputWeight
            };
        }

        private void CompositionKindComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (CompositionKindComboBox.SelectedValue is not string kind)
            {
                return;
            }

            CompositionItemComboBox.ItemsSource = kind == KitchenTechnicalCardCompositionKinds.Ingredient
                ? _options.Ingredients
                : _options.SemiFinisheds;
            CompositionItemComboBox.SelectedIndex = 0;
        }

        private void AddCompositionLine_Click(object sender, RoutedEventArgs e)
        {
            if (CompositionKindComboBox.SelectedValue is not string kind ||
                CompositionItemComboBox.SelectedItem is not KitchenTechnicalCardReferenceDto reference)
            {
                ShowValidation("Выберите позицию состава.");
                return;
            }

            CompositionLines.Add(new CompositionLineViewModel
            {
                Kind = kind,
                KindDisplay = kind == KitchenTechnicalCardCompositionKinds.Ingredient ? "Ингр." : "П/ф",
                ItemId = reference.Id,
                ItemName = reference.Name,
                UnitOfMeasureId = reference.UnitOfMeasureId
            });
            ShowValidation(string.Empty);
        }

        private void RemoveCompositionLine_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element &&
                element.Tag is CompositionLineViewModel line)
            {
                CompositionLines.Remove(line);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var cardName = CardNameTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(cardName))
            {
                ShowValidation("Укажите название техкарты.");
                return;
            }

            Request = new KitchenTechnicalCardUpsertRequest
            {
                CardName = cardName,
                Description = DescriptionTextBox.Text.Trim(),
                IngredientLines = CompositionLines
                    .Where(x => x.Kind == KitchenTechnicalCardCompositionKinds.Ingredient)
                    .Select(CreateLineDto)
                    .ToList(),
                SemiFinishedLines = CompositionLines
                    .Where(x => x.Kind == KitchenTechnicalCardCompositionKinds.SemiFinished)
                    .Select(CreateLineDto)
                    .ToList(),
                Bindings = new List<KitchenTechnicalCardBindingDto>()
            };

            DialogResult = true;
        }

        private static KitchenTechnicalCardCompositionLineDto CreateLineDto(CompositionLineViewModel line)
        {
            return new KitchenTechnicalCardCompositionLineDto
            {
                ItemId = line.ItemId,
                UnitOfMeasureId = line.UnitOfMeasureId,
                GrossWeight = line.GrossWeight,
                ColdLossPercent = line.ColdLossPercent,
                NetWeight = line.NetWeight,
                HotLossPercent = line.HotLossPercent,
                OutputWeight = line.OutputWeight
            };
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void HeaderClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
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
            ValidationText.Visibility = string.IsNullOrWhiteSpace(message) ? Visibility.Collapsed : Visibility.Visible;
        }

        private sealed class CompositionKindOption
        {
            public CompositionKindOption(string displayName, string kind)
            {
                DisplayName = displayName;
                Kind = kind;
            }

            public string DisplayName { get; }
            public string Kind { get; }
        }

        public sealed class CompositionLineViewModel
        {
            public string Kind { get; set; } = string.Empty;
            public string KindDisplay { get; set; } = string.Empty;
            public int ItemId { get; set; }
            public string ItemName { get; set; } = string.Empty;
            public int? UnitOfMeasureId { get; set; }
            public decimal? GrossWeight { get; set; }
            public decimal? ColdLossPercent { get; set; }
            public decimal? NetWeight { get; set; }
            public decimal? HotLossPercent { get; set; }
            public decimal? OutputWeight { get; set; }
        }
    }
}
