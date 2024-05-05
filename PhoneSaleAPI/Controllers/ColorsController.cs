using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneSaleAPI.DTO.Categories;
using PhoneSaleAPI.DTO.Color;
using PhoneSaleAPI.Models;

namespace PhoneSaleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ColorsController : ControllerBase
    {
        private readonly PhoneManagementContext _context;

        public ColorsController(PhoneManagementContext context)
        {
            _context = context;
        }

        // GET: api/Colors
        [HttpGet("GetColors")]
        public async Task<ActionResult<IEnumerable<Color>>> GetColors()
        {
          if (_context.Colors == null)
          {
              return NotFound();
          }
            return await _context.Colors.ToListAsync();
        }

        // GET: api/Colors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Color>> GetColor(string id)
        {
          if (_context.Colors == null)
          {
              return NotFound();
          }
            var color = await _context.Colors.FindAsync(id);

            if (color == null)
            {
                return NotFound();
            }

            return color;
        }

        // PUT: api/Colors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutColor(string id, Color color)
        {
            if (id != color.ColorName)
            {
                return BadRequest();
            }

            _context.Entry(color).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ColorExists(id))
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

        // POST: api/Colors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Color>> PostColor(Color color)
        {
          if (_context.Colors == null)
          {
              return Problem("Entity set 'PhoneManagementContext.Colors'  is null.");
          }
            _context.Colors.Add(color);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ColorExists(color.ColorName))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetColor", new { id = color.ColorName }, color);
        }

        // DELETE: api/Colors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteColor(string id)
        {
            if (_context.Colors == null)
            {
                return NotFound();
            }
            var color = await _context.Colors.FindAsync(id);
            if (color == null)
            {
                return NotFound();
            }

            _context.Colors.Remove(color);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("CreateColorDTO")]
        public async Task<IActionResult> AddColor([FromForm] CreateColorDTO colorDTO)
        {
            if (colorDTO.ColorImage != null)
            {
                var folderName = Path.Combine("Assets", "ColorImg");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                if (!Directory.Exists(pathToSave))
                {
                    Directory.CreateDirectory(pathToSave);
                }

                var fileName = Path.GetFileNameWithoutExtension(colorDTO.ColorImage.FileName);
                var extension = Path.GetExtension(colorDTO.ColorImage.FileName);
                fileName = $"{fileName}{extension}";
                var fullPath = Path.Combine(pathToSave, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await colorDTO.ColorImage.CopyToAsync(stream);
                }

                var dbPath = Path.Combine(folderName, fileName);

                // Kiểm tra xem màu có tồn tại trong cơ sở dữ liệu không
                var existingColor = await _context.Colors.FirstOrDefaultAsync(c => c.ColorName == colorDTO.ColorName);
                if (existingColor != null)
                {
                    return Conflict("Màu đã tồn tại trong cơ sở dữ liệu.");
                }

                var lastColor = await _context.Colors
                                    .OrderByDescending(c => c.ColorName)
                                    .FirstOrDefaultAsync();

                var newColor = new Color
                {
                    ColorName = colorDTO.ColorName,
                    ColorImage = fileName,
                    ColorPrice = colorDTO.ColorPrice
                };

                _context.Colors.Add(newColor);
                await _context.SaveChangesAsync();

                return Ok(new { fileName });
            }
            else
            {
                return BadRequest("Không tìm thấy hình ảnh màu để tải lên.");
            }
        }

        [HttpGet("GetColorImage/{colorName}")]
        public async Task<IActionResult> GetColorImage(string colorName)
        {
            var color = await _context.Colors.FirstOrDefaultAsync(c => c.ColorName == colorName);
            if (color == null)
            {
                return NotFound(); // Trả về mã trạng thái 404 nếu không tìm thấy màu
            }

            var folderName = Path.Combine("Assets", "ColorImg");
            var pathToLoad = Path.Combine(Directory.GetCurrentDirectory(), folderName, color.ColorImage);
            var imageBytes = await System.IO.File.ReadAllBytesAsync(pathToLoad);

            return File(imageBytes, "image/jpeg"); // Trả về file ảnh với kiểu MIME là "image/jpeg"
        }

        [HttpPut("UpdateColor/{colorName}")]
        public async Task<IActionResult> UpdateColor(string colorName, [FromForm] CreateColorDTO colorDTO)
        {
            // Tìm kiếm danh mục để cập nhật
            var existingColor = await _context.Colors.FindAsync(colorName);

            if (existingColor == null)
            {
                return NotFound("Category not found.");
            }

            // Kiểm tra xem có tệp ảnh mới được cung cấp không
            if (colorDTO.ColorImage != null)
            {
                // Xóa ảnh cũ nếu tồn tại
                if (!string.IsNullOrEmpty(existingColor.ColorImage))
                {
                    var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "ColorImg", existingColor.ColorImage);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // Lưu ảnh mới vào thư mục
                var folderName = Path.Combine("Assets", "ColorImg");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                if (!Directory.Exists(pathToSave))
                {
                    Directory.CreateDirectory(pathToSave);
                }

                var fileName = Path.GetFileNameWithoutExtension(colorDTO.ColorImage.FileName);
                var extension = Path.GetExtension(colorDTO.ColorImage.FileName);
                fileName = $"{fileName}{extension}";
                var fullPath = Path.Combine(pathToSave, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await colorDTO.ColorImage.CopyToAsync(stream);
                }

                // Cập nhật thông tin danh mục
                existingColor.ColorImage = fileName;
            }

            // Cập nhật các thông tin khác của danh mục
            existingColor.ColorName = colorDTO.ColorName;
            existingColor.ColorPrice = colorDTO.ColorPrice;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(existingColor);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        private bool ColorExists(string id)
        {
            return (_context.Colors?.Any(e => e.ColorName == id)).GetValueOrDefault();
        }
    }
}
