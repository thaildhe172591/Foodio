# API Endpoints - Quản lý Món ăn

## Base URL: `/api/admin/menu-items`

### 📋 Danh sách API Endpoints

| # | Method | Endpoint | Công dụng | Input | Output |
|---|--------|----------|-----------|-------|--------|
| 1 | GET | `/api/admin/menu-items` | Lấy danh sách món ăn với tìm kiếm và phân trang | `searchTerm`, `categoryId`, `isAvailable`, `minPrice`, `maxPrice`, `sortBy`, `sortOrder`, `page`, `pageSize` | `MenuItemSearchResultDto` |
| 2 | GET | `/api/admin/menu-items/{id}` | Lấy thông tin chi tiết món ăn theo ID | `id` | `MenuItemv2Dto` |
| 3 | POST | `/api/admin/menu-items` | Tạo món ăn mới | `CreateMenuItemDto` | `MenuItemv2Dto` |
| 4 | PUT | `/api/admin/menu-items/{id}` | Cập nhật thông tin món ăn | `id`, `UpdateMenuItemDto` | `204 No Content` |
| 5 | PATCH | `/api/admin/menu-items/{id}/availability-status` | Cập nhật trạng thái còn món/hết món | `id`, `bool` | `{ message: string }` |
| 6 | PATCH | `/api/admin/menu-items/{id}/price` | Cập nhật giá món ăn | `id`, `decimal` | `{ message: string }` |
| 7 | PATCH | `/api/admin/menu-items/{id}/image` | Cập nhật hình ảnh món ăn | `id`, `string` | `{ message: string }` |
| 8 | DELETE | `/api/admin/menu-items/{id}` | Xóa món ăn | `id` | `{ message: string }` |

---

## 📝 DTOs

### CreateMenuItemDto
```json
{
  "name": "Phở bò",
  "description": "Phở bò truyền thống",
  "price": 75000,
  "imageUrl": "https://example.com/pho.jpg",
  "categoryId": "123e4567-e89b-12d3-a456-426614174000",
  "isAvailable": true
}
```

### UpdateMenuItemDto
```json
{
  "name": "Phở bò đặc biệt",
  "description": "Phở bò đặc biệt với nhiều thịt",
  "price": 85000,
  "imageUrl": "https://example.com/pho-dac-biet.jpg",
  "categoryId": "123e4567-e89b-12d3-a456-426614174000",
  "isAvailable": true
}
```

### MenuItemv2Dto
```json
{
  "id": "123e4567-e89b-12d3-a456-426614174000",
  "name": "Phở bò",
  "description": "Phở bò truyền thống",
  "price": 75000,
  "imageUrl": "https://example.com/pho.jpg",
  "categoryId": "123e4567-e89b-12d3-a456-426614174000",
  "isAvailable": true
}
```

### MenuItemSearchDto
```json
{
  "searchTerm": "phở",
  "categoryId": "123e4567-e89b-12d3-a456-426614174000",
  "isAvailable": true,
  "minPrice": 50000,
  "maxPrice": 200000,
  "sortBy": "price",
  "sortOrder": "asc",
  "page": 1,
  "pageSize": 20
}
```

### MenuItemSearchResultDto
```json
{
  "items": [
    {
      "id": "123e4567-e89b-12d3-a456-426614174000",
      "name": "Phở bò",
      "description": "Phở bò truyền thống",
      "price": 75000,
      "imageUrl": "https://example.com/pho.jpg",
      "categoryId": "123e4567-e89b-12d3-a456-426614174000",
      "isAvailable": true
    }
  ],
  "totalCount": 50,
  "currentPage": 1,
  "pageSize": 20,
  "totalPages": 3,
  "hasNextPage": true,
  "hasPreviousPage": false,
  "searchCriteria": {
    "searchTerm": "phở",
    "categoryId": "123e4567-e89b-12d3-a456-426614174000",
    "isAvailable": true,
    "minPrice": 50000,
    "maxPrice": 200000,
    "sortBy": "price",
    "sortOrder": "asc",
    "page": 1,
    "pageSize": 20
  },
  "statistics": {
    "availableCount": 35,
    "unavailableCount": 15,
    "averagePrice": 125000,
    "minPrice": 50000,
    "maxPrice": 200000
  }
}
```

### SearchStatistics
```json
{
  "availableCount": 35,
  "unavailableCount": 15,
  "averagePrice": 125000,
  "minPrice": 50000,
  "maxPrice": 200000
}
```

