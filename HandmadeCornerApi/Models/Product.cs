using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace HandmadeCornerApi.Models;

/// <summary>
/// Model đại diện cho Bảng Sản Phẩm (products) trong SQLite.
/// </summary>
[Table("products")]
public class Product
{
    [Key]
    [Column("id")]
    public string Id { get; set; } = string.Empty; // Mã sản phẩm (VD: "dl-001")

    [Required]
    [Column("category_id")]
    public string CategoryId { get; set; } = string.Empty; // Khóa ngoại liên kết bảng categories

    [Required]
    [Column("name")]
    public string Name { get; set; } = string.Empty; // Tên sản phẩm

    [Column("price")]
    public long Price { get; set; } // Giá bán (VNĐ)

    [Column("stock")]
    public int Stock { get; set; } // Số lượng tồn kho

    [Column("sold")]
    public int Sold { get; set; } = 0; // Số lượng đã bán

    [Column("description")]
    public string? Description { get; set; } // Mô tả chi tiết

    [Column("tags")]
    public string? TagsJson { get; set; } = "[]"; // Lưu danh sách tags dạng chuỗi JSON ["len","handmade"]

    [Column("images")]
    public string? ImagesJson { get; set; } = "[]"; // Lưu danh sách hình ảnh dạng chuỗi JSON ["images/sp.png"]

    [Column("rating")]
    public double Rating { get; set; } = 0.0; // Điểm đánh giá trung bình (0.0 - 5.0)

    [Column("review_count")]
    public int ReviewCount { get; set; } = 0; // Số lượt đánh giá

    [Column("created_at")]
    public string CreatedAt { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

    // ─── Các thuộc tính phụ (không lưu trực tiếp vào bảng) ───
    // Dùng [NotMapped] để EF Core bỏ qua khi tương tác SQL,
    // giúp Frontend nhận mảng Tags và Images dễ dàng
    [NotMapped]
    public List<string> Tags
    {
        get => string.IsNullOrEmpty(TagsJson) ? new List<string>() : (JsonSerializer.Deserialize<List<string>>(TagsJson) ?? new List<string>());
        set => TagsJson = JsonSerializer.Serialize(value);
    }

    [NotMapped]
    public List<string> Images
    {
        get => string.IsNullOrEmpty(ImagesJson) ? new List<string>() : (JsonSerializer.Deserialize<List<string>>(ImagesJson) ?? new List<string>());
        set => ImagesJson = JsonSerializer.Serialize(value);
    }
}
