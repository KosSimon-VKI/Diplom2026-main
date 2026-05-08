using System;
using System.Collections.Generic;

namespace TransferModels.Kitchen
{
    public static class KitchenItemTypes
    {
        public const string Dish = "dish";
        public const string Drink = "drink";
        public const string Topping = "topping";
    }

    public class KitchenOrdersResponse
    {
        public List<KitchenOrderDto> Orders { get; set; } = new List<KitchenOrderDto>();
    }

    public class KitchenOrderDto
    {
        public int OrderId { get; set; }
        public string Comment { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? PickupAt { get; set; }
        public string OrderType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public List<KitchenOrderDishDto> Dishes { get; set; } = new List<KitchenOrderDishDto>();
        public List<KitchenOrderDrinkDto> Drinks { get; set; } = new List<KitchenOrderDrinkDto>();
        public List<KitchenOrderStandaloneToppingDto> Toppings { get; set; } = new List<KitchenOrderStandaloneToppingDto>();
    }

    public class KitchenOrderDishDto
    {
        public int ItemId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public List<KitchenOrderDishToppingDto> Toppings { get; set; } = new List<KitchenOrderDishToppingDto>();
    }

    public class KitchenOrderDishToppingDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
    }

    public class KitchenOrderStandaloneToppingDto
    {
        public int ItemId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }

    public class KitchenOrderDrinkDto
    {
        public int ItemId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public List<KitchenOrderDrinkToppingDto> Toppings { get; set; } = new List<KitchenOrderDrinkToppingDto>();
    }

    public class KitchenOrderDrinkToppingDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
    }

    public class KitchenCompleteOrderItemResponse
    {
        public int OrderId { get; set; }
        public string ItemType { get; set; } = string.Empty;
        public int ItemId { get; set; }
        public bool OrderCompleted { get; set; }
        public string OrderStatus { get; set; } = string.Empty;
    }

    public class KitchenCompleteOrderResponse
    {
        public int OrderId { get; set; }
        public string OrderStatus { get; set; } = string.Empty;
    }

    public class KitchenTechnicalCardResponse
    {
        public int TechnicalCardId { get; set; }
        public string CardName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<KitchenTechnicalCardComponentDto> Components { get; set; } = new List<KitchenTechnicalCardComponentDto>();
    }

    public class KitchenTechnicalCardsResponse
    {
        public List<KitchenTechnicalCardListItemDto> TechnicalCards { get; set; } = new List<KitchenTechnicalCardListItemDto>();
    }

    public class KitchenTechnicalCardListItemDto
    {
        public int TechnicalCardId { get; set; }
        public string CardName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class KitchenTechnicalCardComponentDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal Weight { get; set; }
        public string Unit { get; set; } = string.Empty;
    }

    public class KitchenPreparationsBoardResponse
    {
        public List<KitchenPreparationTaskDto> Tasks { get; set; } = new List<KitchenPreparationTaskDto>();
        public List<KitchenPreparationListItemDto> ExistingPreparations { get; set; } = new List<KitchenPreparationListItemDto>();
        public List<KitchenSemiFinishedOptionDto> SemiFinishedOptions { get; set; } = new List<KitchenSemiFinishedOptionDto>();
    }

    public class KitchenPreparationTaskDto
    {
        public int TaskId { get; set; }
        public int? SemiFinishedId { get; set; }
        public int? TechnicalCardId { get; set; }
        public string TaskText { get; set; } = string.Empty;
        public bool IsLinkedToSemiFinished { get; set; }
        public string SemiFinishedName { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class KitchenPreparationListItemDto
    {
        public int PreparationId { get; set; }
        public int SemiFinishedId { get; set; }
        public int? TechnicalCardId { get; set; }
        public string PreparationName { get; set; } = string.Empty;
        public decimal StockGrams { get; set; }
        public DateTime? ProductionDate { get; set; }
    }

    public class KitchenSemiFinishedOptionDto
    {
        public int SemiFinishedId { get; set; }
        public int? TechnicalCardId { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class KitchenCreatePreparationTaskRequest
    {
        public string TaskText { get; set; } = string.Empty;
        public int? SemiFinishedId { get; set; }
        public string Comment { get; set; } = string.Empty;
    }

    public class KitchenCompletePreparationTaskRequest
    {
        public decimal StockGrams { get; set; }
        public DateTime? ProductionDate { get; set; }
    }
}
