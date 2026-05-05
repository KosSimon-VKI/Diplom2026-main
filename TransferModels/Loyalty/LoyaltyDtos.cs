namespace TransferModels.Loyalty
{
    public class DiscountManagementDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal DiscountPercent { get; set; }
        public int OrdersCount { get; set; }
    }

    public class DiscountUpsertRequest
    {
        public string Name { get; set; } = string.Empty;
        public decimal DiscountPercent { get; set; }
    }

    public class ClientCategoryManagementDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ClientsCount { get; set; }
    }

    public class ClientCategoryUpsertRequest
    {
        public string Name { get; set; } = string.Empty;
    }
}
