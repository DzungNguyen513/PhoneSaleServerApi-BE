using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using PhoneSaleAPI.Models;

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
            .FirstOrDefaultAsync(c => c.Email == model.Email && c.Password == hashedPassword);

        if (customer == null)
        {
            return NotFound(new { success = false, message = "Sai email hoặc mật khẩu" });
        }

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

public class LoginModel
{
    public string Email { get; set; }
    public string Password { get; set; }
}
