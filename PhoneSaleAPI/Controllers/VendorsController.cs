using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneSaleAPI.DTO.Vendor;
using PhoneSaleAPI.Models;

namespace PhoneSaleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendorsController : ControllerBase
    {
        private readonly PhoneManagementContext _context;

        public VendorsController(PhoneManagementContext context)
        {
            _context = context;
        }

        // GET: api/Vendors
        [HttpGet("GetVendors")]
        public async Task<ActionResult<IEnumerable<Vendor>>> GetVendors()
        {
            if (_context.Vendors == null)
            {
                return NotFound();
            }
            return await _context.Vendors.ToListAsync();
        }

        // GET: api/Vendors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Vendor>> GetVendor(string id)
        {
            if (_context.Vendors == null)
            {
                return NotFound();
            }
            var vendor = await _context.Vendors.FindAsync(id);

            if (vendor == null)
            {
                return NotFound();
            }

            return vendor;
        }

        // PUT: api/Vendors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVendor(string id, Vendor vendor)
        {
            if (id != vendor.VendorId)
            {
                return BadRequest();
            }
            TimeZoneInfo vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vnTimeZone);
            vendor.UpdateAt = vietnamTime;
            _context.Entry(vendor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VendorExists(id))
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

        // POST: api/Vendors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Vendor>> PostVendor(Vendor vendor)
        {
            if (_context.Vendors == null)
            {
                return Problem("Entity set 'PhoneManagementContext.Vendors'  is null.");
            }
            _context.Vendors.Add(vendor);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (VendorExists(vendor.VendorId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetVendor", new { id = vendor.VendorId }, vendor);
        }

        // DELETE: api/Vendors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVendor(string id)
        {
            if (_context.Vendors == null)
            {
                return NotFound();
            }
            var vendor = await _context.Vendors.FindAsync(id);
            if (vendor == null)
            {
                return NotFound();
            }
            vendor.Status = 0;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("AdminCreateVendor")]
        public async Task<ActionResult<Vendor>> AdminCreateVendor(CreateVendor vendor)
        {
            if (_context.Vendors == null)
            {
                return Problem("Entity set 'PhoneManagementContext.Vendors' is null.");
            }

            // Auto-generate VendorID
            var newVendorId = GenerateVendorId(); // Assume this method generates a unique VendorID

            var newVendor = new Vendor
            {
                VendorId = newVendorId,
                VendorName = vendor.VendorName,
                Address = vendor.Address,
                PhoneNumber = vendor.PhoneNumber,
                Status = vendor.Status
            };

            _context.Vendors.Add(newVendor);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (VendorExists(newVendorId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetVendor", new { id = newVendorId }, vendor);
        }


        // Method to generate a unique VendorID
        private string GenerateVendorId()
        {
            // Find the maximum existing VendorID
            var maxVendorId = _context.Vendors
                                    .Select(v => v.VendorId)
                                    .Where(id => id.StartsWith("VD"))
                                    .ToList() // Bring data to client-side
                                    .Select(id => int.Parse(id.Substring(2))) // Extract the numeric part
                                    .DefaultIfEmpty(0) // If no existing IDs, default to 0
                                    .Max();

            // Increment and format the new VendorID
            var newIdNumber = maxVendorId + 1;
            var newVendorId = $"VD{newIdNumber.ToString("D3")}"; // D3 ensures it's zero-padded to 3 digits

            return newVendorId;
        }

        // Method to check if a vendor with the given VendorID exists
        private bool VendorExists(string vendorId)
        {
            return _context.Vendors.Any(v => v.VendorId == vendorId);
        }

    }
}
