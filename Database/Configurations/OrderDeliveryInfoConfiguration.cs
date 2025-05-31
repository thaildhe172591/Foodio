using FoodioAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodioAPI.Database.Configurations;

public class OrderDeliveryInfoConfiguration : IEntityTypeConfiguration<OrderDeliveryInfo>
{
    public void Configure(EntityTypeBuilder<OrderDeliveryInfo> builder)
    {
        builder.ToTable("OrderDeliveryInfo");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ReceiverName)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(x => x.ReceiverPhone)
               .IsRequired()
               .HasMaxLength(20);

        builder.Property(x => x.DeliveryAddress)
               .IsRequired()
               .HasMaxLength(255);

        builder.HasOne(x => x.Order)
               .WithOne(o => o.DeliveryInfo)
               .HasForeignKey<OrderDeliveryInfo>(x => x.OrderId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
