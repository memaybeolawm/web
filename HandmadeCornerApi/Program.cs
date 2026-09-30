// ============================================================
// Program.cs – File khởi động và cấu hình ASP.NET Core Web Server
// ============================================================
// File này là điểm xuất phát (Entry Point) của toàn bộ ứng dụng C# / .NET.
// Tại đây ta cấu hình:
// 1. Kết nối Database SQLite qua Entity Framework Core
// 2. Cấu hình CORS, JSON và Controllers
// 3. Cấu hình Static Files để phục vụ trực tiếp giao diện Web HTML/CSS/JS
// 4. Định tuyến URL đẹp mắt (Clean URLs)

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using HandmadeCornerApi.Data;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// ─── 1. Cấu hình Cổng (Port) chạy Server ─────────────────────
// Chạy trên cổng 8888 (khớp với frontend hiện tại)
builder.WebHost.UseUrls("http://0.0.0.0:8888");

// ─── 2. Cấu hình Entity Framework Core kết nối SQLite ─────────
// Xác định đường dẫn file database.sqlite nằm ở thư mục cha (d:\web\database.sqlite)
var rootDirectory = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, ".."));
var dbPath = Path.Combine(rootDirectory, "database.sqlite");
var connectionString = $"Data Source={dbPath}";

builder.Services.AddDbContext<AppDbContext>(options =>
{
    // Cấu hình EF Core sử dụng SQLite
    options.UseSqlite(connectionString);
});

// ─── 3. Cấu hình CORS (Cross-Origin Resource Sharing) ─────────
// Cho phép trình duyệt gọi API từ các domain khác nhau mà không bị chặn
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ─── 4. Cấu hình Controllers & JSON Định Dạng ─────────────────
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Chuyển tên thuộc tính C# (PascalCase: CustomerName) sang JSON (camelCase: customerName)
        // để tương thích hoàn toàn với Javascript frontend
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
    });

var app = builder.Build();

// ─── 5. Tự động Khởi tạo & Seed Database khi Server Bật ───────
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        Console.WriteLine("🔄 Đang khởi tạo cơ sở dữ liệu SQLite qua Entity Framework Core...");
        await DbSeeder.SeedAsync(dbContext);
        Console.WriteLine("✅ Database SQLite đã sẵn sàng!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Lỗi khi khởi tạo Database: {ex.Message}");
    }
}

// ─── 6. Cấu hình Middleware Pipeline ──────────────────────────
app.UseCors();

// Tắt Cache cho các file CSS và JS để trình duyệt luôn nhận phiên bản mới nhất
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value ?? "";
    if (path.EndsWith(".css") || path.EndsWith(".js"))
    {
        context.Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate";
        context.Response.Headers["Pragma"] = "no-cache";
    }
    await next();
});

// Phục vụ Static Files (HTML, CSS, JS, Hình ảnh) từ thư mục gốc d:\web
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(rootDirectory),
    RequestPath = ""
});

// Kích hoạt Routing cho API Controllers
app.UseRouting();
app.MapControllers();

// ─── 7. Định tuyến Clean URLs (Không cần đuôi .html) ──────────
var pages = new[] { "products", "product-detail", "cart", "checkout", "orders" };
foreach (var page in pages)
{
    app.MapGet($"/{page}", () => Results.File(Path.Combine(rootDirectory, $"{page}.html"), "text/html; charset=utf-8"));
}

// Định tuyến các trang Admin
app.MapGet("/admin", () => Results.File(Path.Combine(rootDirectory, "admin", "index.html"), "text/html; charset=utf-8"));
app.MapGet("/admin/products", () => Results.File(Path.Combine(rootDirectory, "admin", "products.html"), "text/html; charset=utf-8"));
app.MapGet("/admin/orders", () => Results.File(Path.Combine(rootDirectory, "admin", "orders.html"), "text/html; charset=utf-8"));
app.MapGet("/admin/reviews", () => Results.File(Path.Combine(rootDirectory, "admin", "reviews.html"), "text/html; charset=utf-8"));

// Fallback: Nếu không khớp route nào khác (và không phải /api/...), trả về trang chủ index.html
app.MapFallback((HttpContext context) =>
{
    var requestPath = context.Request.Path.Value ?? "";
    if (requestPath.StartsWith("/api/"))
    {
        return Results.NotFound(new { error = "API endpoint không tồn tại!" });
    }
    return Results.File(Path.Combine(rootDirectory, "index.html"), "text/html; charset=utf-8");
});

// In thông tin ra Console khi khởi động
Console.WriteLine("==================================================");
Console.WriteLine("🚀 Handmade Corner ASP.NET Core Server is running!");
Console.WriteLine("🌐 Website URL:  http://localhost:8888");
Console.WriteLine("🔐 Admin Login:  http://localhost:8888/admin/login.html");
Console.WriteLine("📊 REST API URL: http://localhost:8888/api");
Console.WriteLine("💾 ORM Engine:   Entity Framework Core + SQLite");
Console.WriteLine("==================================================");

app.Run();
