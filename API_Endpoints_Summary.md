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