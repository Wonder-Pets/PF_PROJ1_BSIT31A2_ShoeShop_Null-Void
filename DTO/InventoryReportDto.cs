namespace WebApplication5.DTO

{
    public class InventoryReportDto
    {
        public int TotalShoes { get; set; }
        public int TotalVariations { get; set; }
        public int LowStockItems { get; set; }
        public decimal TotalInventoryValue { get; set; }
        public int TotalStock { get; set; }
        public decimal TotalValue { get; set; }
    }
}