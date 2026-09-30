using Microsoft.EntityFrameworkCore;
using HandmadeCornerApi.Models;

namespace HandmadeCornerApi.Data;

/// <summary>
/// AppDbContext là "trung tâm điều khiển" của Entity Framework Core.
/// Nó đại diện cho phiên làm việc với cơ sở dữ liệu SQLite và cho phép
/// bạn truy vấn cũng như lưu dữ liệu một cách trực quan bằng C# (thay vì viết SQL chay).
/// </summary>
public class AppDbContext : DbContext
{
    // Constructor nhận cấu hình DbContextOptions (chẳng hạn đường dẫn kết nối SQLite)
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // ─── Khai báo các Bảng (DbSet) trong Cơ sở Dữ Liệu ───────
    // Mỗi DbSet<T> tương ứng với một bảng trong SQLite

    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Admin> Admins { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderItem> OrderItems { get; set; } = null!;
    public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; } = null!;
    public DbSet<Review> Reviews { get; set; } = null!;

    /// <summary>
    /// Phương thức OnModelCreating dùng để cấu hình các ràng buộc đặc biệt,
    /// khóa chính, chỉ mục (Index) hoặc chuyển đổi kiểu dữ liệu nếu cần.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Đảm bảo Email của bảng Users là duy nhất (Unique Index)
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // Đảm bảo Username của bảng Admins là duy nhất
        modelBuilder.Entity<Admin>()
            .HasIndex(a => a.Username)
            .IsUnique();
    }
}
