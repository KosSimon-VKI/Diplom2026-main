using System.Collections.Generic;

namespace TransferModels.Menu
{
    public class MenuItemManagementDto
    {
        public string Type { get; set; } = string.Empty;
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int? UnitOfMeasureId { get; set; }
        public string UnitName { get; set; } = string.Empty;
        public decimal? Quantity { get; set; }
        public decimal PriceRub { get; set; }
        public int? TechnicalCardId { get; set; }
        public string TechnicalCardName { get; set; } = string.Empty;
        public decimal FatsG { get; set; }
        public decimal ProteinsG { get; set; }
        public decimal CarbsG { get; set; }
        public decimal CaloriesKcal { get; set; }
        public decimal Kilojoules { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
    }

    public class MenuItemUpsertRequest
    {
        public string Name { get; set; } = string.Empty;
        public int? CategoryId { get; set; }
        public int? UnitOfMeasureId { get; set; }
        public decimal? Quantity { get; set; }
        public decimal PriceRub { get; set; }
        public int? TechnicalCardId { get; set; }
        public decimal FatsG { get; set; }
        public decimal ProteinsG { get; set; }
        public decimal CarbsG { get; set; }
        public decimal CaloriesKcal { get; set; }
        public decimal Kilojoules { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsAvailable { get; set; } = true;
    }

    public class MenuItemEditOptionsResponse
    {
        public List<MenuItemCategoryOptionDto> Categories { get; set; } = new List<MenuItemCategoryOptionDto>();
        public List<MenuItemOptionDto> UnitsOfMeasure { get; set; } = new List<MenuItemOptionDto>();
        public List<MenuItemOptionDto> TechnicalCards { get; set; } = new List<MenuItemOptionDto>();
    }

    public class MenuItemCategoryOptionDto
    {
        public string Type { get; set; } = string.Empty;
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class MenuItemOptionDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