## API Quản lý Món ăn (Menu Items Management)

### 1. Lấy danh sách món ăn (API tổng hợp)
- **Endpoint**: `GET /api/admin/menu-items`
- **Công dụng**: Lấy danh sách món ăn với đầy đủ chức năng tìm kiếm, lọc, sắp xếp và phân trang
- **Input**: 
  - `searchTerm` (Optional): Từ khóa tìm kiếm theo tên món ăn
  - `categoryId` (Optional): ID danh mục để lọc
  - `isAvailable` (Optional): Trạng thái còn món/hết món (true: còn món, false: hết món)
  - `minPrice` (Optional): Giá tối thiểu
  - `maxPrice` (Optional): Giá tối đa
  - `sortBy` (Optional): Sắp xếp theo trường (name, price, createdDate), mặc định: "name"
  - `sortOrder` (Optional): Thứ tự sắp xếp (asc, desc), mặc định: "asc"
  - `page` (Optional): Số trang, mặc định: 1
  - `pageSize` (Optional): Số lượng món ăn trên trang (1-100), mặc định: 20
- **Output**: `MenuItemSearchResultDto`

### 2. Lấy thông tin chi tiết món ăn
- **Endpoint**: `GET /api/admin/menu-items/{id}`
- **Công dụng**: Lấy thông tin chi tiết của một món ăn theo ID
- **Input**: 
  - `id` (Required): ID của món ăn (path parameter)
- **Output**: `MenuItemv2Dto`

### 3. Tạo món ăn mới
- **Endpoint**: `POST /api/admin/menu-items`
- **Công dụng**: Tạo món ăn mới trong menu
- **Input**: 
  - `CreateMenuItemDto` (Required): Body request
- **Output**: `MenuItemv2Dto` (201 Created)

### 4. Cập nhật thông tin món ăn
- **Endpoint**: `PUT /api/admin/menu-items/{id}`
- **Công dụng**: Cập nhật thông tin của món ăn
- **Input**: 
  - `id` (Required): ID của món ăn (path parameter)
  - `UpdateMenuItemDto` (Required): Body request
- **Output**: `204 No Content`

### 5. Cập nhật trạng thái còn món/hết món
- **Endpoint**: `PATCH /api/admin/menu-items/{id}/availability-status`
- **Công dụng**: Cập nhật trạng thái còn món hoặc hết món
- **Input**: 
  - `id` (Required): ID của món ăn (path parameter)
  - `bool` (Required): true (còn món) hoặc false (hết món) (body)
- **Output**: `{ message: string }`

### 6. Cập nhật giá món ăn
- **Endpoint**: `PATCH /api/admin/menu-items/{id}/price`
- **Công dụng**: Cập nhật giá của món ăn
- **Input**: 
  - `id` (Required): ID của món ăn (path parameter)
  - `decimal` (Required): Giá mới (body)
- **Output**: `{ message: string }`

### 7. Cập nhật hình ảnh món ăn
- **Endpoint**: `PATCH /api/admin/menu-items/{id}/image`
- **Công dụng**: Cập nhật URL hình ảnh của món ăn
- **Input**: 
  - `id` (Required): ID của món ăn (path parameter)
  - `string` (Required): URL hình ảnh mới (body)
- **Output**: `{ message: string }`

### 8. Xóa món ăn
- **Endpoint**: `DELETE /api/admin/menu-items/{id}`
- **Công dụng**: Xóa món ăn khỏi menu (chỉ khi không có trong đơn hàng)
- **Input**: 
  - `id` (Required): ID của món ăn cần xóa (path parameter)
- **Output**: `{ message: string }`

## Business Rules

### Validation Rules
1. **Tên món ăn**: 
   - Không được để trống
   - Không được trùng lặp (không phân biệt hoa thường)

2. **Giá món ăn**: 
   - Không được âm
   - Khoảng giá: giá tối thiểu không được lớn hơn giá tối đa

3. **Danh mục**: 
   - Phải tồn tại trong database

4. **Xóa món ăn**: 
   - Không thể xóa món ăn đã có trong đơn hàng

5. **Phân trang**: 
   - Số trang phải lớn hơn 0
   - Page size phải từ 1-100

### Error Handling
- **400 Bad Request**: Validation errors
- **404 Not Found**: Món ăn không tồn tại
- **500 Internal Server Error**: Lỗi hệ thống

## Ví dụ sử dụng

