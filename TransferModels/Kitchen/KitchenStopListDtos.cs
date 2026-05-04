using System.Collections.Generic;

namespace TransferModels.Kitchen
{
    public class KitchenStopListPositionsResponse
    {
        public List<KitchenStopListPositionDto> Positions { get; set; } = new List<KitchenStopListPositionDto>();
    }

    public class KitchenStopListPositionDto
    {
        public string ItemType { get; set; } = string.Empty;
        public int ItemId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string VolumeWeight { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
        public decimal? ManualRemainingPortions { get; set; }
        public decimal? AutoAvailablePortions { get; set; }
        public decimal? EffectiveRemainingPortions { get; set; }
    }

    public class KitchenStopListItemRequest
    {
        public string ItemType { get; set; } = string.Empty;
        public int ItemId { get; set; }
        public decimal RemainingPortions { get; set; }
    }
}
