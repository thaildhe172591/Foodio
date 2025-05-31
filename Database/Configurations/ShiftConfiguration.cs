using FoodioAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodioAPI.Database.Configurations;

public class ShiftConfiguration : IEntityTypeConfiguration<Shift>
{
    public void Configure(EntityTypeBuilder<Shift> builder)
    {
        builder.ToTable("Shifts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Role)
               .IsRequired()
               .HasMaxLength(50);

        builder.HasOne(x => x.User)
               .WithMany(u => u.Shifts)
               .HasForeignKey(x => x.UserId)
               .IsRequired();
    }
}
