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
    public class BillDetailController : ControllerBase
    {
        private readonly PhoneManagementContext _context;

        public BillDetailController(PhoneManagementContext context)
        {
            _context = context;
        }

        // GET: api/BillDetailKhanhDaSua
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BillDetail>>> GetBillDetails()
        {
          if (_context.BillDetails == null)
          {
              return NotFound();
          }
            return await _context.BillDetails.ToListAsync();
        }

		// GET: api/BillDetail/5
		[HttpGet("{id}")]
		public async Task<ActionResult<IEnumerable<BillDetail>>> GetBillDetail(string id)
		{
			if (_context.BillDetails == null)
			{
				return NotFound();
			}

			var billDetails = await _context.BillDetails.Where(b => b.BillId == id).ToListAsync();

			if (billDetails == null || billDetails.Count == 0)
			{
				return NotFound();
			}

			return billDetails;
		}


		// PUT: api/BillDetail/5
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPut("{id}")]
        public async Task<IActionResult> PutBillDetail(string id, BillDetail billDetail)
        {
            if (id != billDetail.BillId)
            {
                return BadRequest();
            }

            _context.Entry(billDetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BillDetailExists(id))
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

        // POST: api/BillDetail
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BillDetail>> PostBillDetail(BillDetail billDetail)
        {
          if (_context.BillDetails == null)
          {
              return Problem("Entity set 'PhoneManagementContext.BillDetails'  is null.");
          }
            _context.BillDetails.Add(billDetail);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (BillDetailExists(billDetail.BillId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetBillDetail", new { id = billDetail.BillId }, billDetail);
        }

        // DELETE: api/BillDetail/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBillDetail(string id)
        {
            if (_context.BillDetails == null)
            {
                return NotFound();
            }
            var billDetail = await _context.BillDetails.FindAsync(id);
            if (billDetail == null)
            {
                return NotFound();
            }

            _context.BillDetails.Remove(billDetail);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BillDetailExists(string id)
        {
            return (_context.BillDetails?.Any(e => e.BillId == id)).GetValueOrDefault();
        }
    }
}
