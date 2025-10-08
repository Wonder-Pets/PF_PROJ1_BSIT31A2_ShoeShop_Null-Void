using WebApplication5.DTO;
using System.ComponentModel.DataAnnotations;
using WebApplication5.Interfaces;


namespace WebApplication5.Implementations;

public class InventoryService : IInventoryService
{
    private readonly List<ShoeDto> _shoes = [];
    private readonly Dictionary<Guid, int> _stock = [];


    public ShoeDto CreateShoe(CreateShoeDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), true);


        var shoe = new ShoeDto
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Brand = dto.Brand,
            Cost = dto.Cost,
            Price = dto.Price,
            Description = dto.Description,
            ImageUrl = dto.ImageUrl,
            IsActive = dto.IsActive,
            CreatedDate = DateTime.Now
        };


        _shoes.Add(shoe);
        return shoe;
    }


    public IEnumerable<ShoeDto> GetAllShoes() => _shoes;


    public void UpdateStock(Guid shoeColorId, int newQuantity)
    {
        _stock[shoeColorId] = newQuantity;
    }


    public int GetAvailableStock(Guid shoeColorId)
    {
        return _stock.GetValueOrDefault(shoeColorId, 0);
    }
}