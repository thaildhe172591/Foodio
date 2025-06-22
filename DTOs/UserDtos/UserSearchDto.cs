namespace FoodioAPI.DTOs.UserDtos
{
    public class UserSearchDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public string? RoleFilter { get; set; }
        public bool? IsLockedFilter { get; set; }
    }
} 