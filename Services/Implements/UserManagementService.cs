using FoodioAPI.DTOs.UserDtos;
using FoodioAPI.DTOs;
using FoodioAPI.Entities;
using FoodioAPI.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FoodioAPI.Services.Implements;

/// <summary>
/// Service quản lý người dùng cho Admin
/// Cung cấp các chức năng quản lý danh sách người dùng, phân quyền, khóa/mở khóa tài khoản
/// </summary>
public class UserManagementService : IUserManagementService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserManagementService(
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    /// <summary>
    /// Lấy danh sách người dùng với phân trang và tìm kiếm
    /// </summary>
    /// <param name="page">Số trang (bắt đầu từ 1)</param>
    /// <param name="pageSize">Số lượng người dùng trên mỗi trang</param>
    /// <param name="searchTerm">Từ khóa tìm kiếm theo tên người dùng hoặc email</param>
    /// <returns>Dữ liệu phân trang chứa danh sách người dùng</returns>
    /// <exception cref="ValidationException">Khi có lỗi validation</exception>
    public async Task<PaginatedData<UserDto>> GetUsersAsync(int page = 1, int pageSize = 10, string? searchTerm = null)
    {
        // Bắt đầu với query tất cả người dùng
        var query = _userManager.Users.AsQueryable();

        // Áp dụng filter tìm kiếm nếu có
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(u => 
                u.UserName!.Contains(searchTerm) || 
                u.Email!.Contains(searchTerm));
        }

        // Đếm tổng số người dùng để tính phân trang
        var totalCount = await query.CountAsync();
        
        // Lấy danh sách người dùng theo phân trang
        var users = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Chuyển đổi sang DTO và lấy thông tin vai trò
        var userDtos = new List<UserDto>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userDtos.Add(new UserDto
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                Role = string.Join(", ", roles),
                IsLocked = await _userManager.IsLockedOutAsync(user),
                LockoutEnd = await _userManager.GetLockoutEndDateAsync(user),
                EmailConfirmed = user.EmailConfirmed
            });
        }

        // Trả về dữ liệu phân trang
        return new PaginatedData<UserDto>(userDtos, totalCount, page, pageSize);
    }

    /// <summary>
    /// Lấy thông tin chi tiết của một người dùng theo ID
    /// </summary>
    /// <param name="id">ID của người dùng</param>
    /// <returns>Thông tin người dùng hoặc null nếu không tìm thấy</returns>
    public async Task<UserDto?> GetUserByIdAsync(string id)
    {
        // Tìm người dùng theo ID
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return null;

        // Lấy danh sách vai trò của người dùng
        var roles = await _userManager.GetRolesAsync(user);
        
        // Trả về DTO với đầy đủ thông tin
        return new UserDto
        {
            Id = user.Id,
            UserName = user.UserName!,
            Email = user.Email!,
            Role = string.Join(", ", roles),
            IsLocked = await _userManager.IsLockedOutAsync(user),
            LockoutEnd = await _userManager.GetLockoutEndDateAsync(user),
            EmailConfirmed = user.EmailConfirmed
        };
    }

    /// <summary>
    /// Lấy thông tin chi tiết của một người dùng theo email
    /// </summary>
    /// <param name="email">Email của người dùng</param>
    /// <returns>Thông tin người dùng hoặc null nếu không tìm thấy</returns>
    public async Task<UserDto?> GetUserByEmailAsync(string email)
    {
        // Tìm người dùng theo email
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return null;

        // Lấy danh sách vai trò của người dùng
        var roles = await _userManager.GetRolesAsync(user);
        
        // Trả về DTO với đầy đủ thông tin
        return new UserDto
        {
            Id = user.Id,
            UserName = user.UserName!,
            Email = user.Email!,
            Role = string.Join(", ", roles),
            IsLocked = await _userManager.IsLockedOutAsync(user),
            LockoutEnd = await _userManager.GetLockoutEndDateAsync(user),
            EmailConfirmed = user.EmailConfirmed
        };
    }

    /// <summary>
    /// Cập nhật vai trò của người dùng
    /// Xóa tất cả vai trò hiện tại và gán vai trò mới
    /// Sử dụng Role IDs để bảo mật hơn
    /// </summary>
    /// <param name="userId">ID của người dùng cần cập nhật</param>
    /// <param name="roleIds">Danh sách ID của các vai trò mới</param>
    /// <returns>True nếu cập nhật thành công</returns>
    /// <exception cref="NotFoundException">Khi không tìm thấy người dùng</exception>
    /// <exception cref="ValidationException">Khi có lỗi trong quá trình cập nhật vai trò</exception>
    /// <exception cref="BadRequestException">Khi role ID không hợp lệ</exception>
    public async Task<bool> UpdateUserRoleAsync(string userId, List<string> roleIds)
    {
        // Tìm người dùng theo ID
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            throw new NotFoundException("User not found");

        // Validate role IDs - kiểm tra xem tất cả role IDs có tồn tại không
        var validRoles = new List<IdentityRole>();
        foreach (var roleId in roleIds)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
                throw new BadRequestException($"Role ID '{roleId}' không tồn tại trong hệ thống");
            
            // Kiểm tra không cho phép gán role Administrator cho user thường
            if (role.Name == "Administrator" && user.UserName != "admin" && user.UserName != "system")
                throw new BadRequestException("Không thể gán quyền Administrator cho người dùng thường");
            
            validRoles.Add(role);
        }

        // Lấy danh sách vai trò hiện tại
        var existingRoles = await _userManager.GetRolesAsync(user);
        
        // Xóa tất cả vai trò hiện tại
        var removeResult = await _userManager.RemoveFromRolesAsync(user, existingRoles);
        if (!removeResult.Succeeded)
            throw new ValidationException(removeResult.Errors);

        // Gán vai trò mới bằng role names
        var roleNames = validRoles.Select(r => r.Name).ToList();
        var addResult = await _userManager.AddToRolesAsync(user, roleNames);
        if (!addResult.Succeeded)
            throw new ValidationException(addResult.Errors);

        return true;
    }

    /// <summary>
    /// Xóa người dùng khỏi hệ thống
    /// Không thể xóa tài khoản system hoặc admin
    /// </summary>
    /// <param name="userId">ID của người dùng cần xóa</param>
    /// <returns>True nếu xóa thành công</returns>
    /// <exception cref="NotFoundException">Khi không tìm thấy người dùng</exception>
    /// <exception cref="BadRequestException">Khi cố gắng xóa tài khoản system hoặc admin</exception>
    /// <exception cref="ValidationException">Khi có lỗi trong quá trình xóa</exception>
    public async Task<bool> DeleteUserAsync(string userId)
    {
        // Tìm người dùng theo ID
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            throw new NotFoundException("User not found");

        // Kiểm tra không cho phép xóa tài khoản system hoặc admin
        if (user.UserName == "system" || user.UserName == "admin")
            throw new BadRequestException("Cannot delete system or admin user");

        // Thực hiện xóa người dùng
        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
            throw new ValidationException(result.Errors);

        return true;
    }

    /// <summary>
    /// Khóa tài khoản người dùng trong một khoảng thời gian nhất định
    /// Người dùng sẽ không thể đăng nhập cho đến khi hết thời gian khóa
    /// </summary>
    /// <param name="userId">ID của người dùng cần khóa</param>
    /// <param name="lockoutDays">Số ngày khóa tài khoản</param>
    /// <returns>True nếu khóa thành công</returns>
    /// <exception cref="NotFoundException">Khi không tìm thấy người dùng</exception>
    /// <exception cref="ValidationException">Khi có lỗi trong quá trình khóa</exception>
    public async Task<bool> LockUserAsync(string userId, int lockoutDays)
    {
        // Tìm người dùng theo ID
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            throw new NotFoundException("User not found");

        // Tính thời gian mở khóa
        var lockoutEnd = DateTimeOffset.UtcNow.AddDays(lockoutDays);
        
        // Thực hiện khóa tài khoản
        var result = await _userManager.SetLockoutEndDateAsync(user, lockoutEnd);
        
        if (!result.Succeeded)
            throw new ValidationException(result.Errors);

        return true;
    }

    /// <summary>
    /// Mở khóa tài khoản người dùng ngay lập tức
    /// Cho phép người dùng đăng nhập lại vào hệ thống
    /// </summary>
    /// <param name="userId">ID của người dùng cần mở khóa</param>
    /// <returns>True nếu mở khóa thành công</returns>
    /// <exception cref="NotFoundException">Khi không tìm thấy người dùng</exception>
    /// <exception cref="ValidationException">Khi có lỗi trong quá trình mở khóa</exception>
    public async Task<bool> UnlockUserAsync(string userId)
    {
        // Tìm người dùng theo ID
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            throw new NotFoundException("User not found");

        // Mở khóa bằng cách set lockout end date thành null
        var result = await _userManager.SetLockoutEndDateAsync(user, null);
        
        if (!result.Succeeded)
            throw new ValidationException(result.Errors);

        return true;
    }

    /// <summary>
    /// Kiểm tra trạng thái khóa của người dùng
    /// </summary>
    /// <param name="userId">ID của người dùng cần kiểm tra</param>
    /// <returns>True nếu tài khoản đang bị khóa, false nếu không</returns>
    /// <exception cref="NotFoundException">Khi không tìm thấy người dùng</exception>
    public async Task<bool> IsUserLockedAsync(string userId)
    {
        // Tìm người dùng theo ID
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            throw new NotFoundException("User not found");

        // Kiểm tra trạng thái khóa
        return await _userManager.IsLockedOutAsync(user);
    }

    /// <summary>
    /// Lấy thời gian mở khóa của người dùng
    /// </summary>
    /// <param name="userId">ID của người dùng</param>
    /// <returns>Thời gian mở khóa hoặc null nếu tài khoản không bị khóa</returns>
    /// <exception cref="NotFoundException">Khi không tìm thấy người dùng</exception>
    public async Task<DateTimeOffset?> GetUserLockoutEndDateAsync(string userId)
    {
        // Tìm người dùng theo ID
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            throw new NotFoundException("User not found");

        // Lấy thời gian mở khóa
        return await _userManager.GetLockoutEndDateAsync(user);
    }

    /// <summary>
    /// Lấy danh sách tất cả vai trò của một người dùng
    /// </summary>
    /// <param name="userId">ID của người dùng</param>
    /// <returns>Danh sách tên vai trò</returns>
    /// <exception cref="NotFoundException">Khi không tìm thấy người dùng</exception>
    public async Task<List<string>> GetUserRolesAsync(string userId)
    {
        // Tìm người dùng theo ID
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            throw new NotFoundException("User not found");

        // Lấy danh sách vai trò
        var roles = await _userManager.GetRolesAsync(user);
        return roles.ToList();
    }

    /// <summary>
    /// Lấy danh sách tất cả vai trò có trong hệ thống
    /// Dùng để hiển thị dropdown chọn vai trò khi cập nhật quyền người dùng
    /// Chỉ trả về thông tin an toàn, không expose sensitive data
    /// </summary>
    /// <returns>Danh sách vai trò với thông tin an toàn</returns>
    public async Task<List<RoleDto>> GetAllRolesAsync()
    {
        // Lấy tất cả vai trò từ database
        var roles = await _roleManager.Roles
            .Select(r => new { r.Id, r.Name })
            .ToListAsync();

        // Chuyển đổi sang DTO an toàn
        var roleDtos = new List<RoleDto>();
        foreach (var role in roles)
        {
            var isSystemRole = role.Name == "Administrator" || role.Name == "System";
            roleDtos.Add(new RoleDto
            {
                Id = role.Id,
                DisplayName = GetRoleDisplayName(role.Name ?? string.Empty),
                Description = GetRoleDescription(role.Name ?? string.Empty),
                IsSystemRole = isSystemRole
            });
        }

        return roleDtos;
    }

    /// <summary>
    /// Lấy tên hiển thị cho vai trò
    /// </summary>
    private string GetRoleDisplayName(string roleName)
    {
        return roleName switch
        {
            "Administrator" => "Quản trị viên",
            "Manager" => "Quản lý",
            "Staff" => "Nhân viên",
            "User" => "Người dùng",
            "System" => "Hệ thống",
            _ => roleName
        };
    }

    /// <summary>
    /// Lấy mô tả cho vai trò
    /// </summary>
    private string GetRoleDescription(string roleName)
    {
        return roleName switch
        {
            "Administrator" => "Quyền quản trị cao nhất, có thể quản lý tất cả chức năng hệ thống",
            "Manager" => "Quyền quản lý, có thể quản lý nhân viên và đơn hàng",
            "Staff" => "Quyền nhân viên, có thể xử lý đơn hàng và quản lý menu",
            "User" => "Quyền người dùng thường, có thể đặt hàng và xem menu",
            "System" => "Quyền hệ thống, chỉ dành cho các tác vụ hệ thống",
            _ => "Vai trò người dùng"
        };
    }

    /// <summary>
    /// Tạo người dùng mới
    /// </summary>
    /// <param name="dto">Thông tin người dùng cần tạo</param>
    /// <returns>Thông tin người dùng đã tạo</returns>
    /// <exception cref="BadRequestException">Khi email đã tồn tại hoặc role ID không hợp lệ</exception>
    /// <exception cref="ValidationException">Khi có lỗi validation</exception>
    public async Task<UserDto> CreateUserAsync(CreateUserDto dto)
    {
        // Kiểm tra email đã tồn tại chưa
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
            throw new BadRequestException("Email đã tồn tại trong hệ thống");

        // Validate role IDs
        var validRoles = new List<IdentityRole>();
        foreach (var roleId in dto.RoleIds)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
                throw new BadRequestException($"Role ID '{roleId}' không tồn tại trong hệ thống");
            
            validRoles.Add(role);
        }

        // Tạo user mới
        var user = new User
        {
            UserName = dto.Email, // Sử dụng email làm username
            Email = dto.Email,
            EmailConfirmed = dto.EmailConfirmed,
            PhoneNumber = dto.PhoneNumber,
            PhoneNumberConfirmed = dto.PhoneNumberConfirmed
        };

        // Tạo user với password
        var createResult = await _userManager.CreateAsync(user, dto.Password);
        if (!createResult.Succeeded)
            throw new ValidationException(createResult.Errors);

        // Gán vai trò cho user
        var roleNames = validRoles.Select(r => r.Name).ToList();
        var addRoleResult = await _userManager.AddToRolesAsync(user, roleNames);
        if (!addRoleResult.Succeeded)
        {
            // Nếu gán role thất bại, xóa user đã tạo
            await _userManager.DeleteAsync(user);
            throw new ValidationException(addRoleResult.Errors);
        }

        // Trả về thông tin user đã tạo
        var roles = await _userManager.GetRolesAsync(user);
        return new UserDto
        {
            Id = user.Id,
            UserName = user.UserName!,
            Email = user.Email!,
            Role = string.Join(", ", roles),
            IsLocked = false,
            LockoutEnd = null,
            EmailConfirmed = user.EmailConfirmed
        };
    }

    /// <summary>
    /// Lấy danh sách người dùng với các bộ lọc chi tiết
    /// </summary>
    /// <param name="searchDto">Các tham số tìm kiếm và lọc</param>
    /// <returns>Danh sách người dùng với thông tin phân trang</returns>
    public async Task<PaginatedData<UserDto>> GetUsersWithFiltersAsync(UserSearchDto searchDto)
    {
        // Bắt đầu với query cơ bản
        var query = _userManager.Users.AsQueryable();

        // Áp dụng các bộ lọc
        query = ApplyUserFilters(query, searchDto);

        // Áp dụng sắp xếp
        query = ApplyUserSorting(query, searchDto.SortBy, searchDto.SortOrder);

        // Đếm tổng số records
        var totalCount = await query.CountAsync();

        // Áp dụng phân trang
        var skip = (searchDto.Page - 1) * searchDto.PageSize;
        var users = await query
            .Skip(skip)
            .Take(searchDto.PageSize)
            .ToListAsync();

        // Chuyển đổi sang DTO
        var userDtos = new List<UserDto>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var isLocked = await _userManager.IsLockedOutAsync(user);
            var lockoutEnd = await _userManager.GetLockoutEndDateAsync(user);

            userDtos.Add(new UserDto
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                Role = string.Join(", ", roles),
                IsLocked = isLocked,
                LockoutEnd = lockoutEnd,
                EmailConfirmed = user.EmailConfirmed
            });
        }

        return new PaginatedData<UserDto>
        {
            Items = userDtos,
            TotalCount = totalCount,
            Page = searchDto.Page,
            PageSize = searchDto.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / searchDto.PageSize)
        };
    }

    /// <summary>
    /// Áp dụng các bộ lọc cho query người dùng
    /// </summary>
    private IQueryable<User> ApplyUserFilters(IQueryable<User> query, UserSearchDto searchDto)
    {
        // Lọc theo từ khóa tìm kiếm
        if (!string.IsNullOrWhiteSpace(searchDto.SearchKeyword))
        {
            var keyword = searchDto.SearchKeyword.ToLower();
            query = query.Where(u => 
                (u.UserName != null && u.UserName.ToLower().Contains(keyword)) ||
                (u.Email != null && u.Email.ToLower().Contains(keyword))
            );
        }

        // Lọc theo email cụ thể
        if (!string.IsNullOrWhiteSpace(searchDto.Email))
        {
            query = query.Where(u => u.Email != null && u.Email.ToLower() == searchDto.Email.ToLower());
        }

        // Lọc theo username cụ thể
        if (!string.IsNullOrWhiteSpace(searchDto.UserName))
        {
            query = query.Where(u => u.UserName != null && u.UserName.ToLower() == searchDto.UserName.ToLower());
        }

        // Lọc theo trạng thái xác nhận email
        if (searchDto.EmailConfirmed.HasValue)
        {
            query = query.Where(u => u.EmailConfirmed == searchDto.EmailConfirmed.Value);
        }

        // Lọc theo trạng thái xác nhận số điện thoại
        if (searchDto.PhoneNumberConfirmed.HasValue)
        {
            query = query.Where(u => u.PhoneNumberConfirmed == searchDto.PhoneNumberConfirmed.Value);
        }

        return query;
    }

    /// <summary>
    /// Áp dụng sắp xếp cho query người dùng
    /// </summary>
    private IQueryable<User> ApplyUserSorting(IQueryable<User> query, string sortBy, string sortOrder)
    {
        return sortBy.ToLower() switch
        {
            "username" => sortOrder.ToUpper() == "ASC" 
                ? query.OrderBy(u => u.UserName) 
                : query.OrderByDescending(u => u.UserName),
            
            "email" => sortOrder.ToUpper() == "ASC" 
                ? query.OrderBy(u => u.Email) 
                : query.OrderByDescending(u => u.Email),
            
            _ => query.OrderBy(u => u.UserName) // Mặc định sắp xếp theo UserName
        };
    }
} 