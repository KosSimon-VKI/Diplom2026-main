using System.Collections.Generic;

namespace TransferModels.Kitchen
{
    public static class KitchenTechnicalCardBindingKinds
    {
        public const string Dish = "dish";
        public const string Drink = "drink";
        public const string Topping = "topping";
        public const string SemiFinished = "semiFinished";
    }

    public static class KitchenTechnicalCardCompositionKinds
    {
        public const string Ingredient = "ingredient";
        public const string SemiFinished = "semiFinished";
    }

    public class KitchenTechnicalCardEditOptionsResponse
    {
        public List<KitchenTechnicalCardReferenceDto> Ingredients { get; set; } = new List<KitchenTechnicalCardReferenceDto>();
        public List<KitchenTechnicalCardReferenceDto> SemiFinisheds { get; set; } = new List<KitchenTechnicalCardReferenceDto>();
        public List<KitchenTechnicalCardUnitDto> Units { get; set; } = new List<KitchenTechnicalCardUnitDto>();
        public List<KitchenTechnicalCardBindingOptionDto> BindingOptions { get; set; } = new List<KitchenTechnicalCardBindingOptionDto>();
    }

    public class KitchenTechnicalCardEditResponse
    {
        public int TechnicalCardId { get; set; }
        public string CardName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<KitchenTechnicalCardCompositionLineDto> IngredientLines { get; set; } = new List<KitchenTechnicalCardCompositionLineDto>();
        public List<KitchenTechnicalCardCompositionLineDto> SemiFinishedLines { get; set; } = new List<KitchenTechnicalCardCompositionLineDto>();
        public List<KitchenTechnicalCardBindingDto> Bindings { get; set; } = new List<KitchenTechnicalCardBindingDto>();
    }

    public class KitchenTechnicalCardUpsertRequest
    {
        public string CardName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<KitchenTechnicalCardCompositionLineDto> IngredientLines { get; set; } = new List<KitchenTechnicalCardCompositionLineDto>();
        public List<KitchenTechnicalCardCompositionLineDto> SemiFinishedLines { get; set; } = new List<KitchenTechnicalCardCompositionLineDto>();
        public List<KitchenTechnicalCardBindingDto> Bindings { get; set; } = new List<KitchenTechnicalCardBindingDto>();
    }

    public class KitchenTechnicalCardCompositionLineDto
    {
        public int ItemId { get; set; }
        public int? UnitOfMeasureId { get; set; }
        public decimal? GrossWeight { get; set; }
        public decimal? ColdLossPercent { get; set; }
        public decimal? NetWeight { get; set; }
        public decimal? HotLossPercent { get; set; }
        public decimal? OutputWeight { get; set; }
    }

    public class KitchenTechnicalCardBindingDto
    {
        public string Kind { get; set; } = string.Empty;
        public int ItemId { get; set; }
    }

    public class KitchenTechnicalCardReferenceDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? UnitOfMeasureId { get; set; }
        public string UnitName { get; set; } = string.Empty;
    }

    public class KitchenTechnicalCardUnitDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class KitchenTechnicalCardBindingOptionDto
    {
        public string Kind { get; set; } = string.Empty;
        public int ItemId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? TechnicalCardId { get; set; }
    }
}
