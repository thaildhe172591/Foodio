# Tài liệu API Controllers - Hệ thống Quản lý Nhà hàng Foodio

## Tổng quan
Hệ thống Foodio cung cấp bộ API controllers đầy đủ để quản lý nhà hàng, bao gồm xác thực, quản lý người dùng, menu, đơn hàng, thống kê và giỏ hàng.

---

## 1. AuthController (`/api/auth`)

### Mục đích
Quản lý xác thực người dùng, đăng nhập, đăng ký, quên mật khẩu và quản lý token.

### Các endpoint chính

#### 1.1 Đăng nhập
- **POST** `/api/auth/login`
- **Body**: `{ "email": "string", "password": "string" }`
- **Response**: Thông báo đăng nhập thành công + set cookie token
- **Mô tả**: Xác thực và tạo JWT token, hỗ trợ cookie-based authentication

#### 1.2 Đăng nhập Google
- **POST** `/api/auth/login-google`
- **Body**: `{ "idToken": "string", "email": "string", "name": "string" }`
- **Response**: Thông báo đăng nhập thành công
- **Mô tả**: Xác thực qua Google OAuth, tự động tạo tài khoản nếu chưa tồn tại

#### 1.3 Đăng ký
- **POST** `/api/auth/register`
- **Body**: `{ "email": "string", "password": "string", "confirmPassword": "string" }`
- **Response**: Thông báo đăng ký thành công + gửi email xác nhận
- **Mô tả**: Tạo tài khoản mới với vai trò Customer

#### 1.4 Quên mật khẩu
- **POST** `/api/auth/forgot-password`
- **Body**: `{ "email": "string" }`
- **Response**: Thông báo gửi email reset password
- **Mô tả**: Gửi link reset password qua email

#### 1.5 Đổi mật khẩu
- **POST** `/api/auth/change-password`
- **Body**: `{ "email": "string", "token": "string", "newPassword": "string" }`
- **Response**: Thông báo đổi mật khẩu thành công
- **Mô tả**: Reset password bằng token từ email

#### 1.6 Đăng xuất
- **POST** `/api/auth/logout`
- **Response**: Thông báo đăng xuất thành công
- **Mô tả**: Xóa token và cookie

#### 1.7 Refresh Token
- **GET** `/api/auth/refresh-token`
- **Response**: Token mới
- **Mô tả**: Làm mới access token bằng refresh token

#### 1.8 Xác nhận Email
- **GET** `/api/auth/confirm-email?token=&email=`
- **Response**: Redirect đến trang xác nhận thành công
- **Mô tả**: Xác nhận email từ link trong email

---

## 2. UsersController (`/api/admin/users`)

### Mục đích
Quản lý người dùng dành cho Admin: CRUD, phân quyền, khóa/mở khóa tài khoản.

### Các endpoint chính

#### 2.1 Lấy danh sách người dùng
- **GET** `/api/admin/users?page=1&pageSize=10&searchTerm=`
- **Response**: Danh sách người dùng với phân trang
- **Mô tả**: Lấy danh sách có tìm kiếm và phân trang cơ bản

#### 2.2 Tìm kiếm nâng cao
- **GET** `/api/admin/users/search?searchKeyword=&roleId=&isLocked=`
- **Response**: Danh sách người dùng với bộ lọc chi tiết
- **Mô tả**: Tìm kiếm theo từ khóa, vai trò, trạng thái khóa

#### 2.3 Lấy thông tin chi tiết
- **GET** `/api/admin/users/get-by-id/{id}`
- **Response**: Thông tin chi tiết người dùng
- **Mô tả**: Lấy thông tin đầy đủ bao gồm vai trò

#### 2.4 Tạo người dùng mới
- **POST** `/api/admin/users`
- **Body**: `{ "userName": "string", "email": "string", "password": "string", "phoneNumber": "string", "roleIds": ["string"] }`
- **Response**: Thông tin người dùng đã tạo
- **Mô tả**: Admin tạo tài khoản mới với vai trò

#### 2.5 Cập nhật thông tin
- **PUT** `/api/admin/users/{id}`
- **Body**: `{ "email": "string", "phoneNumber": "string", "userName": "string", "roleIds": ["string"] }`
- **Response**: Thông báo cập nhật thành công
- **Mô tả**: Cập nhật thông tin cơ bản và vai trò

#### 2.6 Cập nhật vai trò
- **PUT** `/api/admin/users/update-roles/{id}`
- **Body**: `{ "roleIds": ["string"] }`
- **Response**: Thông báo cập nhật vai trò thành công
- **Mô tả**: Chỉ cập nhật vai trò của người dùng

#### 2.7 Khóa tài khoản
- **POST** `/api/admin/users/lock/{id}`
- **Body**: `{ "lockoutDays": 30 }`
- **Response**: Thông báo khóa thành công
- **Mô tả**: Khóa tài khoản trong số ngày nhất định

