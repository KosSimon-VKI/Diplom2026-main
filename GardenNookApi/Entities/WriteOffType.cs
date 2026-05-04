using System.Collections.Generic;

namespace GardenNookApi.Entities;

public partial class WriteOffType
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public virtual ICollection<IngredientWriteOffActItem> IngredientWriteOffActItems { get; set; } = new List<IngredientWriteOffActItem>();

    public virtual ICollection<SemiFinishedWriteOffActItem> SemiFinishedWriteOffActItems { get; set; } = new List<SemiFinishedWriteOffActItem>();
}
