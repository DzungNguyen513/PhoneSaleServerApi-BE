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
        [HttpGet("GetCartItems/{customerId}")]
        public ActionResult<IEnumerable<CartItemDto>> GetCartItems(string customerId)
        {
            var cartItems = (from product in _context.Products
                             join cartDetail in _context.ShoppingCartDetails on product.ProductId equals cartDetail.ProductId
                             join cart in _context.ShoppingCarts on cartDetail.ShoppingCartId equals cart.ShoppingCartId
                             where cart.CustomerId == customerId
                             select new CartItemDto
                             {
                                 ShoppingCartId = cart.ShoppingCartId,
                                 ProductID = product.ProductId,
                                 ProductName = product.ProductName,
                                 Price = (int)cartDetail.Price,
                                 Amount = (int)cartDetail.Amount,
                                 Img = product.Img
                             }).ToList();

            if (cartItems == null || cartItems.Count == 0)
            {
                return NotFound();
            }

            return Ok(cartItems);
        }
        [HttpDelete("{shoppingCartId}/{productId}")]
        public async Task<IActionResult> DeleteProductFromCart(string shoppingCartId, string productId)
        {
            if (_context.ShoppingCartDetails == null)
            {
                return NotFound("Không tìm thấy ShoppingCartDetail");
            }
            var shoppingCartDetail = await _context.ShoppingCartDetails
                .Where(detail => detail.ShoppingCartId == shoppingCartId && detail.ProductId == productId)
                .FirstOrDefaultAsync();

            if (shoppingCartDetail == null)
            {
                return NotFound("Không có sản phẩm trong giỏ hàng");
            }

            _context.ShoppingCartDetails.Remove(shoppingCartDetail);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }

}
