namespace WebApplication5.DTO
{
    public class PurchaseOrderDto
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string SupplierName { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public DateTime ExpectedDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public List<PurchaseOrderItemDto> Items { get; set; } = new();
    }


    public class PurchaseOrderItemDto
    {
        public string ColorName { get; set; } = string.Empty;
        public int QuantityOrdered { get; set; }
        public int QuantityReceived { get; set; }
        public decimal UnitCost { get; set; }
    }
}