using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TransferModels.Kitchen;

namespace GardenNookWpf.Views.Kitchen
{
    public partial class AddPreparationTaskWindow : Window
    {
        private readonly List<KitchenSemiFinishedOptionDto> _options;
        private List<KitchenSemiFinishedOptionDto> _filteredOptions = new List<KitchenSemiFinishedOptionDto>();

        public int? SelectedSemiFinishedId { get; private set; }
        public string SelectedSemiFinishedName { get; private set; } = string.Empty;
        public string? CommentText { get; private set; }

        public AddPreparationTaskWindow(IReadOnlyCollection<KitchenSemiFinishedOptionDto> options)
        {
            _options = (options ?? new List<KitchenSemiFinishedOptionDto>())
                .OrderBy(x => x.Name)
                .ThenBy(x => x.SemiFinishedId)
                .ToList();

            InitializeComponent();
            ApplyFilter(string.Empty);
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            HideValidation();

            if (SemiFinishedListBox.SelectedItem is not KitchenSemiFinishedOptionDto option)
            {
                ShowValidation("Выберите полуфабрикат.");
                return;
            }

            SelectedSemiFinishedId = option.SemiFinishedId;
            SelectedSemiFinishedName = option.Name ?? string.Empty;
            CommentText = string.IsNullOrWhiteSpace(CommentTextBox.Text)
                ? null
                : CommentTextBox.Text.Trim();

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

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter(SearchTextBox.Text);
        }

        private void ApplyFilter(string? searchText)
        {
            var filter = (searchText ?? string.Empty).Trim();
            IEnumerable<KitchenSemiFinishedOptionDto> source = _options;

            if (!string.IsNullOrWhiteSpace(filter))
            {
                source = source.Where(x =>
                    x.Name.IndexOf(filter, StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                    x.SemiFinishedId.ToString().Contains(filter, StringComparison.CurrentCultureIgnoreCase));
            }

            _filteredOptions = source
                .OrderBy(x => x.Name)
                .ThenBy(x => x.SemiFinishedId)
                .ToList();

            SemiFinishedListBox.ItemsSource = _filteredOptions;
            SemiFinishedListBox.SelectedIndex = _filteredOptions.Count > 0 ? 0 : -1;

            if (_filteredOptions.Count == 0 && !string.IsNullOrWhiteSpace(filter))
            {
                ShowValidation("По вашему запросу ничего не найдено.");
            }
            else
            {
                HideValidation();
            }
        }
    }
}
