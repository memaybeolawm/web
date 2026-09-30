namespace HandmadeCornerApi.Models.Dtos;

// ─── Các DTO cho Authentication (Đăng nhập / Đăng ký) ───

public class RegisterDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
}

public class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class AdminLoginDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class UpdateProfileDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
}

// ─── Các DTO cho Quản lý Sản Phẩm ───

public class ProductUpsertDto
{
    public string? Id { get; set; }
    public string CategoryId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public long Price { get; set; }
    public int Stock { get; set; }
    public string? Description { get; set; }
    public List<string>? Tags { get; set; }
    public List<string>? Images { get; set; }
}

// ─── Các DTO cho Đơn hàng ───

public class CreateOrderCustomerInfoDto
{
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? District { get; set; }
    public string? Ward { get; set; }
    public string? Note { get; set; }
    public bool GiftWrap { get; set; }
}

public class CreateOrderItemDto
{
    public string ProductId { get; set; } = string.Empty;
    public int Quantity { get; set; }
}

public class CreateOrderRequestDto
{
    public CreateOrderCustomerInfoDto CustomerInfo { get; set; } = new();
    public List<CreateOrderItemDto> Items { get; set; } = new();
    public string PaymentMethod { get; set; } = "Chuyển khoản QR Pay";
    public long? UserId { get; set; }
}

public class UpdateOrderStatusDto
{
    public string Status { get; set; } = string.Empty;
    public string? Note { get; set; }
}

// ─── Các DTO cho Đánh giá ───

public class CreateReviewDto
{
    public string ProductId { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public int Rating { get; set; } = 5;
    public string? Comment { get; set; }
}
