using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        private bool ColorExists(string id)
        {
            return (_context.Colors?.Any(e => e.ColorName == id)).GetValueOrDefault();
        }
    }
}
