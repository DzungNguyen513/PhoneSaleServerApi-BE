using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneSaleAPI.DTO.Account;
using PhoneSaleAPI.DTO.Customer;
using PhoneSaleAPI.Models;

namespace PhoneSaleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly PhoneManagementContext _context;

        public AccountController(PhoneManagementContext context)
        {
            _context = context;
        }

        // GET: api/Account
        [HttpGet("GetAllAccounts")]
        public async Task<ActionResult<IEnumerable<Account>>> GetAccounts()
        {
          if (_context.Accounts == null)
          {
              return NotFound();
          }
            return await _context.Accounts.ToListAsync();
        }

        // GET: api/Account/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Account>> GetAccount(string id)
        {
          if (_context.Accounts == null)
          {
              return NotFound();
          }
            var account = await _context.Accounts.FindAsync(id);

            if (account == null)
            {
                return NotFound();
            }

            return account;
        }

        // PUT: api/Account/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAccount(string id, Account account)
        {
            if (id != account.AccountId)
            {
                return BadRequest();
            }

            _context.Entry(account).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Account
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Account>> PostAccount(Account account)
        {
            if (_context.Accounts == null)
            {
                return Problem("Entity set 'PhoneManagementContext.Accounts'  is null.");
            }
            _context.Accounts.Add(account);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AccountExists(account.Username))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetAccount", new { id = account.Username }, account);
        }

        // DELETE: api/Account/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(string id)
        {
            if (_context.Accounts == null)
            {
                return NotFound();
            }
            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }

            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AccountExists(string id)
        {
            return (_context.Accounts?.Any(e => e.AccountId == id)).GetValueOrDefault();
        }

        // POST: api/Account
        [HttpPost("CreateAccount")]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccount model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ" });
            }

            var existingAccount = await _context.Accounts.FirstOrDefaultAsync(c => c.Username == model.Username);
            if (existingAccount != null)
            {
                return Conflict(new { success = false, message = "Tài khoản đã tồn tại trong hệ thống" });
            }

            var hashedPassword = HashPassword(model.Password);

            var newAccountId = await GenerateNewAccountId();

            Account account = new Account
            {
                AccountId = newAccountId,
                Username = model.Username,
                Password = hashedPassword,
                Status = 1
            };

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Đăng ký thành công", accountId = newAccountId });
        }

        private async Task<string> GenerateNewAccountId()
        {
            var lastAccountId = await _context.Accounts
                 .OrderByDescending(c => c.AccountId)
                 .Select(c => c.AccountId)
                 .FirstOrDefaultAsync();

            int nextIdNumber = 1;
            if (lastAccountId != null && lastAccountId.StartsWith("ACC"))
            {
                int.TryParse(lastAccountId.Substring(3), out nextIdNumber);
                nextIdNumber++;
            }

            return $"ACC{nextIdNumber:000}";
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        [HttpPut("updateAccountStatus/{accountId}")]
        public async Task<IActionResult> UpdateAccountStatus(string accountId, [FromBody] AccountStatusUpdate statusUpdate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var account = await _context.Accounts.FindAsync(accountId);
            if (account == null)
            {
                return NotFound($"Không tìm thấy khách hàng có ID {accountId}");
            }

            // Cập nhật trạng thái
            account.Status = statusUpdate.Status;
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();

            return Ok(account);
        }
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePassword change)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var account = await _context.Accounts
                .FirstOrDefaultAsync(c => c.Username == change.UserName && c.Status == 1);

            if (account == null)
            {
                return NotFound(new { success = false, message = "Không tìm thấy tài khoản" });
            }

            var hashedOldPassword = HashPassword(change.OldPassword);
            var hashedNewPassword = HashPassword(change.NewPassword);

            if (account.Password != hashedOldPassword)
            {
                return BadRequest(new { success = false, message = "Mật khẩu cũ không chính xác" });
            }

            if (hashedOldPassword == hashedNewPassword)
            {
                return BadRequest(new { success = false, message = "Mật khẩu mới không được trùng với mật khẩu cũ" });
            }

            if (change.NewPassword != change.ConfirmNewPassword)
            {
                return BadRequest(new { success = false, message = "Mật khẩu mới và xác nhận mật khẩu mới không trùng khớp" });
            }

            account.Password = hashedNewPassword;
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Đổi mật khẩu thành công" });
        }

        [HttpGet("LastLogin/{username}")]
        public IActionResult GetLastLogin(string username)
        {
            var account = _context.Accounts.FirstOrDefault(a => a.Username == username);
            if (account == null)
            {
                return NotFound();
            }

            return Ok(new { LastLogin = account.LastLogin });
        }
    }
}
