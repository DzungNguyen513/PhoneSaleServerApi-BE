using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneSaleAPI.DTO.ShoppingCart;
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
        public async Task<ActionResult<IEnumerable<CartItemDto>>> GetCartItems(string customerId)
        {
            var cartItems = await (from product in _context.Products
                                   join cartDetail in _context.ShoppingCartDetails on product.ProductId equals cartDetail.ProductId
                                   join cart in _context.ShoppingCarts on cartDetail.ShoppingCartId equals cart.ShoppingCartId
                                   join color in _context.Colors on cartDetail.ColorName equals color.ColorName
                                   join storage in _context.Storages on cartDetail.StorageGb equals storage.StorageGb
                                   join image in _context.ProductImages on new { product.ProductId, cartDetail.ColorName } equals new { image.ProductId, image.ColorName } into productImages
                                   from pi in productImages.DefaultIfEmpty()
                                   where cart.CustomerId == customerId
                                   select new CartItemDto
                                   {
                                       ShoppingCartId = cart.ShoppingCartId,
                                       ProductID = product.ProductId,
                                       ProductName = product.ProductName,
                                       OriginalPrice = (int)(product.Price + color.ColorPrice + storage.StoragePrice),
                                       DiscountedPrice = (int)((product.Price + color.ColorPrice + storage.StoragePrice) * (1 - (product.Discount / 100.0))),
                                       ColorName = cartDetail.ColorName,
                                       StorageGB = cartDetail.StorageGb,
                                       Amount = (int)cartDetail.Amount,
                                       Img = pi != null ? pi.ImagePath : null
                                   }).ToListAsync();
            if (!cartItems.Any())
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

        [HttpGet("GetShoppingCart/{id}")]
        public async Task<ActionResult<ShoppingCartDetail>> GetShoppingCart(String id)
        {
            var shoppingCartDetail = await _context.ShoppingCartDetails.FindAsync(id);

            if (shoppingCartDetail == null)
            {
                return NotFound(); // Trả về mã 404 Not Found nếu không tìm thấy chi tiết giỏ hàng
            }

            return shoppingCartDetail; // Trả về chi tiết giỏ hàng nếu tìm thấy thành công
        }

        [HttpPost("PostShoppingCartDetail")]
        public async Task<ActionResult<int>> PostShoppingCartDetail([FromBody] CartDetailDTO parameters)
        {
            if (!ModelState.IsValid)
            {
                return 0;
            }

            // Kiểm tra xem sản phẩm đã tồn tại trong giỏ hàng chưa
            var existingDetail = await _context.ShoppingCartDetails
                .FirstOrDefaultAsync(d =>
                    d.ShoppingCartId == parameters.ShoppingCartId &&
                    d.ProductId == parameters.ProductId &&
                    d.ColorName == parameters.ColorName &&
                    d.StorageGb == parameters.StorageGb);

            if (existingDetail != null)
            {
                // Cộng thêm số lượng và giá tiền
                existingDetail.Amount += parameters.Amount;
                existingDetail.Total += parameters.Total;
            }
            else
            {
                // Tạo một chi tiết giỏ hàng mới
                var shoppingCartDetail = new ShoppingCartDetail
                {
                    ShoppingCartId = parameters.ShoppingCartId,
                    ProductId = parameters.ProductId,
                    ColorName = parameters.ColorName,
                    StorageGb = parameters.StorageGb,
                    Amount = parameters.Amount,
                    Total = parameters.Total
                };

                // Thêm chi tiết giỏ hàng vào cơ sở dữ liệu
                _context.ShoppingCartDetails.Add(shoppingCartDetail);
            }

            try
            {
                await _context.SaveChangesAsync();
                return 1;
            }
            catch (DbUpdateException)
            {
                // Xử lý ngoại lệ DbUpdateException
                return 0;
            }
        }

            private bool ShoppingCartExists(string id)
            {
                return (_context.ShoppingCarts?.Any(e => e.ShoppingCartId == id)).GetValueOrDefault();
            }

        }

    

}
