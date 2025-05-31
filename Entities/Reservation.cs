using System.ComponentModel.DataAnnotations;
using FoodioAPI.Entities;

namespace FoodioAPI.Entities;

public class Reservation
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "UserId is required.")]
    public string UserId { get; set; } = default!;

    [Required(ErrorMessage = "DiningTableId is required.")]
    public Guid DiningTableId { get; set; }

    [Required(ErrorMessage = "Reservation time is required.")]
    public DateTime Time { get; set; }

    [Required(ErrorMessage = "Reservation status is required.")]
    [MaxLength(50, ErrorMessage = "Status cannot exceed 50 characters.")]
    public string Status { get; set; } = default!;

    // Navigation
    public virtual User User { get; set; } = default!;
    public virtual DiningTable DiningTable { get; set; } = default!;
}
