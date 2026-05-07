using System.Windows;
using System.Windows.Input;

namespace GardenNookWpf.Views.Shell.Sections.Reports
{
    public partial class InventoryDetailsWindow : Window
    {
        public InventoryDetailsWindow(ReportsView.InventoryReportItemViewModel item)
        {
            InitializeComponent();
            DataContext = item;

            IngredientNameText.Text = item.Name;
            SummaryText.Text = "Суммарный расход: " + item.ExpectedConsumptionDisplay + " " + item.UnitName
                + " | Остаток: " + item.ActualStockDisplay + " " + item.UnitName
                + " | Разница: " + item.DifferenceDisplay + " " + item.UnitName
                + " | Разница, руб.: " + item.DifferenceCostDisplay;
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
    }
}
