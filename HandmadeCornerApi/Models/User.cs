using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HandmadeCornerApi.Models;

/// <summary>
/// Model Khách hàng (users) trong SQLite.
/// </summary>
[Table("users")]
public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public long Id { get; set; } // Khóa chính tự động tăng

    [Required]
    [Column("email")]
    public string Email { get; set; } = string.Empty; // Email đăng nhập duy nhất

    [Required]
    [Column("password")]
    public string Password { get; set; } = string.Empty; // Mật khẩu

    [Required]
    [Column("name")]
    public string Name { get; set; } = string.Empty; // Họ và tên khách hàng

    [Column("phone")]
    public string? Phone { get; set; } // Số điện thoại liên hệ

    [Column("address")]
    public string? Address { get; set; } // Địa chỉ nhận hàng

    [Column("city")]
    public string? City { get; set; } // Tỉnh / Thành phố

    [Column("created_at")]
    public string CreatedAt { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
}

/// <summary>
/// Model Quản trị viên (admins) trong SQLite.
/// </summary>
[Table("admins")]
public class Admin
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public long Id { get; set; } // ID tự tăng

    [Required]
    [Column("username")]
    public string Username { get; set; } = string.Empty; // Tên đăng nhập Admin

    [Required]
    [Column("password")]
    public string Password { get; set; } = string.Empty; // Mật khẩu Admin

    [Required]
    [Column("name")]
    public string Name { get; set; } = string.Empty; // Tên hiển thị của Quản trị viên

    [Column("role")]
    public string Role { get; set; } = "admin"; // Vai trò phân quyền

    [Column("created_at")]
    public string CreatedAt { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
}
