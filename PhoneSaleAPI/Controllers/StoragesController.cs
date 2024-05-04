using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneSaleAPI.DTO.Storage;
using PhoneSaleAPI.Models;

namespace PhoneSaleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoragesController : ControllerBase
    {
        private readonly PhoneManagementContext _context;

        public StoragesController(PhoneManagementContext context)
        {
            _context = context;
        }

        // GET: api/Storages
        [HttpGet("GetStorages")]
        public async Task<ActionResult<IEnumerable<Storage>>> GetStorages()
        {
          if (_context.Storages == null)
          {
              return NotFound();
          }
            return await _context.Storages.ToListAsync();
        }

        // GET: api/Storages/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Storage>> GetStorage(int id)
        {
          if (_context.Storages == null)
          {
              return NotFound();
          }
            var storage = await _context.Storages.FindAsync(id);

            if (storage == null)
            {
                return NotFound();
            }

            return storage;
        }

        // PUT: api/Storages/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStorage(int id, Storage storage)
        {
            if (id != storage.StorageGb)
            {
                return BadRequest();
            }

            _context.Entry(storage).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StorageExists(id))
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

        // POST: api/Storages
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Storage>> PostStorage(Storage storage)
        {
            // Kiểm tra xem model có hợp lệ không
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Thêm lưu trữ vào cơ sở dữ liệu và lưu thay đổi
            _context.Storages.Add(storage);
            await _context.SaveChangesAsync();

            // Trả về mã phản hồi 201 Created cùng với dữ liệu lưu trữ mới đã được tạo
            return CreatedAtAction(nameof(GetStorage), new { id = storage.StorageGb }, storage);
        }

        // DELETE: api/Storages/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStorage(int id)
        {
            if (_context.Storages == null)
            {
                return NotFound();
            }
            var storage = await _context.Storages.FindAsync(id);
            if (storage == null)
            {
                return NotFound();
            }

            _context.Storages.Remove(storage);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("AdminCreateStorage")]
        public async Task<ActionResult<Storage>> AdminCreateStorage(CreateStorageDTO storageDTO)
        {
            // Validate input
            if (storageDTO == null)
            {
                return BadRequest("Storage data is null.");
            }

            if (storageDTO.StorageGb <= 0)
            {
                return BadRequest("Storage capacity must be greater than 0.");
            }

            if (storageDTO.StoragePrice <= 0)
            {
                return BadRequest("Storage price must be greater than 0.");
            }

            // Create a new Storage entity
            var storage = new Storage
            {
                StorageGb = storageDTO.StorageGb,
                StoragePrice = storageDTO.StoragePrice
            };

            // Add to database and save changes
            _context.Storages.Add(storage);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // Handle unique constraint violation or other database-related errors
                return Conflict();
            }

            // Return the newly created Storage entity
            return CreatedAtAction("GetStorage", new { id = storage.StorageGb }, storage);
        }


        private bool StorageExists(int id)
        {
            return (_context.Storages?.Any(e => e.StorageGb == id)).GetValueOrDefault();
        }
    }
}
