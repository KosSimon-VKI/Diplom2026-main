namespace TransferModels.Menu
{
    public class MenuCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int ItemsCount { get; set; }
    }

    public class MenuCategoryRequest
    {
        public string Name { get; set; } = string.Empty;
    }
}
