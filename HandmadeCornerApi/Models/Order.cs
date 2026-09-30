using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HandmadeCornerApi.Models;

/// <summary>
/// Model Bảng Đơn hàng (orders) trong SQLite.
/// </summary>
[Table("orders")]
public class Order
{
    [Key]
    [Column("id")]
    public string Id { get; set; } = string.Empty; // Mã đơn hàng (VD: "ORD-1712345678901")

    [Column("user_id")]
    public long? UserId { get; set; } // Khóa ngoại liên kết bảng users (có thể null nếu khách không đăng nhập)

    [Required]
    [Column("customer_name")]
    public string CustomerName { get; set; } = string.Empty; // Tên người nhận

    [Required]
    [Column("customer_phone")]
    public string CustomerPhone { get; set; } = string.Empty; // SĐT người nhận

    [Column("customer_email")]
    public string? CustomerEmail { get; set; } // Email người nhận

    [Required]
    [Column("customer_address")]
    public string CustomerAddress { get; set; } = string.Empty; // Địa chỉ chi tiết

    [Column("customer_city")]
    public string CustomerCity { get; set; } = string.Empty; // Tỉnh / Thành phố

    [Column("customer_district")]
    public string? CustomerDistrict { get; set; } // Quận / Huyện

    [Column("customer_ward")]
    public string? CustomerWard { get; set; } // Phường / Xã

    [Column("customer_note")]
    public string? CustomerNote { get; set; } // Ghi chú đơn hàng

    [Column("gift_wrap")]
    public int GiftWrap { get; set; } = 0; // Gói quà: 1 là có (phí 15.000đ), 0 là không

    [Column("total")]
    public long Total { get; set; } // Tổng giá trị đơn hàng (VNĐ)

    [Column("payment_method")]
    public string PaymentMethod { get; set; } = "Chuyển khoản QR Pay"; // Phương thức thanh toán (COD, Chuyển khoản QR Pay...)

    [Column("status")]
    public string Status { get; set; } = "pending"; // Trạng thái: pending, confirmed, preparing, shipping, completed, cancelled

    [Column("created_at")]
    public string CreatedAt { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

    [Column("updated_at")]
    public string UpdatedAt { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
}

/// <summary>
/// Model Bảng Chi tiết Đơn hàng (order_items) trong SQLite.
/// Mỗi dòng lưu 1 sản phẩm nằm trong 1 đơn hàng cụ thể.
/// </summary>
[Table("order_items")]
public class OrderItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public long Id { get; set; } // Khóa chính tự tăng

    [Required]
    [Column("order_id")]
    public string OrderId { get; set; } = string.Empty; // Khóa ngoại liên kết bảng orders

    [Required]
    [Column("product_id")]
    public string ProductId { get; set; } = string.Empty; // Mã sản phẩm

    [Required]
    [Column("product_name")]
    public string ProductName { get; set; } = string.Empty; // Tên sản phẩm tại thời điểm mua

    [Column("product_image")]
    public string? ProductImage { get; set; } // Link ảnh sản phẩm

    [Column("price")]
    public long Price { get; set; } // Đơn giá tại thời điểm mua

    [Column("quantity")]
    public int Quantity { get; set; } // Số lượng mua
}

/// <summary>
/// Model Lịch sử thay đổi trạng thái đơn hàng (order_status_history).
/// Dùng để hiển thị Timeline trạng thái đơn hàng.
/// </summary>
[Table("order_status_history")]
public class OrderStatusHistory
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [Column("order_id")]
    public string OrderId { get; set; } = string.Empty;

    [Required]
    [Column("status")]
    public string Status { get; set; } = string.Empty;

    [Column("time")]
    public string Time { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

    [Column("note")]
    public string? Note { get; set; }
}
