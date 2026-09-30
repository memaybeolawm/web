# 🧶 Handmade Corner – Hướng Dẫn ASP.NET Core & Entity Framework Core (Cho Người Mới Học)

Dự án Backend của Handmade Corner đã được chuyển đổi hoàn toàn sang **ASP.NET Core Web API** với kiến trúc **MVC** và kết nối cơ sở dữ liệu **SQLite** thông qua ORM **Entity Framework Core (EF Core)**.

---

## 📁 1. Cấu Trúc Thư Mục Chuẩn MVC

```text
d:\web\HandmadeCornerApi\
│
├── 📂 Models/                  # [M] MODEL: Định nghĩa cấu trúc bảng dữ liệu trong C#
│   ├── Category.cs             # Bảng danh mục sản phẩm (categories)
│   ├── Product.cs              # Bảng sản phẩm (products)
│   ├── User.cs                 # Bảng khách hàng (users) & admin (admins)
│   ├── Order.cs                # Bảng đơn hàng (orders) & chi tiết đơn (order_items)
│   ├── Review.cs               # Bảng đánh giá sản phẩm (reviews)
│   └── Dtos.cs                 # DTO: Các class truyền nhận dữ liệu API Request/Response
│
├── 📂 Data/                    # Tầng kết nối Cơ Sở Dữ Liệu qua Entity Framework Core
│   ├── AppDbContext.cs         # DbContext: "Trái tim" kết nối SQLite qua EF Core
│   └── DbSeeder.cs             # Tự động tạo bảng và nạp dữ liệu mẫu ban đầu
│
├── 📂 Controllers/             # [C] CONTROLLER: Xử lý logic và tiếp nhận Request từ web
│   ├── CategoriesController.cs # API /api/categories
│   ├── ProductsController.cs   # API /api/products
│   ├── AuthController.cs       # API /api/auth/register, login, profile, admin/login
│   ├── OrdersController.cs     # API /api/orders (Đặt hàng, xem đơn, hủy đơn)
│   ├── ReviewsController.cs    # API /api/reviews
│   └── StatsController.cs      # API /api/stats (Thống kê Dashboard Admin)
│
├── 📂 Views / Static Web       # [V] VIEW: Toàn bộ HTML/CSS/JS nằm ở thư mục cha d:\web\
│
└── Program.cs                  # File khởi động chính (Cổng 8888, CORS, Static Files, Seed DB)
```

---

## 💡 2. Các Khái Niệm Quan Trọng Cần Nhớ (Dành Cho Người Mới)

### A. Entity Framework Core (EF Core) là gì?
- **ORM (Object-Relational Mapping):** Là cầu nối giúp bạn thao tác với cơ sở dữ liệu (SQLite) bằng **đối tượng C#** thay vì phải viết câu lệnh SQL thủ công.
- Thay vì viết: `SELECT * FROM products WHERE price >= 100000;`
- Trong EF Core bạn chỉ cần viết: `await _context.Products.Where(p => p.Price >= 100000).ToListAsync();`
- EF Core sẽ tự động biên dịch code C# thành câu lệnh SQL chuẩn và an toàn nhất (chống hoàn toàn SQL Injection).

### B. Mô hình MVC hoạt động như thế nào?
1. **Model (Dữ liệu):** Định nghĩa các thực thể như `Product`, `Order`, `User`.
2. **Controller (Điều khiển):** Tiếp nhận yêu cầu từ trình duyệt, gọi `AppDbContext` để lấy/ghi dữ liệu, sau đó trả kết quả về dưới dạng JSON.
3. **View (Giao diện):** Các trang HTML/CSS/JS (`index.html`, `products.html`, `orders.html`, `admin/index.html`) hiển thị dữ liệu cho người dùng.

### C. Database Transaction trong EF Core
Khi đặt hàng, hệ thống cần thực hiện 2 việc cùng lúc:
1. Tạo đơn hàng mới trong bảng `orders`
2. Trừ số lượng tồn kho trong bảng `products`

Nếu một trong hai việc bị lỗi, **Transaction** sẽ `Rollback` (hoàn tác) lại toàn bộ để dữ liệu không bao giờ bị sai lệch.

---

## 🚀 3. Lệnh Khởi Động Server

Mở terminal PowerShell tại thư mục `d:\web\HandmadeCornerApi` và chạy:

```powershell
dotnet run
```

Server sẽ khởi chạy tại:
- 🌐 Trang chủ: [http://localhost:8888](http://localhost:8888)
- 🔐 Quản trị Admin: [http://localhost:8888/admin/login.html](http://localhost:8888/admin/login.html)
- 📊 REST API: [http://localhost:8888/api](http://localhost:8888/api)
