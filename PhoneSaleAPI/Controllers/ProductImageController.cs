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
    public class ProductImageController : ControllerBase
    {
        private readonly PhoneManagementContext _context;

        public ProductImageController(PhoneManagementContext context)
        {
            _context = context;
        }
        [HttpPost("{productId}/images")]
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



    }
}
