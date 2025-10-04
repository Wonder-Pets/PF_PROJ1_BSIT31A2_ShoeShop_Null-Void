using Microsoft.EntityFrameworkCore;
using ShoeShop.Repository;
using ShoeShop.Repository.Data;
using ShoeShop.Repository.Entities;

namespace ShoeShop.Repository.Console;

class Program
{
    static async Task Main(string[] args)
    {
        System.Console.WriteLine("🚀 ShoeShop Repository Layer Testing Application");
        System.Console.WriteLine(new string('=', 60));
        
        // Setup database context
        var options = new DbContextOptionsBuilder<ShoeShopDbContext>()
            .UseSqlite("Data Source=shoeshop_test.db")
            .EnableSensitiveDataLogging() // For testing only
            .Options;

        using var context = new ShoeShopDbContext(options);
        
        try
        {
            // Initialize database and seed data
            await InitializeDatabaseAsync(context);
            
            // Run all tests
            await RunAllTestsAsync(context);
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"❌ Error: {ex.Message}");
            System.Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
        
        System.Console.WriteLine("\n✅ Testing completed. Press any key to exit...");
        System.Console.ReadKey();
    }
    
    static async Task InitializeDatabaseAsync(ShoeShopDbContext context)
    {
        System.Console.WriteLine("\n📊 Initializing Database...");
        
        // Ensure database is deleted and recreated for fresh testing
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        
        // Seed data
        await DatabaseSeeder.SeedAsync(context);
        
        System.Console.WriteLine("✅ Database initialized with seed data");
    }
    
    static async Task RunAllTestsAsync(ShoeShopDbContext context)
    {
        System.Console.WriteLine("\n🧪 Running Repository Layer Tests...");
        
        await TestBasicCrudOperations(context);
        await TestRelationshipsAndNavigationProperties(context);
        await TestComplexQueries(context);
        await TestDataIntegrity(context);
        await TestInventoryScenarios(context);
    }
    
    static async Task TestBasicCrudOperations(ShoeShopDbContext context)
    {
        System.Console.WriteLine("\n1️⃣  Testing Basic CRUD Operations");
        System.Console.WriteLine(new string('-', 40));
        
        // Test Shoe CRUD
        System.Console.WriteLine("Testing Shoe entity CRUD...");
        
        // Create
        var newShoe = new Shoe
        {
            Name = "Test Sneaker",
            Brand = "TestBrand",
            Cost = 50.00m,
            Price = 100.00m,
            Description = "A test sneaker for CRUD operations",
            IsActive = true
        };
        
        context.Shoes.Add(newShoe);
        await context.SaveChangesAsync();
        System.Console.WriteLine($"  ✅ Created: {newShoe.Name} (ID: {newShoe.Id})");
        
        // Read
        var retrievedShoe = await context.Shoes.FindAsync(newShoe.Id);
        System.Console.WriteLine($"  ✅ Retrieved: {retrievedShoe?.Name}");
        
        // Update
        if (retrievedShoe != null)
        {
            retrievedShoe.Price = 120.00m;
            await context.SaveChangesAsync();
            System.Console.WriteLine($"  ✅ Updated price to: ${retrievedShoe.Price}");
        }
        
        // Test Color Variation CRUD
        System.Console.WriteLine("Testing ShoeColorVariation entity CRUD...");
        
        var colorVariation = new ShoeColorVariation
        {
            ShoeId = newShoe.Id,
            ColorName = "Test Red",
            HexCode = "#FF0000",
            StockQuantity = 25,
            ReorderLevel = 5
        };
        
        context.ShoeColorVariations.Add(colorVariation);
        await context.SaveChangesAsync();
        System.Console.WriteLine($"  ✅ Created color variation: {colorVariation.ColorName} (Stock: {colorVariation.StockQuantity})");
    }
    
