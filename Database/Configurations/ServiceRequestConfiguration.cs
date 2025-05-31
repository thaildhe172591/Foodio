using FoodioAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodioAPI.Database.Configurations;

public class ServiceRequestConfiguration : IEntityTypeConfiguration<ServiceRequest>
{
    public void Configure(EntityTypeBuilder<ServiceRequest> builder)
    {
        builder.ToTable("ServiceRequests");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Type)
               .IsRequired()
               .HasMaxLength(50);

        builder.Property(x => x.Status)
               .IsRequired()
               .HasMaxLength(50);

        builder.Property(x => x.CreatedAt)
               .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasOne(x => x.DiningTable)
               .WithMany(x => x.ServiceRequests)
               .HasForeignKey(x => x.TableId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
