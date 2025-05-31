using FoodioAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodioAPI.Database.Configurations;

public class OrderItemStatusConfiguration : IEntityTypeConfiguration<OrderItemStatus>
{
    public void Configure(EntityTypeBuilder<OrderItemStatus> builder)
    {
        builder.ToTable("OrderItemStatuses");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
               .IsRequired()
               .HasMaxLength(50);

        builder.Property(x => x.Name)
               .IsRequired()
               .HasMaxLength(100);

        builder.HasIndex(x => x.Code).IsUnique();
    }
}
