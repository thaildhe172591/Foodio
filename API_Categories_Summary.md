# API Endpoints - Quản lý Danh mục

## Base URL: `/api/admin/categories`

### 📋 Danh sách API Endpoints

| # | Method | Endpoint | Công dụng | Input | Output |
|---|--------|----------|-----------|-------|--------|
| 1 | GET | `/api/admin/categories` | Lấy danh sách danh mục với tìm kiếm và phân trang | `searchTerm`, `sortBy`, `sortOrder`, `page`, `pageSize` | `CategorySearchResultDto` |
| 2 | GET | `/api/admin/categories/{id}` | Lấy thông tin chi tiết danh mục theo ID | `id` | `CategoryDto` |
| 3 | POST | `/api/admin/categories` | Tạo danh mục mới | `CreateCategoryDto` | `CategoryDto` |
| 4 | PUT | `/api/admin/categories/{id}` | Cập nhật thông tin danh mục | `id`, `UpdateCategoryDto` | `204 No Content` |
| 5 | DELETE | `/api/admin/categories/{id}` | Xóa danh mục | `id` | `{ message: string }` |
| 6 | GET | `/api/admin/categories/search` | Tìm kiếm danh mục theo tên | `searchTerm` | `IEnumerable<CategoryDto>` |

---

## 📝 DTOs

### CreateCategoryDto
```json
{
  "name": "Món tráng miệng"
}
```

### UpdateCategoryDto
```json
{
  "name": "Món tráng miệng mới"
}
```

### CategoryDto
```json
{
  "id": "123e4567-e89b-12d3-a456-426614174000",
  "name": "Món chính",
  "menuItemCount": 15
}
```

### CategorySearchDto
```json
{
  "searchTerm": "món",
  "sortBy": "name",
  "sortOrder": "asc",
  "page": 1,
  "pageSize": 20
}
```

### CategorySearchResultDto
```json
{
  "items": [
    {
      "id": "123e4567-e89b-12d3-a456-426614174000",
      "name": "Món chính",
      "menuItemCount": 15
    }
  ],
  "totalCount": 25,
  "currentPage": 1,
  "pageSize": 20,
  "totalPages": 2,
  "hasNextPage": true,
  "hasPreviousPage": false,
  "searchCriteria": {
    "searchTerm": "món",
    "sortBy": "name",
    "sortOrder": "asc",
    "page": 1,
    "pageSize": 20
  },
  "statistics": {
    "totalCategories": 25,
    "categoriesWithItems": 20,
    "emptyCategories": 5,
    "totalMenuItems": 150,
    "averageItemsPerCategory": 6.0
  }
}
```

### CategoryStatistics
```json
{
  "totalCategories": 25,
  "categoriesWithItems": 20,
  "emptyCategories": 5,
  "totalMenuItems": 150,
  "averageItemsPerCategory": 6.0
}
```

## API Quản lý Danh mục (Category Management)

### 1. Lấy danh sách danh mục (API tổng hợp)
- **Endpoint**: `GET /api/admin/categories`
- **Công dụng**: Lấy danh sách danh mục với đầy đủ chức năng tìm kiếm, sắp xếp và phân trang
- **Input**: 
  - `searchTerm` (Optional): Từ khóa tìm kiếm theo tên danh mục
  - `sortBy` (Optional): Sắp xếp theo trường (name, menuItemCount), mặc định: "name"
  - `sortOrder` (Optional): Thứ tự sắp xếp (asc, desc), mặc định: "asc"
  - `page` (Optional): Số trang, mặc định: 1
  - `pageSize` (Optional): Số lượng danh mục trên trang (1-100), mặc định: 20
- **Output**: `CategorySearchResultDto`

### 2. Lấy thông tin chi tiết danh mục
- **Endpoint**: `GET /api/admin/categories/{id}`
- **Công dụng**: Lấy thông tin chi tiết của một danh mục theo ID
- **Input**: 
  - `id` (Required): ID của danh mục (path parameter)
- **Output**: `CategoryDto`

### 3. Tạo danh mục mới
- **Endpoint**: `POST /api/admin/categories`
- **Công dụng**: Tạo danh mục mới trong hệ thống
- **Input**: 
  - `CreateCategoryDto` (Required): Body request
