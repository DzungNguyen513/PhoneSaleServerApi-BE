﻿using System;
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
    public class ShoppingCartController : ControllerBase
    {
        private readonly PhoneManagementContext _context;

        public ShoppingCartController(PhoneManagementContext context)
        {
            _context = context;
        }

        // GET: api/ShoppingCart
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShoppingCart>>> GetShoppingCarts()
        {
          if (_context.ShoppingCarts == null)
          {
              return NotFound();
          }
            return await _context.ShoppingCarts.ToListAsync();
        }

        // GET: api/ShoppingCart/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ShoppingCart>> GetShoppingCart(string id)
        {
          if (_context.ShoppingCarts == null)
          {
              return NotFound();
          }
            var shoppingCart = await _context.ShoppingCarts.FindAsync(id);

            if (shoppingCart == null)
            {
                return NotFound();
            }

            return shoppingCart;
        }

        // PUT: api/ShoppingCart/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutShoppingCart(string id, ShoppingCart shoppingCart)
        {
            if (id != shoppingCart.ShoppingCartId)
            {
                return BadRequest();
            }

            _context.Entry(shoppingCart).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShoppingCartExists(id))
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

        // POST: api/ShoppingCart
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ShoppingCart>> PostShoppingCart(ShoppingCart shoppingCart)
        {
          if (_context.ShoppingCarts == null)
          {
              return Problem("Entity set 'PhoneManagementContext.ShoppingCarts'  is null.");
          }
            _context.ShoppingCarts.Add(shoppingCart);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ShoppingCartExists(shoppingCart.ShoppingCartId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetShoppingCart", new { id = shoppingCart.ShoppingCartId }, shoppingCart);
        }

        // DELETE: api/ShoppingCart/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShoppingCart(string id)
        {
            if (_context.ShoppingCarts == null)
            {
                return NotFound();
            }
            var shoppingCart = await _context.ShoppingCarts.FindAsync(id);
            if (shoppingCart == null)
            {
                return NotFound();
            }

            _context.ShoppingCarts.Remove(shoppingCart);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpGet("GetShoppingCartIdByCustomerId/{customerId}")]
        public ActionResult<string> GetShoppingCartIdByCustomerId(string customerId)
        {
            var shoppingCart = _context.ShoppingCarts
                .FirstOrDefault(sc => sc.CustomerId == customerId);

            if (shoppingCart == null)
            {
                return NotFound(); // Trả về mã lỗi 404 nếu không tìm thấy ShoppingCart cho customerId cụ thể
            }

            return shoppingCart.ShoppingCartId;
        }

        private bool ShoppingCartExists(string id)
        {
            return (_context.ShoppingCarts?.Any(e => e.ShoppingCartId == id)).GetValueOrDefault();
        }


        [HttpGet("GetByCustomerId/{customerId}")]
        public async Task<ActionResult<ShoppingCart>> GetShoppingCartByCustomerId(string customerId)
        {
            var shoppingCart = await _context.ShoppingCarts
                .FirstOrDefaultAsync(sc => sc.CustomerId == customerId);

            if (shoppingCart == null)
            {
                return NotFound(); // Trả về 404 nếu không tìm thấy giỏ hàng
            }

            return shoppingCart;
        }
    }
}
