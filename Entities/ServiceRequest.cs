using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace FoodioAPI.Entities;

public class ServiceRequest
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "TableId is required.")]
    public Guid TableId { get; set; }

    [Required(ErrorMessage = "Type is required.")]
    [MaxLength(50, ErrorMessage = "Type cannot exceed 50 characters.")]
    public string Type { get; set; } = default!;

    [Required(ErrorMessage = "Status is required.")]
    [MaxLength(50, ErrorMessage = "Status cannot exceed 50 characters.")]
    public string Status { get; set; } = default!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public virtual DiningTable DiningTable { get; set; } = default!;
}
