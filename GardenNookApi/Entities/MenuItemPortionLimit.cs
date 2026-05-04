using System;

namespace GardenNookApi.Entities;

public partial class MenuItemPortionLimit
{
    public int Id { get; set; }

    public string ItemType { get; set; } = string.Empty;

    public int ItemId { get; set; }

    public decimal RemainingPortions { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
