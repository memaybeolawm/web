using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HandmadeCornerApi.Data;
using HandmadeCornerApi.Models;
using HandmadeCornerApi.Models.Dtos;

namespace HandmadeCornerApi.Controllers;

/// <summary>
/// Controller xử lý xác thực tài khoản (Khách hàng & Admin).
/// </summary>
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;

    public AuthController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// POST /api/auth/register
    /// Đăng ký tài khoản Khách hàng mới
    /// </summary>
    [HttpPost("api/auth/register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password) || string.IsNullOrWhiteSpace(dto.Name))
            {
                return BadRequest(new { success = false, message = "Vui lòng nhập đầy đủ Email, Mật khẩu và Họ tên!" });
            }

            var cleanEmail = dto.Email.Trim().ToLower();

            // Kiểm tra email đã tồn tại chưa bằng EF Core AnyAsync
            var exists = await _context.Users.AnyAsync(u => u.Email == cleanEmail);
            if (exists)
            {
                return BadRequest(new { success = false, message = "Email này đã được đăng ký tài khoản!" });
            }

            var newUser = new User
            {
                Email = cleanEmail,
                Password = dto.Password,
                Name = dto.Name.Trim(),
                Phone = dto.Phone,
                Address = dto.Address,
                City = dto.City,
                CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
            };

            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Đăng ký tài khoản thành công!",
                user = new
                {
                    id = newUser.Id,
                    email = newUser.Email,
                    name = newUser.Name,
                    phone = newUser.Phone,
                    address = newUser.Address,
                    city = newUser.City,
                    createdAt = newUser.CreatedAt
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// POST /api/auth/login
    /// Đăng nhập Khách hàng
    /// </summary>
    [HttpPost("api/auth/login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            {
                return BadRequest(new { success = false, message = "Vui lòng nhập email và mật khẩu!" });
            }

            var cleanEmail = dto.Email.Trim().ToLower();

            // Tìm user khớp email và mật khẩu qua EF Core
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == cleanEmail && u.Password == dto.Password);

            if (user == null)
            {
                return Unauthorized(new { success = false, message = "Email hoặc mật khẩu không chính xác!" });
            }

            return Ok(new
            {
                success = true,
                message = "Đăng nhập thành công!",
                user = new
                {
                    id = user.Id,
                    email = user.Email,
                    name = user.Name,
                    phone = user.Phone,
                    address = user.Address,
                    city = user.City,
                    createdAt = user.CreatedAt
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// PUT /api/auth/profile
    /// Cập nhật thông tin cá nhân khách hàng
    /// </summary>
    [HttpPut("api/auth/profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
    {
        try
        {
            var user = await _context.Users.FindAsync(dto.Id);
            if (user == null)
            {
                return NotFound(new { success = false, message = "Không tìm thấy người dùng!" });
            }

            user.Name = dto.Name;
            user.Phone = dto.Phone;
            user.Address = dto.Address;
            user.City = dto.City;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                user = new
                {
                    id = user.Id,
                    email = user.Email,
                    name = user.Name,
                    phone = user.Phone,
                    address = user.Address,
                    city = user.City,
                    createdAt = user.CreatedAt
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// POST /api/admin/login
    /// Đăng nhập Quản trị viên (Admin)
    /// </summary>
    [HttpPost("api/admin/login")]
    public async Task<IActionResult> AdminLogin([FromBody] AdminLoginDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
            {
                return BadRequest(new { success = false, message = "Nhập tên đăng nhập và mật khẩu Admin!" });
            }

            var cleanUsername = dto.Username.Trim();

            var admin = await _context.Admins
                .FirstOrDefaultAsync(a => a.Username == cleanUsername && a.Password == dto.Password);

            if (admin == null)
            {
                return Unauthorized(new { success = false, message = "Tài khoản hoặc mật khẩu Admin không đúng!" });
            }

            var token = "admin-token-" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            return Ok(new
            {
                success = true,
                message = "Đăng nhập Admin thành công!",
                admin = new
                {
                    id = admin.Id,
                    username = admin.Username,
                    name = admin.Name,
                    role = admin.Role
                },
                token
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }
}
