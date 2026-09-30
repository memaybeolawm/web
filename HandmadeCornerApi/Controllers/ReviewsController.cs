using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HandmadeCornerApi.Data;
using HandmadeCornerApi.Models;
using HandmadeCornerApi.Models.Dtos;

namespace HandmadeCornerApi.Controllers;

/// <summary>
/// Controller xử lý Đánh giá sản phẩm (Reviews).
/// Route: /api/reviews
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ReviewsController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// GET /api/reviews
    /// Lấy danh sách đánh giá (hỗ trợ lọc theo productId)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetReviews([FromQuery] string? productId)
    {
        try
        {
            var query = _context.Reviews.AsQueryable();

            if (!string.IsNullOrEmpty(productId))
            {
                query = query.Where(r => r.ProductId == productId);
            }

            var reviews = await query
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new
                {
                    id = r.Id,
                    productId = r.ProductId,
                    orderId = r.OrderId,
                    customerName = r.CustomerName,
                    rating = r.Rating,
                    comment = r.Comment,
                    createdAt = r.CreatedAt
                })
                .ToListAsync();

            return Ok(reviews);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// POST /api/reviews
    /// Gửi đánh giá cho sản phẩm dựa trên mã đơn hàng đã hoàn thành
    /// Tự động cập nhật điểm rating trung bình của sản phẩm
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateReview([FromBody] CreateReviewDto dto)
    {
        try
        {
            var targetOrderId = (dto.OrderId ?? "").Trim();
            if (string.IsNullOrEmpty(targetOrderId) || targetOrderId.ToLower() == "demo" || targetOrderId.ToLower() == "demo-order")
            {
                targetOrderId = "demo-order";
            }
            else
            {
                var order = await _context.Orders.FindAsync(targetOrderId);
                if (order == null)
                {
                    return BadRequest(new { success = false, message = $"Mã đơn hàng \"{targetOrderId}\" không tồn tại. Thử dùng mã mẫu \"demo-order\"!" });
                }

                if (order.Status != "completed")
                {
                    return BadRequest(new { success = false, message = $"Đơn hàng \"{targetOrderId}\" chưa hoàn thành (trạng thái: {order.Status}). Bạn chỉ có thể đánh giá sau khi đã nhận hàng thành công!" });
                }
            }

            // Kiểm tra xem đơn hàng này đã từng đánh giá sản phẩm này chưa
            var existing = await _context.Reviews
                .AnyAsync(r => r.ProductId == dto.ProductId && r.OrderId == targetOrderId);

            if (existing)
            {
                return BadRequest(new { success = false, message = "Đơn hàng này đã gửi đánh giá cho sản phẩm này rồi!" });
            }

            var reviewId = "rv-" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var now = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

            var review = new Review
            {
                Id = reviewId,
                ProductId = dto.ProductId,
                OrderId = targetOrderId,
                CustomerName = dto.CustomerName,
                Rating = dto.Rating,
                Comment = dto.Comment,
                CreatedAt = now
            };

            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();

            // Tính toán lại điểm đánh giá trung bình (Rating) và số lượng Review
            var allReviewsForProduct = await _context.Reviews
                .Where(r => r.ProductId == dto.ProductId)
                .Select(r => r.Rating)
                .ToListAsync();

            var avgRating = allReviewsForProduct.Count > 0 ? Math.Round(allReviewsForProduct.Average(), 1) : 0;

            var product = await _context.Products.FindAsync(dto.ProductId);
            if (product != null)
            {
                product.Rating = avgRating;
                product.ReviewCount = allReviewsForProduct.Count;
                await _context.SaveChangesAsync();
            }

            return Ok(new { success = true, id = reviewId });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// DELETE /api/reviews/{id}
    /// Xóa đánh giá (Dành cho Admin)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReview(string id)
    {
        try
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound(new { error = "Đánh giá không tồn tại!" });
            }

            var productId = review.ProductId;
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            // Cập nhật lại rating của sản phẩm
            var allReviews = await _context.Reviews
                .Where(r => r.ProductId == productId)
                .Select(r => r.Rating)
                .ToListAsync();

            var avgRating = allReviews.Count > 0 ? Math.Round(allReviews.Average(), 1) : 0;
            var product = await _context.Products.FindAsync(productId);
            if (product != null)
            {
                product.Rating = avgRating;
                product.ReviewCount = allReviews.Count;
                await _context.SaveChangesAsync();
            }

            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}