    static async Task TestRelationshipsAndNavigationProperties(ShoeShopDbContext context)
    {
        System.Console.WriteLine("\n2️⃣  Testing Relationships and Navigation Properties");
        System.Console.WriteLine(new string('-', 50));
        
        // Test Shoe -> ColorVariations navigation
        var shoeWithColors = await context.Shoes
            .Include(s => s.ColorVariations)
            .FirstAsync();
            
        System.Console.WriteLine($"Shoe '{shoeWithColors.Name}' has {shoeWithColors.ColorVariations.Count} color variations:");
        foreach (var color in shoeWithColors.ColorVariations.Take(3))
        {
            System.Console.WriteLine($"  - {color.ColorName} ({color.HexCode}): {color.StockQuantity} in stock");
        }
        
        // Test PurchaseOrder -> Supplier navigation
        var orderWithSupplier = await context.PurchaseOrders
            .Include(po => po.Supplier)
            .FirstAsync();
            
        System.Console.WriteLine($"\nPurchase Order '{orderWithSupplier.OrderNumber}' from supplier: {orderWithSupplier.Supplier.Name}");
        
        // Test PurchaseOrder -> Items -> ColorVariation -> Shoe navigation
        var orderWithItems = await context.PurchaseOrders
            .Include(po => po.PurchaseOrderItems)
                .ThenInclude(poi => poi.ShoeColorVariation)
                    .ThenInclude(scv => scv.Shoe)
            .FirstAsync();
            
        System.Console.WriteLine($"\nOrder '{orderWithItems.OrderNumber}' contains {orderWithItems.PurchaseOrderItems.Count} items:");
        foreach (var item in orderWithItems.PurchaseOrderItems.Take(3))
        {
            System.Console.WriteLine($"  - {item.ShoeColorVariation.Shoe.Name} ({item.ShoeColorVariation.ColorName}): {item.QuantityOrdered} units @ ${item.UnitCost}");
        }
    }
    
    static async Task TestComplexQueries(ShoeShopDbContext context)
    {
        System.Console.WriteLine("\n3️⃣  Testing Complex Queries");
        System.Console.WriteLine(new string('-', 30));
        
        // Low stock items query
        var lowStockItems = await context.ShoeColorVariations
            .Include(scv => scv.Shoe)
            .Where(scv => scv.StockQuantity <= scv.ReorderLevel && scv.IsActive)
            .OrderBy(scv => scv.StockQuantity)
            .ToListAsync();
            
        System.Console.WriteLine($"Found {lowStockItems.Count} items with low stock:");
        foreach (var item in lowStockItems.Take(5))
        {
            System.Console.WriteLine($"  - {item.Shoe.Name} ({item.ColorName}): {item.StockQuantity} <= {item.ReorderLevel}");
        }
        
        // Shoes by brand query
        var brandGrouping = await context.Shoes
            .Where(s => s.IsActive)
            .GroupBy(s => s.Brand)
            .Select(g => new { Brand = g.Key, Count = g.Count(), AvgPrice = g.Average(s => s.Price) })
            .OrderByDescending(g => g.Count)
            .ToListAsync();
            
        System.Console.WriteLine($"\nShoes by brand:");
        foreach (var brand in brandGrouping)
        {
            System.Console.WriteLine($"  - {brand.Brand}: {brand.Count} shoes, avg price ${brand.AvgPrice:F2}");
        }
        
        // Pending purchase orders
        var pendingOrders = await context.PurchaseOrders
            .Include(po => po.Supplier)
            .Where(po => po.Status == PurchaseOrderStatus.Pending || po.Status == PurchaseOrderStatus.Confirmed)
            .OrderBy(po => po.ExpectedDate)
            .ToListAsync();
            
        System.Console.WriteLine($"\nPending orders ({pendingOrders.Count}):");
        foreach (var order in pendingOrders)
        {
            System.Console.WriteLine($"  - {order.OrderNumber}: {order.Supplier.Name}, Expected: {order.ExpectedDate?.ToString("yyyy-MM-dd")}, ${order.TotalAmount}");
        }
    }
    
    static async Task TestDataIntegrity(ShoeShopDbContext context)
    {
        System.Console.WriteLine("\n4️⃣  Testing Data Integrity and Constraints");
        System.Console.WriteLine(new string('-', 45));
        
        try
        {
            // Test unique constraint on ShoeColorVariation
            var existingVariation = await context.ShoeColorVariations.FirstAsync();
            var duplicateVariation = new ShoeColorVariation
            {
                ShoeId = existingVariation.ShoeId,
                ColorName = existingVariation.ColorName, // Duplicate color for same shoe
                HexCode = "#000000",
                StockQuantity = 10
            };
            
            context.ShoeColorVariations.Add(duplicateVariation);
            await context.SaveChangesAsync();
            
            System.Console.WriteLine("  ❌ Unique constraint test failed - duplicate was allowed");
        }
        catch (Exception)
        {
            System.Console.WriteLine("  ✅ Unique constraint working - duplicate shoe-color combination rejected");
            context.ChangeTracker.Clear(); // Clear any failed changes
        }
        
        // Test foreign key constraints
        try
        {
            var invalidVariation = new ShoeColorVariation
            {
                ShoeId = 99999, // Non-existent shoe ID
                ColorName = "Invalid Color",
                StockQuantity = 5
            };
            
            context.ShoeColorVariations.Add(invalidVariation);
            await context.SaveChangesAsync();
            
            System.Console.WriteLine("  ❌ Foreign key constraint test failed - invalid reference was allowed");
        }
        catch (Exception)
        {
            System.Console.WriteLine("  ✅ Foreign key constraint working - invalid shoe reference rejected");
            context.ChangeTracker.Clear();
        }
        
        // Test cascade delete
        var testShoe = new Shoe
        {
            Name = "Delete Test Shoe",
            Brand = "TestBrand",
            Cost = 30.00m,
            Price = 60.00m
        };
        
        context.Shoes.Add(testShoe);
        await context.SaveChangesAsync();
        
        var testColor = new ShoeColorVariation
        {
            ShoeId = testShoe.Id,
            ColorName = "Delete Test Color",
            StockQuantity = 5
        };
        
        context.ShoeColorVariations.Add(testColor);
        await context.SaveChangesAsync();
        
        var colorCountBefore = await context.ShoeColorVariations.CountAsync();
        
        context.Shoes.Remove(testShoe);
        await context.SaveChangesAsync();
        
        var colorCountAfter = await context.ShoeColorVariations.CountAsync();
        
        if (colorCountAfter < colorCountBefore)
        {
            System.Console.WriteLine("  ✅ Cascade delete working - color variation was deleted with shoe");
        }
        else
        {
            System.Console.WriteLine("  ❌ Cascade delete not working - color variation remained after shoe deletion");
        }
    }
    
