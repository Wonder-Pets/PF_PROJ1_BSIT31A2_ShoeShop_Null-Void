using WebApplication5.DTO;


namespace WebApplication5.Interfaces
{
    public interface IPurchaseOrderService
    {
        PurchaseOrderDto CreateOrder(CreatePurchaseOrderDto dto);
        void ReceiveOrder(Guid orderId);
        IEnumerable<PurchaseOrderDto> GetAllOrders();
    }
}