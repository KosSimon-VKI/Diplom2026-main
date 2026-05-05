using System.Collections.Generic;

namespace TransferModels.Inventory
{
    public class InventoryIngredientDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Stock { get; set; }
        public int? UnitOfMeasureId { get; set; }
        public string UnitName { get; set; } = string.Empty;
        public decimal CostRub { get; set; }
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }

    public class InventoryIngredientRequest
    {
        public string Name { get; set; } = string.Empty;
        public decimal Stock { get; set; }
        public int? UnitOfMeasureId { get; set; }
        public decimal CostRub { get; set; }
        public int? CategoryId { get; set; }
    }

    public class InventoryIngredientSupplyRequest
    {
        public List<InventoryIngredientSupplyLineRequest> Lines { get; set; } = new List<InventoryIngredientSupplyLineRequest>();
    }

    public class InventoryIngredientSupplyLineRequest
    {
        public int IngredientId { get; set; }
        public decimal Quantity { get; set; }
    }

    public class InventorySemiFinishedDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal CostRub { get; set; }
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int? UnitOfMeasureId { get; set; }
        public string UnitName { get; set; } = string.Empty;
        public int? TechnicalCardId { get; set; }
        public string TechnicalCardName { get; set; } = string.Empty;
        public decimal FatsG { get; set; }
        public decimal ProteinsG { get; set; }
        public decimal CarbsG { get; set; }
        public decimal CaloriesKcal { get; set; }
        public decimal Kilojoules { get; set; }
    }

    public class InventorySemiFinishedRequest
    {
        public string Name { get; set; } = string.Empty;
        public decimal CostRub { get; set; }
        public int? CategoryId { get; set; }
        public int? UnitOfMeasureId { get; set; }
        public int? TechnicalCardId { get; set; }
        public decimal FatsG { get; set; }
        public decimal ProteinsG { get; set; }
        public decimal CarbsG { get; set; }
        public decimal CaloriesKcal { get; set; }
        public decimal Kilojoules { get; set; }
    }

    public class InventoryEditOptionsResponse
    {
        public List<InventoryOptionDto> UnitsOfMeasure { get; set; } = new List<InventoryOptionDto>();
        public List<InventoryOptionDto> IngredientCategories { get; set; } = new List<InventoryOptionDto>();
        public List<InventoryOptionDto> SemiFinishedCategories { get; set; } = new List<InventoryOptionDto>();
        public List<InventoryOptionDto> TechnicalCards { get; set; } = new List<InventoryOptionDto>();
    }

    public class InventoryOptionDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
