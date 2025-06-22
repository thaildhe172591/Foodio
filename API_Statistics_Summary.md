# API Thống Kê Hệ Thống - Foodio

## Tổng Quan
API thống kê hệ thống cung cấp các endpoint để lấy thông tin thống kê toàn diện về doanh thu, đơn hàng, người dùng và các chỉ số quan trọng khác của hệ thống Foodio. API được xây dựng theo mô hình 3-layer (Presentation, Business Logic, Data Access) với đầy đủ comment chi tiết.

## Kiến Trúc 3-Layer

### 1. Presentation Layer (Controllers)
- **StatisticsController**: Xử lý HTTP requests, validation, và response formatting
- Có đầy đủ comment XML documentation cho Swagger
- Sử dụng ProducesResponseType để định nghĩa response types
- Validation chặt chẽ cho input parameters

### 2. Business Logic Layer (Services)
- **IStatisticsService**: Interface định nghĩa các method thống kê
- **StatisticsService**: Implementation chứa business logic
- Tách biệt logic xử lý dữ liệu khỏi controller
- Sử dụng private methods để tái sử dụng code

### 3. Data Access Layer (Repositories)
- Sử dụng Entity Framework Core trực tiếp trong service
- Optimized queries với Include và GroupBy
- Async/await cho tất cả database operations

## Bảng Tổng Hợp API

| Method | Endpoint | Mô Tả | Parameters | Response Type |
|--------|----------|-------|------------|---------------|
| GET | `/api/admin/statistics/system` | Thống kê toàn hệ thống | `startDate`, `endDate` | `SystemStatisticsDto` |
| GET | `/api/admin/statistics/revenue/by-order-type` | Doanh thu theo loại đơn hàng | `startDate`, `endDate` | `IEnumerable<RevenueByOrderTypeDto>` |
| GET | `/api/admin/statistics/revenue/by-user` | Doanh thu theo người dùng | `startDate`, `endDate`, `top` | `IEnumerable<RevenueByUserDto>` |
| GET | `/api/admin/statistics/revenue/by-time` | Doanh thu theo thời gian | `startDate`, `endDate` | `IEnumerable<RevenueByTimeDto>` |
| GET | `/api/admin/statistics/revenue/by-month` | Doanh thu theo tháng | `year` | `IEnumerable<RevenueByMonthDto>` |
| GET | `/api/admin/statistics/revenue/by-year` | Doanh thu theo năm | - | `IEnumerable<RevenueByYearDto>` |
| GET | `/api/admin/statistics/revenue/by-time-range` | Doanh thu theo khoảng thời gian | `startDate`, `endDate` | `RevenueByTimeRangeDto` |
| GET | `/api/admin/statistics/top-users` | Top người dùng có doanh thu cao | `top`, `startDate`, `endDate` | `IEnumerable<TopUserDto>` |
| GET | `/api/admin/statistics/top-order-types` | Top loại đơn hàng có doanh thu cao | `top`, `startDate`, `endDate` | `IEnumerable<TopOrderTypeDto>` |

## DTOs

### SystemStatisticsDto
```csharp
/// <summary>
/// DTO cho thống kê toàn hệ thống
/// </summary>
public class SystemStatisticsDto
{
    /// <summary>
    /// Tổng doanh thu toàn hệ thống
    /// </summary>
    public decimal TotalRevenue { get; set; }

    /// <summary>
    /// Tổng số đơn hàng
    /// </summary>
    public int TotalOrders { get; set; }

    /// <summary>
    /// Tổng số người dùng
    /// </summary>
    public int TotalUsers { get; set; }

    /// <summary>
    /// Tổng số món ăn
    /// </summary>
    public int TotalMenuItems { get; set; }

    /// <summary>
    /// Tổng số danh mục
    /// </summary>
    public int TotalCategories { get; set; }

    /// <summary>
    /// Doanh thu trung bình mỗi đơn hàng
    /// </summary>
    public decimal AverageOrderValue { get; set; }

    /// <summary>
    /// Thống kê theo loại đơn hàng
    /// </summary>
    public List<RevenueByOrderTypeDto> RevenueByOrderType { get; set; } = new();

    /// <summary>
    /// Thống kê theo người dùng
    /// </summary>
    public List<RevenueByUserDto> RevenueByUser { get; set; } = new();

    /// <summary>
    /// Thống kê theo thời gian
    /// </summary>
    public List<RevenueByTimeDto> RevenueByTime { get; set; } = new();

    /// <summary>
    /// Thống kê theo tháng
    /// </summary>
    public List<RevenueByMonthDto> RevenueByMonth { get; set; } = new();

    /// <summary>
    /// Thống kê theo năm
    /// </summary>
    public List<RevenueByYearDto> RevenueByYear { get; set; } = new();
}
```

