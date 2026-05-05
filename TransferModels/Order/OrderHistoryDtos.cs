using System;
using System.Collections.Generic;

namespace TransferModels.Orders
{
    public class OrderHistoryResponse
    {
        public List<OrderHistoryListItemDto> Orders { get; set; } = new List<OrderHistoryListItemDto>();
    }

    public class OrderHistoryListItemDto
    {
        public int OrderId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? PickupAt { get; set; }
        public int? ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string ClientPhone { get; set; } = string.Empty;
        public int? OrderTypeId { get; set; }
        public string OrderType { get; set; } = string.Empty;
        public int? StatusId { get; set; }
        public string Status { get; set; } = string.Empty;
        public int? DiscountId { get; set; }
        public string DiscountName { get; set; } = string.Empty;
        public decimal DiscountPercent { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalCalories { get; set; }
        public string Comment { get; set; } = string.Empty;
        public string CompositionSummary { get; set; } = string.Empty;
    }

    public class OrderHistoryDetailsDto : OrderHistoryListItemDto
    {
        public List<OrderHistoryDishItemDto> Dishes { get; set; } = new List<OrderHistoryDishItemDto>();
        public List<OrderHistoryDrinkItemDto> Drinks { get; set; } = new List<OrderHistoryDrinkItemDto>();
        public List<OrderHistoryToppingItemDto> Toppings { get; set; } = new List<OrderHistoryToppingItemDto>();
        public List<OrderHistoryOptionDto> OrderTypes { get; set; } = new List<OrderHistoryOptionDto>();
        public List<OrderHistoryOptionDto> Statuses { get; set; } = new List<OrderHistoryOptionDto>();
        public List<OrderHistoryDiscountOptionDto> Discounts { get; set; } = new List<OrderHistoryDiscountOptionDto>();
    }

    public class OrderHistoryOptionDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class OrderHistoryDiscountOptionDto : OrderHistoryOptionDto
    {
        public decimal DiscountPercent { get; set; }
    }

    public class OrderHistoryDishItemDto
    {
        public int ItemId { get; set; }
        public int DishId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal FinalPrice { get; set; }
        public List<OrderHistoryLinkedToppingDto> Toppings { get; set; } = new List<OrderHistoryLinkedToppingDto>();
    }

    public class OrderHistoryDrinkItemDto
    {
        public int ItemId { get; set; }
        public int DrinkId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal FinalPrice { get; set; }
        public int? MilkIngredientId { get; set; }
        public string MilkIngredientName { get; set; } = string.Empty;
        public int? CoffeeIngredientId { get; set; }
        public string CoffeeIngredientName { get; set; } = string.Empty;
        public List<OrderHistoryLinkedToppingDto> Toppings { get; set; } = new List<OrderHistoryLinkedToppingDto>();
    }

    public class OrderHistoryToppingItemDto
    {
        public int ItemId { get; set; }
        public int ToppingId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class OrderHistoryLinkedToppingDto
    {
        public int ToppingId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal FinalPrice { get; set; }
    }

    public class OrderHistoryUpdateRequest
    {
        public int OrderTypeId { get; set; }
        public int? StatusId { get; set; }
        public int? DiscountId { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime? PickupAt { get; set; }
        public List<OrderDishItemRequest> Dishes { get; set; } = new List<OrderDishItemRequest>();
        public List<OrderDrinkItemRequest> Drinks { get; set; } = new List<OrderDrinkItemRequest>();
        public List<OrderToppingItemRequest> Toppings { get; set; } = new List<OrderToppingItemRequest>();
    }
}
