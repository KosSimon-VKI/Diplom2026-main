using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Win32;
using TransferModels.Reports;

namespace GardenNookWpf.Views.Shell.Sections.Reports
{
    public partial class ReportsView : UserControl, IMainSectionView, INotifyPropertyChanged
    {
        private const string ApiBaseAddress = "https://localhost:7235";
        private const string ReportsAddress = ApiBaseAddress + "/api/reports";
        private const string ExportAddress = ReportsAddress + "/export";

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly HttpClient _httpClient;
        private readonly List<InventoryReportItemViewModel> _allInventoryItems = new List<InventoryReportItemViewModel>();
        private bool _isLoadedOnce;
        private bool _isBusy;
        private string _selectedPeriod = "week";

        public ReportsView(HttpClient httpClient, string userRole)
        {
            _httpClient = httpClient;
            PopularSeries = new SeriesCollection();
            UnpopularSeries = new SeriesCollection();
            InventorySeries = new SeriesCollection();

            InitializeComponent();
            DataContext = this;
            UpdatePeriodButtons();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public bool IsBusy => _isBusy;

        public ObservableCollection<MenuReportItemViewModel> PopularItems { get; } = new ObservableCollection<MenuReportItemViewModel>();

        public ObservableCollection<MenuReportItemViewModel> UnpopularItems { get; } = new ObservableCollection<MenuReportItemViewModel>();

        public ObservableCollection<AbcReportItemViewModel> AbcItems { get; } = new ObservableCollection<AbcReportItemViewModel>();

        public ObservableCollection<InventoryReportItemViewModel> InventoryItems { get; } = new ObservableCollection<InventoryReportItemViewModel>();

        public SeriesCollection PopularSeries { get; }

        public SeriesCollection UnpopularSeries { get; }

        public SeriesCollection InventorySeries { get; }

        public string[] PopularLabels { get; private set; } = Array.Empty<string>();

        public string[] UnpopularLabels { get; private set; } = Array.Empty<string>();

        public string[] InventoryLabels { get; private set; } = Array.Empty<string>();

        public Func<double, string> WholeNumberAxisFormatter { get; } =
            value => value.ToString("0", CultureInfo.CurrentCulture);

        public double PopularAxisStep { get; private set; } = 1d;

        public double UnpopularAxisStep { get; private set; } = 1d;

        public async Task ActivateAsync()
        {
            if (_isLoadedOnce)
            {
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
                SetStatus("Загрузка отчетов...", false);

                using var response = await _httpClient.GetAsync(BuildReportAddress());
                if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                {
                    throw new InvalidOperationException("нет доступа к отчетам.");
                }

                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var report = JsonSerializer.Deserialize<ReportsResponse>(json, JsonOptions)
                    ?? new ReportsResponse();

                ApplyReport(report);
                _isLoadedOnce = true;
                var warningText = report.Warnings.Count == 0
                    ? string.Empty
                    : " Предупреждения: " + string.Join(" ", report.Warnings.Take(3));
                SetStatus("Отчет сформирован: " + report.PeriodName + "." + warningText, false);
            }
            catch (Exception ex)
            {
                ClearReport();
                SetStatus("Не удалось загрузить отчеты: " + ex.Message, true);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void ApplyReport(ReportsResponse report)
        {
            PopularItems.Clear();
            foreach (var item in report.PopularItems.Select(x => new MenuReportItemViewModel(x)))
            {
                PopularItems.Add(item);
            }

            UnpopularItems.Clear();
            foreach (var item in report.UnpopularItems.Select(x => new MenuReportItemViewModel(x)))
            {
                UnpopularItems.Add(item);
            }

            AbcItems.Clear();
            foreach (var item in report.AbcItems.Select(x => new AbcReportItemViewModel(x)))
            {
                AbcItems.Add(item);
            }

            _allInventoryItems.Clear();
            _allInventoryItems.AddRange(report.InventoryItems.Select(x => new InventoryReportItemViewModel(x)));
            ApplyInventoryFilter();

            PopularLabels = PopularItems.Select(x => ShortenLabel(x.Name)).ToArray();
            UnpopularLabels = UnpopularItems.Select(x => ShortenLabel(x.Name)).ToArray();
            PopularAxisStep = CalculateSalesAxisStep(PopularItems);
            UnpopularAxisStep = CalculateSalesAxisStep(UnpopularItems);
            RebuildMenuSeries(PopularSeries, PopularItems, "#FF606E52");
            RebuildMenuSeries(UnpopularSeries, UnpopularItems, "#FF91A56E");
            RebuildInventorySeries();

            OnPropertyChanged(nameof(PopularLabels));
            OnPropertyChanged(nameof(UnpopularLabels));
            OnPropertyChanged(nameof(InventoryLabels));
            OnPropertyChanged(nameof(PopularAxisStep));
            OnPropertyChanged(nameof(UnpopularAxisStep));
        }

        private void ClearReport()
        {
            PopularItems.Clear();
            UnpopularItems.Clear();
            AbcItems.Clear();
            _allInventoryItems.Clear();
            InventoryItems.Clear();
            PopularSeries.Clear();
            UnpopularSeries.Clear();
            InventorySeries.Clear();
            PopularLabels = Array.Empty<string>();
            UnpopularLabels = Array.Empty<string>();
            InventoryLabels = Array.Empty<string>();
            PopularAxisStep = 1d;
            UnpopularAxisStep = 1d;
            OnPropertyChanged(nameof(PopularLabels));
            OnPropertyChanged(nameof(UnpopularLabels));
            OnPropertyChanged(nameof(InventoryLabels));
            OnPropertyChanged(nameof(PopularAxisStep));
            OnPropertyChanged(nameof(UnpopularAxisStep));
        }

        private static void RebuildMenuSeries(
            SeriesCollection target,
            IEnumerable<MenuReportItemViewModel> items,
            string color)
        {
            target.Clear();
            target.Add(new ColumnSeries
            {
                Title = "Продано",
                Values = new ChartValues<decimal>(items.Select(x => x.Source.QuantitySold)),
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color)),
                FontSize = 16,
                FontWeight = FontWeights.SemiBold,
                DataLabels = true
            });
        }

        private static double CalculateSalesAxisStep(IEnumerable<MenuReportItemViewModel> items)
        {
            var max = items.Select(x => (double)x.Source.QuantitySold).DefaultIfEmpty(0d).Max();
            if (max <= 5d)
            {
                return 1d;
            }

            return Math.Ceiling(max / 5d);
        }

        private void RebuildInventorySeries()
        {
            var topPositiveDiffs = InventoryItems
                .Where(x => x.Source.Difference > 0m)
                .OrderByDescending(x => x.Source.Difference)
                .Take(8)
                .ToList();
            var topNegativeDiffs = InventoryItems
                .Where(x => x.Source.Difference < 0m)
                .OrderBy(x => x.Source.Difference)
                .Take(8)
                .ToList();
            var chartItems = topPositiveDiffs
                .Concat(topNegativeDiffs)
                .ToList();

            InventoryLabels = chartItems
                .Select(x => ShortenLabel(x.Name))
                .ToArray();

            InventorySeries.Clear();
            InventorySeries.Add(new ColumnSeries
            {
                Title = "Излишек",
                Values = new ChartValues<decimal>(
                    chartItems.Select(x => x.Source.Difference > 0m ? x.Source.Difference : 0m)),
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF606E52")),
                FontSize = 16,
                FontWeight = FontWeights.SemiBold,
                DataLabels = true,
                LabelPoint = point => FormatInventoryChartLabel(point, chartItems)
            });
            InventorySeries.Add(new ColumnSeries
            {
                Title = "Недостача",
                Values = new ChartValues<decimal>(
                    chartItems.Select(x => x.Source.Difference < 0m ? x.Source.Difference : 0m)),
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFB33131")),
                FontSize = 16,
                FontWeight = FontWeights.SemiBold,
                DataLabels = true,
                LabelPoint = point => FormatInventoryChartLabel(point, chartItems)
            });

            OnPropertyChanged(nameof(InventoryLabels));
        }

        private static string FormatInventoryChartLabel(ChartPoint point, List<InventoryReportItemViewModel> chartItems)
        {
            if (Math.Abs(point.Y) < 0.000001d)
            {
                return string.Empty;
            }

            var index = (int)Math.Round(point.X);
            var unitName = index >= 0 && index < chartItems.Count
                ? chartItems[index].UnitName
                : string.Empty;
            var value = point.Y.ToString("0.##", CultureInfo.CurrentCulture);

            return string.IsNullOrWhiteSpace(unitName)
                ? value
                : value + " " + unitName;
        }

        private async void PeriodButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is not string period || _isBusy)
            {
                return;
            }

