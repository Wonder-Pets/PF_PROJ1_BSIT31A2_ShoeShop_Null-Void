using WebApplication5.DTO;


namespace WebApplication5.Interfaces
{
    public interface IReportService
    {
        InventoryReportDto GenerateInventoryReport();
    }
}