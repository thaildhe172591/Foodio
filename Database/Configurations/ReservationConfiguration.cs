using FoodioAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodioAPI.Database.Configurations;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("Reservations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Status)
               .IsRequired()
               .HasMaxLength(50);

        builder.Property(x => x.Time)
               .IsRequired();

        builder.HasOne(x => x.User)
               .WithMany(u => u.Reservations)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.DiningTable)
               .WithMany(t => t.Reservations)
               .HasForeignKey(x => x.DiningTableId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
