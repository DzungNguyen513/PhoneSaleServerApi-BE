using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneSaleAPI.Models;

namespace PhoneSaleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartDetailController : ControllerBase
    {
        private readonly PhoneManagementContext _context;

        public ShoppingCartDetailController(PhoneManagementContext context)
        {
            _context = context;
        }

        // GET: api/ShoppingCartDetail
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShoppingCartDetail>>> GetShoppingCartDetails()
        {
          if (_context.ShoppingCartDetails == null)
          {
              return NotFound();
          }
            return await _context.ShoppingCartDetails.ToListAsync();
        }

        // GET: api/ShoppingCartDetail/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ShoppingCartDetail>> GetShoppingCartDetail(string id)
        {
          if (_context.ShoppingCartDetails == null)
          {
              return NotFound();
          }
            var shoppingCartDetail = await _context.ShoppingCartDetails.FindAsync(id);

            if (shoppingCartDetail == null)
            {
                return NotFound();
            }

            return shoppingCartDetail;
        }

        // PUT: api/ShoppingCartDetail/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutShoppingCartDetail(string id, ShoppingCartDetail shoppingCartDetail)
        {
            if (id != shoppingCartDetail.ShoppingCartId)
            {
                return BadRequest();
            }

            _context.Entry(shoppingCartDetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShoppingCartDetailExists(id))
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

        // POST: api/ShoppingCartDetail
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ShoppingCartDetail>> PostShoppingCartDetail(ShoppingCartDetail shoppingCartDetail)
        {
          if (_context.ShoppingCartDetails == null)
          {
              return Problem("Entity set 'PhoneManagementContext.ShoppingCartDetails'  is null.");
          }
            _context.ShoppingCartDetails.Add(shoppingCartDetail);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ShoppingCartDetailExists(shoppingCartDetail.ShoppingCartId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetShoppingCartDetail", new { id = shoppingCartDetail.ShoppingCartId }, shoppingCartDetail);
        }

        // DELETE: api/ShoppingCartDetail/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShoppingCartDetail(string id)
        {
            if (_context.ShoppingCartDetails == null)
            {
                return NotFound();
            }
            var shoppingCartDetail = await _context.ShoppingCartDetails.FindAsync(id);
            if (shoppingCartDetail == null)
            {
                return NotFound();
            }

            _context.ShoppingCartDetails.Remove(shoppingCartDetail);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ShoppingCartDetailExists(string id)
        {
            return (_context.ShoppingCartDetails?.Any(e => e.ShoppingCartId == id)).GetValueOrDefault();
        }

        [HttpGet("Details/{customerId}")]
        public async Task<ActionResult<IEnumerable<ShoppingCartDetailDTO>>> GetShoppingCartDetails(string customerId)
        {
            var shoppingCart = await _context.ShoppingCarts
                                             .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (shoppingCart == null)
            {
                return NotFound();
            }

            var shoppingCartDetails = await _context.ShoppingCartDetails
                .Where(d => d.ShoppingCartId == shoppingCart.ShoppingCartId)
                .Include(d => d.Product)
                .Select(d => new ShoppingCartDetailDTO
                {
                    ProductId = d.ProductId,
                    ProductName = d.Product.ProductName,
                    Price = (decimal)d.Price,
                    Quantity = (int)d.Amount,
                    Total = (decimal)d.Total
                })
                .ToListAsync();

            return shoppingCartDetails;
        }

    }
    public class ShoppingCartDetailDTO
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }
    }

}