#### 2.8 Mở khóa tài khoản
- **POST** `/api/admin/users/unlock/{id}`
- **Response**: Thông báo mở khóa thành công
- **Mô tả**: Mở khóa tài khoản ngay lập tức

#### 2.9 Xóa người dùng
- **DELETE** `/api/admin/users/delete/{id}`
- **Response**: Thông báo xóa thành công
- **Mô tả**: Xóa người dùng (không thể xóa admin/system)

#### 2.10 Lấy danh sách vai trò
- **GET** `/api/admin/users/get-all-roles`
- **Response**: Danh sách tất cả vai trò trong hệ thống
- **Mô tả**: Dùng cho dropdown chọn vai trò

---

## 3. MenuItemsController (`/api/admin/menu-items`)

### Mục đích
Quản lý món ăn trong menu: CRUD, tìm kiếm, lọc, sắp xếp, phân trang.

### Các endpoint chính

#### 3.1 Lấy danh sách món ăn
- **GET** `/api/admin/menu-items?searchTerm=&categoryId=&isAvailable=&minPrice=&maxPrice=&sortBy=name&sortOrder=asc&page=1&pageSize=20`
- **Response**: Danh sách món ăn với phân trang và thống kê
- **Mô tả**: API tổng hợp với đầy đủ tính năng tìm kiếm, lọc, sắp xếp

#### 3.2 Lấy thông tin món ăn
- **GET** `/api/admin/menu-items/{id}`
- **Response**: Thông tin chi tiết món ăn
- **Mô tả**: Lấy thông tin đầy đủ của một món ăn

#### 3.3 Tạo món ăn mới
- **POST** `/api/admin/menu-items`
- **Body**: `{ "name": "string", "description": "string", "price": 0, "imageUrl": "string", "categoryId": "guid", "isAvailable": true }`
- **Response**: Thông tin món ăn đã tạo
- **Mô tả**: Tạo món ăn mới với validation đầy đủ

#### 3.4 Cập nhật món ăn
- **PUT** `/api/admin/menu-items/{id}`
- **Body**: `{ "name": "string", "description": "string", "price": 0, "imageUrl": "string", "categoryId": "guid", "isAvailable": true }`
- **Response**: 204 No Content
- **Mô tả**: Cập nhật thông tin đầy đủ món ăn

#### 3.5 Cập nhật trạng thái
- **PATCH** `/api/admin/menu-items/{id}/availability-status`
- **Body**: `true/false`
- **Response**: Thông báo cập nhật thành công
- **Mô tả**: Chỉ cập nhật trạng thái còn món/hết món

#### 3.6 Cập nhật giá
- **PATCH** `/api/admin/menu-items/{id}/price`
- **Body**: `decimal`
- **Response**: Thông báo cập nhật giá thành công
- **Mô tả**: Chỉ cập nhật giá món ăn

#### 3.7 Cập nhật hình ảnh
- **PATCH** `/api/admin/menu-items/{id}/image`
- **Body**: `"string"`
- **Response**: Thông báo cập nhật hình ảnh thành công
- **Mô tả**: Chỉ cập nhật URL hình ảnh

#### 3.8 Xóa món ăn
- **DELETE** `/api/admin/menu-items/{id}`
- **Response**: Thông báo xóa thành công
- **Mô tả**: Xóa món ăn (kiểm tra ràng buộc với đơn hàng)

---

## 4. CategoryController (`/api/admin/categories`)

### Mục đích
Quản lý danh mục món ăn: CRUD, tìm kiếm, sắp xếp, phân trang.

### Các endpoint chính

#### 4.1 Lấy danh sách danh mục
- **GET** `/api/admin/categories?searchTerm=&sortBy=name&sortOrder=asc&page=1&pageSize=20`
- **Response**: Danh sách danh mục với phân trang
- **Mô tả**: Lấy danh sách có tìm kiếm và sắp xếp

#### 4.2 Lấy thông tin danh mục
- **GET** `/api/admin/categories/{id}`
- **Response**: Thông tin chi tiết danh mục
- **Mô tả**: Lấy thông tin đầy đủ của một danh mục

#### 4.3 Tạo danh mục mới
- **POST** `/api/admin/categories`
- **Body**: `{ "name": "string", "description": "string" }`
- **Response**: Thông tin danh mục đã tạo
- **Mô tả**: Tạo danh mục mới với validation

#### 4.4 Cập nhật danh mục
- **PUT** `/api/admin/categories/{id}`
- **Body**: `{ "name": "string", "description": "string" }`
- **Response**: 204 No Content
- **Mô tả**: Cập nhật thông tin danh mục

