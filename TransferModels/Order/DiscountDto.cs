namespace TransferModels.Orders
{
    public class DiscountDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal DiscountPercent { get; set; }
    }
}
