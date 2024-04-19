using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhoneSaleAPI.DTO;
using PhoneSaleAPI.Models;

namespace PhoneSaleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly PhoneManagementContext _context;
        public RegisterController(PhoneManagementContext context)
        {
            _context = context;
        }
        [HttpPost("register")]
        public IActionResult Register([FromBody] CreateUserDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Tạo một đối tượng User từ dữ liệu nhận được
            var user = new Account
            {
                Username = model.Username,
                Password = model.Password
                // Bạn có thể thêm các trường khác tùy thuộc vào cấu trúc của model User trong cơ sở dữ liệu
            };

            // Thêm đối tượng User vào cơ sở dữ liệu
            _context.Accounts.Add(user);
            _context.SaveChanges(); // Lưu các thay đổi vào cơ sở dữ liệu

            return Ok("User created successfully");
        }
    }
}
