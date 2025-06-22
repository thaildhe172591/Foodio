namespace FoodioAPI.DTOs.UserDtos
{
    public class UserDto
    {
        public string Id { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? PhoneNumber { get; set; }
        public DateTime? CreatedDate { get; set; }
        public List<string> Roles { get; set; } = new();
        public string Role { get; set; } = default!;
        public bool IsLocked { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool EmailConfirmed { get; set; }
    }
}
