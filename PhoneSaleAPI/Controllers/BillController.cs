﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneSaleAPI.DTO.Bill;
using PhoneSaleAPI.Models;

namespace PhoneSaleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillController : ControllerBase
    {
        private readonly PhoneManagementContext _context;

        public BillController(PhoneManagementContext context)
        {
            _context = context;
        }

        // GET: api/Bill
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bill>>> GetBills()
        {
          if (_context.Bills == null)
          {
              return NotFound();
          }
            return await _context.Bills.ToListAsync();
        }

        // GET: api/Bill/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Bill>> GetBill(string id)
        {
          if (_context.Bills == null)
          {
              return NotFound();
          }
            var bill = await _context.Bills.FindAsync(id);

            if (bill == null)
            {
                return NotFound();
            }

            return bill;
        }

        // PUT: api/Bill/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBill(string id, Bill bill)
        {
            if (id != bill.BillId)
            {
                return BadRequest();
            }

            _context.Entry(bill).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BillExists(id))
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

        // POST: api/Bill
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Bill>> PostBill(Bill bill)
        {
          if (_context.Bills == null)
          {
              return Problem("Entity set 'PhoneManagementContext.Bills'  is null.");
          }
            _context.Bills.Add(bill);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (BillExists(bill.BillId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetBill", new { id = bill.BillId }, bill);
        }

        // DELETE: api/Bill/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBill(string id)
        {
            if (_context.Bills == null)
            {
                return NotFound();
            }
            var bill = await _context.Bills.FindAsync(id);
            if (bill == null)
            {
                return NotFound();
            }

            _context.Bills.Remove(bill);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BillExists(string id)
        {
            return (_context.Bills?.Any(e => e.BillId == id)).GetValueOrDefault();
        }
        [HttpPost("CreateBill")]
        public async Task<IActionResult> CreateBill([FromBody] BillCreateDTO billDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newBillId = GenerateBillId();

            var bill = new Bill
            {
                BillId = newBillId,
                CustomerId = billDTO.CustomerId,
                CustomerName = billDTO.CustomerName,
                DeliveryAddress = billDTO.DeliveryAddress,
                CustomerPhone = billDTO.CustomerPhone,
                Note = billDTO.Note,
                Status = 0,
                TotalBill = 0 // Tính sau khi tính chi tiết
            };

            _context.Bills.Add(bill);

            foreach (var detail in billDTO.BillDetails)
            {
                var product = await _context.Products.FindAsync(detail.ProductID);
                if (product == null)
                {
                    return NotFound($"Không tìm thấy sản phẩm với ID: {detail.ProductID}");
                }

                var discount = product.Discount; 

                var billDetail = new BillDetail
                {
                    BillId = bill.BillId,
                    ProductId = detail.ProductID,
                    ColorName = detail.ColorName,
                    StorageGb = detail.StorageGB,
                    Amount = detail.Amount,
                    Price = detail.Price,
                    Discount = discount, 
                    Total = detail.Amount * (detail.Price - (detail.Price * discount / 100))
                };

                _context.BillDetails.Add(billDetail);
                bill.TotalBill += billDetail.Total;
            }

            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Đặt mua thành công! Xin chờ phản hồi của người bán!", billId = bill.BillId });
        }

        private string GenerateBillId()
        {
            var lastBill = _context.Bills.OrderByDescending(b => b.BillId).FirstOrDefault();

            if (lastBill == null)
            {
                return "Bill001";
            }

            string lastId = lastBill.BillId.Substring(4); 
            int newId = int.Parse(lastId) + 1;
            return $"Bill{newId.ToString().PadLeft(3, '0')}";
        }

    }
}
