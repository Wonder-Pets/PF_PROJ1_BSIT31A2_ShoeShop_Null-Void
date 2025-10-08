namespace WebApplication5.DTO
{
    public class ShoeDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public decimal Cost { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}