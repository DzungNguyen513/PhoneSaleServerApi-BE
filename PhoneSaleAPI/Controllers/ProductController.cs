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
    public class ProductController : ControllerBase
    {
        private readonly PhoneManagementContext _context;
        private readonly IWebHostEnvironment _environment;
        public ProductController(PhoneManagementContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: api/Product
        [HttpGet("GetProducts")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
          if (_context.Products == null)
          {
              return NotFound();
          }
            return await _context.Products.ToListAsync();
        }

        // GET: api/Product/5
        [HttpGet("GetProduct/{productId}")]
        public async Task<ActionResult<Product>> GetProduct(string productId)
        {
            var product = await _context.Products.FindAsync(productId);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }


        // PUT: api/Product/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(string id, Product product)
        {
            if (id != product.ProductId)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
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

        // POST: api/Product
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
          if (_context.Products == null)
          {
              return Problem("Entity set 'PhoneManagementContext.Products'  is null.");
          }
            _context.Products.Add(product);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ProductExists(product.ProductId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetProduct", new { id = product.ProductId }, product);
        }

        // DELETE: api/Product/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            if (_context.Products == null)
            {
                return NotFound();
            }
            var product = await _context.Products.FindAsync(id);
            var productDetail = await _context.ProductDetails.FindAsync(id);
            if (product == null && productDetail == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            _context.ProductDetails.Remove(productDetail);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(string id)
        {
            return (_context.Products?.Any(e => e.ProductId == id)).GetValueOrDefault();
        }

        [HttpPost("CreateProduct")]
        public async Task<IActionResult> CreateProduct([FromBody] ProductCreateModel productDTO)
        {
            var lastProduct = await _context.Products.OrderByDescending(p => p.ProductId).FirstOrDefaultAsync();
            var productIdNumber = lastProduct != null ? int.Parse(lastProduct.ProductId.Replace("PRD", "")) + 1 : 1;
            var newProductId = $"PRD{productIdNumber:000}";

            var product = new Product
            {
                ProductId = newProductId,
                ProductName = productDTO.ProductName,
                Price = productDTO.Price,
                CategoryId = productDTO.CategoryId,
                VendorId = productDTO.VendorId,
                Detail = productDTO.Detail,
                Status = productDTO.Status
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return Ok(new { product.ProductId });
        }

        [HttpPost("CreateProductAdmin")]
        public async Task<IActionResult> CreateProductAdmin([FromBody] AdminCreateProduct productDTO)
        {
            if (productDTO != null)
            {
                // Kiểm tra các trường bắt buộc (đã bỏ qua để giảm chiều dài code)

                try
                {
                    // Tạo GUID mới
                    var guid = Guid.NewGuid();

                    // Sử dụng GUID để tạo mã sản phẩm mới
                    var newProductId = $"PRD{guid.ToString().Substring(0, 6).ToUpper()}";

                    var product = new Product
                    {
                        ProductId = newProductId,
                        ProductName = productDTO.ProductName,
                        Price = productDTO.Price,
                        Discount = productDTO.Discount,
                        CategoryId = productDTO.CategoryId,
                        VendorId = productDTO.VendorId,
                        Detail = productDTO.Detail,
                        Status = productDTO.Status
                    };

                    var productDetail = new ProductDetail
                    {
                        ProductId = newProductId,
                        ColorName = productDTO.ColorName,
                        StorageGb = productDTO.StorageGB,
                        Amount = productDTO.Amount
                    };

                    _context.Products.Add(product);
                    await _context.SaveChangesAsync();
                    _context.ProductDetails.Add(productDetail);
                    await _context.SaveChangesAsync();


                    return Ok(new { product.ProductId });
                }
                catch (Exception ex)
                {
                    // Xử lý nếu có lỗi trong quá trình lưu sản phẩm vào CSDL
                    return StatusCode(500, $"Internal server error: {ex.InnerException?.Message}");
                }
            }
            else
            {
                return BadRequest("Invalid product data.");
            }
        }

        [HttpGet("GetProductImages/{productId}")]
        public async Task<ActionResult<IEnumerable<ProductImage>>> GetProductImages(string productId)
        {
            var productImages = await _context.ProductImages
                .Where(pi => pi.ProductId == productId)
                .ToListAsync();

            if (productImages == null || productImages.Count == 0)
            {
                return NotFound("Không tìm thấy ảnh cho sản phẩm có id là " + productId);
            }

            return Ok(productImages);
        }

        [HttpGet("GetProductDetails/{productId}")]
        public async Task<ActionResult<IEnumerable<ProductImage>>> GetProductDetails(string productId)
        {
            var productDetail = await _context.ProductDetails
                .Where(pi => pi.ProductId == productId)
                .ToListAsync();

            if (productDetail == null || productDetail.Count == 0)
            {
                return NotFound("Không tìm thấy  sản phẩm có id là " + productId);
            }

            return Ok(productDetail);
        }

        [HttpGet("GetALLProductDetails")]
        public async Task<ActionResult<IEnumerable<ProductDetail>>> GetALLProductDetail()
        {
            var productDetails = await _context.ProductDetails.ToListAsync();

            if (productDetails == null)
            {
                return NotFound();
            }

            return productDetails;
        }


    }
}