### RevenueByOrderTypeDto
```csharp
/// <summary>
/// DTO cho thống kê doanh thu theo loại đơn hàng
/// </summary>
public class RevenueByOrderTypeDto
{
    /// <summary>
    /// Tên loại đơn hàng
    /// </summary>
    public string OrderTypeName { get; set; } = string.Empty;

    /// <summary>
    /// Tổng doanh thu
    /// </summary>
    public decimal TotalRevenue { get; set; }

    /// <summary>
    /// Số lượng đơn hàng
    /// </summary>
    public int OrderCount { get; set; }
}
```

### RevenueByUserDto
```csharp
/// <summary>
/// DTO cho thống kê doanh thu theo người dùng
/// </summary>
public class RevenueByUserDto
{
    /// <summary>
    /// Tên người dùng
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Tổng doanh thu
    /// </summary>
    public decimal TotalRevenue { get; set; }

    /// <summary>
    /// Số lượng đơn hàng
    /// </summary>
    public int OrderCount { get; set; }
}
```

### RevenueByTimeDto
```csharp
/// <summary>
/// DTO cho thống kê doanh thu theo thời gian
/// </summary>
public class RevenueByTimeDto
{
    /// <summary>
    /// Ngày
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Tổng doanh thu
    /// </summary>
    public decimal TotalRevenue { get; set; }

    /// <summary>
    /// Số lượng đơn hàng
    /// </summary>
    public int OrderCount { get; set; }
}
```

### RevenueByMonthDto
```csharp
/// <summary>
/// DTO cho thống kê doanh thu theo tháng
/// </summary>
public class RevenueByMonthDto
{
    /// <summary>
    /// Năm
    /// </summary>
    public int Year { get; set; }

    /// <summary>
    /// Tháng
    /// </summary>
    public int Month { get; set; }

    /// <summary>
    /// Tên tháng
    /// </summary>
    public string MonthName { get; set; } = string.Empty;

    /// <summary>
    /// Tổng doanh thu
    /// </summary>
    public decimal TotalRevenue { get; set; }

    /// <summary>
    /// Số lượng đơn hàng
    /// </summary>
    public int OrderCount { get; set; }
}
```

### RevenueByYearDto
```csharp
/// <summary>
/// DTO cho thống kê doanh thu theo năm
/// </summary>
public class RevenueByYearDto
{
    /// <summary>
    /// Năm
    /// </summary>
    public int Year { get; set; }

    /// <summary>
    /// Tổng doanh thu
    /// </summary>
    public decimal TotalRevenue { get; set; }

    /// <summary>
    /// Số lượng đơn hàng
    /// </summary>
    public int OrderCount { get; set; }
}
```

### RevenueByTimeRangeDto
```csharp
/// <summary>
/// DTO cho thống kê doanh thu theo khoảng thời gian
/// </summary>
public class RevenueByTimeRangeDto
{
    /// <summary>
    /// Ngày bắt đầu
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Ngày kết thúc
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Tổng doanh thu
    /// </summary>
    public decimal TotalRevenue { get; set; }

    /// <summary>
    /// Số lượng đơn hàng
    /// </summary>
    public int OrderCount { get; set; }

    /// <summary>
    /// Doanh thu trung bình mỗi ngày
    /// </summary>
    public decimal AverageDailyRevenue { get; set; }

    /// <summary>
    /// Số đơn hàng trung bình mỗi ngày
    /// </summary>
    public double AverageDailyOrders { get; set; }
}
```

### TopUserDto
```csharp
/// <summary>
/// DTO cho thống kê top người dùng
/// </summary>
public class TopUserDto
{
    /// <summary>
    /// Tên người dùng
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Email người dùng
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Tổng doanh thu
    /// </summary>
    public decimal TotalRevenue { get; set; }

    /// <summary>
    /// Số lượng đơn hàng
    /// </summary>
    public int OrderCount { get; set; }

    /// <summary>
    /// Doanh thu trung bình mỗi đơn hàng
    /// </summary>
    public decimal AverageOrderValue { get; set; }
}
```

