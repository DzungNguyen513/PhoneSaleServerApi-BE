﻿using System;
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
                             join image in _context.ProductImages on product.ProductId equals image.ProductId into productImages
                             from pi in productImages.Where(x => (bool)x.IsPrimary).DefaultIfEmpty()
                             where cart.CustomerId == customerId
                             select new CartItemDto
                             {
                                 ShoppingCartId = cart.ShoppingCartId,
                                 ProductID = product.ProductId,
                                 ProductName = product.ProductName,
                                 Price = (int)product.Price,
                                 ColorName = cartDetail.ColorName,
                                 StorageGB = cartDetail.StorageGb,
                                 Amount = (int)cartDetail.Amount,
                                 Img = pi != null ? pi.ImagePath : null 
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

        [HttpPut("UpdateAmount")]
        public async Task<IActionResult> UpdateProductAmount([FromBody] UpdateAmountRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cartDetail = await _context.ShoppingCartDetails.FirstOrDefaultAsync(cd => cd.ShoppingCartId == request.ShoppingCartId && cd.ProductId == request.ProductId);

            if (cartDetail == null)
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm trong giỏ hàng." });
            }

            cartDetail.Amount = request.NewAmount;
            _context.Entry(cartDetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw; 
            }

            return NoContent();
        }
    }

}
