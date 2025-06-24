using FoodioAPI.DTOs;
using FoodioAPI.DTOs.UserDtos;
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
    /// Lấy danh sách tất cả người dùng trong hệ thống với phân trang và tìm kiếm
    /// </summary>
    /// <param name="page">Số trang (mặc định: 1)</param>
    /// <param name="pageSize">Số lượng item trên mỗi trang (mặc định: 10)</param>
    /// <param name="searchTerm">Từ khóa tìm kiếm theo tên người dùng hoặc email</param>
    /// <returns>Danh sách người dùng với thông tin phân trang</returns>
    /// <exception cref="ValidationException">Khi có lỗi validation</exception>
    public async Task<PaginatedData<UserDto>> GetUsersAsync(int page = 1, int pageSize = 10, string? searchTerm = null)
    {
        // Xây dựng query cơ bản
        var query = _userManager.Users.AsQueryable();

        // Áp dụng tìm kiếm nếu có
        if (!string.IsNullOrEmpty(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            query = query.Where(u =>
                u.UserName!.ToLower().Contains(searchTerm) ||
                u.Email!.ToLower().Contains(searchTerm));
        }

        // Tính tổng số bản ghi
        var totalCount = await query.CountAsync();

        // Áp dụng phân trang
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
                PhoneNumber = user.PhoneNumber,
                CreatedDate = DateTime.UtcNow, // Tạm thời sử dụng thời gian hiện tại
                Roles = roles.ToList(),
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
    /// <exception cref="NotFoundException">Khi không tìm thấy người dùng</exception>
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
            PhoneNumber = user.PhoneNumber,
            CreatedDate = DateTime.UtcNow, // Tạm thời sử dụng thời gian hiện tại
            Roles = roles.ToList(),
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
    /// <exception cref="NotFoundException">Khi không tìm thấy người dùng</exception>
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
            PhoneNumber = user.PhoneNumber,
            CreatedDate = DateTime.UtcNow, // Tạm thời sử dụng thời gian hiện tại
            Roles = roles.ToList(),
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
    /// Cập nhật thông tin cơ bản của người dùng. Cho phép cập nhật email, số điện thoại, họ tên và (tùy chọn) vai trò.
    /// </summary>
    public async Task<bool> UpdateUserAsync(string userId, UpdateUserDto dto)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) throw new NotFoundException("User not found");

        // Email
        if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email != user.Email)
        {
            // Kiểm tra email trùng
            var existingEmail = await _userManager.FindByEmailAsync(dto.Email);
            if (existingEmail != null && existingEmail.Id != userId)
                throw new BadRequestException("Email đã tồn tại trong hệ thống");

            user.Email = dto.Email;
            user.UserName = dto.Email; // đồng bộ username với email
        }

        // Phone
        if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
            user.PhoneNumber = dto.PhoneNumber;

        // FirstName + LastName hiện đang được lưu trong UserName khi tạo, nếu muốn tách cần field riêng.
        // Ta sẽ cập nhật UserName thành FirstName LastName (nếu cung cấp)
        if (!string.IsNullOrWhiteSpace(dto.UserName))
        {
            user.UserName = dto.UserName;
        }

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new ValidationException(result.Errors);

        // update roles if provided
        if (dto.RoleIds != null && dto.RoleIds.Any())
        {
            await UpdateUserRoleAsync(userId, dto.RoleIds);
        }

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
    /// <returns>True nếu người dùng đang bị khóa</returns>
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
    /// Lấy thời gian kết thúc khóa của người dùng
    /// </summary>
    /// <param name="userId">ID của người dùng</param>
    /// <returns>Thời gian kết thúc khóa hoặc null nếu không bị khóa</returns>
    /// <exception cref="NotFoundException">Khi không tìm thấy người dùng</exception>
    public async Task<DateTimeOffset?> GetUserLockoutEndDateAsync(string userId)
    {
        // Tìm người dùng theo ID
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            throw new NotFoundException("User not found");

        // Lấy thời gian kết thúc khóa
        return await _userManager.GetLockoutEndDateAsync(user);
    }

    /// <summary>
    /// Lấy danh sách vai trò của người dùng
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
    /// Lấy danh sách tất cả vai trò trong hệ thống
    /// </summary>
    /// <returns>Danh sách vai trò với thông tin chi tiết</returns>
    /// <exception cref="NotFoundException">Khi không tìm thấy vai trò</exception>
    public async Task<List<RoleDto>> GetAllRolesAsync()
    {
        var roles = await _roleManager.Roles.ToListAsync();
        var roleDtos = new List<RoleDto>();

        foreach (var role in roles)
        {
            roleDtos.Add(new RoleDto
            {
                Id = role.Id,
                Name = role.Name!,
                DisplayName = GetRoleDisplayName(role.Name!),
                Description = GetRoleDescription(role.Name!)
            });
        }

        return roleDtos;
    }

    /// <summary>
    /// Tạo người dùng mới
    /// </summary>
    /// <param name="dto">Thông tin người dùng cần tạo</param>
    /// <returns>Thông tin người dùng đã tạo</returns>
    /// <exception cref="ValidationException">Khi có lỗi trong quá trình tạo</exception>
    /// <exception cref="BadRequestException">Khi role ID không hợp lệ</exception>
    public async Task<UserDto> CreateUserAsync(CreateUserDto dto)
    {
        // Tạo user mới
        var user = new User
        {
            UserName = dto.UserName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            EmailConfirmed = dto.EmailConfirmed,
            PhoneNumberConfirmed = dto.PhoneNumberConfirmed
        };

        // Tạo user
        var createResult = await _userManager.CreateAsync(user, dto.Password);
        if (!createResult.Succeeded)
            throw new ValidationException(createResult.Errors);

        // Gán vai trò nếu có
        if (dto.RoleIds.Any())
        {
            var roleNames = new List<string>();
            foreach (var roleId in dto.RoleIds)
            {
                var role = await _roleManager.FindByIdAsync(roleId);
                if (role == null)
                    throw new BadRequestException($"Role ID '{roleId}' không tồn tại trong hệ thống");
                roleNames.Add(role.Name!);
            }

            var addRoleResult = await _userManager.AddToRolesAsync(user, roleNames);
            if (!addRoleResult.Succeeded)
                throw new ValidationException(addRoleResult.Errors);
        }

        // Lấy thông tin vai trò đã gán
        var roles = await _userManager.GetRolesAsync(user);

        // Trả về DTO với đầy đủ thông tin
        return new UserDto
        {
            Id = user.Id,
            UserName = user.UserName!,
            Email = user.Email!,
            PhoneNumber = user.PhoneNumber,
            CreatedDate = DateTime.UtcNow, // Tạm thời sử dụng thời gian hiện tại
            Roles = roles.ToList(),
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
    /// <exception cref="ValidationException">Khi có lỗi validation</exception>
    public async Task<PaginatedData<UserDto>> SearchWithStaff(UserSearchDto searchDto)
    {
        // Xây dựng query cơ bản
        var query = _userManager.Users.AsQueryable();

        // ===== Lọc theo Role (RoleId hoặc RoleName) =====
        if (!string.IsNullOrWhiteSpace(searchDto.RoleId) || !string.IsNullOrWhiteSpace(searchDto.RoleName))
        {
            // Xác định tên vai trò cần lọc
            string? roleName = null;

            if (!string.IsNullOrWhiteSpace(searchDto.RoleId))
            {
                var role = await _roleManager.FindByIdAsync(searchDto.RoleId);
                if (role == null)
                    throw new BadRequestException($"Role ID '{searchDto.RoleId}' không tồn tại trong hệ thống");

                roleName = role.Name;
            }

            // Nếu người dùng truyền trực tiếp RoleName thì ưu tiên, ngược lại dùng tên vai trò lấy từ RoleId
            if (!string.IsNullOrWhiteSpace(searchDto.RoleName))
            {
                roleName = searchDto.RoleName;
            }

            if (!string.IsNullOrWhiteSpace(roleName))
            {
                // Lấy danh sách UserIds thuộc vai trò này
                var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);
                var userIdsInRole = usersInRole.Select(u => u.Id).ToList();

                // Áp dụng vào query
                query = query.Where(u => userIdsInRole.Contains(u.Id));
            }
        }

        // Áp dụng các bộ lọc
        query = ApplyUserFilters(query, searchDto);

        // Tính tổng số bản ghi
        var totalCount = await query.CountAsync();

        // Áp dụng sắp xếp
        query = ApplyUserSorting(query, searchDto.SortBy, searchDto.SortOrder);

        // Áp dụng phân trang
        var users = await query
            .Skip((searchDto.Page - 1) * searchDto.PageSize)
            .Take(searchDto.PageSize)
            .ToListAsync();

        // Chuyển đổi sang DTO
        var userDtos = new List<UserDto>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userDtos.Add(new UserDto
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber,
                CreatedDate = DateTime.UtcNow, // Tạm thời sử dụng thời gian hiện tại
                Roles = roles.ToList(),
                Role = string.Join(", ", roles),
                IsLocked = await _userManager.IsLockedOutAsync(user),
                LockoutEnd = await _userManager.GetLockoutEndDateAsync(user),
                EmailConfirmed = user.EmailConfirmed
            });
        }

        return new PaginatedData<UserDto>
        {
            Data = userDtos,
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
        // Tìm kiếm theo từ khóa
        if (!string.IsNullOrEmpty(searchDto.SearchKeyword))
        {
            var keyword = searchDto.SearchKeyword.ToLower();
            query = query.Where(u =>
                u.UserName!.ToLower().Contains(keyword) ||
                u.Email!.ToLower().Contains(keyword));
        }

        // Lọc theo email
        if (!string.IsNullOrEmpty(searchDto.Email))
        {
            query = query.Where(u => u.Email == searchDto.Email);
        }

        // Lọc theo username
        if (!string.IsNullOrEmpty(searchDto.UserName))
        {
            query = query.Where(u => u.UserName == searchDto.UserName);
        }

        // Lọc theo trạng thái xác nhận email
        if (searchDto.EmailConfirmed.HasValue)
        {
            query = query.Where(u => u.EmailConfirmed == searchDto.EmailConfirmed.Value);
        }

        // Lọc theo trạng thái khóa
        if (searchDto.IsLocked.HasValue)
        {
            if (searchDto.IsLocked.Value)
            {
                query = query.Where(u => u.LockoutEnd > DateTimeOffset.UtcNow);
            }
            else
            {
                query = query.Where(u => u.LockoutEnd == null || u.LockoutEnd <= DateTimeOffset.UtcNow);
            }
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
            _ => query.OrderBy(u => u.UserName)
        };
    }

    /// <summary>
    /// Lấy tên hiển thị của vai trò
    /// </summary>
    private string GetRoleDisplayName(string roleName)
    {
        return roleName switch
        {
            "Admin" => "Quản trị viên",
            "Manager" => "Quản lý",
            "Staff" => "Nhân viên",
            "Customer" => "Khách hàng",
            _ => roleName
        };
    }

    /// <summary>
    /// Lấy mô tả của vai trò
    /// </summary>
    private string GetRoleDescription(string roleName)
    {
        return roleName switch
        {
            "Admin" => "Quản trị viên hệ thống với toàn quyền",
            "Manager" => "Quản lý với quyền quản lý nhân viên và đơn hàng",
            "Staff" => "Nhân viên với quyền xử lý đơn hàng",
            "Customer" => "Khách hàng với quyền đặt hàng và xem thông tin cá nhân",
            _ => "Vai trò trong hệ thống"
        };
    }
}