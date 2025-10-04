# 🗄️ Repository Layer Documentation
**Student A Assignment - Database Layer Implementation**

---

## 📋 Overview

The Repository Layer is the foundation of the ShoeShop Inventory Management System, responsible for data access, entity management, and database operations. This layer implements the complete database schema with proper relationships, constraints, and data integrity measures.

## 🏗️ Architecture

The repository layer follows Entity Framework Core best practices with:
- **Entity Models** with EF Core annotations (NOT validation annotations)
- **DbContext** with Fluent API configuration
- **Database Migrations** for schema management
- **Seed Data** for development and testing
- **Design-time Factory** for migration support

---

## 📊 Database Schema

### Entity Relationship Diagram (Conceptual)

```
┌─────────────┐    1:N    ┌──────────────────┐    N:1    ┌────────────────┐
│   Suppliers │◄─────────►│ PurchaseOrders   │◄─────────►│PurchaseOrderItem│
└─────────────┘           └──────────────────┘           └────────────────┘
                                                                   │ N:1
                                                                   ▼
┌─────────────┐    1:N    ┌──────────────────┐                ┌────────────────┐
│    Shoes    │◄─────────►│ShoeColorVariation│◄───────────────┤ShoeColorVariation│
└─────────────┘           └──────────────────┘    1:N         └────────────────┘
                                   │                                   ▲
                                   │ 1:N                               │ N:1
                                   ▼                                   │
                           ┌──────────────────┐                       │
                           │  StockPullOuts   │───────────────────────┘
                           └──────────────────┘
```

---

## 🏷️ Entity Models

### 1. Shoe Entity
**Purpose**: Core shoe product information
```csharp
public class Shoe
{
    public int Id { get; set; }                    // Primary Key
    public string Name { get; set; }               // Product name (max 100 chars)
    public string Brand { get; set; }              // Brand name (max 50 chars)  
    public decimal Cost { get; set; }              // Purchase cost from supplier
    public decimal Price { get; set; }             // Selling price
    public string? Description { get; set; }       // Product description (max 500 chars)
    public string? ImageUrl { get; set; }          // Image URL (max 255 chars)
    public bool IsActive { get; set; }             // Active status (default: true)
    public DateTime CreatedDate { get; set; }      // Creation timestamp
    
    // Navigation Properties
    public virtual ICollection<ShoeColorVariation> ColorVariations { get; set; }
}
```

**Key Features**:
- Indexed on `Brand` and `IsActive` for performance
- Decimal precision (10,2) for monetary values
- Cascade delete with color variations

### 2. ShoeColorVariation Entity
**Purpose**: Color variations of shoes with stock tracking
```csharp
public class ShoeColorVariation
{
    public int Id { get; set; }                    // Primary Key
    public int ShoeId { get; set; }                // Foreign Key to Shoe
    public string ColorName { get; set; }          // Color name (max 30 chars)
    public string? HexCode { get; set; }           // Color hex code (max 7 chars)
    public int StockQuantity { get; set; }         // Current stock (default: 0)
    public int ReorderLevel { get; set; }          // Reorder threshold (default: 5)
    public bool IsActive { get; set; }             // Active status (default: true)
    
    // Navigation Properties
    public virtual Shoe Shoe { get; set; }
    public virtual ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; }
    public virtual ICollection<StockPullOut> StockPullOuts { get; set; }
}
```

**Key Features**:
- Unique constraint on (ShoeId, ColorName) combination
- Indexed for low stock queries (StockQuantity, ReorderLevel)
- Stock management with reorder levels

### 3. Supplier Entity
**Purpose**: Supplier contact and company information
```csharp
public class Supplier
{
    public int Id { get; set; }                    // Primary Key
    public string Name { get; set; }               // Company name (max 100 chars)
    public string? ContactEmail { get; set; }      // Contact email (max 100 chars)
    public string? ContactPhone { get; set; }      // Phone number (max 20 chars)
    public string? Address { get; set; }           // Address (max 300 chars)
    public bool IsActive { get; set; }             // Active status (default: true)
    
    // Navigation Properties
    public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; }
}
```

**Key Features**:
- Indexed on `Name` for quick lookups
- Optional contact information

