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
        public async Task<ActionResult<Product>> GetProduct(string ProductId)
        {
          if (_context.Products == null)
          {
              return NotFound();
          }
            var product = await _context.Products.FindAsync(ProductId);

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
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
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
        
        [HttpGet("CalculateProductDetailPrice/{productId}")]
        public async Task<ActionResult<int?>> CalculateProductDetailPrice(string productId, string? colorName, int? storageGb)
        {
            // Tìm kiếm thông tin về dung lượng và màu sắc (nếu được cung cấp)
            var storage = storageGb != null ? await _context.Storages.FirstOrDefaultAsync(s => s.StorageGb == storageGb) : null;
            var color = !string.IsNullOrEmpty(colorName) ? await _context.Colors.FirstOrDefaultAsync(c => c.ColorName == colorName) : null;

            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product == null)
            {
                return NotFound("Không tìm thấy thông tin sản phẩm.");
            }

            int totalPrice = (int)product.Price + (storage?.StoragePrice ?? 0) + (color?.ColorPrice ?? 0);

            return Ok(totalPrice);
        }
        [HttpGet("TotalAmountByProductId/{productId}")]

        public ActionResult<int> TotalAmountByProductId(string productId)
        {
            // Truy vấn từ cơ sở dữ liệu để lấy tổng số lượng theo ProductId
            int totalAmount = _context.ProductDetails
                                        .Where(pd => pd.ProductId == productId)
                                        .Sum(pd => pd.Amount) ?? 0;

            return Ok(totalAmount);



        }

        [HttpGet("AmountByColorStorage/{productId}")]

        public ActionResult<int> AmountByColorStorage(string productId, string? colorName, int? storageGb)
        {
            
            var query = _context.ProductDetails.Where(pd => pd.ProductId == productId);

            if (!string.IsNullOrEmpty(colorName))
            {
                query = query.Where(pd => pd.ColorName == colorName);
            }

            if (storageGb != null && storageGb != 0)
            {
                query = query.Where(pd => pd.StorageGb == storageGb);
            }

            int totalAmount = query.Sum(pd => pd.Amount) ?? 0;

            return Ok(totalAmount);
        }
       




    }
}
