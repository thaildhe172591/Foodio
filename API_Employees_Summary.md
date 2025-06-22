# Cập nhật API và Razor Pages cho Quản lý Nhân viên

## Tổng quan
Đã cập nhật API và Razor Pages để hiển thị đầy đủ thông tin nhân viên với các trường: FirstName, LastName, PhoneNumber, CreatedDate, Roles.

## Các thay đổi chính

### 1. Cập nhật UserDto
- **File**: `DTOs/UserDtos/UserDto.cs`
- **Thay đổi**: Thêm các trường mới:
  - `FirstName` (string?)
  - `LastName` (string?)
  - `PhoneNumber` (string?)
  - `CreatedDate` (DateTime?)
  - `Roles` (List<string>)

### 2. Cập nhật UserManagementService
- **File**: `Services/Implements/UserManagementService.cs`
- **Thay đổi**: 
  - Map đầy đủ thông tin từ User entity sang UserDto
  - Tạm thời lấy FirstName, LastName từ UserName bằng cách split
  - Sử dụng DateTime.UtcNow cho CreatedDate
  - Lấy danh sách Roles từ UserManager

### 3. Cập nhật RoleDto
- **File**: `DTOs/UserDtos/RoleDto.cs`
- **Thay đổi**: Thêm trường `Name` để có tên gốc của vai trò

### 4. Cập nhật Employees.cshtml
- **File**: `Pages/Admin/Employees.cshtml`
- **Thay đổi**:
  - Hiển thị đầy đủ thông tin nhân viên
  - Thêm avatar với chữ cái đầu
  - Hiển thị FirstName, LastName, PhoneNumber, CreatedDate, Roles
  - Cải thiện UI với dropdown actions
  - Thêm thống kê tổng quan
  - Sử dụng modal cho các thao tác

### 5. Cập nhật Employees.cshtml.cs
- **File**: `Pages/Admin/Employees.cshtml.cs`
- **Thay đổi**:
  - Sử dụng IHttpClientFactory thay vì HttpClient
  - Thêm các handler: ViewEmployee, EditEmployee, UpdateRoles
  - Thêm các handler POST: LockEmployee, UnlockEmployee, DeleteEmployee, UpdateEmployeeRoles
  - Thêm UpdateEmployeeDto cho việc cập nhật thông tin

### 6. Tạo Partial Views
- **File**: `Pages/Admin/_ViewEmployeePartial.cshtml`
  - Hiển thị chi tiết nhân viên trong modal
- **File**: `Pages/Admin/_EditEmployeePartial.cshtml`
  - Form chỉnh sửa thông tin nhân viên
- **File**: `Pages/Admin/_UpdateRolesPartial.cshtml`
  - Form cập nhật vai trò nhân viên

### 7. Cập nhật DependencyInjection
- **File**: `DependencyInjection.cs`
- **Thay đổi**: Thêm cấu hình HttpClient với tên "API"

### 8. Cập nhật Layout Admin
- **File**: `Pages/Admin/_Layout.cshtml`
- **Thay đổi**: Thêm _ToastrScripts để có thông báo

## Các tính năng mới

### 1. Hiển thị thông tin đầy đủ
- Avatar với chữ cái đầu của tên
- Họ và tên đầy đủ
- Email với badge xác nhận
- Số điện thoại
- Danh sách vai trò
- Ngày tạo tài khoản
- Trạng thái khóa/mở khóa

### 2. Thao tác CRUD
- **Xem chi tiết**: Modal hiển thị đầy đủ thông tin
- **Chỉnh sửa**: Cập nhật thông tin cơ bản và vai trò
- **Phân quyền**: Cập nhật vai trò riêng biệt
- **Khóa/Mở khóa**: Quản lý trạng thái tài khoản
- **Xóa**: Xóa nhân viên với xác nhận

### 3. Tìm kiếm và lọc
- Tìm kiếm theo tên hoặc email
- Lọc theo vai trò
- Lọc theo trạng thái (hoạt động/khóa)

### 4. Thống kê tổng quan
- Tổng số nhân viên
- Số nhân viên đang hoạt động
- Số nhân viên đã khóa
- Số nhân viên đã xác nhận email

### 5. Giao diện cải tiến
- Responsive design
- Dropdown actions
- Modal dialogs
- Toastr notifications
- Pagination cải tiến

## Lưu ý kỹ thuật

### 1. Tạm thời xử lý dữ liệu
- FirstName, LastName được tạm thời lấy từ UserName bằng cách split
- CreatedDate sử dụng DateTime.UtcNow
- Trong tương lai có thể cần thêm các trường này vào User entity

### 2. Bảo mật
- Sử dụng Role IDs thay vì Role Names
- Validation đầy đủ cho các input
- Xác nhận trước khi thực hiện các thao tác quan trọng

### 3. Performance
- Sử dụng IHttpClientFactory để quản lý HttpClient
- Lazy loading cho các modal
- Pagination để tránh load quá nhiều dữ liệu

## Hướng dẫn sử dụng

### 1. Truy cập trang
- URL: `/Admin/Employees`
- Yêu cầu quyền Admin hoặc Manager

### 2. Thêm nhân viên mới
- Click "Thêm nhân viên"
- Điền thông tin: Email, Password, PhoneNumber, Roles
- Chọn EmailConfirmed, PhoneNumberConfirmed nếu cần

### 3. Quản lý nhân viên
- **Xem chi tiết**: Click dropdown → "Xem chi tiết"
- **Chỉnh sửa**: Click dropdown → "Chỉnh sửa"
- **Phân quyền**: Click dropdown → "Phân quyền"
- **Khóa/Mở khóa**: Click dropdown → "Khóa tài khoản" / "Mở khóa"
- **Xóa**: Click dropdown → "Xóa"

### 4. Tìm kiếm và lọc
- Sử dụng form tìm kiếm ở đầu trang
- Chọn vai trò và trạng thái để lọc
- Click "Làm mới" để reset bộ lọc

## Kết luận
Đã hoàn thành việc cập nhật API và Razor Pages cho quản lý nhân viên với đầy đủ thông tin và tính năng. Hệ thống hiện tại có thể hiển thị và quản lý nhân viên một cách hiệu quả với giao diện thân thiện và bảo mật cao. 