### 1. Lấy tất cả món ăn
```http
GET /api/admin/menu-items
```

### 2. Lọc theo trạng thái còn món
```http
GET /api/admin/menu-items?isAvailable=true
```

### 3. Lọc theo danh mục
```http
GET /api/admin/menu-items?categoryId=123e4567-e89b-12d3-a456-426614174000
```

### 4. Tìm kiếm theo tên
```http
GET /api/admin/menu-items?searchTerm=phở
```

### 5. Lọc theo khoảng giá
```http
GET /api/admin/menu-items?minPrice=50000&maxPrice=200000
```

### 6. Sắp xếp theo giá tăng dần
```http
GET /api/admin/menu-items?sortBy=price&sortOrder=asc
```

### 7. Phân trang
```http
GET /api/admin/menu-items?page=2&pageSize=10
```

### 8. Kết hợp nhiều điều kiện
```http
GET /api/admin/menu-items?searchTerm=phở&categoryId=123&isAvailable=true&minPrice=50000&maxPrice=200000&sortBy=price&sortOrder=asc&page=1&pageSize=20
```

### 9. Tạo món ăn mới
```http
POST /api/admin/menu-items
Content-Type: application/json

{
  "name": "Phở bò",
  "description": "Phở bò truyền thống",
  "price": 75000,
  "imageUrl": "https://example.com/pho.jpg",
  "categoryId": "123e4567-e89b-12d3-a456-426614174000",
  "isAvailable": true
}
```

### 10. Cập nhật món ăn
```http
PUT /api/admin/menu-items/123e4567-e89b-12d3-a456-426614174000
Content-Type: application/json

{
  "name": "Phở bò đặc biệt",
  "description": "Phở bò đặc biệt với nhiều thịt",
  "price": 85000,
  "imageUrl": "https://example.com/pho-dac-biet.jpg",
  "categoryId": "123e4567-e89b-12d3-a456-426614174000",
  "isAvailable": true
}
```

### 11. Cập nhật trạng thái còn món
```http
PATCH /api/admin/menu-items/123e4567-e89b-12d3-a456-426614174000/availability-status
Content-Type: application/json

true
```

### 12. Cập nhật giá món ăn
```http
PATCH /api/admin/menu-items/123e4567-e89b-12d3-a456-426614174000/price
Content-Type: application/json

85000
```

### 13. Cập nhật hình ảnh món ăn
```http
PATCH /api/admin/menu-items/123e4567-e89b-12d3-a456-426614174000/image
Content-Type: application/json

"https://example.com/pho-new.jpg"
```

### 14. Xóa món ăn
```http
DELETE /api/admin/menu-items/123e4567-e89b-12d3-a456-426614174000
```

## Tính năng

### 1. CRUD Operations
- ✅ **Create**: Tạo món ăn mới
- ✅ **Read**: Lấy danh sách và chi tiết món ăn
- ✅ **Update**: Cập nhật thông tin món ăn
- ✅ **Delete**: Xóa món ăn (với validation)

### 2. Partial Update Operations
- ✅ **Cập nhật trạng thái**: Còn món/hết món
- ✅ **Cập nhật giá**: Thay đổi giá món ăn
- ✅ **Cập nhật hình ảnh**: Thay đổi URL hình ảnh

### 3. Tìm kiếm và Lọc
- ✅ **Tìm kiếm theo tên**: Không phân biệt hoa thường
- ✅ **Lọc theo danh mục**: Theo ID danh mục
- ✅ **Lọc theo trạng thái**: Còn món/hết món
- ✅ **Lọc theo giá**: Khoảng giá tối thiểu/tối đa
- ✅ **Sắp xếp**: Theo tên, giá, ngày tạo
- ✅ **Phân trang**: Hỗ trợ phân trang với thông tin đầy đủ

### 4. Thống kê
- ✅ **Số món ăn còn món**
- ✅ **Số món ăn hết món**
- ✅ **Giá trung bình**
- ✅ **Giá thấp nhất/cao nhất**

### 5. Validation
- ✅ **Dữ liệu đầu vào**: Kiểm tra tên, giá không rỗng/âm
- ✅ **Trùng lặp**: Kiểm tra tên không trùng
- ✅ **Ràng buộc**: Không xóa món ăn có trong đơn hàng
- ✅ **Danh mục**: Kiểm tra danh mục tồn tại
- ✅ **Phân trang**: Validation tham số phân trang 