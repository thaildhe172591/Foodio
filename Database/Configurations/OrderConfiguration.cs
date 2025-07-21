using FoodioAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodioAPI.Database.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId)
               .IsRequired(false);

        builder.Property(x => x.Total)
               .IsRequired()
               .HasColumnType("decimal(10,2)");

        builder.Property(x => x.CreatedAt)
               .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasOne(x => x.User)
               .WithMany(u => u.Orders)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.DiningTable)
               .WithMany(t => t.Orders)
               .HasForeignKey(x => x.TableId)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.OrderType)
               .WithMany(ot => ot.Orders)
               .HasForeignKey(x => x.OrderTypeId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Status)
               .WithMany(s => s.Orders)
               .HasForeignKey(x => x.StatusId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.OrderItems)
               .WithOne(oi => oi.Order)
               .HasForeignKey(oi => oi.OrderId);

        builder.HasMany(x => x.Deliveries)
               .WithOne(d => d.Order)
               .HasForeignKey(d => d.OrderId);

        builder.HasMany(x => x.OrderShippers)
               .WithOne(os => os.Order)
               .HasForeignKey(os => os.OrderId);
    }
}
