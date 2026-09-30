using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HandmadeCornerApi.Models;

/// <summary>
/// Model Đánh giá sản phẩm (reviews) trong SQLite.
/// </summary>
[Table("reviews")]
public class Review
{
    [Key]
    [Column("id")]
    public string Id { get; set; } = string.Empty; // Mã đánh giá (VD: "rv-1712345678")

    [Required]
    [Column("product_id")]
    public string ProductId { get; set; } = string.Empty; // Mã sản phẩm được đánh giá

    [Required]
    [Column("order_id")]
    public string OrderId { get; set; } = string.Empty; // Mã đơn hàng tương ứng

    [Required]
    [Column("customer_name")]
    public string CustomerName { get; set; } = string.Empty; // Tên người đánh giá

    [Column("rating")]
    public int Rating { get; set; } = 5; // Số sao (1 đến 5 sao)

    [Column("comment")]
    public string? Comment { get; set; } // Nội dung bình luận / nhận xét

    [Column("created_at")]
    public string CreatedAt { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
}