    static async Task TestInventoryScenarios(ShoeShopDbContext context)
    {
        System.Console.WriteLine("\n5️⃣  Testing Real-world Inventory Scenarios");
        System.Console.WriteLine(new string('-', 45));
        
        // Scenario 1: Process a purchase order receipt
        System.Console.WriteLine("Scenario: Processing purchase order receipt...");
        
        var pendingOrder = await context.PurchaseOrders
            .Include(po => po.PurchaseOrderItems)
                .ThenInclude(poi => poi.ShoeColorVariation)
            .Where(po => po.Status == PurchaseOrderStatus.Pending)
            .FirstOrDefaultAsync();
            
        if (pendingOrder != null)
        {
            System.Console.WriteLine($"  Processing order: {pendingOrder.OrderNumber}");
            
            foreach (var item in pendingOrder.PurchaseOrderItems.Take(2))
            {
                var stockBefore = item.ShoeColorVariation.StockQuantity;
                item.QuantityReceived = item.QuantityOrdered;
                item.ShoeColorVariation.StockQuantity += item.QuantityReceived;
                
                System.Console.WriteLine($"    - Updated stock: {stockBefore} -> {item.ShoeColorVariation.StockQuantity} (+{item.QuantityReceived})");
            }
            
            pendingOrder.Status = PurchaseOrderStatus.Received;
            await context.SaveChangesAsync();
            
            System.Console.WriteLine($"  ✅ Order {pendingOrder.OrderNumber} marked as received");
        }
        
        // Scenario 2: Create a stock pull-out request
        System.Console.WriteLine("\nScenario: Creating stock pull-out request...");
        
        var variationForPullout = await context.ShoeColorVariations
            .Include(scv => scv.Shoe)
            .Where(scv => scv.StockQuantity > 5)
            .FirstAsync();
            
        var stockPullOut = new StockPullOut
        {
            ShoeColorVariationId = variationForPullout.Id,
            Quantity = 3,
            Reason = "Quality Control",
            ReasonDetails = "Routine quality inspection sample",
            RequestedBy = "Test QC Inspector",
            Status = StockPullOutStatus.Pending
        };
        
        context.StockPullOuts.Add(stockPullOut);
        await context.SaveChangesAsync();
        
        System.Console.WriteLine($"  ✅ Created pull-out request: {stockPullOut.Quantity} units of {variationForPullout.Shoe.Name} ({variationForPullout.ColorName})");
        
        // Scenario 3: Generate inventory summary report
        System.Console.WriteLine("\nScenario: Generating inventory summary...");
        
        var inventorySummary = await context.ShoeColorVariations
            .Include(scv => scv.Shoe)
            .GroupBy(scv => scv.Shoe.Brand)
            .Select(g => new 
            {
                Brand = g.Key,
                TotalVariations = g.Count(),
                TotalStock = g.Sum(scv => scv.StockQuantity),
                LowStockCount = g.Count(scv => scv.StockQuantity <= scv.ReorderLevel),
                TotalValue = g.Sum(scv => scv.StockQuantity * scv.Shoe.Cost)
            })
            .ToListAsync();
            
        System.Console.WriteLine($"\n📊 Inventory Summary by Brand:");
        foreach (var summary in inventorySummary)
        {
            System.Console.WriteLine($"  {summary.Brand}:");
            System.Console.WriteLine($"    - {summary.TotalVariations} color variations");
            System.Console.WriteLine($"    - {summary.TotalStock} total units in stock");
            System.Console.WriteLine($"    - {summary.LowStockCount} variations need reordering");
            System.Console.WriteLine($"    - ${summary.TotalValue:F2} total inventory value");
        }
    }
}
