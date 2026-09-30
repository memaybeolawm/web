using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HandmadeCornerApi.Data;
using HandmadeCornerApi.Models;
using HandmadeCornerApi.Models.Dtos;
using System.Text.Json;

namespace HandmadeCornerApi.Controllers;

/// <summary>
/// Controller xử lý các nghiệp vụ liên quan đến Sản Phẩm (Products).
/// Bao gồm: Lọc theo danh mục, tìm kiếm, sắp xếp, chi tiết, thêm, sửa, xóa sản phẩm.
/// Route: /api/products
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductsController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// GET /api/products
    /// Lọc sản phẩm theo query params:
    /// - cat: mã danh mục (VD: do-len)
    /// - q: từ khóa tìm kiếm (tên, mô tả, tags)
    /// - minPrice, maxPrice: khoảng giá
    /// - sort: price-asc, price-desc, popular, rating
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetProducts(
        [FromQuery] string? cat,
        [FromQuery] string? q,
        [FromQuery] long? minPrice,
        [FromQuery] long? maxPrice,
        [FromQuery] string? sort)
    {
        try
        {
            // Bắt đầu câu truy vấn IQueryable với EF Core
            var query = _context.Products.AsQueryable();

            // 1. Lọc theo danh mục
            if (!string.IsNullOrEmpty(cat))
            {
                query = query.Where(p => p.CategoryId == cat);
            }

            // 2. Tìm kiếm theo từ khóa
            if (!string.IsNullOrEmpty(q))
            {
                var search = q.ToLower().Trim();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(search) ||
                    (p.Description != null && p.Description.ToLower().Contains(search)) ||
                    (p.TagsJson != null && p.TagsJson.ToLower().Contains(search)));
            }

            // 3. Lọc theo khoảng giá
            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            // 4. Sắp xếp kết quả
            query = sort switch
            {
                "price-asc"  => query.OrderBy(p => p.Price),
                "price-desc" => query.OrderByDescending(p => p.Price),
                "popular"    => query.OrderByDescending(p => p.Sold),
                "rating"     => query.OrderByDescending(p => p.Rating),
                _            => query.OrderByDescending(p => p.CreatedAt) // Mặc định: Mới nhất
            };

            var list = await query.ToListAsync();

            // Định dạng lại dữ liệu trả về cho chuẩn với frontend JavaScript
            var response = list.Select(p => new
            {
                id = p.Id,
                categoryId = p.CategoryId,
                name = p.Name,
                price = p.Price,
                stock = p.Stock,
                sold = p.Sold,
                description = p.Description,
                tags = p.Tags,
                images = p.Images,
                rating = p.Rating,
                reviewCount = p.ReviewCount,
                createdAt = p.CreatedAt
            });

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// GET /api/products/{id}
    /// Lấy thông tin chi tiết của 1 sản phẩm theo mã ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(string id)
    {
        try
        {
            var p = await _context.Products.FindAsync(id);
            if (p == null)
            {
                return NotFound(new { error = "Sản phẩm không tồn tại!" });
            }

            return Ok(new
            {
                id = p.Id,
                categoryId = p.CategoryId,
                name = p.Name,
                price = p.Price,
                stock = p.Stock,
                sold = p.Sold,
                description = p.Description,
                tags = p.Tags,
                images = p.Images,
                rating = p.Rating,
                reviewCount = p.ReviewCount,
                createdAt = p.CreatedAt
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// POST /api/products
    /// Thêm sản phẩm mới (Dành cho trang Admin)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] ProductUpsertDto dto)
    {
        try
        {
            var productId = !string.IsNullOrEmpty(dto.Id) ? dto.Id : "prd-" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            var product = new Product
            {
                Id = productId,
                CategoryId = dto.CategoryId,
                Name = dto.Name,
                Price = dto.Price,
                Stock = dto.Stock,
                Sold = 0,
                Description = dto.Description,
                TagsJson = JsonSerializer.Serialize(dto.Tags ?? new List<string>()),
                ImagesJson = JsonSerializer.Serialize(dto.Images ?? new List<string>()),
                Rating = 0,
                ReviewCount = 0,
                CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
            };

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, id = productId });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// PUT /api/products/{id}
    /// Cập nhật thông tin sản phẩm (Dành cho trang Admin)
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(string id, [FromBody] ProductUpsertDto dto)
    {
        try
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(new { error = "Sản phẩm không tồn tại!" });
            }

            product.CategoryId = dto.CategoryId;
            product.Name = dto.Name;
            product.Price = dto.Price;
            product.Stock = dto.Stock;
            product.Description = dto.Description;
            product.TagsJson = JsonSerializer.Serialize(dto.Tags ?? new List<string>());
            product.ImagesJson = JsonSerializer.Serialize(dto.Images ?? new List<string>());

            await _context.SaveChangesAsync();
            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// DELETE /api/products/{id}
    /// Xóa sản phẩm khỏi database
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(string id)
    {
        try
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(new { error = "Sản phẩm không tồn tại!" });
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}
