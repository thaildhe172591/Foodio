using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace FoodioAPI.Entities;

public class OrderSession
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "TableId is required.")]
    public Guid TableId { get; set; }

    [Required(ErrorMessage = "Token is required.")]
    [MaxLength(255, ErrorMessage = "Token cannot exceed 255 characters.")]
    public string Token { get; set; } = default!;

    public DateTime ExpireAt { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation property
    public virtual DiningTable DiningTable { get; set; } = default!;
}
