using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneSaleAPI.DTO.Product;
using PhoneSaleAPI.Models;

namespace PhoneSaleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductImageController : ControllerBase
    {
        private readonly PhoneManagementContext _context;

        public ProductImageController(PhoneManagementContext context)
        {
            _context = context;
        }
        [HttpPost("{productId}")]
        public async Task<IActionResult> AddProductImage(string productId, [FromForm] ProductImageDTO productImageDTO)
        {
            if (productImageDTO.ImageFile != null)
            {
                var folderName = Path.Combine("Assets", "Images", productId);
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                if (!Directory.Exists(pathToSave))
                {
                    Directory.CreateDirectory(pathToSave);
                }

                var fileName = Path.GetFileNameWithoutExtension(productImageDTO.ImageFile.FileName);
                var extension = Path.GetExtension(productImageDTO.ImageFile.FileName);
                fileName = $"{fileName}{extension}";
                var fullPath = Path.Combine(pathToSave, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await productImageDTO.ImageFile.CopyToAsync(stream);
                }

                var dbPath = Path.Combine(folderName, fileName);

                var lastImage = await _context.ProductImages
                                    .OrderByDescending(img => img.ProductImageId)
                                    .FirstOrDefaultAsync();

                int lastNumber = 0;
                if (lastImage != null)
                {
                    var lastId = lastImage.ProductImageId.Replace("PRDIMG", "");
                    lastNumber = int.Parse(lastId);
                }
                var newProductImageId = $"PRDIMG{lastNumber + 1:000}";

                var newProductImage = new ProductImage
                {
                    ProductImageId = newProductImageId,
                    ProductId = productId,
                    ColorName = productImageDTO.ColorName,
                    ImagePath = fileName,
                    IsPrimary = productImageDTO.IsPrimary
                };

                _context.ProductImages.Add(newProductImage);
                await _context.SaveChangesAsync();

                return Ok(new { fileName });
            }
            else
            {
                return BadRequest("Không tìm thấy hình ảnh để tải lên.");
            }
        }
        [HttpGet("GetProductImagesByPath/{productId}")]
        public async Task<ActionResult<IEnumerable<string>>> GetProductImagesByPath(string productId)
        {
            try
            {
                var folderPath = Path.Combine("Assets", "Images", productId);
                var imageFiles = Directory.GetFiles(folderPath);

                if (imageFiles.Length == 0)
                {
                    return NotFound($"Không tìm thấy ảnh cho sản phẩm có id là {productId}");
                }

                var imagePaths = new List<string>();
                foreach (var imagePath in imageFiles)
                {
                    var relativePath = Path.Combine(folderPath, Path.GetFileName(imagePath));
                    imagePaths.Add(relativePath);
                }

                return Ok(imagePaths);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Đã xảy ra lỗi khi lấy ảnh: {ex.Message}");
            }
        }

        [HttpPut("UpdateProductImage/{productId}/{productImageId}")]
        public async Task<IActionResult> UpdateProductImage(string productId, string productImageId, [FromForm] ProductImageDTO productImageDTO)
        {
            // Tìm kiếm ảnh sản phẩm để cập nhật
            var existingProductImage = await _context.ProductImages.FindAsync(productImageId);

            if (existingProductImage == null)
            {
                return NotFound("Product image not found.");
            }

            // Kiểm tra xem có tệp ảnh mới được cung cấp không
            if (productImageDTO.ImageFile != null)
            {
                // Xóa ảnh cũ nếu tồn tại
                var folderPath = Path.Combine("Assets", "Images", productId);
                var oldImagePath = Path.Combine(folderPath, existingProductImage.ImagePath);

                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }

                // Lưu ảnh mới vào thư mục
                var fileName = Path.GetFileNameWithoutExtension(productImageDTO.ImageFile.FileName);
                var extension = Path.GetExtension(productImageDTO.ImageFile.FileName);
                fileName = $"{fileName}{extension}";
                var fullPath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await productImageDTO.ImageFile.CopyToAsync(stream);
                }

                // Cập nhật đường dẫn ảnh mới
                existingProductImage.ImagePath = fileName;
            }

            // Cập nhật các thông tin khác của ảnh sản phẩm

            try
            {
                await _context.SaveChangesAsync();
                return Ok(existingProductImage);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{productId}")]
        public async Task<ActionResult<IEnumerable<ProductImage>>> GetProductImagesByProductId(string productId)
        {
            try
            {
                var productImages = await _context.ProductImages.Where(image => image.ProductId == productId).ToListAsync();
                if (productImages == null || !productImages.Any())
                {
                    return NotFound();
                }

                var imagePaths = await GetProductImagesPaths(productId);
                var productImagesDTO = new List<ProductImage>();

                foreach (var productImage in productImages)
                {
                    var imagePath = imagePaths.FirstOrDefault(path => Path.GetFileName(path) == productImage.ImagePath);
                    var productImageDTO = new ProductImage
                    {
                        ProductImageId = productImage.ProductImageId,
                        ProductId = productImage.ProductId,
                        ColorName = productImage.ColorName,
                        ImagePath = imagePath,
                        IsPrimary = productImage.IsPrimary,
                        CreateAt = productImage.CreateAt,
                        UpdateAt = productImage.UpdateAt
                    };
                    productImagesDTO.Add(productImageDTO);
                }

                return Ok(productImagesDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Đã xảy ra lỗi khi lấy ảnh: {ex.Message}");
            }
        }

        private async Task<List<string>> GetProductImagesPaths(string productId)
        {
            try
            {
                var folderPath = Path.Combine("Assets", "Images", productId);
                var imageFiles = Directory.GetFiles(folderPath);

                var imagePaths = new List<string>();
                foreach (var imagePath in imageFiles)
                {
                    var relativePath = Path.Combine(folderPath, Path.GetFileName(imagePath));
                    imagePaths.Add(relativePath);
                }

                return imagePaths;
            }
            catch (Exception ex)
            {
                throw new Exception($"Đã xảy ra lỗi khi lấy đường dẫn ảnh: {ex.Message}");
            }
        }

        [HttpDelete("DeleteProductImage/{productImageId}")]
        public async Task<IActionResult> DeleteProductImage(string productImageId)
        {
            var existingProductImage = await _context.ProductImages.FindAsync(productImageId);

            if (existingProductImage == null)
            {
                return NotFound("Product image not found.");
            }

            try
            {
                // Xóa ảnh từ thư mục
                var folderPath = Path.Combine("Assets", "Images", existingProductImage.ProductId);
                var imagePath = Path.Combine(folderPath, existingProductImage.ImagePath);

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                // Xóa ảnh khỏi cơ sở dữ liệu
                _context.ProductImages.Remove(existingProductImage);
                await _context.SaveChangesAsync();

                return Ok("Product image deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
