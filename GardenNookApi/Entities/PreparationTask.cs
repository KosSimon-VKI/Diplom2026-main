using System;

namespace GardenNookApi.Entities;

public partial class PreparationTask
{
    public int Id { get; set; }

    public int? SemiFinishedId { get; set; }

    public string TaskText { get; set; } = string.Empty;

    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual SemiFinished? SemiFinished { get; set; }
}
