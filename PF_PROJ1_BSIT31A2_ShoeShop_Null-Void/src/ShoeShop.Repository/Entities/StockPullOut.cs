using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoeShop.Repository.Entities;

public class StockPullOut
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("ShoeColorVariation")]
    public int ShoeColorVariationId { get; set; }

    public int Quantity { get; set; }

    [Required]
    [MaxLength(100)]
    public string Reason { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? ReasonDetails { get; set; }

    [Required]
    [MaxLength(100)]
    public string RequestedBy { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? ApprovedBy { get; set; }

    public DateTime PullOutDate { get; set; } = DateTime.UtcNow;

    public StockPullOutStatus Status { get; set; } = StockPullOutStatus.Pending;

    // Navigation property
    public virtual ShoeColorVariation ShoeColorVariation { get; set; } = null!;
}
