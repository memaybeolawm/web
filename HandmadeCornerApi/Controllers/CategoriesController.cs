using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HandmadeCornerApi.Data;
using HandmadeCornerApi.Models;

namespace HandmadeCornerApi.Controllers;

/// <summary>
/// Controller xử lý các yêu cầu liên quan đến Danh Mục Sản Phẩm (Categories).
/// Route: /api/categories
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _context;

    // Entity Framework Core AppDbContext được tự động tiêm (Dependency Injection) qua Constructor
    public CategoriesController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// GET /api/categories
    /// Lấy toàn bộ danh sách danh mục sản phẩm từ bảng categories
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetCategories()
    {
        try
        {
            // Dùng EF Core ToListAsync() để lấy toàn bộ dữ liệu bất đồng bộ (non-blocking)
            var categories = await _context.Categories.ToListAsync();
            return Ok(categories); // Trả về HTTP 200 kèm mảng JSON
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}
