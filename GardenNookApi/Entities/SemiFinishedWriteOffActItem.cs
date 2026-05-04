namespace GardenNookApi.Entities;

public partial class SemiFinishedWriteOffActItem
{
    public int Id { get; set; }

    public int WriteOffActId { get; set; }

    public int SemiFinishedId { get; set; }

    public decimal Quantity { get; set; }

    public int? UnitOfMeasureId { get; set; }

    public int WriteOffTypeId { get; set; }

    public virtual WriteOffAct WriteOffAct { get; set; } = null!;

    public virtual SemiFinished SemiFinished { get; set; } = null!;

    public virtual UnitsOfMeasure? UnitOfMeasure { get; set; }

    public virtual WriteOffType WriteOffType { get; set; } = null!;
}
