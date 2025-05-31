using FoodioAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodioAPI.Database.Configurations;

public class OrderShipperConfiguration : IEntityTypeConfiguration<OrderShipper>
{
    public void Configure(EntityTypeBuilder<OrderShipper> builder)
    {
        builder.ToTable("OrderShippers");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.AssignedAt)
               .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasOne(x => x.Order)
               .WithMany(x => x.OrderShippers)
               .HasForeignKey(x => x.OrderId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Shipper)
               .WithMany(x => x.OrderShippers)
               .HasForeignKey(x => x.ShipperId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
