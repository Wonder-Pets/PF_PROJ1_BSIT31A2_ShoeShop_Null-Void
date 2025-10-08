using System.ComponentModel.DataAnnotations;


namespace WebApplication5.DTO
{
    public class CreateShoeDto
    {
        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;


        [Required, StringLength(50)]
        public string Brand { get; set; } = string.Empty;


        [Range(1, double.MaxValue, ErrorMessage = "Cost must be positive.")]
        public decimal Cost { get; set; }


        [Range(1, double.MaxValue, ErrorMessage = "Price must be positive.")]
        public decimal Price { get; set; }


        [Required]
        public string Description { get; set; } = string.Empty;


        public string? ImageUrl { get; set; }


        public bool IsActive { get; set; } = true;
    }
}