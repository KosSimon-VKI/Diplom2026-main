using System;
using System.Collections.Generic;

namespace TransferModels.Kitchen
{
    public class KitchenWriteOffBoardResponse
    {
        public List<KitchenWriteOffTypeDto> WriteOffTypes { get; set; } = new List<KitchenWriteOffTypeDto>();
        public List<KitchenWriteOffSemiFinishedOptionDto> SemiFinishedOptions { get; set; } = new List<KitchenWriteOffSemiFinishedOptionDto>();
        public List<KitchenWriteOffIngredientOptionDto> IngredientOptions { get; set; } = new List<KitchenWriteOffIngredientOptionDto>();
        public List<KitchenWriteOffActDto> Acts { get; set; } = new List<KitchenWriteOffActDto>();
    }

    public class KitchenWriteOffTypeDto
    {
        public int WriteOffTypeId { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class KitchenWriteOffSemiFinishedOptionDto
    {
        public int SemiFinishedId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? UnitOfMeasureId { get; set; }
        public string UnitName { get; set; } = string.Empty;
        public decimal AvailableStock { get; set; }
    }

    public class KitchenWriteOffIngredientOptionDto
    {
        public int IngredientId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? UnitOfMeasureId { get; set; }
        public string UnitName { get; set; } = string.Empty;
        public decimal AvailableStock { get; set; }
    }

    public class KitchenWriteOffActDto
    {
        public int ActId { get; set; }
        public DateTime Date { get; set; }
        public string Comment { get; set; } = string.Empty;
        public int? StaffId { get; set; }
        public string StaffFullName { get; set; } = string.Empty;
        public List<KitchenWriteOffActLineDto> IngredientLines { get; set; } = new List<KitchenWriteOffActLineDto>();
        public List<KitchenWriteOffActLineDto> SemiFinishedLines { get; set; } = new List<KitchenWriteOffActLineDto>();
    }

    public class KitchenWriteOffActLineDto
    {
        public int LineId { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public int? UnitOfMeasureId { get; set; }
        public string UnitName { get; set; } = string.Empty;
        public int WriteOffTypeId { get; set; }
        public string WriteOffTypeName { get; set; } = string.Empty;
    }

    public class KitchenCreateWriteOffActRequest
    {
        public DateTime? Date { get; set; }
        public string Comment { get; set; } = string.Empty;
        public List<KitchenCreateIngredientWriteOffLineRequest> IngredientLines { get; set; } = new List<KitchenCreateIngredientWriteOffLineRequest>();
        public List<KitchenCreateSemiFinishedWriteOffLineRequest> SemiFinishedLines { get; set; } = new List<KitchenCreateSemiFinishedWriteOffLineRequest>();
    }

    public class KitchenCreateIngredientWriteOffLineRequest
    {
        public int IngredientId { get; set; }
        public decimal Quantity { get; set; }
        public int WriteOffTypeId { get; set; }
    }

    public class KitchenCreateSemiFinishedWriteOffLineRequest
    {
        public int SemiFinishedId { get; set; }
        public decimal Quantity { get; set; }
        public int WriteOffTypeId { get; set; }
    }
}
