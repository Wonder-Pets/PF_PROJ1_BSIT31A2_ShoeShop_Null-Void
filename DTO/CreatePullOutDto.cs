using System.ComponentModel.DataAnnotations;



namespace WebApplication5.DTO
{
    public class CreatePullOutDto
    {
        [Required]
        public Guid ShoeColorVariationId { get; set; }


        [Range(1, 1000)]
        public int Quantity { get; set; }


        [Required]
        public string Reason { get; set; } = string.Empty;


        public string? ReasonDetails { get; set; }


        [Required]
        public string RequestedBy { get; set; } = string.Empty;
    }
}