### TopOrderTypeDto
```csharp
/// <summary>
/// DTO cho thống kê top loại đơn hàng
/// </summary>
public class TopOrderTypeDto
{
    /// <summary>
    /// Tên loại đơn hàng
    /// </summary>
    public string OrderTypeName { get; set; } = string.Empty;

    /// <summary>
    /// Tổng doanh thu
    /// </summary>
    public decimal TotalRevenue { get; set; }

    /// <summary>
    /// Số lượng đơn hàng
    /// </summary>
    public int OrderCount { get; set; }

    /// <summary>
    /// Tỷ lệ phần trăm so với tổng doanh thu
    /// </summary>
    public decimal RevenuePercentage { get; set; }
}
```

## Chi Tiết API

### 1. GET /api/admin/statistics/system
**Mô tả:** Lấy thống kê toàn hệ thống bao gồm tất cả các chỉ số quan trọng.

**Parameters:**
- `startDate` (optional): Ngày bắt đầu lọc dữ liệu
- `endDate` (optional): Ngày kết thúc lọc dữ liệu

**Response:**
```json
{
    "totalRevenue": 15000000,
    "totalOrders": 1500,
    "totalUsers": 500,
    "totalMenuItems": 100,
    "totalCategories": 10,
    "averageOrderValue": 10000,
    "revenueByOrderType": [
        {
            "orderTypeName": "Dine-in",
            "totalRevenue": 8000000,
            "orderCount": 800
        }
    ],
    "revenueByUser": [...],
    "revenueByTime": [...],
    "revenueByMonth": [...],
    "revenueByYear": [...]
}
```

### 2. GET /api/admin/statistics/revenue/by-order-type
**Mô tả:** Lấy thống kê doanh thu theo loại đơn hàng.

**Parameters:**
- `startDate` (optional): Ngày bắt đầu
- `endDate` (optional): Ngày kết thúc

**Response:**
```json
[
    {
        "orderTypeName": "Dine-in",
        "totalRevenue": 8000000,
        "orderCount": 800
    },
    {
        "orderTypeName": "Takeaway",
        "totalRevenue": 5000000,
        "orderCount": 500
    }
]
```

### 3. GET /api/admin/statistics/revenue/by-user
**Mô tả:** Lấy thống kê doanh thu theo người dùng.

**Parameters:**
- `startDate` (optional): Ngày bắt đầu
- `endDate` (optional): Ngày kết thúc
- `top` (optional, default: 10): Số lượng top người dùng

**Response:**
```json
[
    {
        "userName": "user1",
        "totalRevenue": 500000,
        "orderCount": 50
    }
]
```

### 4. GET /api/admin/statistics/revenue/by-time
**Mô tả:** Lấy thống kê doanh thu theo thời gian (ngày).

**Parameters:**
- `startDate` (optional): Ngày bắt đầu
- `endDate` (optional): Ngày kết thúc

**Response:**
```json
[
    {
        "date": "2024-01-01",
        "totalRevenue": 100000,
        "orderCount": 10
    }
]
```

### 5. GET /api/admin/statistics/revenue/by-month
**Mô tả:** Lấy thống kê doanh thu theo tháng.

**Parameters:**
- `year` (optional, default: năm hiện tại): Năm cần thống kê

**Response:**
```json
[
    {
        "year": 2024,
        "month": 1,
        "monthName": "Tháng 1",
        "totalRevenue": 3000000,
        "orderCount": 300
    }
]
```

### 6. GET /api/admin/statistics/revenue/by-year
**Mô tả:** Lấy thống kê doanh thu theo năm.

**Response:**
```json
[
    {
        "year": 2024,
        "totalRevenue": 15000000,
        "orderCount": 1500
    }
]
```

### 7. GET /api/admin/statistics/revenue/by-time-range
**Mô tả:** Lấy thống kê doanh thu theo khoảng thời gian cụ thể.

**Parameters:**
- `startDate` (required): Ngày bắt đầu
- `endDate` (required): Ngày kết thúc

**Response:**
```json
{
    "startDate": "2024-01-01",
    "endDate": "2024-01-31",
    "totalRevenue": 3000000,
    "orderCount": 300,
    "averageDailyRevenue": 100000,
    "averageDailyOrders": 10
}
```

### 8. GET /api/admin/statistics/top-users
**Mô tả:** Lấy danh sách top người dùng có doanh thu cao nhất.

