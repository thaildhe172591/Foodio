# API Endpoints - Quản lý Người dùng

## Base URL: `/api/admin/users`

### 📋 Danh sách API Endpoints

| # | Method | Endpoint | Công dụng | Input | Output |
|---|--------|----------|-----------|-------|--------|
| 1 | POST | `/api/admin/users` | Tạo người dùng mới | `CreateUserDto` | `UserDto` |
| 2 | GET | `/api/admin/users` | Lấy danh sách người dùng | `page`, `pageSize`, `searchTerm` | `PaginatedData<UserDto>` |
| 3 | GET | `/api/admin/users/get-by-id/{id}` | Lấy người dùng theo ID | `id` | `UserDto` |
| 4 | GET | `/api/admin/users/get-by-email/{email}` | Lấy người dùng theo Email | `email` | `UserDto` |
| 5 | PUT | `/api/admin/users/update-roles/{id}` | Cập nhật vai trò người dùng | `id`, `UpdateUserRoleDto` | `bool` |
| 6 | DELETE | `/api/admin/users/delete/{id}` | Xóa người dùng | `id` | `bool` |
| 7 | POST | `/api/admin/users/lock/{id}` | Khóa người dùng | `id`, `LockUserDto` | `bool` |
| 8 | POST | `/api/admin/users/unlock/{id}` | Mở khóa người dùng | `id` | `bool` |
| 9 | GET | `/api/admin/users/check-lock-status/{id}` | Kiểm tra trạng thái khóa | `id` | `bool` |
| 10 | GET | `/api/admin/users/get-lockout-end/{id}` | Lấy thời gian mở khóa | `id` | `DateTimeOffset?` |
| 11 | GET | `/api/admin/users/get-user-roles/{id}` | Lấy vai trò của người dùng | `id` | `List<string>` |
| 12 | GET | `/api/admin/users/get-all-roles` | Lấy tất cả vai trò hệ thống | - | `List<RoleDto>` |

---

## 📝 DTOs

### CreateUserDto
```json
{
  "email": "user@example.com",
  "password": "password123",
  "phoneNumber": "+14155552671",
  "roleIds": ["role-id-1", "role-id-2"],
  "emailConfirmed": false
}
```

### UpdateUserRoleDto
```json
{
  "roleIds": ["role-id-1", "role-id-2"]
}
```

### LockUserDto
```json
{
  "lockoutDays": 7
}
```

### UserDto
```json
{
  "id": "string",
  "userName": "string", 
  "email": "string",
  "role": "string",
  "isLocked": false,
  "lockoutEnd": "2024-01-01T00:00:00Z",
  "emailConfirmed": true
}
```

### RoleDto
```json
{
  "id": "role-id",
  "displayName": "Quản trị viên",
  "description": "Quyền quản trị cao nhất",
  "isSystemRole": true
}
```

### PaginatedData<T>
```json
{
  "data": [],
  "totalCount": 100,
  "page": 1,
  "pageSize": 10,
  "totalPages": 10
}
```

## API Quản lý Người dùng (User Management)

### 1. Lấy danh sách người dùng (Cơ bản)
- **Endpoint**: `GET /api/admin/users`
- **Công dụng**: Lấy danh sách tất cả người dùng với phân trang và tìm kiếm cơ bản
- **Input**: 
  - `page` (Optional): Số trang, mặc định: 1
  - `pageSize` (Optional): Số lượng item trên mỗi trang, mặc định: 10
  - `searchTerm` (Optional): Từ khóa tìm kiếm theo tên người dùng hoặc email
- **Output**: `Response<PaginatedData<UserDto>>`

