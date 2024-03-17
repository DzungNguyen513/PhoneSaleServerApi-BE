using System;
using System.Collections.Generic;
using System.Linq;
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
    public class ShoppingCartDetailController : ControllerBase
    {
        private readonly PhoneManagementContext _context;

        public ShoppingCartDetailController(PhoneManagementContext context)
        {
            _context = context;
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

}
