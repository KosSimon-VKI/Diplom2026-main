using System;
using System.Collections.Generic;

namespace TransferModels.Reports
{
    public class ReportsResponse
    {
        public string Period { get; set; } = string.Empty;
        public string PeriodName { get; set; } = string.Empty;
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public List<ReportMenuItemDto> PopularItems { get; set; } = new List<ReportMenuItemDto>();
        public List<ReportMenuItemDto> UnpopularItems { get; set; } = new List<ReportMenuItemDto>();
        public List<InventoryReportItemDto> InventoryItems { get; set; } = new List<InventoryReportItemDto>();
    }

    public class ReportMenuItemDto
    {
        public string ItemType { get; set; } = string.Empty;
        public string ItemTypeName { get; set; } = string.Empty;
        public int ItemId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal QuantitySold { get; set; }
        public decimal Revenue { get; set; }
    }

    public class InventoryReportItemDto
    {
        public string ItemType { get; set; } = string.Empty;
        public string ItemTypeName { get; set; } = string.Empty;
        public int ItemId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string UnitName { get; set; } = string.Empty;
        public decimal ExpectedConsumption { get; set; }
        public decimal ActualStock { get; set; }
        public decimal Difference { get; set; }
    }
}
