using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HandmadeCornerApi.Data;
using HandmadeCornerApi.Models;
using HandmadeCornerApi.Models.Dtos;

namespace HandmadeCornerApi.Controllers;

/// <summary>
/// Controller xử lý các nghiệp vụ liên quan đến Đơn hàng (Orders).
/// Bao gồm: Đặt hàng, tra cứu đơn hàng, lọc theo trạng thái, cập nhật trạng thái, khách hàng tự hủy đơn.
/// Route: /api/orders
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly AppDbContext _context;

    public OrdersController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// GET /api/orders
    /// Lấy danh sách đơn hàng có hỗ trợ lọc:
    /// - status: trạng thái (pending, confirmed, preparing, shipping, completed, cancelled)
    /// - userId: lọc theo tài khoản khách hàng
    /// - q: tìm kiếm theo mã đơn, tên hoặc số điện thoại người nhận
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetOrders(
        [FromQuery] string? status,
        [FromQuery] long? userId,
        [FromQuery] string? q)
    {
        try
        {
            var query = _context.Orders.AsQueryable();

            if (userId.HasValue)
            {
                query = query.Where(o => o.UserId == userId.Value);
            }

            if (!string.IsNullOrEmpty(status) && status != "all")
            {
                query = query.Where(o => o.Status == status);
            }

            if (!string.IsNullOrEmpty(q))
            {
                var term = q.ToLower().Trim();
                query = query.Where(o =>
                    o.Id.ToLower().Contains(term) ||
                    o.CustomerName.ToLower().Contains(term) ||
                    o.CustomerPhone.Contains(term));
            }

            var orders = await query.OrderByDescending(o => o.CreatedAt).ToListAsync();

            // Lấy danh sách item và lịch sử trạng thái cho từng đơn
            var result = new List<object>();
            foreach (var o in orders)
            {
                var items = await _context.OrderItems
                    .Where(i => i.OrderId == o.Id)
                    .Select(i => new
                    {
                        productId = i.ProductId,
                        productName = i.ProductName,
                        productImage = i.ProductImage,
                        price = i.Price,
                        quantity = i.Quantity
                    }).ToListAsync();

                var history = await _context.OrderStatusHistories
                    .Where(h => h.OrderId == o.Id)
                    .OrderBy(h => h.Time)
                    .Select(h => new
                    {
                        status = h.Status,
                        time = h.Time,
                        note = h.Note
                    }).ToListAsync();

                result.Add(new
                {
                    id = o.Id,
                    userId = o.UserId,
                    customer = new
                    {
                        name = o.CustomerName,
                        phone = o.CustomerPhone,
                        email = o.CustomerEmail,
                        address = o.CustomerAddress,
                        city = o.CustomerCity,
                        district = o.CustomerDistrict,
                        ward = o.CustomerWard,
                        note = o.CustomerNote,
                        giftWrap = o.GiftWrap == 1
                    },
                    items,
                    total = o.Total,
                    paymentMethod = o.PaymentMethod,
                    status = o.Status,
                    createdAt = o.CreatedAt,
                    updatedAt = o.UpdatedAt,
                    statusHistory = history
                });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// GET /api/orders/{id}
    /// Lấy thông tin chi tiết của 1 đơn hàng cụ thể
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(string id)
    {
        try
        {
            var o = await _context.Orders.FindAsync(id);
            if (o == null)
            {
                return NotFound(new { error = "Đơn hàng không tồn tại!" });
            }

            var items = await _context.OrderItems
                .Where(i => i.OrderId == o.Id)
                .Select(i => new
                {
                    productId = i.ProductId,
                    productName = i.ProductName,
                    productImage = i.ProductImage,
                    price = i.Price,
                    quantity = i.Quantity
                }).ToListAsync();

            var history = await _context.OrderStatusHistories
                .Where(h => h.OrderId == o.Id)
                .OrderBy(h => h.Time)
                .Select(h => new
                {
                    status = h.Status,
                    time = h.Time,
                    note = h.Note
                }).ToListAsync();

            return Ok(new
            {
                id = o.Id,
                userId = o.UserId,
                customer = new
                {
                    name = o.CustomerName,
                    phone = o.CustomerPhone,
                    email = o.CustomerEmail,
                    address = o.CustomerAddress,
                    city = o.CustomerCity,
                    district = o.CustomerDistrict,
                    ward = o.CustomerWard,
                    note = o.CustomerNote,
                    giftWrap = o.GiftWrap == 1
                },
                items,
                total = o.Total,
                paymentMethod = o.PaymentMethod,
                status = o.Status,
                createdAt = o.CreatedAt,
                updatedAt = o.UpdatedAt,
                statusHistory = history
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// POST /api/orders
    /// Tạo đơn hàng mới, kiểm tra tồn kho và trừ số lượng sản phẩm
    /// Sử dụng EF Core Database Transaction để đảm bảo tính toàn vẹn dữ liệu
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequestDto dto)
    {
        if (dto.Items == null || dto.Items.Count == 0)
        {
            return BadRequest(new { success = false, message = "Giỏ hàng trống!" });
        }

        // Bắt đầu Transaction với EF Core
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var orderId = "ORD-" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var now = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            long total = 0;

            // 1. Kiểm tra tồn kho và tính tổng tiền
            var orderItemsList = new List<OrderItem>();
            foreach (var item in dto.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null || product.Stock < item.Quantity)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = $"Sản phẩm \"{product?.Name ?? item.ProductId}\" không đủ hàng trong kho!"
                    });
                }

                total += product.Price * item.Quantity;

                // Trừ tồn kho và tăng số lượng đã bán
                product.Stock -= item.Quantity;
                product.Sold += item.Quantity;

                var firstImage = product.Images.Count > 0 ? product.Images[0] : "";
                orderItemsList.Add(new OrderItem
                {
                    OrderId = orderId,
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ProductImage = firstImage,
                    Price = product.Price,
                    Quantity = item.Quantity
                });
            }

            // Cộng thêm phí gói quà nếu có (15.000đ)
            if (dto.CustomerInfo.GiftWrap)
            {
                total += 15000;
            }

            // 2. Tạo đơn hàng mới
            var order = new Order
            {
                Id = orderId,
                UserId = dto.UserId,
                CustomerName = dto.CustomerInfo.Name,
                CustomerPhone = dto.CustomerInfo.Phone,
                CustomerEmail = dto.CustomerInfo.Email,
                CustomerAddress = dto.CustomerInfo.Address,
                CustomerCity = dto.CustomerInfo.City,
                CustomerDistrict = dto.CustomerInfo.District,
                CustomerWard = dto.CustomerInfo.Ward,
                CustomerNote = dto.CustomerInfo.Note,
                GiftWrap = dto.CustomerInfo.GiftWrap ? 1 : 0,
                Total = total,
                PaymentMethod = dto.PaymentMethod,
                Status = "pending",
                CreatedAt = now,
                UpdatedAt = now
            };

            await _context.Orders.AddAsync(order);
            await _context.OrderItems.AddRangeAsync(orderItemsList);

            // 3. Ghi lịch sử tạo đơn
            await _context.OrderStatusHistories.AddAsync(new OrderStatusHistory
            {
                OrderId = orderId,
                Status = "pending",
                Time = now,
                Note = "Đơn hàng được tạo mới"
            });

            await _context.SaveChangesAsync();
            await transaction.CommitAsync(); // Xác nhận giao dịch thành công

            return Ok(new { success = true, orderId });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(); // Hoàn tác nếu có lỗi
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// PATCH /api/orders/{id}/status
    /// Cập nhật trạng thái đơn hàng (Admin hoặc Khách hàng hủy đơn)
    /// Nếu hủy đơn -> tự động hoàn trả số lượng hàng tồn kho
    /// </summary>
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateOrderStatus(string id, [FromBody] UpdateOrderStatusDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound(new { error = "Đơn hàng không tồn tại!" });
            }

            var now = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

            // Nếu chuyển sang trạng thái cancelled và trước đó chưa hủy -> hoàn lại tồn kho
            if (dto.Status == "cancelled" && order.Status != "cancelled")
            {
                var items = await _context.OrderItems.Where(i => i.OrderId == id).ToListAsync();
                foreach (var item in items)
                {
                    var prod = await _context.Products.FindAsync(item.ProductId);
                    if (prod != null)
                    {
                        prod.Stock += item.Quantity;
                        prod.Sold = Math.Max(0, prod.Sold - item.Quantity);
                    }
                }
            }

            order.Status = dto.Status;
            order.UpdatedAt = now;

            // Ghi nhận lịch sử trạng thái
            await _context.OrderStatusHistories.AddAsync(new OrderStatusHistory
            {
                OrderId = id,
                Status = dto.Status,
                Time = now,
                Note = dto.Note ?? ""
            });

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(500, new { error = ex.Message });
        }
    }
}
