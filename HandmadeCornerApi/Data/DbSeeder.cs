using HandmadeCornerApi.Models;
using System.Text.Json;

namespace HandmadeCornerApi.Data;

/// <summary>
/// Lớp DbSeeder giúp:
/// 1. Tự động kiểm tra và tạo database/bảng nếu chưa tồn tại qua EF Core.
/// 2. Nạp dữ liệu mẫu ban đầu (Danh mục, Sản phẩm, Tài khoản Admin & Khách hàng mẫu, Đánh giá).
/// </summary>
public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // Tạo database và tất cả bảng nếu chưa có
        await context.Database.EnsureCreatedAsync();

        // 1. Seed Categories nếu chưa có
        if (!context.Categories.Any())
        {
            var categories = new List<Category>
            {
                new() { Id = "do-len",        Name = "Đồ Len",              Icon = "🧶", Description = "Sản phẩm handmade từ len sợi cao cấp" },
                new() { Id = "trang-suc",     Name = "Trang Sức Handmade",   Icon = "💍", Description = "Vòng tay, nhẫn, dây chuyền thủ công tinh xảo" },
                new() { Id = "nen-thom",      Name = "Nến & Đồ Thơm",       Icon = "🕯️", Description = "Nến thơm sáp đậu nành, tinh dầu thiên nhiên" },
                new() { Id = "gom-su",        Name = "Gốm Sứ Thủ Công",     Icon = "🏺", Description = "Chậu, cốc, đĩa gốm vẽ tay mộc mạc" },
                new() { Id = "qua-tang",      Name = "Quà Tặng & Hộp Quà",  Icon = "🎁", Description = "Hộp quà sinh nhật, quà cặp đôi ngọt ngào" },
                new() { Id = "sticker-washi", Name = "Sticker & Washi Tape", Icon = "🎨", Description = "Sticker cute, washi tape trang trí sổ" },
                new() { Id = "so-tay",        Name = "Sổ Tay & Bookmark",    Icon = "📒", Description = "Sổ tay bìa da, bookmark hoa khô ép" },
                new() { Id = "thieu-nut",     Name = "Thêu & Đan Len",       Icon = "🪡", Description = "Tranh thêu tay, bộ kit thêu DIY" },
                new() { Id = "phu-kien",      Name = "Phụ Kiện Tóc",        Icon = "🎀", Description = "Cài tóc, kẹp tóc, scrunchies handmade" },
                new() { Id = "macrame",       Name = "Macrame & Thắt Bện",   Icon = "🪢", Description = "Dây treo chậu cây, trang trí tường macrame" }
            };

            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
            Console.WriteLine("🌱 [DbSeeder] Đã nạp danh mục sản phẩm mẫu.");
        }

        // 2. Seed Products nếu chưa có
        if (!context.Products.Any())
        {
            var products = new List<Product>
            {
                // Đồ len
                new() { Id = "dl-001", CategoryId = "do-len", Name = "Túi Len Tote Nhỏ", Price = 150000, Stock = 8, Sold = 12, Description = "Túi len tote nhỏ xinh đan tay tỉ mỉ", TagsJson = JsonSerializer.Serialize(new[]{"túi","len","tote"}), ImagesJson = JsonSerializer.Serialize(new[]{"images/tui-len-tote-nho.png"}) },
                new() { Id = "dl-002", CategoryId = "do-len", Name = "Mũ Len Beanie Vintage", Price = 180000, Stock = 15, Sold = 34, Description = "Mũ len beanie phong cách vintage ấm áp", TagsJson = JsonSerializer.Serialize(new[]{"mũ","len","beanie"}), ImagesJson = JsonSerializer.Serialize(new[]{"images/mu-len-beanie.png"}) },
                new() { Id = "dl-003", CategoryId = "do-len", Name = "Áo Len Crop Top Đan Tay", Price = 450000, Stock = 5, Sold = 8, Description = "Áo len crop top đan tay thời trang trendy", TagsJson = JsonSerializer.Serialize(new[]{"áo","len","crop"}), ImagesJson = JsonSerializer.Serialize(new[]{"images/ao-len-crop.png"}) },
                new() { Id = "dl-004", CategoryId = "do-len", Name = "Khăn Len Dệt Tay", Price = 220000, Stock = 20, Sold = 45, Description = "Khăn len mềm mại, giữ ấm mùa đông", TagsJson = JsonSerializer.Serialize(new[]{"khăn","len","dệt"}), ImagesJson = JsonSerializer.Serialize(new[]{"images/khan-len-det-tay.png"}) },
                new() { Id = "dl-005", CategoryId = "do-len", Name = "Túi Len Đựng Điện Thoại", Price = 80000, Stock = 30, Sold = 67, Description = "Túi len mini đựng điện thoại tiện lợi", TagsJson = JsonSerializer.Serialize(new[]{"túi","len","mini"}), ImagesJson = JsonSerializer.Serialize(new[]{"images/tui-len-dung-dt.png"}) },
                new() { Id = "dl-006", CategoryId = "do-len", Name = "Giỏ Len Đựng Đồ", Price = 280000, Stock = 12, Sold = 23, Description = "Giỏ len đan tay đựng đồ decor phòng", TagsJson = JsonSerializer.Serialize(new[]{"giỏ","len","đựng đồ"}), ImagesJson = JsonSerializer.Serialize(new[]{"images/gio-len-dung-do.png"}) },
                
                // Trang sức
                new() { Id = "ts-001", CategoryId = "trang-suc", Name = "Vòng Tay Đá Tự Nhiên", Price = 120000, Stock = 25, Sold = 45, Description = "Vòng tay đá thạch anh tự nhiên phong thủy", TagsJson = JsonSerializer.Serialize(new[]{"vòng","đá","tự nhiên"}), ImagesJson = JsonSerializer.Serialize(new[]{"images/vong-tay-da-tu-nhien.png"}) },
                new() { Id = "ts-002", CategoryId = "trang-suc", Name = "Nhẫn Dây Bện Macrame", Price = 85000, Stock = 40, Sold = 55, Description = "Nhẫn bện macrame thủ công tinh tế", TagsJson = JsonSerializer.Serialize(new[]{"nhẫn","macrame","dây"}), ImagesJson = JsonSerializer.Serialize(new[]{"images/nhan-day-ben-macrame.png"}) },
                new() { Id = "ts-003", CategoryId = "trang-suc", Name = "Bông Tai Khuyên Hoa Cúc", Price = 95000, Stock = 30, Sold = 42, Description = "Bông tai hoa cúc đất sét tự khô", TagsJson = JsonSerializer.Serialize(new[]{"bông tai","hoa","cute"}), ImagesJson = JsonSerializer.Serialize(new[]{"images/bong-tai-hoa-cuc.png"}) },
                new() { Id = "ts-004", CategoryId = "trang-suc", Name = "Dây Chuyền Ngọc Trai Nước Ngọt", Price = 350000, Stock = 10, Sold = 18, Description = "Dây chuyền ngọc trai nước ngọt thanh lịch", TagsJson = JsonSerializer.Serialize(new[]{"dây chuyền","ngọc trai"}), ImagesJson = JsonSerializer.Serialize(new[]{"images/day-chuyen-ngoc-trai.png"}) },
                new() { Id = "ts-005", CategoryId = "trang-suc", Name = "Lắc Tay Vàng Handmade", Price = 420000, Stock = 8, Sold = 12, Description = "Lắc tay mạ vàng thủ công cao cấp", TagsJson = JsonSerializer.Serialize(new[]{"lắc","vàng","handmade"}), ImagesJson = JsonSerializer.Serialize(new[]{"images/lac-tay-vang.png"}) },
                
                // Nến thơm
                new() { Id = "nt-001", CategoryId = "nen-thom", Name = "Nến Thơm Hoa Hồng", Price = 95000, Stock = 50, Sold = 120, Description = "Nến thơm hoa hồng Pháp lãng mạn", TagsJson = JsonSerializer.Serialize(new[]{"nến","thơm","hoa hồng"}), ImagesJson = JsonSerializer.Serialize(new[]{"images/nen-thom-hoa-hong.png"}) },
                new() { Id = "nt-002", CategoryId = "nen-thom", Name = "Nến Thơm Lavender", Price = 110000, Stock = 45, Sold = 98, Description = "Nến thơm hoa oải hương giúp ngủ ngon", TagsJson = JsonSerializer.Serialize(new[]{"nến","lavender","thơm"}), ImagesJson = JsonSerializer.Serialize(new[]{"images/nen-thom-lavender.png"}) },
                new() { Id = "nt-003", CategoryId = "nen-thom", Name = "Set Nến Tealight 12 Cái", Price = 180000, Stock = 35, Sold = 67, Description = "Bộ 12 viên nến thơm tealight", TagsJson = JsonSerializer.Serialize(new[]{"nến","tealight","set"}), ImagesJson = JsonSerializer.Serialize(new[]{"images/set-nen-tealight.png"}) },

                // Gốm sứ
                new() { Id = "gs-001", CategoryId = "gom-su", Name = "Chậu Gốm Mini Cây Cảnh", Price = 145000, Stock = 20, Sold = 35, Description = "Chậu gốm nhỏ trồng sen đá mộc mạc", TagsJson = JsonSerializer.Serialize(new[]{"chậu","gốm","cây cảnh"}), ImagesJson = JsonSerializer.Serialize(new[]{"images/chau-gom-mini.png"}) },
                new() { Id = "gs-002", CategoryId = "gom-su", Name = "Cốc Gốm Vẽ Tay Mèo", Price = 165000, Stock = 18, Sold = 29, Description = "Cốc gốm vẽ tay chú mèo đáng yêu", TagsJson = JsonSerializer.Serialize(new[]{"cốc","gốm","mèo"}), ImagesJson = JsonSerializer.Serialize(new[]{"images/coc-gom-meo.png"}) },
                new() { Id = "gs-003", CategoryId = "gom-su", Name = "Đĩa Gốm Decor Tròn", Price = 195000, Stock = 15, Sold = 22, Description = "Đĩa gốm trang trí bàn ăn thủ công", TagsJson = JsonSerializer.Serialize(new[]{"đĩa","gốm","decor"}), ImagesJson = JsonSerializer.Serialize(new[]{"images/dia-gom-decor.png"}) },

                // Quà tặng
                new() { Id = "qt-001", CategoryId = "qua-tang", Name = "Hộp Quà Sinh Nhật Handmade", Price = 320000, Stock = 20, Sold = 38, Description = "Hộp quà sinh nhật trang trí hoa lụa tinh tế", TagsJson = JsonSerializer.Serialize(new[]{"hộp quà","sinh nhật"}), ImagesJson = JsonSerializer.Serialize(new[]{"images/hop-qua-sinh-nhat.png"}) },
                new() { Id = "qt-002", CategoryId = "qua-tang", Name = "Gift Box Cặp Đôi", Price = 450000, Stock = 12, Sold = 20, Description = "Set quà tặng tình yêu lãng mạn", TagsJson = JsonSerializer.Serialize(new[]{"gift box","cặp đôi"}), ImagesJson = JsonSerializer.Serialize(new[]{"images/gift-box-cap-doi.png"}) },

                // Sticker & Sổ tay
                new() { Id = "st-001", CategoryId = "sticker-washi", Name = "Sticker Set Cute Animals", Price = 45000, Stock = 100, Sold = 90, Description = "Bộ sticker động vật chibi chống nước", TagsJson = JsonSerializer.Serialize(new[]{"sticker","cute"}), ImagesJson = JsonSerializer.Serialize(new[]{"images/sticker-cute-animals.png"}) },
                new() { Id = "sot-001", CategoryId = "so-tay", Name = "Sổ Tay Bìa Da Vintage", Price = 220000, Stock = 30, Sold = 55, Description = "Sổ tay vintage bìa da sáp handmade", TagsJson = JsonSerializer.Serialize(new[]{"sổ tay","bìa da"}), ImagesJson = JsonSerializer.Serialize(new[]{"images/so-tay-bia-da.png"}) },
                new() { Id = "sot-002", CategoryId = "so-tay", Name = "Bookmark Hoa Khô Ép", Price = 35000, Stock = 60, Sold = 75, Description = "Kẹp sách hoa khô ép resin trong suốt", TagsJson = JsonSerializer.Serialize(new[]{"bookmark","hoa khô"}), ImagesJson = JsonSerializer.Serialize(new[]{"images/bookmark-hoa-kho.png"}) }
            };

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
            Console.WriteLine("🌱 [DbSeeder] Đã nạp sản phẩm mẫu.");
        }

        // 3. Seed Admin mặc định
        if (!context.Admins.Any())
        {
            var admin = new Admin
            {
                Username = "admin",
                Password = "admin123", // Mật khẩu quản trị mặc định
                Name = "Quản trị viên",
                Role = "admin"
            };
            await context.Admins.AddAsync(admin);
            await context.SaveChangesAsync();
            Console.WriteLine("👤 [DbSeeder] Admin mặc định: username=admin | password=admin123");
        }

        // 4. Seed Khách hàng mẫu
        if (!context.Users.Any())
        {
            var customer = new User
            {
                Email = "khach@gmail.com",
                Password = "123456",
                Name = "Nguyễn Thị Khách",
                Phone = "0987654321",
                Address = "456 Lê Lợi, Phường 1",
                City = "TP. Hồ Chí Minh"
            };
            await context.Users.AddAsync(customer);
            await context.SaveChangesAsync();
            Console.WriteLine("👤 [DbSeeder] Khách hàng mẫu: khach@gmail.com | 123456");
        }

        // 5. Seed Demo Order
        if (!context.Orders.Any(o => o.Id == "demo-order"))
        {
            var demoOrder = new Order
            {
                Id = "demo-order",
                CustomerName = "Khách hàng Demo",
                CustomerPhone = "0901234567",
                CustomerEmail = "demo@handmade.vn",
                CustomerAddress = "123 Nguyễn Trãi",
                CustomerCity = "TP. Hồ Chí Minh",
                Total = 500000,
                PaymentMethod = "Chuyển khoản QR Pay",
                Status = "completed"
            };
            await context.Orders.AddAsync(demoOrder);
            await context.SaveChangesAsync();
        }

        // 6. Seed Sample Reviews & Cập nhật rating
        if (!context.Reviews.Any())
        {
            var reviews = new List<Review>
            {
                new() { Id = "rv-001", ProductId = "st-001", OrderId = "demo-order", CustomerName = "Minh Anh", Rating = 5, Comment = "Sticker rất cute, dán rất chắc chắn!" },
                new() { Id = "rv-002", ProductId = "sot-002", OrderId = "demo-order", CustomerName = "Thu Hương", Rating = 5, Comment = "Bookmark hoa khô ép đẹp mê ly, đóng gói cẩn thận." },
                new() { Id = "rv-003", ProductId = "ts-001", OrderId = "demo-order", CustomerName = "Lan Anh", Rating = 5, Comment = "Vòng đá tự nhiên đẹp và rất nhẹ tay." },
                new() { Id = "rv-004", ProductId = "dl-005", OrderId = "demo-order", CustomerName = "Như Quỳnh", Rating = 5, Comment = "Túi len mini nhỏ gọn, màu len xinh xắn." }
            };
            await context.Reviews.AddRangeAsync(reviews);
            await context.SaveChangesAsync();

            // Cập nhật rating sản phẩm tương ứng
            foreach (var r in reviews)
            {
                var prod = await context.Products.FindAsync(r.ProductId);
                if (prod != null)
                {
                    prod.Rating = r.Rating;
                    prod.ReviewCount = 1;
                }
            }
            await context.SaveChangesAsync();
        }
    }
}
