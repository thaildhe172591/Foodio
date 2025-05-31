using FoodioAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodioAPI.Database.Configurations;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable("CartItems");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Note)
               .HasMaxLength(255);

        builder.HasOne(x => x.Cart)
               .WithMany(x => x.CartItems)
               .HasForeignKey(x => x.CartId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.MenuItem)
               .WithMany(x => x.CartItems)
               .HasForeignKey(x => x.MenuItemId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
