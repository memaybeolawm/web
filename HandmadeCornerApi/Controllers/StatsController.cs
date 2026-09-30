using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HandmadeCornerApi.Data;

namespace HandmadeCornerApi.Controllers;

/// <summary>
/// Controller cung cấp số liệu thống kê cho Dashboard Admin và chức năng Reset CSDL.
/// Route: /api/stats
/// </summary>
[ApiController]
public class StatsController : ControllerBase
{
    private readonly AppDbContext _context;

    public StatsController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// GET /api/stats
    /// Lấy toàn bộ số liệu tổng quan hệ thống:
    /// - Tổng số đơn hàng
    /// - Đơn hoàn thành
    /// - Đơn chờ xác nhận
    /// - Tổng doanh thu
    /// - Số lượng sản phẩm, đánh giá, người dùng
    /// </summary>
    [HttpGet("api/stats")]
    public async Task<IActionResult> GetStats()
    {
        try
        {
            var totalOrders = await _context.Orders.CountAsync();
            var completedOrders = await _context.Orders.CountAsync(o => o.Status == "completed");
            var pendingOrders = await _context.Orders.CountAsync(o => o.Status == "pending");

            // Tính tổng doanh thu từ các đơn hàng đã 'completed'
            var totalRevenue = await _context.Orders
                .Where(o => o.Status == "completed")
                .SumAsync(o => (long?)o.Total) ?? 0;

            var totalProducts = await _context.Products.CountAsync();
            var totalReviews = await _context.Reviews.CountAsync();
            var totalUsers = await _context.Users.CountAsync();

            return Ok(new
            {
                totalOrders,
                completedOrders,
                pendingOrders,
                totalRevenue,
                totalProducts,
                totalReviews,
                totalUsers
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// POST /api/reset
    /// Xóa và nạp lại toàn bộ dữ liệu mẫu ban đầu
    /// </summary>
    [HttpPost("api/reset")]
    public async Task<IActionResult> ResetDatabase()
    {
        try
        {
            // Xóa toàn bộ database và tạo mới
            await _context.Database.EnsureDeletedAsync();
            await DbSeeder.SeedAsync(_context);

            return Ok(new { success = true, message = "Đã reset cơ sở dữ liệu về mặc định!" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}
