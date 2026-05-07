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
        public DateTime GeneratedAt { get; set; }
        public string GeneratedBy { get; set; } = string.Empty;
        public List<string> Warnings { get; set; } = new List<string>();
        public List<ReportMenuItemDto> PopularItems { get; set; } = new List<ReportMenuItemDto>();
        public List<ReportMenuItemDto> UnpopularItems { get; set; } = new List<ReportMenuItemDto>();
        public List<AbcReportItemDto> AbcItems { get; set; } = new List<AbcReportItemDto>();
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
        public decimal OrderConsumption { get; set; }
        public decimal WriteOffConsumption { get; set; }
        public decimal PreparationConsumption { get; set; }
        public decimal ExpectedConsumption { get; set; }
        public decimal ActualStock { get; set; }
        public decimal Difference { get; set; }
        public decimal UnitCostRub { get; set; }
        public decimal DifferenceCostRub { get; set; }
        public List<InventoryDetailDto> Details { get; set; } = new List<InventoryDetailDto>();
    }

    public class InventoryDetailDto
    {
        public string SourceType { get; set; } = string.Empty;
        public string SourceName { get; set; } = string.Empty;
        public DateTime? SourceDate { get; set; }
        public decimal Quantity { get; set; }
        public string UnitName { get; set; } = string.Empty;
    }

    public class AbcReportItemDto
    {
        public string ItemType { get; set; } = string.Empty;
        public string ItemTypeName { get; set; } = string.Empty;
        public int ItemId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal QuantitySold { get; set; }
        public decimal Revenue { get; set; }
        public decimal RevenueSharePercent { get; set; }
        public decimal CumulativeSharePercent { get; set; }
        public string Group { get; set; } = string.Empty;
    }
}
