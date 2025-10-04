using Microsoft.EntityFrameworkCore;
using ShoeShop.Repository.Entities;

namespace ShoeShop.Repository.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(ShoeShopDbContext context)
    {
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        // Check if data already exists
        if (await context.Shoes.AnyAsync())
        {
            return; // Database has been seeded
        }

        // Seed Suppliers first
        var suppliers = new List<Supplier>
        {
            new Supplier 
            { 
                Name = "Nike Wholesale Distributors",
                ContactEmail = "orders@nikewholesale.com",
                ContactPhone = "+1-555-0101",
                Address = "123 Industrial Blvd, Portland, OR 97201",
                IsActive = true
            },
            new Supplier 
            { 
                Name = "Adidas Supply Chain",
                ContactEmail = "supply@adidas.com",
                ContactPhone = "+1-555-0102",
                Address = "456 Sports Avenue, Boston, MA 02101",
                IsActive = true
            },
            new Supplier 
            { 
                Name = "Premium Athletic Footwear Co.",
                ContactEmail = "sales@premiumfootwear.com",
                ContactPhone = "+1-555-0103",
                Address = "789 Shoe Street, Los Angeles, CA 90210",
                IsActive = true
            }
        };

        await context.Suppliers.AddRangeAsync(suppliers);
        await context.SaveChangesAsync();

        // Seed Shoes (at least 15 shoes as required)
        var shoes = new List<Shoe>
        {
            // Nike Shoes
            new Shoe 
            { 
                Name = "Nike Air Max 270", 
                Brand = "Nike", 
                Cost = 65.00m, 
                Price = 130.00m,
                Description = "The Nike Air Max 270 delivers visible heel Air cushioning at its greatest volume yet.",
                IsActive = true,
                CreatedDate = DateTime.UtcNow.AddDays(-30)
            },
            new Shoe 
            { 
                Name = "Nike React Infinity Run", 
                Brand = "Nike", 
                Cost = 80.00m, 
                Price = 160.00m,
                Description = "Created to help reduce running injuries, the Nike React Infinity Run keeps you on the run.",
                IsActive = true,
                CreatedDate = DateTime.UtcNow.AddDays(-25)
            },
            new Shoe 
            { 
                Name = "Nike Air Force 1", 
                Brand = "Nike", 
                Cost = 45.00m, 
                Price = 90.00m,
                Description = "The radiance lives on in the Nike Air Force 1, the basketball original.",
                IsActive = true,
                CreatedDate = DateTime.UtcNow.AddDays(-20)
            },
            new Shoe 
            { 
                Name = "Nike Pegasus 39", 
                Brand = "Nike", 
                Cost = 60.00m, 
                Price = 130.00m,
                Description = "A responsive shoe that provides a comfortable ride for everyday runs.",
                IsActive = true,
                CreatedDate = DateTime.UtcNow.AddDays(-15)
            },
            new Shoe 
            { 
                Name = "Nike Dunk Low", 
                Brand = "Nike", 
                Cost = 50.00m, 
                Price = 110.00m,
                Description = "Created for the hardwood but taken to the streets.",
                IsActive = true,
                CreatedDate = DateTime.UtcNow.AddDays(-10)
            },

            // Adidas Shoes
            new Shoe 
            { 
                Name = "Adidas Ultraboost 22", 
                Brand = "Adidas", 
                Cost = 90.00m, 
                Price = 180.00m,
                Description = "Our most responsive running shoe, designed to give you incredible energy return.",
                IsActive = true,
                CreatedDate = DateTime.UtcNow.AddDays(-28)
            },
            new Shoe 
            { 
                Name = "Adidas Stan Smith", 
                Brand = "Adidas", 
                Cost = 40.00m, 
                Price = 80.00m,
                Description = "Clean and simple with an all-white colorway and green accents.",
                IsActive = true,
                CreatedDate = DateTime.UtcNow.AddDays(-22)
            },
            new Shoe 
            { 
                Name = "Adidas NMD R1", 
                Brand = "Adidas", 
                Cost = 65.00m, 
                Price = 130.00m,
                Description = "A modern interpretation of the iconic running aesthetic.",
                IsActive = true,
                CreatedDate = DateTime.UtcNow.AddDays(-18)
            },
            new Shoe 
            { 
                Name = "Adidas Gazelle", 
                Brand = "Adidas", 
                Cost = 42.50m, 
                Price = 85.00m,
                Description = "An authentic icon. The Gazelle started life as a training shoe.",
                IsActive = true,
                CreatedDate = DateTime.UtcNow.AddDays(-12)
            },
            new Shoe 
            { 
                Name = "Adidas Superstar", 
                Brand = "Adidas", 
                Cost = 37.50m, 
                Price = 85.00m,
                Description = "The original basketball shoe that changed the game.",
                IsActive = true,
                CreatedDate = DateTime.UtcNow.AddDays(-8)
            },

            // Other Brand Shoes
            new Shoe 
            { 
                Name = "Converse Chuck Taylor All Star", 
                Brand = "Converse", 
                Cost = 27.50m, 
                Price = 55.00m,
                Description = "The original basketball sneaker, now a timeless icon.",
                IsActive = true,
                CreatedDate = DateTime.UtcNow.AddDays(-26)
            },
            new Shoe 
            { 
                Name = "Puma RS-X", 
                Brand = "Puma", 
                Cost = 55.00m, 
                Price = 110.00m,
                Description = "A reinvention of the iconic running system.",
                IsActive = true,
                CreatedDate = DateTime.UtcNow.AddDays(-24)
            },
            new Shoe 
            { 
                Name = "New Balance 990v5", 
                Brand = "New Balance", 
                Cost = 87.50m, 
                Price = 175.00m,
                Description = "The finest in running shoe construction, made in USA.",
                IsActive = true,
                CreatedDate = DateTime.UtcNow.AddDays(-16)
            },
            new Shoe 
            { 
                Name = "Vans Old Skool", 
                Brand = "Vans", 
                Cost = 32.50m, 
                Price = 65.00m,
                Description = "The first Vans shoe to feature the iconic side stripe.",
                IsActive = true,
                CreatedDate = DateTime.UtcNow.AddDays(-14)
            },
            new Shoe 
            { 
                Name = "Reebok Classic Leather", 
                Brand = "Reebok", 
                Cost = 35.00m, 
                Price = 70.00m,
                Description = "Timeless style meets comfortable cushioning.",
                IsActive = true,
                CreatedDate = DateTime.UtcNow.AddDays(-6)
            }
        };

        await context.Shoes.AddRangeAsync(shoes);
        await context.SaveChangesAsync();

        // Seed Color Variations for each shoe
        var colorVariations = new List<ShoeColorVariation>();
        var random = new Random(42); // Fixed seed for consistent results

        foreach (var shoe in shoes)
        {
            var colors = GetRandomColors(random, 2, 4); // 2-4 colors per shoe
            foreach (var color in colors)
            {
                colorVariations.Add(new ShoeColorVariation
                {
                    ShoeId = shoe.Id,
                    ColorName = color.Name,
                    HexCode = color.HexCode,
                    StockQuantity = random.Next(0, 100), // Random stock between 0-100
                    ReorderLevel = random.Next(5, 15), // Random reorder level between 5-15
                    IsActive = true
                });
            }
        }

        await context.ShoeColorVariations.AddRangeAsync(colorVariations);
        await context.SaveChangesAsync();

        // Seed Sample Purchase Orders
        var purchaseOrders = new List<PurchaseOrder>
        {
            new PurchaseOrder
            {
                OrderNumber = "PO-2024-001",
                SupplierId = suppliers[0].Id, // Nike supplier
                OrderDate = DateTime.UtcNow.AddDays(-5),
                ExpectedDate = DateTime.UtcNow.AddDays(10),
                Status = PurchaseOrderStatus.Confirmed,
                TotalAmount = 5250.00m
            },
            new PurchaseOrder
            {
                OrderNumber = "PO-2024-002",
                SupplierId = suppliers[1].Id, // Adidas supplier
                OrderDate = DateTime.UtcNow.AddDays(-3),
                ExpectedDate = DateTime.UtcNow.AddDays(7),
                Status = PurchaseOrderStatus.Pending,
                TotalAmount = 3200.00m
            },
            new PurchaseOrder
            {
                OrderNumber = "PO-2024-003",
                SupplierId = suppliers[2].Id, // Premium supplier
                OrderDate = DateTime.UtcNow.AddDays(-10),
                ExpectedDate = DateTime.UtcNow.AddDays(-2),
                Status = PurchaseOrderStatus.Received,
                TotalAmount = 4800.00m
            }
        };

        await context.PurchaseOrders.AddRangeAsync(purchaseOrders);
        await context.SaveChangesAsync();

        // Seed Purchase Order Items
        var orderItems = new List<PurchaseOrderItem>();
        var availableVariations = colorVariations.Take(20).ToList(); // Use first 20 variations

        // Items for first order
        for (int i = 0; i < 5; i++)
        {
            var variation = availableVariations[i];
            orderItems.Add(new PurchaseOrderItem
            {
                PurchaseOrderId = purchaseOrders[0].Id,
                ShoeColorVariationId = variation.Id,
                QuantityOrdered = random.Next(10, 30),
                QuantityReceived = 0,
                UnitCost = shoes.First(s => s.Id == variation.ShoeId).Cost
            });
        }

        // Items for second order
        for (int i = 5; i < 10; i++)
        {
            var variation = availableVariations[i];
            orderItems.Add(new PurchaseOrderItem
            {
                PurchaseOrderId = purchaseOrders[1].Id,
                ShoeColorVariationId = variation.Id,
                QuantityOrdered = random.Next(15, 25),
                QuantityReceived = 0,
                UnitCost = shoes.First(s => s.Id == variation.ShoeId).Cost
            });
        }

        // Items for third order (received)
        for (int i = 10; i < 15; i++)
        {
            var variation = availableVariations[i];
            var ordered = random.Next(20, 40);
            orderItems.Add(new PurchaseOrderItem
            {
                PurchaseOrderId = purchaseOrders[2].Id,
                ShoeColorVariationId = variation.Id,
                QuantityOrdered = ordered,
                QuantityReceived = ordered, // Fully received
                UnitCost = shoes.First(s => s.Id == variation.ShoeId).Cost
            });
        }

        await context.PurchaseOrderItems.AddRangeAsync(orderItems);
        await context.SaveChangesAsync();

        // Seed Sample Stock Pull-outs
        var pullOuts = new List<StockPullOut>
        {
            new StockPullOut
            {
                ShoeColorVariationId = availableVariations[0].Id,
                Quantity = 3,
                Reason = "Damaged",
                ReasonDetails = "Water damage during storage",
                RequestedBy = "John Doe",
                ApprovedBy = "Jane Manager",
                PullOutDate = DateTime.UtcNow.AddDays(-2),
                Status = StockPullOutStatus.Completed
            },
            new StockPullOut
            {
                ShoeColorVariationId = availableVariations[1].Id,
                Quantity = 5,
                Reason = "Customer Return",
                ReasonDetails = "Defective manufacturing",
                RequestedBy = "Mike Smith",
                PullOutDate = DateTime.UtcNow.AddDays(-1),
                Status = StockPullOutStatus.Pending
            },
            new StockPullOut
            {
                ShoeColorVariationId = availableVariations[2].Id,
                Quantity = 2,
                Reason = "Promotional",
                ReasonDetails = "Sample for marketing event",
                RequestedBy = "Sarah Jones",
                ApprovedBy = "Jane Manager",
                PullOutDate = DateTime.UtcNow.AddHours(-12),
                Status = StockPullOutStatus.Approved
            }
        };

        await context.StockPullOuts.AddRangeAsync(pullOuts);
        await context.SaveChangesAsync();
    }

    private static List<(string Name, string HexCode)> GetRandomColors(Random random, int min, int max)
    {
        var allColors = new List<(string Name, string HexCode)>
        {
            ("Black", "#000000"),
            ("White", "#FFFFFF"),
            ("Red", "#FF0000"),
            ("Blue", "#0066CC"),
            ("Green", "#00CC66"),
            ("Yellow", "#FFD700"),
            ("Orange", "#FF8C00"),
            ("Purple", "#800080"),
            ("Pink", "#FF69B4"),
            ("Brown", "#8B4513"),
            ("Gray", "#808080"),
            ("Navy", "#000080"),
            ("Maroon", "#800000"),
            ("Teal", "#008080"),
            ("Silver", "#C0C0C0")
        };

        var count = random.Next(min, max + 1);
        var selectedColors = new List<(string Name, string HexCode)>();
        var availableColors = new List<(string Name, string HexCode)>(allColors);

        for (int i = 0; i < count && availableColors.Any(); i++)
        {
            var index = random.Next(availableColors.Count);
            selectedColors.Add(availableColors[index]);
            availableColors.RemoveAt(index);
        }

        return selectedColors;
    }
}
