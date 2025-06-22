using System.ComponentModel.DataAnnotations;

namespace FoodioAPI.DTOs.UserDtos
{
    /// <summary>
    /// DTO cho việc tìm kiếm và lọc danh sách người dùng
    /// </summary>
    public class UserSearchDto
    {
        /// <summary>
        /// Số trang (mặc định: 1)
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "Số trang phải lớn hơn 0")]
        public int Page { get; set; } = 1;

        /// <summary>
        /// Số lượng item trên mỗi trang (mặc định: 10, tối đa: 100)
        /// </summary>
        [Range(1, 100, ErrorMessage = "Số lượng item trên trang phải từ 1-100")]
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// Từ khóa tìm kiếm theo tên người dùng hoặc email
        /// </summary>
        [StringLength(100, ErrorMessage = "Từ khóa tìm kiếm không được quá 100 ký tự")]
        public string? SearchKeyword { get; set; }

        /// <summary>
        /// Email cụ thể để tìm kiếm
        /// </summary>
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string? Email { get; set; }

        /// <summary>
        /// Tên người dùng cụ thể để tìm kiếm
        /// </summary>
        [StringLength(256, ErrorMessage = "Tên người dùng không được quá 256 ký tự")]
        public string? UserName { get; set; }

        /// <summary>
        /// ID của vai trò để lọc người dùng theo vai trò
        /// </summary>
        public string? RoleId { get; set; }

        /// <summary>
        /// Tên vai trò để lọc người dùng theo vai trò
        /// </summary>
        [StringLength(100, ErrorMessage = "Tên vai trò không được quá 100 ký tự")]
        public string? RoleName { get; set; }

        /// <summary>
        /// Lọc theo trạng thái xác nhận email
        /// null: không lọc, true: đã xác nhận, false: chưa xác nhận
        /// </summary>
        public bool? EmailConfirmed { get; set; }

        /// <summary>
        /// Lọc theo trạng thái khóa tài khoản
        /// null: không lọc, true: đang bị khóa, false: không bị khóa
        /// </summary>
        public bool? IsLocked { get; set; }

        /// <summary>
        /// Lọc theo trạng thái xác nhận số điện thoại
        /// null: không lọc, true: đã xác nhận, false: chưa xác nhận
        /// </summary>
        public bool? PhoneNumberConfirmed { get; set; }

        /// <summary>
        /// Sắp xếp theo trường (mặc định: UserName)
        /// Các giá trị: UserName, Email
        /// </summary>
        [RegularExpression("^(UserName|Email)$", ErrorMessage = "Trường sắp xếp không hợp lệ")]
        public string SortBy { get; set; } = "UserName";

        /// <summary>
        /// Thứ tự sắp xếp (mặc định: ASC)
        /// ASC: tăng dần, DESC: giảm dần
        /// </summary>
        [RegularExpression("^(ASC|DESC)$", ErrorMessage = "Thứ tự sắp xếp phải là ASC hoặc DESC")]
        public string SortOrder { get; set; } = "ASC";
    }
} 