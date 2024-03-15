using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhoneSaleAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace PhoneSaleAPI.Controllers
{
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

            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == model.Email && c.Password == model.Password);
            if (customer == null)
            {
                return NotFound(new { success = false, message = "Sai email hoặc mật khẩu" });
            }

            return Ok(new { success = true, message = "Đăng nhập thành công" });
        }
    }

    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
