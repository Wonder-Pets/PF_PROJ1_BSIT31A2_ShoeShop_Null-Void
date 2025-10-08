using WebApplication5.DTO;
using System.ComponentModel.DataAnnotations;
using WebApplication5.Interfaces;

namespace WebApplication5.Implementations;

public class PurchaseOrderService(IInventoryService inventoryService) : IPurchaseOrderService
{
    private readonly List<PurchaseOrderDto> _orders = [];


    public PurchaseOrderDto CreateOrder(CreatePurchaseOrderDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), true);

        var order = new PurchaseOrderDto
        {
            Id = Guid.NewGuid(),
            OrderNumber = $"PO-{DateTime.Now:yyyyMMddHHmmss}",
            SupplierName = $"Supplier-{dto.SupplierId.ToString()[..4]}",
            OrderDate = dto.OrderDate,
            ExpectedDate = dto.ExpectedDate,
            Status = "Pending",
            TotalAmount = dto.Items.Sum(i => i.UnitCost * i.QuantityOrdered),
            Items = dto.Items.Select(i => new PurchaseOrderItemDto
            {
                ColorName = i.ShoeColorVariationId.ToString(),
                QuantityOrdered = i.QuantityOrdered,
                QuantityReceived = 0,
                UnitCost = i.UnitCost
            }).ToList()
        };

        _orders.Add(order);
        return order;
    }


    public void ReceiveOrder(Guid orderId)
    {
        var order = _orders.FirstOrDefault(o => o.Id == orderId);
        ArgumentNullException.ThrowIfNull(order, "Order not found.");

        foreach (var item in order.Items)
        {
            inventoryService.UpdateStock(Guid.Parse(item.ColorName), item.QuantityOrdered);
            item.QuantityReceived = item.QuantityOrdered;
        }

        order.Status = "Received";
    }

    public IEnumerable<PurchaseOrderDto> GetAllOrders() => _orders;
}
