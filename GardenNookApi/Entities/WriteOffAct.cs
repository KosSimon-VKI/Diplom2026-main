using System;
using System.Collections.Generic;

namespace GardenNookApi.Entities;

public partial class WriteOffAct
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public string? Comment { get; set; }

    public int? StaffId { get; set; }

    public virtual Staff? Staff { get; set; }

    public virtual ICollection<IngredientWriteOffActItem> IngredientItems { get; set; } = new List<IngredientWriteOffActItem>();

    public virtual ICollection<SemiFinishedWriteOffActItem> SemiFinishedItems { get; set; } = new List<SemiFinishedWriteOffActItem>();
}
