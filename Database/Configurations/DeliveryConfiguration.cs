using FoodioAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodioAPI.Database.Configurations;

public class DeliveryConfiguration : IEntityTypeConfiguration<Delivery>
{
    public void Configure(EntityTypeBuilder<Delivery> builder)
    {
        builder.ToTable("Deliveries");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Fee)
               .HasColumnType("decimal(10,2)")
               .IsRequired();

        builder.HasOne(x => x.Order)
               .WithMany(x => x.Deliveries)
               .HasForeignKey(x => x.OrderId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Shipper)
               .WithMany(x => x.ShippedDeliveries)
               .HasForeignKey(x => x.ShipperId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.DeliveryStatus)
               .WithMany(x => x.Deliveries)
               .HasForeignKey(x => x.StatusId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