#### 4.5 Xóa danh mục
- **DELETE** `/api/admin/categories/{id}`
- **Response**: Thông báo xóa thành công
- **Mô tả**: Xóa danh mục (kiểm tra ràng buộc với món ăn)

#### 4.6 Tìm kiếm danh mục
- **GET** `/api/admin/categories/search?searchTerm=`
- **Response**: Danh sách danh mục khớp từ khóa
- **Mô tả**: API tìm kiếm đơn giản

---

## 5. MenuController (`/api/menu`)

### Mục đích
API công khai cho khách hàng xem menu.

### Các endpoint chính

#### 5.1 Lấy danh sách món ăn
- **POST** `/api/menu/list`
- **Body**: `{ "search": "string", "categoryId": "guid" }`
- **Response**: Danh sách món ăn công khai
- **Mô tả**: API cho khách hàng xem menu với bộ lọc cơ bản

---

## 6. CartController (`/api/cart`)

### Mục đích
Quản lý giỏ hàng của khách hàng: thêm món, xem giỏ hàng, đặt hàng.

### Các endpoint chính

#### 6.1 Thêm vào giỏ hàng
- **POST** `/api/cart/add`
- **Body**: `{ "menuItemId": "guid", "quantity": 1, "note": "string" }`
- **Response**: Thông báo thêm thành công
- **Mô tả**: Thêm món vào giỏ hàng của user hiện tại

#### 6.2 Thêm món từ menu
- **POST** `/api/cart/add-menu-item`
- **Body**: `{ "menuItemId": "guid", "quantity": 1 }`
- **Response**: Thông báo thêm thành công
- **Mô tả**: Thêm món từ menu vào giỏ hàng

#### 6.3 Xem giỏ hàng
- **GET** `/api/cart/my-cart`
- **Response**: Danh sách món trong giỏ hàng
- **Mô tả**: Lấy tất cả món trong giỏ hàng của user

#### 6.4 Lấy giỏ hàng hiện tại
- **GET** `/api/cart/current`
- **Response**: Thông tin giỏ hàng chi tiết
- **Mô tả**: Lấy giỏ hàng với thông tin tổng tiền

#### 6.5 Cập nhật thông tin giao hàng
- **POST** `/api/cart/delivery-info`
- **Body**: `{ "address": "string", "phone": "string", "note": "string" }`
- **Response**: Thông báo lưu thành công
- **Mô tả**: Lưu thông tin giao hàng cho đơn hàng

#### 6.6 Xác nhận đơn hàng
- **POST** `/api/cart/confirm`
- **Body**: `{ "orderType": "string", "paymentMethod": "string" }`
- **Response**: Thông báo đặt hàng thành công
- **Mô tả**: Chuyển giỏ hàng thành đơn hàng

---

## 7. StatisticsController (`/api/admin/statistics`)

### Mục đích
Cung cấp thống kê tổng quan về doanh thu, đơn hàng, người dùng cho Admin.

### Các endpoint chính

#### 7.1 Thống kê tổng hệ thống
- **GET** `/api/admin/statistics/system?startDate=&endDate=`
- **Response**: Thống kê tổng quan toàn hệ thống
- **Mô tả**: Tổng doanh thu, đơn hàng, người dùng, món ăn

#### 7.2 Doanh thu theo loại đơn hàng
- **GET** `/api/admin/statistics/revenue/by-order-type?startDate=&endDate=`
- **Response**: Doanh thu phân tích theo từng loại đơn hàng
- **Mô tả**: Dine-in, Takeaway, Delivery...

#### 7.3 Doanh thu theo người dùng
- **GET** `/api/admin/statistics/revenue/by-user?top=10&startDate=&endDate=`
- **Response**: Top người dùng có doanh thu cao nhất
- **Mô tả**: Xác định khách hàng VIP

#### 7.4 Doanh thu theo thời gian
- **GET** `/api/admin/statistics/revenue/by-time?startDate=&endDate=`
- **Response**: Doanh thu theo từng ngày
- **Mô tả**: Biểu đồ doanh thu theo thời gian

#### 7.5 Doanh thu theo tháng
- **GET** `/api/admin/statistics/revenue/by-month?year=2024`
- **Response**: Doanh thu từng tháng trong năm
- **Mô tả**: Phân tích xu hướng theo tháng

#### 7.6 Doanh thu theo năm
- **GET** `/api/admin/statistics/revenue/by-year`
- **Response**: Doanh thu từng năm
- **Mô tả**: Xu hướng dài hạn

#### 7.7 Doanh thu theo khoảng thời gian
- **GET** `/api/admin/statistics/revenue/by-time-range?startDate=&endDate=`
- **Response**: Thống kê tổng hợp cho khoảng thời gian
- **Mô tả**: Doanh thu trung bình, min, max

