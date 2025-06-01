using FoodioAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodioAPI.Database.Configurations;

public class OrderItemStatusHistoryConfiguration : IEntityTypeConfiguration<OrderItemStatusHistory>
{
    public void Configure(EntityTypeBuilder<OrderItemStatusHistory> builder)
    {
        builder.ToTable("OrderItemStatusHistory");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ChangedAt)
               .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasOne(x => x.OrderItem)
               .WithMany(x => x.StatusHistories)
               .HasForeignKey(x => x.OrderItemId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.OrderItemStatus)
               .WithMany(x => x.OrderItemStatusHistory)
               .HasForeignKey(x => x.StatusId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