**Parameters:**
- `top` (optional, default: 10): Số lượng top
- `startDate` (optional): Ngày bắt đầu
- `endDate` (optional): Ngày kết thúc

**Response:**
```json
[
    {
        "userName": "user1",
        "email": "user1@example.com",
        "totalRevenue": 500000,
        "orderCount": 50,
        "averageOrderValue": 10000
    }
]
```

### 9. GET /api/admin/statistics/top-order-types
**Mô tả:** Lấy danh sách top loại đơn hàng có doanh thu cao nhất.

**Parameters:**
- `top` (optional, default: 5): Số lượng top
- `startDate` (optional): Ngày bắt đầu
- `endDate` (optional): Ngày kết thúc

**Response:**
```json
[
    {
        "orderTypeName": "Dine-in",
        "totalRevenue": 8000000,
        "orderCount": 800,
        "revenuePercentage": 53.33
    }
]
```

## Business Rules

### 1. Quyền Truy Cập
- Tất cả API thống kê yêu cầu quyền Admin
- Hiện tại đang tạm thời bỏ qua authorization để test

### 2. Validation
- `startDate` không được lớn hơn `endDate`
- `top` parameter phải là số dương
- `year` parameter phải là năm hợp lệ (1900-2100)

### 3. Xử Lý Dữ Liệu
- Tất cả doanh thu được tính theo trường `Total` của Order
- Thời gian được tính theo trường `CreatedAt` của Order
- Sắp xếp kết quả theo doanh thu giảm dần (trừ API theo thời gian)

### 4. Performance
- Sử dụng Entity Framework với Include để tối ưu query
- Sử dụng async/await cho tất cả database operations
- GroupBy và Select để tính toán thống kê
- Private methods để tái sử dụng logic

### 5. Architecture
- Tuân thủ mô hình 3-layer
- Dependency Injection cho loose coupling
- Interface segregation principle
- Single responsibility principle

## Error Handling

### 1. Validation Errors
```json
{
    "message": "Ngày bắt đầu không thể lớn hơn ngày kết thúc"
}
```

### 2. Server Errors
```json
{
    "message": "Lỗi khi lấy thống kê hệ thống",
    "error": "Chi tiết lỗi"
}
```

### 3. HTTP Status Codes
- `200 OK`: Thành công
- `400 Bad Request`: Lỗi validation
- `500 Internal Server Error`: Lỗi server

## Ví Dụ Sử Dụng

### 1. Lấy thống kê toàn hệ thống tháng 1/2024
```http
GET /api/admin/statistics/system?startDate=2024-01-01&endDate=2024-01-31
```

### 2. Lấy top 5 người dùng có doanh thu cao nhất
```http
GET /api/admin/statistics/top-users?top=5
```

### 3. Lấy thống kê doanh thu theo tháng năm 2024
```http
GET /api/admin/statistics/revenue/by-month?year=2024
```

### 4. Lấy thống kê doanh thu theo khoảng thời gian
```http
GET /api/admin/statistics/revenue/by-time-range?startDate=2024-01-01&endDate=2024-03-31
```

## Tính Năng Đặc Biệt

### 1. Thống Kê Toàn Diện
- API `/system` cung cấp tất cả thông tin thống kê trong một lần gọi
- Bao gồm tổng quan và chi tiết theo nhiều chiều

### 2. Lọc Theo Thời Gian
- Hỗ trợ lọc dữ liệu theo khoảng thời gian tùy chọn
- Thống kê theo ngày, tháng, năm

### 3. Top Rankings
- Top người dùng có doanh thu cao nhất
- Top loại đơn hàng có doanh thu cao nhất
- Tính toán tỷ lệ phần trăm

### 4. Tính Toán Trung Bình
- Doanh thu trung bình mỗi đơn hàng
- Doanh thu trung bình mỗi ngày
- Số đơn hàng trung bình mỗi ngày

### 5. Đa Dạng Thống Kê
- Theo loại đơn hàng
- Theo người dùng
- Theo thời gian (ngày/tháng/năm)
- Theo khoảng thời gian tùy chọn

### 6. Documentation Chi Tiết
- XML comments cho Swagger
- ProducesResponseType attributes
- Ví dụ request/response
- Mô tả business rules

### 7. Code Quality
- Clean Architecture principles
- SOLID principles
- Async/await patterns
- Error handling best practices 