using System.Windows;

namespace GardenNookWpf.Views.Kitchen.Shell.Controllers
{
    public sealed class StopListPositionDisplayModel
    {
        public string ItemType { get; set; } = string.Empty;
        public int ItemId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
        public decimal? ManualRemainingPortions { get; set; }
        public decimal? AutoAvailablePortions { get; set; }
        public decimal? EffectiveRemainingPortions { get; set; }

        public string ItemTypeDisplay { get; set; } = string.Empty;
        public string StateDisplay { get; set; } = string.Empty;
        public string CategoryDisplay { get; set; } = string.Empty;
        public Visibility CategoryVisibility { get; set; }
        public string VolumeWeightDisplay { get; set; } = string.Empty;
        public Visibility VolumeWeightVisibility { get; set; }
        public string ManualRemainingDisplay { get; set; } = string.Empty;
        public Visibility ManualRemainingVisibility { get; set; }
        public string AutoAvailableDisplay { get; set; } = string.Empty;
        public Visibility AutoAvailableVisibility { get; set; }
        public string EffectiveRemainingDisplay { get; set; } = string.Empty;
        public Visibility EffectiveRemainingVisibility { get; set; }
    }
}
