using System.ComponentModel.DataAnnotations;

namespace FoodioAPI.Entities;

public class DiningTable
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Table number is required.")]
    public int TableNumber { get; set; }

    [Required(ErrorMessage = "Table status is required.")]
    [MaxLength(50, ErrorMessage = "Status cannot exceed 50 characters.")]
    public string Status { get; set; } = default!;

    // Navigation
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public virtual ICollection<TableOrder> TableOrders { get; set; } = new List<TableOrder>();
    public virtual ICollection<OrderSession> OrderSessions { get; set; } = new List<OrderSession>();
    public virtual ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();
}