#### 7.8 Top người dùng
- **GET** `/api/admin/statistics/top-users?top=10&startDate=&endDate=`
- **Response**: Danh sách top người dùng
- **Mô tả**: Thông tin chi tiết khách hàng VIP

#### 7.9 Top loại đơn hàng
- **GET** `/api/admin/statistics/top-order-types?top=5&startDate=&endDate=`
- **Response**: Loại đơn hàng phổ biến nhất
- **Mô tả**: Phân tích hiệu quả từng kênh bán

---

## Các tính năng chung

### 1. Authentication & Authorization
- **JWT Token**: Sử dụng JWT cho xác thực
- **Cookie-based**: Lưu token trong HTTP-only cookie
- **Role-based**: Phân quyền theo vai trò (Admin, Manager, Staff, Customer)
- **Refresh Token**: Tự động làm mới token

### 2. Validation & Error Handling
- **Model Validation**: Validation đầy đủ cho tất cả input
- **Custom Exceptions**: Các exception tùy chỉnh
- **Consistent Response**: Format response thống nhất
- **Error Messages**: Thông báo lỗi tiếng Việt

### 3. Search & Filtering
- **Pagination**: Phân trang cho tất cả danh sách
- **Sorting**: Sắp xếp theo nhiều tiêu chí
- **Advanced Search**: Tìm kiếm nâng cao với nhiều bộ lọc
- **Performance**: Tối ưu query database

### 4. Security Features
- **Input Sanitization**: Làm sạch dữ liệu đầu vào
- **SQL Injection Prevention**: Sử dụng Entity Framework
- **XSS Protection**: Encode output
- **CSRF Protection**: Anti-forgery token

### 5. Documentation
- **Swagger**: API documentation tự động
- **XML Comments**: Mô tả chi tiết cho mỗi endpoint
- **Examples**: Ví dụ request/response
- **Error Codes**: Mô tả các mã lỗi

---

## Hướng dẫn sử dụng

### 1. Authentication Flow
```
1. POST /api/auth/register → Đăng ký tài khoản
2. GET /api/auth/confirm-email → Xác nhận email
3. POST /api/auth/login → Đăng nhập
4. Sử dụng các API khác với token
5. GET /api/auth/refresh-token → Làm mới token khi hết hạn
6. POST /api/auth/logout → Đăng xuất
```

### 2. Admin Management Flow
```
1. Đăng nhập với tài khoản Admin
2. Quản lý người dùng: /api/admin/users
3. Quản lý danh mục: /api/admin/categories  
4. Quản lý món ăn: /api/admin/menu-items
5. Xem thống kê: /api/admin/statistics
```

### 3. Customer Order Flow
```
1. Đăng nhập/đăng ký
2. Xem menu: /api/menu/list
3. Thêm vào giỏ: /api/cart/add
4. Xem giỏ hàng: /api/cart/current
5. Cập nhật thông tin: /api/cart/delivery-info
6. Đặt hàng: /api/cart/confirm
```

---

## Lưu ý kỹ thuật

### 1. Database
- **Entity Framework Core**: ORM chính
- **SQL Server**: Database backend
- **Migrations**: Quản lý schema database
- **Relationships**: Thiết kế quan hệ chuẩn

### 2. Performance
- **Async/Await**: Tất cả operations đều async
- **Pagination**: Giới hạn số lượng record trả về
- **Caching**: Cache cho dữ liệu ít thay đổi
- **Query Optimization**: Tối ưu EF queries

### 3. Monitoring
- **Logging**: Log chi tiết các operations
- **Error Tracking**: Theo dõi lỗi hệ thống
- **Performance Metrics**: Đo lường hiệu suất
- **Health Checks**: Kiểm tra sức khỏe hệ thống

### 4. Deployment
- **Configuration**: Quản lý config theo environment
- **Secrets**: Bảo mật thông tin nhạy cảm
- **Docker**: Container hóa application
- **CI/CD**: Tự động deploy

---

## Kết luận

Hệ thống API Foodio được thiết kế toàn diện với đầy đủ tính năng cần thiết cho một ứng dụng quản lý nhà hàng. Từ xác thực người dùng, quản lý menu, đơn hàng đến thống kê doanh thu, tất cả đều được implement với tiêu chuẩn cao về bảo mật, hiệu suất và khả năng mở rộng.

**Điểm mạnh:**
- API design chuẩn RESTful
- Authentication & Authorization đầy đủ
- Validation và Error Handling tốt
- Documentation chi tiết
- Performance tối ưu
- Bảo mật cao

**Phù hợp cho:**
- Nhà hàng vừa và nhỏ
- Chuỗi nhà hàng
- Food delivery services
- Café và fast food

Hệ thống có thể dễ dàng mở rộng thêm các tính năng như loyalty program, promotions, inventory management, và tích hợp với các dịch vụ bên thứ ba. 