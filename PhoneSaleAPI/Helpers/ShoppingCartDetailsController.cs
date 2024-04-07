using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneSaleAPI.Models;

namespace PhoneSaleAPI.Helpers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartDetailsController : ControllerBase
    {
        private readonly PhoneManagementContext _context;

        public ShoppingCartDetailsController(PhoneManagementContext context)
        {
            _context = context;
        }

        // GET: api/ShoppingCartDetails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShoppingCartDetail>>> GetShoppingCartDetails()
        {
          if (_context.ShoppingCartDetails == null)
          {
              return NotFound();
          }
            return await _context.ShoppingCartDetails.ToListAsync();
        }

        // GET: api/ShoppingCartDetails/5
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

        // PUT: api/ShoppingCartDetails/5
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

        // POST: api/ShoppingCartDetails
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

        // DELETE: api/ShoppingCartDetails/5
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
        
        
    }
}