### 2. Lấy danh sách người dùng (Chi tiết)
- **Endpoint**: `GET /api/admin/users/search`
- **Công dụng**: Lấy danh sách người dùng với các bộ lọc chi tiết
- **Input**: `UserSearchDto` (query parameters)
  - `page` (Optional): Số trang, mặc định: 1
  - `pageSize` (Optional): Số lượng item trên trang (1-100), mặc định: 10
  - `searchKeyword` (Optional): Từ khóa tìm kiếm theo username/email
  - `email` (Optional): Email cụ thể để tìm kiếm
  - `userName` (Optional): Username cụ thể để tìm kiếm
  - `roleId` (Optional): ID vai trò để lọc
  - `roleName` (Optional): Tên vai trò để lọc
  - `emailConfirmed` (Optional): Lọc theo trạng thái xác nhận email
  - `isLocked` (Optional): Lọc theo trạng thái khóa
  - `phoneNumberConfirmed` (Optional): Lọc theo trạng thái xác nhận phone
  - `sortBy` (Optional): Sắp xếp theo trường (UserName/Email), mặc định: UserName
  - `sortOrder` (Optional): Thứ tự sắp xếp (ASC/DESC), mặc định: ASC
- **Output**: `Response<PaginatedData<UserDto>>`

### 3. Lấy người dùng theo ID
- **Endpoint**: `GET /api/admin/users/get-by-id/{id}`
- **Công dụng**: Lấy thông tin chi tiết người dùng theo ID
- **Input**: 
  - `id` (Required): ID của người dùng (path parameter)
- **Output**: `Response<UserDto>`

### 4. Lấy người dùng theo Email
- **Endpoint**: `GET /api/admin/users/get-by-email/{email}`
- **Công dụng**: Lấy thông tin chi tiết người dùng theo email
- **Input**: 
  - `email` (Required): Email của người dùng (path parameter)
- **Output**: `Response<UserDto>`

### 5. Cập nhật vai trò người dùng
- **Endpoint**: `PUT /api/admin/users/update-roles/{id}`
- **Công dụng**: Cập nhật vai trò của người dùng (sử dụng Role IDs)
- **Input**: 
  - `id` (Required): ID của người dùng (path parameter)
  - `UpdateUserRoleDto` (Required): Body request
- **Output**: `Response<bool>`

### 6. Xóa người dùng
- **Endpoint**: `DELETE /api/admin/users/delete/{id}`
- **Công dụng**: Xóa người dùng khỏi hệ thống
- **Input**: 
  - `id` (Required): ID của người dùng cần xóa (path parameter)
- **Output**: `Response<bool>`

### 7. Khóa người dùng
- **Endpoint**: `POST /api/admin/users/lock/{id}`
- **Công dụng**: Khóa tài khoản người dùng trong thời gian nhất định
- **Input**: 
  - `id` (Required): ID của người dùng (path parameter)
  - `LockUserDto` (Required): Body request
- **Output**: `Response<bool>`

### 8. Mở khóa người dùng
- **Endpoint**: `POST /api/admin/users/unlock/{id}`
- **Công dụng**: Mở khóa tài khoản người dùng ngay lập tức
- **Input**: 
  - `id` (Required): ID của người dùng (path parameter)
- **Output**: `Response<bool>`

### 9. Kiểm tra trạng thái khóa
- **Endpoint**: `GET /api/admin/users/check-lock-status/{id}`
- **Công dụng**: Kiểm tra xem người dùng có đang bị khóa không
- **Input**: 
  - `id` (Required): ID của người dùng (path parameter)
- **Output**: `Response<bool>`

### 10. Lấy thời gian mở khóa
- **Endpoint**: `GET /api/admin/users/get-lockout-end/{id}`
- **Công dụng**: Lấy thời gian mở khóa của người dùng
- **Input**: 
  - `id` (Required): ID của người dùng (path parameter)
- **Output**: `Response<DateTimeOffset?>`

### 11. Lấy vai trò của người dùng
- **Endpoint**: `GET /api/admin/users/get-user-roles/{id}`
- **Công dụng**: Lấy danh sách vai trò của một người dùng
- **Input**: 
  - `id` (Required): ID của người dùng (path parameter)
- **Output**: `Response<List<string>>`

### 12. Lấy tất cả vai trò
- **Endpoint**: `GET /api/admin/users/get-all-roles`
- **Công dụng**: Lấy danh sách tất cả vai trò có trong hệ thống
- **Input**: Không có
- **Output**: `Response<List<RoleDto>>`