            _selectedPeriod = period;
            UpdatePeriodButtons();
            await ReloadAsync();
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await ReloadAsync();
        }

        private async void ExportExcelButton_Click(object sender, RoutedEventArgs e)
        {
            await ExportAsync("xlsx", "Excel (*.xlsx)|*.xlsx");
        }

        private async void ExportPdfButton_Click(object sender, RoutedEventArgs e)
        {
            await ExportAsync("pdf", "PDF (*.pdf)|*.pdf");
        }

        private void InventoryDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is not InventoryReportItemViewModel item)
            {
                return;
            }

            var window = new InventoryDetailsWindow(item)
            {
                Owner = Window.GetWindow(this)
            };
            window.ShowDialog();
        }

        private void InventorySearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyInventoryFilter();
        }

        private void ApplyInventoryFilter()
        {
            if (InventoryItems == null)
            {
                return;
            }

            var query = (InventorySearchTextBox?.Text ?? string.Empty).Trim();
            var filtered = string.IsNullOrWhiteSpace(query)
                ? _allInventoryItems
                : _allInventoryItems
                    .Where(x => x.Name.IndexOf(query, StringComparison.CurrentCultureIgnoreCase) >= 0)
                    .ToList();

            InventoryItems.Clear();
            foreach (var item in filtered)
            {
                InventoryItems.Add(item);
            }

            RebuildInventorySeries();
        }

        private async Task ExportAsync(string format, string filter)
        {
            try
            {
                var dialog = new SaveFileDialog
                {
                    Filter = filter,
                    FileName = BuildDefaultFileName(format),
                    AddExtension = true,
                    DefaultExt = "." + format
                };

                if (dialog.ShowDialog(Window.GetWindow(this)) != true)
                {
                    return;
                }

                SetBusy(true);
                SetStatus("Сохранение файла...", false);

                using var response = await _httpClient.GetAsync(BuildExportAddress(format));
                if (!response.IsSuccessStatusCode)
                {
                    SetStatus(await ReadApiMessageAsync(response), true);
                    return;
                }

                var content = await response.Content.ReadAsByteArrayAsync();
                await File.WriteAllBytesAsync(dialog.FileName, content);
                SetStatus("Файл сохранен: " + dialog.FileName, false);
            }
            catch (Exception ex)
            {
                SetStatus("Не удалось сохранить файл: " + ex.Message, true);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private string BuildReportAddress()
        {
            return ReportsAddress + "?period=" + Uri.EscapeDataString(_selectedPeriod);
        }

        private string BuildExportAddress(string format)
        {
            return ExportAddress
                + "?period=" + Uri.EscapeDataString(_selectedPeriod)
                + "&format=" + Uri.EscapeDataString(format);
        }

        private string BuildDefaultFileName(string extension)
        {
            return "Отчет_" + ResolvePeriodName(_selectedPeriod).Replace(" ", "_") + "." + extension;
        }

        private void UpdatePeriodButtons()
        {
            if (PeriodButtonsPanel == null)
            {
                return;
            }

            foreach (var child in PeriodButtonsPanel.Children.OfType<Button>())
            {
                var isSelected = string.Equals(child.Tag as string, _selectedPeriod, StringComparison.Ordinal);
                child.Background = isSelected
                    ? (Brush)Application.Current.Resources["ModalColorPrimaryDarkBrush"]
                    : (Brush)Application.Current.Resources["ModalColorWhiteBrush"];
                child.Foreground = isSelected
                    ? (Brush)Application.Current.Resources["ModalColorWhiteBrush"]
                    : (Brush)Application.Current.Resources["ModalColorTextBrush"];
            }
        }

        private void SetBusy(bool isBusy)
        {
            _isBusy = isBusy;
            RootGrid.IsEnabled = !isBusy;
        }

        private void SetStatus(string message, bool isError)
        {
            StatusText.Text = message;
            StatusText.Foreground = isError
                ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF742C27"))
                : (Brush)Application.Current.Resources["ModalColorPrimaryDarkBrush"];
            StatusText.Visibility = string.IsNullOrWhiteSpace(message)
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        private static async Task<string> ReadApiMessageAsync(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(content))
            {
                return "Не удалось выполнить операцию.";
            }

            var trimmed = content.Trim();
            if (trimmed.StartsWith("\"") && trimmed.EndsWith("\""))
            {
                try
                {
                    return JsonSerializer.Deserialize<string>(trimmed) ?? trimmed.Trim('"');
                }
                catch
                {
                    return trimmed.Trim('"');
                }
            }

            return trimmed;
        }

        private static string ResolvePeriodName(string period)
        {
            return period switch
            {
                "month" => "Месяц",
                "threeMonths" => "3 месяца",
                "halfYear" => "Полгода",
                "allTime" => "Все время",
                _ => "Неделя"
            };
        }

        private static string ShortenLabel(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            return value.Length <= 18 ? value : value.Substring(0, 15) + "...";
        }

        private static string FormatDecimal(decimal value)
        {
            return value.ToString("0.##", CultureInfo.InvariantCulture);
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public sealed class MenuReportItemViewModel
        {
            public MenuReportItemViewModel(ReportMenuItemDto source)
            {
                Source = source;
            }

            public ReportMenuItemDto Source { get; }
            public string ItemTypeName => Source.ItemTypeName;
            public string Name => Source.Name;
            public string QuantityDisplay => FormatDecimal(Source.QuantitySold);
            public string RevenueDisplay => FormatDecimal(Source.Revenue) + " руб.";
        }

        public sealed class InventoryReportItemViewModel
        {
            public InventoryReportItemViewModel(InventoryReportItemDto source)
            {
                Source = source;
                Details = new ObservableCollection<InventoryDetailViewModel>(
                    (source.Details ?? new List<InventoryDetailDto>()).Select(x => new InventoryDetailViewModel(x)));
            }

            public InventoryReportItemDto Source { get; }
            public string ItemTypeName => Source.ItemTypeName;
            public string Name => Source.Name;
            public string UnitName => Source.UnitName;
            public ObservableCollection<InventoryDetailViewModel> Details { get; }
            public string OrderConsumptionDisplay => FormatDecimal(Source.OrderConsumption);
            public string WriteOffConsumptionDisplay => FormatDecimal(Source.WriteOffConsumption);
            public string PreparationConsumptionDisplay => FormatDecimal(Source.PreparationConsumption);
            public string ExpectedConsumptionDisplay => FormatDecimal(Source.ExpectedConsumption);
            public string ActualStockDisplay => FormatDecimal(Source.ActualStock);
            public string DifferenceDisplay => FormatDecimal(Source.Difference);
            public string UnitCostDisplay => FormatDecimal(Source.UnitCostRub);
            public string DifferenceCostDisplay => FormatDecimal(Source.DifferenceCostRub);
        }

        public sealed class AbcReportItemViewModel
        {
            public AbcReportItemViewModel(AbcReportItemDto source)
            {
                Source = source;
            }

            public AbcReportItemDto Source { get; }
            public string Group => Source.Group;
            public string ItemTypeName => Source.ItemTypeName;
            public string Name => Source.Name;
            public string QuantityDisplay => FormatDecimal(Source.QuantitySold);
            public string RevenueDisplay => FormatDecimal(Source.Revenue) + " руб.";
            public string RevenueShareDisplay => FormatDecimal(Source.RevenueSharePercent) + "%";
            public string CumulativeShareDisplay => FormatDecimal(Source.CumulativeSharePercent) + "%";
        }

        public sealed class InventoryDetailViewModel
        {
            public InventoryDetailViewModel(InventoryDetailDto source)
            {
                Source = source;
            }

            public InventoryDetailDto Source { get; }
            public string SourceType => Source.SourceType;
            public string SourceName => Source.SourceName;
            public string SourceDateDisplay => Source.SourceDate.HasValue
                ? Source.SourceDate.Value.ToString("dd.MM.yyyy", CultureInfo.CurrentCulture)
                : string.Empty;
            public string QuantityDisplay => FormatDecimal(Source.Quantity) + " " + Source.UnitName;
        }
    }
}
