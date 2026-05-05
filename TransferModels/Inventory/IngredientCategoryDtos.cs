namespace TransferModels.Inventory
{
    public class IngredientCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ItemsCount { get; set; }
    }

    public class IngredientCategoryRequest
    {
        public string Name { get; set; } = string.Empty;
    }
}
