using Microsoft.EntityFrameworkCore;
using ShoeShop.Repository.Entities;

namespace ShoeShop.Repository;

public class ShoeShopDbContext : DbContext
{
    public ShoeShopDbContext(DbContextOptions<ShoeShopDbContext> options) : base(options)
    {
    }

    // DbSets for all entities
    public DbSet<Shoe> Shoes { get; set; }
    public DbSet<ShoeColorVariation> ShoeColorVariations { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
    public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }
    public DbSet<StockPullOut> StockPullOuts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Shoe entity
        modelBuilder.Entity<Shoe>(entity =>
        {
            entity.ToTable("Shoes");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Brand).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Cost).HasColumnType("decimal(10,2)");
            entity.Property(e => e.Price).HasColumnType("decimal(10,2)");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.ImageUrl).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("datetime('now')");

            // Index for performance
            entity.HasIndex(e => e.Brand).HasDatabaseName("IX_Shoes_Brand");
            entity.HasIndex(e => e.IsActive).HasDatabaseName("IX_Shoes_IsActive");
        });

        // Configure ShoeColorVariation entity
        modelBuilder.Entity<ShoeColorVariation>(entity =>
        {
            entity.ToTable("ShoeColorVariations");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ColorName).IsRequired().HasMaxLength(30);
            entity.Property(e => e.HexCode).HasMaxLength(7);
            entity.Property(e => e.StockQuantity).HasDefaultValue(0);
            entity.Property(e => e.ReorderLevel).HasDefaultValue(5);
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            // Foreign key relationship
            entity.HasOne(e => e.Shoe)
                  .WithMany(s => s.ColorVariations)
                  .HasForeignKey(e => e.ShoeId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Unique constraint for shoe-color combination
            entity.HasIndex(e => new { e.ShoeId, e.ColorName })
                  .IsUnique()
                  .HasDatabaseName("IX_ShoeColorVariations_ShoeId_ColorName");

            // Index for low stock queries
            entity.HasIndex(e => new { e.StockQuantity, e.ReorderLevel })
                  .HasDatabaseName("IX_ShoeColorVariations_Stock_Reorder");
        });

        // Configure Supplier entity
        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.ToTable("Suppliers");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ContactEmail).HasMaxLength(100);
            entity.Property(e => e.ContactPhone).HasMaxLength(20);
            entity.Property(e => e.Address).HasMaxLength(300);
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            // Index for performance
            entity.HasIndex(e => e.Name).HasDatabaseName("IX_Suppliers_Name");
        });

        // Configure PurchaseOrder entity
        modelBuilder.Entity<PurchaseOrder>(entity =>
        {
            entity.ToTable("PurchaseOrders");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(10,2)").HasDefaultValue(0);
            entity.Property(e => e.OrderDate).HasDefaultValueSql("datetime('now')");
            entity.Property(e => e.Status).HasConversion<int>();

            // Foreign key relationship
            entity.HasOne(e => e.Supplier)
                  .WithMany(s => s.PurchaseOrders)
                  .HasForeignKey(e => e.SupplierId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Unique constraint for order number
            entity.HasIndex(e => e.OrderNumber)
                  .IsUnique()
                  .HasDatabaseName("IX_PurchaseOrders_OrderNumber");

            // Index for status queries
            entity.HasIndex(e => e.Status).HasDatabaseName("IX_PurchaseOrders_Status");
        });

        // Configure PurchaseOrderItem entity
        modelBuilder.Entity<PurchaseOrderItem>(entity =>
        {
            entity.ToTable("PurchaseOrderItems");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.QuantityReceived).HasDefaultValue(0);
            entity.Property(e => e.UnitCost).HasColumnType("decimal(10,2)");

            // Foreign key relationships
            entity.HasOne(e => e.PurchaseOrder)
                  .WithMany(po => po.PurchaseOrderItems)
                  .HasForeignKey(e => e.PurchaseOrderId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.ShoeColorVariation)
                  .WithMany(scv => scv.PurchaseOrderItems)
                  .HasForeignKey(e => e.ShoeColorVariationId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Unique constraint to prevent duplicate items in same order
            entity.HasIndex(e => new { e.PurchaseOrderId, e.ShoeColorVariationId })
                  .IsUnique()
                  .HasDatabaseName("IX_PurchaseOrderItems_Order_ShoeColor");
        });

        // Configure StockPullOut entity
        modelBuilder.Entity<StockPullOut>(entity =>
        {
            entity.ToTable("StockPullOuts");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Reason).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ReasonDetails).HasMaxLength(500);
            entity.Property(e => e.RequestedBy).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ApprovedBy).HasMaxLength(100);
            entity.Property(e => e.PullOutDate).HasDefaultValueSql("datetime('now')");
            entity.Property(e => e.Status).HasConversion<int>();

            // Foreign key relationship
            entity.HasOne(e => e.ShoeColorVariation)
                  .WithMany(scv => scv.StockPullOuts)
                  .HasForeignKey(e => e.ShoeColorVariationId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Index for status and date queries
            entity.HasIndex(e => e.Status).HasDatabaseName("IX_StockPullOuts_Status");
            entity.HasIndex(e => e.PullOutDate).HasDatabaseName("IX_StockPullOuts_Date");
        });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // This method can be used for additional configuration if needed
        // Connection string should be provided through dependency injection
        if (!optionsBuilder.IsConfigured)
        {
            // Default to SQLite for development if no options provided
            optionsBuilder.UseSqlite("Data Source=shoeshop.db");
        }
    }
}
