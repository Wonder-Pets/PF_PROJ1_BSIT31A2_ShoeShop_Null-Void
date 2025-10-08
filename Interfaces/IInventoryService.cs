using WebApplication5.DTO;


namespace WebApplication5.Interfaces
{
    public interface IInventoryService
    {
        ShoeDto CreateShoe(CreateShoeDto dto);
        IEnumerable<ShoeDto> GetAllShoes();
        void UpdateStock(Guid shoeColorId, int newQuantity);
        int GetAvailableStock(Guid shoeColorId);
    }
}