### 4. PurchaseOrder Entity
**Purpose**: Purchase orders for restocking inventory
```csharp
public class PurchaseOrder
{
    public int Id { get; set; }                    // Primary Key
    public string OrderNumber { get; set; }        // Unique order identifier (max 50 chars)
    public int SupplierId { get; set; }            // Foreign Key to Supplier
    public DateTime OrderDate { get; set; }        // Order creation date
    public DateTime? ExpectedDate { get; set; }    // Expected delivery date
    public PurchaseOrderStatus Status { get; set; } // Order status (enum)
    public decimal TotalAmount { get; set; }       // Total order value
    
    // Navigation Properties
    public virtual Supplier Supplier { get; set; }
    public virtual ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; }
}

public enum PurchaseOrderStatus
{
    Pending = 0,
    Confirmed = 1, 
    Shipped = 2,
    Received = 3,
    Cancelled = 4
}
```

**Key Features**:
- Unique constraint on `OrderNumber`
- Indexed on `Status` for filtering
- Restrict delete to preserve history

### 5. PurchaseOrderItem Entity
**Purpose**: Individual items within purchase orders
```csharp
public class PurchaseOrderItem
{
    public int Id { get; set; }                    // Primary Key
    public int PurchaseOrderId { get; set; }       // Foreign Key to PurchaseOrder
    public int ShoeColorVariationId { get; set; }  // Foreign Key to ShoeColorVariation
    public int QuantityOrdered { get; set; }       // Quantity ordered
    public int QuantityReceived { get; set; }      // Quantity received (default: 0)
    public decimal UnitCost { get; set; }          // Cost per unit
    
    // Navigation Properties
    public virtual PurchaseOrder PurchaseOrder { get; set; }
    public virtual ShoeColorVariation ShoeColorVariation { get; set; }
}
```

**Key Features**:
- Unique constraint on (PurchaseOrderId, ShoeColorVariationId)
- Cascade delete with purchase orders
- Restrict delete with shoe color variations

### 6. StockPullOut Entity
**Purpose**: Track inventory removals with audit trail
```csharp
public class StockPullOut
{
    public int Id { get; set; }                    // Primary Key
    public int ShoeColorVariationId { get; set; }  // Foreign Key to ShoeColorVariation
    public int Quantity { get; set; }              // Quantity being removed
    public string Reason { get; set; }             // Pull-out reason (max 100 chars)
    public string? ReasonDetails { get; set; }     // Additional details (max 500 chars)
    public string RequestedBy { get; set; }        // Requester name (max 100 chars)
    public string? ApprovedBy { get; set; }        // Approver name (max 100 chars)
    public DateTime PullOutDate { get; set; }      // Pull-out timestamp
    public StockPullOutStatus Status { get; set; } // Status (enum)
    
    // Navigation Properties
    public virtual ShoeColorVariation ShoeColorVariation { get; set; }
}

public enum StockPullOutStatus
{
    Pending = 0,
    Approved = 1,
    Completed = 2,
    Rejected = 3
}
```

**Key Features**:
- Indexed on `Status` and `PullOutDate` for reporting
- Audit trail with requester and approver information
- Restrict delete to preserve history

---

## 🔧 DbContext Configuration

### Connection Strings
- **Development**: SQLite (`shoeshop.db`)
- **Production**: SQL Server (configurable via dependency injection)