- **Output**: `CategoryDto` (201 Created)

### 4. Cập nhật thông tin danh mục
- **Endpoint**: `PUT /api/admin/categories/{id}`
- **Công dụng**: Cập nhật thông tin của danh mục
- **Input**: 
  - `id` (Required): ID của danh mục (path parameter)
  - `UpdateCategoryDto` (Required): Body request
- **Output**: `204 No Content`

### 5. Xóa danh mục
- **Endpoint**: `DELETE /api/admin/categories/{id}`
- **Công dụng**: Xóa danh mục khỏi hệ thống (chỉ khi không có món ăn)
- **Input**: 
  - `id` (Required): ID của danh mục cần xóa (path parameter)
- **Output**: `{ message: string }`

### 6. Tìm kiếm danh mục theo tên
- **Endpoint**: `GET /api/admin/categories/search`
- **Công dụng**: Tìm kiếm danh mục theo tên (API đơn giản)
- **Input**: 
  - `searchTerm` (Required): Từ khóa tìm kiếm (query parameter)
- **Output**: `IEnumerable<CategoryDto>`

## Business Rules

### Validation Rules
1. **Tên danh mục**: 
   - Không được để trống
   - Tối đa 100 ký tự
   - Không được trùng lặp (không phân biệt hoa thường)

2. **Xóa danh mục**: 
   - Không thể xóa danh mục đã có món ăn
   - Phải xóa tất cả món ăn trong danh mục trước khi xóa danh mục

3. **Phân trang**: 
   - Số trang phải lớn hơn 0
   - Page size phải từ 1-100

### Error Handling
- **400 Bad Request**: Validation errors
- **404 Not Found**: Danh mục không tồn tại
- **500 Internal Server Error**: Lỗi hệ thống

## Ví dụ sử dụng

### 1. Lấy tất cả danh mục
```http
GET /api/admin/categories
```

### 2. Tìm kiếm danh mục chứa "món"
```http
GET /api/admin/categories?searchTerm=món
```

### 3. Sắp xếp theo số lượng món ăn giảm dần
```http
GET /api/admin/categories?sortBy=menuItemCount&sortOrder=desc
```

### 4. Phân trang
```http
GET /api/admin/categories?page=2&pageSize=10
```

### 5. Kết hợp nhiều điều kiện
```http
GET /api/admin/categories?searchTerm=món&sortBy=menuItemCount&sortOrder=desc&page=1&pageSize=20
```

### 6. Tạo danh mục mới
```http
POST /api/admin/categories
Content-Type: application/json

{
  "name": "Món tráng miệng"
}
```

### 7. Cập nhật danh mục
```http
PUT /api/admin/categories/123e4567-e89b-12d3-a456-426614174000
Content-Type: application/json

{
  "name": "Món tráng miệng mới"
}
```

### 8. Xóa danh mục
```http
DELETE /api/admin/categories/123e4567-e89b-12d3-a456-426614174000
```

### 9. Tìm kiếm đơn giản
```http
GET /api/admin/categories/search?searchTerm=món chính
```

## Tính năng

### 1. CRUD Operations
- ✅ **Create**: Tạo danh mục mới
- ✅ **Read**: Lấy danh sách và chi tiết danh mục
- ✅ **Update**: Cập nhật thông tin danh mục
- ✅ **Delete**: Xóa danh mục (với validation)

### 2. Tìm kiếm và Lọc
- ✅ **Tìm kiếm theo tên**: Không phân biệt hoa thường
- ✅ **Sắp xếp**: Theo tên hoặc số lượng món ăn
- ✅ **Phân trang**: Hỗ trợ phân trang với thông tin đầy đủ

### 3. Thống kê
- ✅ **Tổng số danh mục**
- ✅ **Số danh mục có món ăn**
- ✅ **Số danh mục trống**
- ✅ **Tổng số món ăn**
- ✅ **Số món ăn trung bình mỗi danh mục**

### 4. Validation
- ✅ **Dữ liệu đầu vào**: Kiểm tra tên không rỗng
- ✅ **Trùng lặp**: Kiểm tra tên không trùng
- ✅ **Ràng buộc**: Không xóa danh mục có món ăn
- ✅ **Phân trang**: Validation tham số phân trang 