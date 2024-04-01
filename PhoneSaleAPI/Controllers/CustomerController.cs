using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneSaleAPI.DTO;
using PhoneSaleAPI.Models;

namespace PhoneSaleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly PhoneManagementContext _context;

        public CustomerController(PhoneManagementContext context)
        {
            _context = context;
        }

        // GET: api/Customer
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
          if (_context.Customers == null)
          {
              return NotFound();
          }
            return await _context.Customers.ToListAsync();
        }

        // GET: api/Customer/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(string id)
        {
          if (_context.Customers == null)
          {
              return NotFound();
          }
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return customer;
        }

        // PUT: api/Customer/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(string id, Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return BadRequest();
            }

            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
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

        // POST: api/Customer
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {
          if (_context.Customers == null)
          {
              return Problem("Entity set 'PhoneManagementContext.Customers'  is null.");
          }
            _context.Customers.Add(customer);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CustomerExists(customer.CustomerId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCustomer", new { id = customer.CustomerId }, customer);
        }

        // DELETE: api/Customer/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(string id)
        {
            if (_context.Customers == null)
            {
                return NotFound();
            }
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CustomerExists(string id)
        {
            return (_context.Customers?.Any(e => e.CustomerId == id)).GetValueOrDefault();
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterCustomerModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ" });
            }

            var existingCustomer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == model.Email);
            if (existingCustomer != null)
            {
                return Conflict(new { success = false, message = "Email đã tồn tại trong hệ thống" });
            }

            var hashedPassword = HashPassword(model.Password);

            var newCustomerId = await GenerateNewCustomerId();

            Customer customer = new Customer
            {
                CustomerId = newCustomerId,
                Email = model.Email,
                Password = hashedPassword,
                Status = 1
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            var shoppingCart = new ShoppingCart
            {
                ShoppingCartId = $"SPC{newCustomerId.Substring(3)}", 
                CustomerId = newCustomerId,
                TotalCart = 0,
                Status = 1,
            };

            _context.ShoppingCarts.Add(shoppingCart);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Đăng ký thành công", customerId = newCustomerId });
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        private async Task<string> GenerateNewCustomerId()
        {
            var lastCustomerId = await _context.Customers
                .OrderByDescending(c => c.CustomerId)
                .Select(c => c.CustomerId)
                .FirstOrDefaultAsync();

            int nextIdNumber = 1;
            if (lastCustomerId != null && lastCustomerId.StartsWith("MKH"))
            {
                int.TryParse(lastCustomerId.Substring(3), out nextIdNumber);
                nextIdNumber++;
            }

            return $"MKH{nextIdNumber:000}";
        }
        [HttpGet("GetCustomerIDByEmail/{email}")]
        public async Task<IActionResult> GetCustomerIDByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest(new { error = "Email is required" });
            }

            var customer = await _context.Customers
                                          .FirstOrDefaultAsync(c => c.Email == email);
            if (customer == null)
            {
                return NotFound(new { error = "Customer not found" });
            }

            return Ok(new { customerId = customer.CustomerId });
        }

    }

}
