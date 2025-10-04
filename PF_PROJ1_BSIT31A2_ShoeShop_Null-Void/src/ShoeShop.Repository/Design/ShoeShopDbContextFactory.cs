using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ShoeShop.Repository.Design;

public class ShoeShopDbContextFactory : IDesignTimeDbContextFactory<ShoeShopDbContext>
{
    public ShoeShopDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ShoeShopDbContext>();
        
        // Use SQLite for design-time migrations
        optionsBuilder.UseSqlite("Data Source=shoeshop.db");

        return new ShoeShopDbContext(optionsBuilder.Options);
    }
}
