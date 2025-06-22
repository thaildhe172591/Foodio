namespace FoodioAPI.DTOs.Auth;
    public class LockUserDTO
    {
        public string UserName { get; set; } = string.Empty;
        public int LockoutDays { get; set; }
    }

