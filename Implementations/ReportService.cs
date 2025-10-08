using WebApplication5.DTO;
using WebApplication5.Interfaces;

namespace WebApplication5.Implementations;

public class ReportService(IInventoryService inventoryService) : IReportService
{


    public InventoryReportDto GenerateInventoryReport()
    {
        var shoes = inventoryService.GetAllShoes().ToList();
        var totalShoes = shoes.Count;
        var totalVariations = totalShoes * 3; // mock variations
        var lowStockItems = 2; // sample
        var totalValue = shoes.Sum(s => s.Cost * 10); // assume 10 pairs each
        var totalStock = shoes.Count * 10; // mock total stock

        return new InventoryReportDto
        {
            TotalShoes = totalShoes,
            TotalVariations = totalVariations,
            LowStockItems = lowStockItems,
            TotalInventoryValue = totalValue,
            TotalStock = totalStock,
            TotalValue = totalValue
        };
    }
}
