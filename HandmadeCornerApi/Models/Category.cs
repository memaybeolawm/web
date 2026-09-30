using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HandmadeCornerApi.Models;

/// <summary>
/// Model đại diện cho Bảng Danh Mục sản phẩm (categories) trong SQLite.
/// Mỗi danh mục như: "Đồ Len", "Gốm Sứ", "Trang Sức"...
/// </summary>
[Table("categories")]
public class Category
{
    [Key]
    [Column("id")]
    public string Id { get; set; } = string.Empty; // Mã danh mục (VD: "do-len", "gom-su")

    [Required]
    [Column("name")]
    public string Name { get; set; } = string.Empty; // Tên hiển thị (VD: "Đồ Len Thủ Công")

    [Column("icon")]
    public string? Icon { get; set; } // Emoji hoặc biểu tượng (VD: "🧶", "🏺")

    [Column("description")]
    public string? Description { get; set; } // Mô tả ngắn về danh mục
}
