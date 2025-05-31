using FoodioAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodioAPI.Database.Configurations;

public class OrderSessionConfiguration : IEntityTypeConfiguration<OrderSession>
{
    public void Configure(EntityTypeBuilder<OrderSession> builder)
    {
        builder.ToTable("OrderSessions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Token)
               .HasMaxLength(255)
               .IsRequired();

        builder.Property(x => x.IsActive)
               .HasDefaultValue(true);

        builder.HasOne(x => x.DiningTable)
               .WithMany(x => x.OrderSessions)
               .HasForeignKey(x => x.TableId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
