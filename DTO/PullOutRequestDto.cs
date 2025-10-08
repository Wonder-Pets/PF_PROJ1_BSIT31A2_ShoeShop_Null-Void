namespace WebApplication5.DTO
{
    public class PullOutRequestDto
    {
        public Guid Id { get; set; }
        public string ColorName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string RequestedBy { get; set; } = string.Empty;
        public string? ApprovedBy { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime PullOutDate { get; set; }
    }
}