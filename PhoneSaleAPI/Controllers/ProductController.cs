using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using PhoneSaleAPI.DTO.Product;
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
            var products = await _context.Products
                                   .Where(p => p.Status == 1)
                                   .ToListAsync();

            if (products == null || products.Count == 0)
            {
                return NotFound();
            }

            return products;
        }

        [HttpGet("GetProductCustomer")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductCustomer()
        {
            var products = await _context.Products
                                   .Where(p => p.Status == 1)
                                   .ToListAsync();

            if (products == null || products.Count == 0)
            {
                return NotFound();
            }

            return products;
        }

        // GET: api/Product/5
        [HttpGet("GetProduct/{productId}")]
        public async Task<ActionResult<Product>> GetProduct(string productId)
        {
            var product = await _context.Products.FindAsync(productId);

            if (product == null || product.Status != 1)
            {
                return NotFound();
            }

            return product;
        }

        [HttpGet("GetProductCustomer/{productId}")]
        public async Task<ActionResult<Product>> GetProductCustomer(string productId)
        {
            var product = await _context.Products.FindAsync(productId);

            if (product == null || product.Status != 1)
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
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            product.Status = 0; // Chuyển trạng thái của sản phẩm sang 1

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

                    _context.Products.Add(product);
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

        [HttpPost("CreateProductDetails")]
        public async Task<IActionResult> CreateProductDetail(ProductDetailDTO productDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Tạo một đối tượng Product từ dữ liệu DTO
                var product = new ProductDetail
                {
                    ProductId = productDTO.ProductId,
                    ColorName = productDTO.ColorName,
                    StorageGb = productDTO.StorageGb,
                    Amount = productDTO.Amount
                    // Khởi tạo các trường dữ liệu khác của sản phẩm ở đây nếu cần
                };

                // Thêm sản phẩm mới vào cơ sở dữ liệu
                _context.ProductDetails.Add(product);
                await _context.SaveChangesAsync();

                return Ok(new { product.ProductId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/ProductDetails/5
        [HttpPut("EditProductDetails/{productId}/{storageGb}/{colorName}")]
        public async Task<IActionResult> PutProductDetail(string productId, int storageGb, string colorName, ProductDetailDTO productDetailDTO)
        {

            // Kiểm tra xem chi tiết sản phẩm có tồn tại trong cơ sở dữ liệu không
            var existingDetail = await _context.ProductDetails.FirstOrDefaultAsync(d =>
                d.ProductId == productId && d.StorageGb == storageGb && d.ColorName == colorName);

            if (existingDetail == null)
            {
                return NotFound("Product detail not found.");
            }

            // Cập nhật thuộc tính của existingDetail
            existingDetail.Amount = productDetailDTO.Amount;



            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Kiểm tra xem chi tiết sản phẩm vẫn tồn tại trong cơ sở dữ liệu sau khi cập nhật
                if (!ProductDetailExists(productId, storageGb, colorName))
                {
                    return NotFound("Product detail not found.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool ProductDetailExists(string productId, int storageGb, string colorName)
        {
            return _context.ProductDetails.Any(d => d.ProductId == productId && d.StorageGb == storageGb && d.ColorName == colorName);
        }

        [HttpGet("FilterByCategory/{CategoryId}")]
        public ActionResult<IEnumerable<Product>> FilterByCategory(string CategoryId)
        {
            
            var products = _context.Products
                                    .Where(p => p.CategoryId == CategoryId && p.Status == 1)
                                    .ToList();

            if (!products.Any())
            {
                return NotFound();
            }

            return products;
        }



        [HttpGet("SearchProducts/{searchString}")]
        public ActionResult<IEnumerable<Product>> SearchProducts(string searchString, String? categoryId, int? priceMin, int? priceMax)
        {
            var productsQuery = _context.Products.Where(p => p.Status == 1 && (p.ProductName.Contains(searchString) || p.Detail.Contains(searchString)));

            if (!string.IsNullOrEmpty(categoryId))
            {
                productsQuery = productsQuery.Where(p => p.CategoryId == categoryId);
            }

            if (priceMin.HasValue && priceMax.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price >= priceMin && p.Price <= priceMax);
            }

            
            var products = productsQuery.ToList();
           
            return products;
        }
        [HttpGet("SortProductsByPrice/{i}")]
        public ActionResult<IEnumerable<Product>> SortProductsByPrice(int i)
        {
            IQueryable<Product> productsQuery = _context.Products.Where(p => p.Status == 1);

            if (i == 1)
            {
                // Sắp xếp tăng dần theo giá
                productsQuery = productsQuery.OrderBy(p => p.Price);
            }
            else if (i == 0)
            {
                // Sắp xếp giảm dần theo giá
                productsQuery = productsQuery.OrderByDescending(p => p.Price);
            }

            var sortedProducts = productsQuery.ToList();
            return sortedProducts;
        }



    }
    }

