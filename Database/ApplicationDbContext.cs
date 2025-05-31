using System.Reflection;
using FoodioAPI.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FoodioAPI.Database;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<User>(options)
{
    public virtual DbSet<User> User { get; set; }
    public virtual DbSet<MenuItem> MenuItems { get; set; }
    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<DiningTable> DiningTables { get; set; }
    public virtual DbSet<Reservation> Reservations { get; set; }
    public virtual DbSet<Order> Orders { get; set; }
    public virtual DbSet<OrderItem> OrderItems { get; set; }
    public virtual DbSet<OrderType> OrderTypes { get; set; }
    public virtual DbSet<OrderDeliveryInfo> OrderDeliveryInfos { get; set; }
    public virtual DbSet<Delivery> Deliveries { get; set; }
    public virtual DbSet<OrderShipper> OrderShippers { get; set; }
    public virtual DbSet<TableOrder> TableOrders { get; set; }
    public virtual DbSet<Cart> Carts { get; set; }
    public virtual DbSet<CartItem> CartItems { get; set; }
    public virtual DbSet<OrderSession> OrderSessions { get; set; }
    public virtual DbSet<ServiceRequest> ServiceRequests { get; set; }
    public virtual DbSet<Shift> Shifts { get; set; }
    public virtual DbSet<OrderItemStatusHistory> OrderItemStatusHistories { get; set; }

    // Lookup tables
    public virtual DbSet<OrderStatus> OrderStatuses { get; set; }
    public virtual DbSet<OrderItemStatus> OrderItemStatuses { get; set; }
    public virtual DbSet<DeliveryStatus> DeliveryStatuses { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("dbo");
        builder.Entity<IdentityRole>().ToTable("Roles");
        builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
        builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
        builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
        builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
        builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}