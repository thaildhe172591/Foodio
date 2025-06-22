using Microsoft.AspNetCore.Identity;

namespace FoodioAPI.Entities;

public class User : IdentityUser
{
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpiryTime { get; set; } = DateTime.UtcNow;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    public virtual ICollection<Delivery> ShippedDeliveries { get; set; } = new List<Delivery>();
    public virtual ICollection<OrderShipper> OrderShippers { get; set; } = new List<OrderShipper>();
    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();
    public virtual ICollection<Shift> Shifts { get; set; } = new List<Shift>();
}