### 13. Tạo người dùng mới
- **Endpoint**: `POST /api/admin/users`
- **Công dụng**: Tạo người dùng mới với vai trò cụ thể
- **Input**: 
  - `CreateUserDto` (Required): Body request
- **Output**: `Response<UserDto>`

## DTOs sử dụng

### UserSearchDto (Cho tìm kiếm chi tiết)
```csharp
{
    "page": 1,                    // Optional: Số trang (mặc định: 1)
    "pageSize": 10,               // Optional: Số lượng item trên trang (1-100, mặc định: 10)
    "searchKeyword": "string",    // Optional: Từ khóa tìm kiếm theo username/email
    "email": "string",            // Optional: Email cụ thể để tìm kiếm
    "userName": "string",         // Optional: Username cụ thể để tìm kiếm
    "roleId": "string",           // Optional: ID vai trò để lọc
    "roleName": "string",         // Optional: Tên vai trò để lọc
    "emailConfirmed": true,       // Optional: Lọc theo trạng thái xác nhận email
    "isLocked": true,             // Optional: Lọc theo trạng thái khóa
    "phoneNumberConfirmed": true, // Optional: Lọc theo trạng thái xác nhận phone
    "sortBy": "UserName",         // Optional: Sắp xếp theo trường (UserName/Email, mặc định: UserName)
    "sortOrder": "ASC"            // Optional: Thứ tự sắp xếp (ASC/DESC, mặc định: ASC)
}
```

### CreateUserDto
```csharp
{
    "email": "string",           // Required: Email hợp lệ
    "password": "string",        // Required: Mật khẩu (6-100 ký tự)
    "phoneNumber": "string",     // Optional: Số điện thoại
    "roleIds": ["string"],       // Required: Danh sách Role IDs (ít nhất 1 vai trò)
    "emailConfirmed": false,     // Optional: Xác nhận email (mặc định: false)
    "phoneNumberConfirmed": false // Optional: Xác nhận số điện thoại (mặc định: false)
}
```

### UpdateUserRoleDto
```csharp
{
    "roleIds": ["string"]        // Required: Danh sách Role IDs mới (ít nhất 1 vai trò)
}
```

### LockUserDto
```csharp
{
    "lockoutDays": 0             // Required: Số ngày khóa (phải > 0)
}
```

### UserDto
```csharp
{
    "id": "string",              // ID người dùng
    "userName": "string",        // Tên đăng nhập
    "email": "string",           // Email
    "role": "string",            // Danh sách vai trò (comma-separated)
    "isLocked": false,           // Trạng thái khóa
    "lockoutEnd": "DateTimeOffset?", // Thời gian mở khóa
    "emailConfirmed": false      // Trạng thái xác nhận email
}
```

### RoleDto
```csharp
{
    "id": "string",              // ID vai trò
    "displayName": "string",     // Tên hiển thị tiếng Việt
    "description": "string",     // Mô tả vai trò
    "isSystemRole": false        // Có phải vai trò hệ thống không
}
```

## Lưu ý quan trọng

### Quyền truy cập
- Tất cả API đều yêu cầu quyền **Admin**
- Cần có JWT token hợp lệ trong header: `Authorization: Bearer {token}`

### Validation
- **Email**: Phải đúng định dạng email
- **Password**: Từ 6-100 ký tự
- **PhoneNumber**: Tối đa 20 ký tự, định dạng số điện thoại
- **PageSize**: Từ 1-100
- **RoleIds**: Phải tồn tại trong hệ thống
- **LockoutDays**: Phải > 0

### Bảo mật
- Không thể xóa hoặc thay đổi vai trò của tài khoản **Administrator** và **System**
- Email phải unique trong hệ thống
- Password được hash tự động
- Refresh token được quản lý tự động

### Error Handling
- **400 Bad Request**: Dữ liệu đầu vào không hợp lệ
- **404 Not Found**: Không tìm thấy người dùng/vai trò
- **401 Unauthorized**: Token không hợp lệ hoặc thiếu quyền
- **500 Internal Server Error**: Lỗi hệ thống