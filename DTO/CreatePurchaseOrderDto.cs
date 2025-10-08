using System.ComponentModel.DataAnnotations;


namespace WebApplication5.DTO
{
    public class CreatePurchaseOrderDto
    {
        [Required]
        public Guid SupplierId { get; set; }


        [Required]
        public DateTime ExpectedDate { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [MinLength(1)]
        public List<CreatePurchaseOrderItemDto> Items { get; set; } = [];
    }


    public class CreatePurchaseOrderItemDto
    {
        [Required]
        public Guid ShoeColorVariationId { get; set; }


        [Range(1, 10000)]
        public int QuantityOrdered { get; set; }


        [Range(1, double.MaxValue)]
        public decimal UnitCost { get; set; }
    }
}