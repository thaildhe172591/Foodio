using FoodioAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodioAPI.Database.Configurations;

public class TableOrderConfiguration : IEntityTypeConfiguration<TableOrder>
{
    public void Configure(EntityTypeBuilder<TableOrder> builder)
    {
        builder.ToTable("TableOrders");

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.DiningTable)
               .WithMany(x => x.TableOrders)
               .HasForeignKey(x => x.TableId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Order)
               .WithMany(x => x.TableOrders)
               .HasForeignKey(x => x.OrderId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
