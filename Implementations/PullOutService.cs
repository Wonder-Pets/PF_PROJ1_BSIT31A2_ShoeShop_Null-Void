using System.ComponentModel.DataAnnotations;
using WebApplication5.DTO;
using WebApplication5.Interfaces;

namespace WebApplication5.Implementations;

public class PullOutService(IInventoryService inventoryService) : IPullOutService
{
    private readonly List<PullOutRequestDto> _pullOuts = [];

    public PullOutRequestDto CreatePullOut(CreatePullOutDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), true);

        var availableStock = inventoryService.GetAvailableStock(dto.ShoeColorVariationId);
        if (dto.Quantity > availableStock)
            throw new Exception($"Cannot pull out {dto.Quantity}. Only {availableStock} in stock.");

        var pullOut = new PullOutRequestDto
        {
            Id = Guid.NewGuid(),
            ColorName = dto.ShoeColorVariationId.ToString(),
            Quantity = dto.Quantity,
            Reason = dto.Reason,
            RequestedBy = dto.RequestedBy,
            Status = dto.Quantity <= 5 ? "Auto-Approved" : "Pending",
            PullOutDate = DateTime.Now
        };

        if (pullOut.Status == "Auto-Approved")
        {
            inventoryService.UpdateStock(dto.ShoeColorVariationId, availableStock - dto.Quantity);
        }

        _pullOuts.Add(pullOut);
        return pullOut;
    }

    public void ApprovePullOut(Guid id, string approvedBy)
    {
        var pullOut = _pullOuts.FirstOrDefault(p => p.Id == id);
        ArgumentNullException.ThrowIfNull(pullOut, "Pull-out not found.");

        if (pullOut.Status == "Pending")
        {
            var currentStock = inventoryService.GetAvailableStock(Guid.Parse(pullOut.ColorName));
            inventoryService.UpdateStock(Guid.Parse(pullOut.ColorName), currentStock - pullOut.Quantity);
            pullOut.Status = "Approved";
            pullOut.ApprovedBy = approvedBy;
        }
    }

    public IEnumerable<PullOutRequestDto> GetAllPullOuts() => _pullOuts;
}
