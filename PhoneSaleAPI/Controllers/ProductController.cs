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
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
          if (_context.Products == null)
          {
              return NotFound();
          }
            return await _context.Products.ToListAsync();
        }

        // GET: api/Product/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(string id)
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
        public async Task<IActionResult> CreateProduct([FromForm] ProductCreateModel productDTO)
        {
            if (productDTO.ImageFile != null)
            {
                var folderName = Path.Combine("Assets", "Images");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                if (!Directory.Exists(pathToSave))
                {
                    Directory.CreateDirectory(pathToSave);
                }

                var fileName = Path.GetFileNameWithoutExtension(productDTO.ImageFile.FileName);
                var extension = Path.GetExtension(productDTO.ImageFile.FileName);
                fileName = $"{fileName}{extension}";
                var fullPath = Path.Combine(pathToSave, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await productDTO.ImageFile.CopyToAsync(stream);
                }

                var dbPath = Path.Combine(folderName, fileName);

                var lastProduct = await _context.Products.OrderByDescending(p => p.ProductId).FirstOrDefaultAsync();
                var productIdNumber = lastProduct != null ? int.Parse(lastProduct.ProductId.Replace("PRD", "")) + 1 : 1;
                var newProductId = $"PRD{productIdNumber:000}";

                var product = new Product
                {
                    ProductId = newProductId,
                    ProductName = productDTO.ProductName,
                    StorageGb = productDTO.StorageGB,
                    ColorName = productDTO.ColorName,
                    Amount = productDTO.Amount,
                    Price = productDTO.Price,
                    CategoryId = productDTO.CategoryId,
                    VendorId = productDTO.VendorId,
                    Detail = productDTO.Detail,
                    Img = fileName,
                    Status = productDTO.Status
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                return Ok(new { product.ProductId, product.Img });
            }
            else
            {
                return BadRequest("Không thể thêm hình ảnh");
            }
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
                        StorageGb = productDTO.StorageGB,
                        ColorName = productDTO.ColorName,
                        Amount = productDTO.Amount,
                        Price = productDTO.Price,
                        CategoryId = productDTO.CategoryId,
                        VendorId = productDTO.VendorId,
                        Detail = productDTO.Detail,
                        Img = productDTO.ImageFile,
                        Status = productDTO.Status
                    };

                    _context.Products.Add(product);
                    await _context.SaveChangesAsync();

                    return Ok(new { product.ProductId, product.Img });
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



    }
}