### Fluent API Configuration
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Entity configurations with:
    // - Table names
    // - Key definitions
    // - Property constraints (max length, data types)
    // - Default values
    // - Indexes for performance
    // - Unique constraints
    // - Foreign key relationships
    // - Cascade/restrict delete behaviors
}
```

### Design-time Factory
Located in `Design/ShoeShopDbContextFactory.cs` to support EF Core migrations without dependency injection.

---

## 📈 Database Migrations

### Initial Migration
Creates all tables with proper relationships and constraints.

**Command used**:
```bash
dotnet ef migrations add InitialCreate --project src/ShoeShop.Repository
```

### Migration Files
- `Migrations/[timestamp]_InitialCreate.cs` - Migration code
- `Migrations/[timestamp]_InitialCreate.Designer.cs` - Migration metadata  
- `Migrations/ShoeShopDbContextModelSnapshot.cs` - Current model snapshot

---

## 🌱 Seed Data

### DatabaseSeeder Class
Located in `Data/DatabaseSeeder.cs`, provides comprehensive test data:

**Suppliers (3)**:
- Nike Wholesale Distributors
- Adidas Supply Chain  
- Premium Athletic Footwear Co.

**Shoes (15+ products)**:
- 5 Nike shoes (Air Max 270, React Infinity Run, Air Force 1, Pegasus 39, Dunk Low)
- 5 Adidas shoes (Ultraboost 22, Stan Smith, NMD R1, Gazelle, Superstar)
- 5+ Other brands (Converse, Puma, New Balance, Vans, Reebok)

**Color Variations**: 2-4 colors per shoe with realistic stock quantities

**Purchase Orders**: Sample orders in various statuses (Pending, Confirmed, Received)

**Stock Pull-outs**: Sample pull-out requests with different reasons

---

## 🧪 Testing Console Application

### Test Coverage
The console application (`tests/ShoeShop.Repository.Console`) provides comprehensive testing:

#### 1. Basic CRUD Operations
- Create, Read, Update operations on all entities
- Data persistence verification

#### 2. Relationships & Navigation Properties
- Shoe ↔ ColorVariations navigation
- PurchaseOrder ↔ Supplier navigation
- Complex navigation with ThenInclude
- Deep relationship traversal testing

#### 3. Complex Queries
- **Low Stock Items**: Items below reorder level
- **Brand Grouping**: Shoes grouped by brand with statistics
- **Pending Orders**: Orders awaiting processing

#### 4. Data Integrity Testing
- **Unique Constraints**: Duplicate prevention
- **Foreign Key Constraints**: Invalid reference rejection
- **Cascade Delete**: Automatic child record deletion

#### 5. Real-world Inventory Scenarios
- **Purchase Order Processing**: Receiving inventory and updating stock
- **Stock Pull-out Requests**: Creating removal requests
- **Inventory Reporting**: Summary by brand with metrics

### Running Tests
```bash
dotnet run --project tests/ShoeShop.Repository.Console
```

---

## 🔍 Key Features & Benefits

### Performance Optimizations
- **Strategic Indexing**: On frequently queried columns (Brand, Status, Dates)
- **Compound Indexes**: For complex queries (ShoeId + ColorName)
- **Lazy Loading**: Virtual navigation properties for efficient data loading

### Data Integrity
- **Unique Constraints**: Prevent duplicate shoe-color combinations
- **Foreign Key Constraints**: Maintain referential integrity
- **Cascade/Restrict Deletes**: Preserve historical data while allowing cleanup

### Business Logic Support
- **Stock Management**: Current quantities and reorder levels
- **Order Tracking**: Complete purchase order lifecycle
- **Audit Trail**: Full tracking of stock movements and changes

### Developer Experience
- **Comprehensive Seeding**: Rich test data for development
- **Robust Testing**: Extensive test suite covering all scenarios
- **Clear Documentation**: Well-documented entities and relationships

---

## 📁 File Structure

```
src/ShoeShop.Repository/
├── Entities/
│   ├── Shoe.cs
│   ├── ShoeColorVariation.cs
│   ├── Supplier.cs
│   ├── PurchaseOrder.cs
│   ├── PurchaseOrderItem.cs
│   ├── StockPullOut.cs
│   ├── PurchaseOrderStatus.cs
│   └── StockPullOutStatus.cs
├── Data/
│   └── DatabaseSeeder.cs
├── Design/
│   └── ShoeShopDbContextFactory.cs
├── Migrations/
│   ├── [timestamp]_InitialCreate.cs
│   ├── [timestamp]_InitialCreate.Designer.cs
│   └── ShoeShopDbContextModelSnapshot.cs
├── ShoeShopDbContext.cs
└── ShoeShop.Repository.csproj

tests/ShoeShop.Repository.Console/
├── Program.cs
└── ShoeShop.Repository.Console.csproj
```

---

## ✅ Assignment Deliverables Completed

- [x] **Entity Models** with proper EF Core annotations (NOT validation annotations)
- [x] **DbContext** with Fluent API configuration and relationships
- [x] **Migration Scripts** with comprehensive seed data (15+ shoes, 3 suppliers)
- [x] **Console Application** demonstrating all database operations
- [x] **Documentation** of database schema and relationships

---

## 🚀 Next Steps for Team Integration

1. **Service Layer (Student B)** can reference this repository layer
2. **UI Layer (Student C)** can design based on the entity structure  
3. **Controllers (Student D)** can integrate with both repository and service layers
4. **Team Integration** through proper Git workflow and pull requests

The Repository Layer provides a solid, well-tested foundation for the entire ShoeShop Inventory Management System.

---

**Implementation by**: Student A  
**Date**: October 2025  
**Status**: ✅ Complete and Ready for Integration
