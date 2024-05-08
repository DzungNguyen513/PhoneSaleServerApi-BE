using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneSaleAPI.DTO.Categories;
using PhoneSaleAPI.Models;

namespace PhoneSaleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly PhoneManagementContext _context;

        public CategoriesController(PhoneManagementContext context)
        {
            _context = context;
        }

        // GET: api/Categories

        [HttpGet("GetCategories")]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
          if (_context.Categories == null)
          {
              return NotFound();
          }
            return await _context.Categories.ToListAsync();
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(string id)
        {
          if (_context.Categories == null)
          {
              return NotFound();
          }
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }

        // PUT: api/Categories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(string id, Category category)
        {
            if (id != category.CategoryId)
            {
                return BadRequest();
            }

            // Lấy thời gian Việt Nam
            TimeZoneInfo vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vnTimeZone);

            category.UpdateAt = vietnamTime; // Cập nhật thời gian sửa
            await _context.SaveChangesAsync();

            _context.Entry(category).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
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


        // POST: api/Categories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory(Category category)
        {
          if (_context.Categories == null)
          {
              return Problem("Entity set 'PhoneManagementContext.Categories'  is null.");
          }
            _context.Categories.Add(category);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CategoryExists(category.CategoryId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCategory", new { id = category.CategoryId }, category);
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(string id)
        {
            if (_context.Categories == null)
            {
                return NotFound();
            }
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            category.Status = 0;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("CreateCategory")]
        public async Task<IActionResult> AddCategory([FromForm] CreateCategoriesDTO categoryDTO)
        {
            if (categoryDTO.CategoryImage != null)
            {
                var folderName = Path.Combine("Assets", "CategoriesImg");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                if (!Directory.Exists(pathToSave))
                {
                    Directory.CreateDirectory(pathToSave);
                }

                var fileName = Path.GetFileNameWithoutExtension(categoryDTO.CategoryImage.FileName);
                var extension = Path.GetExtension(categoryDTO.CategoryImage.FileName);
                fileName = $"{fileName}{extension}";
                var fullPath = Path.Combine(pathToSave, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await categoryDTO.CategoryImage.CopyToAsync(stream);
                }

                var lastCategory = await _context.Categories
                                    .OrderByDescending(c => c.CategoryId)
                                    .FirstOrDefaultAsync();

                int lastNumber = 0;
                if (lastCategory != null)
                {
                    var lastId = lastCategory.CategoryId.Replace("CT", "");
                    lastNumber = int.Parse(lastId);
                }

                var newCategoryId = $"CT{lastNumber + 1:000}";

                var newCategory = new Category
                {
                    CategoryId = newCategoryId,
                    CategoryName = categoryDTO.CategoryName,
                    CategoryImage = fileName,
                    Status = categoryDTO.Status
                };

                _context.Categories.Add(newCategory);
                await _context.SaveChangesAsync();

                return Ok(new { fileName });
            }
            else
            {
                return BadRequest("Không tìm thấy hình ảnh danh mục để tải lên.");
            }
        }

        [HttpGet("GetCategoryImage/{categoryId}")]
        public async Task<IActionResult> GetCategoryImage(string categoryId)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == categoryId);
            if (category == null)
            {
                return NotFound(); // Trả về mã trạng thái 404 nếu không tìm thấy màu
            }

            var folderName = Path.Combine("Assets", "CategoriesImg");
            var pathToLoad = Path.Combine(Directory.GetCurrentDirectory(), folderName, category.CategoryImage);
            var imageBytes = await System.IO.File.ReadAllBytesAsync(pathToLoad);

            return File(imageBytes, "image/jpeg"); // Trả về file ảnh với kiểu MIME là "image/jpeg"
        }

        [HttpPut("UpdateCategory/{categoryId}")]
        public async Task<IActionResult> UpdateCategory(string categoryId, [FromForm] CreateCategoriesDTO categoryDTO)
        {
            // Tìm kiếm danh mục để cập nhật
            var existingCategory = await _context.Categories.FindAsync(categoryId);

            if (existingCategory == null)
            {
                return NotFound("Category not found.");
            }

            // Kiểm tra xem có tệp ảnh mới được cung cấp không
            if (categoryDTO.CategoryImage != null)
            {
                // Xóa ảnh cũ nếu tồn tại
                if (!string.IsNullOrEmpty(existingCategory.CategoryImage))
                {
                    var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "CategoriesImg", existingCategory.CategoryImage);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // Lưu ảnh mới vào thư mục
                var folderName = Path.Combine("Assets", "CategoriesImg");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                if (!Directory.Exists(pathToSave))
                {
                    Directory.CreateDirectory(pathToSave);
                }

                var fileName = Path.GetFileNameWithoutExtension(categoryDTO.CategoryImage.FileName);
                var extension = Path.GetExtension(categoryDTO.CategoryImage.FileName);
                fileName = $"{fileName}{extension}";
                var fullPath = Path.Combine(pathToSave, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await categoryDTO.CategoryImage.CopyToAsync(stream);
                }

                // Cập nhật thông tin danh mục
                existingCategory.CategoryImage = fileName;
            }

            // Cập nhật các thông tin khác của danh mục
            existingCategory.CategoryName = categoryDTO.CategoryName;
            existingCategory.Status = categoryDTO.Status;
            TimeZoneInfo vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vnTimeZone);
            existingCategory.UpdateAt = vietnamTime;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(existingCategory);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }




        private bool CategoryExists(string id)
        {
            return (_context.Categories?.Any(e => e.CategoryId == id)).GetValueOrDefault();
        }
    }
}
