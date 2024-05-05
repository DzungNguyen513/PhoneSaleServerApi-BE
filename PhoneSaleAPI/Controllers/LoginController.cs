using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using PhoneSaleAPI.Models;
using PhoneSaleAPI.DTO.Customer;
using PhoneSaleAPI.DTO.Account;
using System.Security.Principal;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly PhoneManagementContext _context;

    public LoginController(PhoneManagementContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var hashedPassword = HashPassword(model.Password);

        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Email == model.Email && c.Password == hashedPassword && c.Status == 1);

        if (customer == null)
        {
            var lockedCustomer = await _context.Customers
                .AnyAsync(c => c.Email == model.Email && c.Status == 0);

            if (lockedCustomer)
            {
                return BadRequest(new { success = false, message = "Tài khoản của bạn đã bị khóa" });
            }

            return NotFound(new { success = false, message = "Sai email hoặc mật khẩu" });
        }

        TimeZoneInfo vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        DateTime serverTime = DateTime.UtcNow;
        DateTime serverTimeUtc = DateTime.SpecifyKind(serverTime, DateTimeKind.Utc); // Đặt Kind của thời gian thành Utc
        DateTime vietnamTime = TimeZoneInfo.ConvertTime(serverTimeUtc, vnTimeZone); ;

        customer.LastLogin = vietnamTime; // Cập nhật thời gian đăng nhập
        _context.SaveChanges();
        return Ok(new { success = true, message = "Đăng nhập thành công" });
    }

    [HttpPost("LoginAccount")]
    public async Task<IActionResult> LoginAccount([FromBody] LoginAccount model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var hashedPassword = HashPassword(model.Password);

        var account = await _context.Accounts
            .FirstOrDefaultAsync(c => c.Username == model.UserName && c.Password == hashedPassword && c.Status == 1);

        if (account == null)
        {
            var lockedAccount = await _context.Accounts
                .AnyAsync(c => c.Username == model.UserName && c.Status == 0);

            if (lockedAccount)
            {
                return BadRequest(new { success = false, message = "Tài khoản của bạn đã bị khóa" });
            }

            return NotFound(new { success = false, message = "Sai tài khoản hoặc mật khẩu" });
        }


        TimeZoneInfo vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        DateTime serverTime = DateTime.UtcNow;
        DateTime serverTimeUtc = DateTime.SpecifyKind(serverTime, DateTimeKind.Utc); // Đặt Kind của thời gian thành Utc
        DateTime vietnamTime = TimeZoneInfo.ConvertTime(serverTimeUtc, vnTimeZone); ;

        account.LastLogin = vietnamTime; // Cập nhật thời gian đăng nhập
        _context.SaveChanges();
        return Ok(new { success = true, message = "Đăng nhập thành công" });
    }

    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }
    }

}


