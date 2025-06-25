namespace FoodioAPI.Constants;

public static class UserRole
{
    public const string Admin = "Admin";
    public const string Cashier = "Cashier";
    public const string Kitchen = "Kitchen";
    public const string Shipper = "Shipper";
    public const string Customer = "Customer";
    public static readonly string[] All = [Admin, Customer, Cashier, Kitchen, Shipper];
}
