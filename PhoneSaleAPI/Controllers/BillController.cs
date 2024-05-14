using System;
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

			var bills = await _context.Bills
				.OrderByDescending(b => b.DateBill)
				.ToListAsync();

			return bills;
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
			var bill = await _context.Bills.FindAsync(id);
			if (bill == null)
			{
				return NotFound();
			}

			bill.Status = BillStatus.DaHuy;

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
                TotalBill = 0 
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
                    Total = detail.Amount * detail.Price
                };

                _context.BillDetails.Add(billDetail);
                bill.TotalBill += billDetail.Total;

                var productDetail = await _context.ProductDetails
                    .FirstOrDefaultAsync(pd => pd.ProductId == detail.ProductID
                                            && pd.ColorName == detail.ColorName
                                            && pd.StorageGb == detail.StorageGB);

                if (productDetail != null)
                {                  
                    if (productDetail.Amount >= detail.Amount)
                    {
                        productDetail.Amount -= detail.Amount;
                        _context.ProductDetails.Update(productDetail);
                    }
                    else
                    {
                        return BadRequest($"Số lượng sản phẩm {productDetail.ProductId} - {productDetail.ColorName} - {productDetail.StorageGb} không đủ");
                    }
                }
                else
                {
                    return NotFound($"Không tìm thấy ProductDetail cho sản phẩm {detail.ProductID}, màu {detail.ColorName}, và dung lượng {detail.StorageGB}");
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Đặt mua thành công! Xin chờ phản hồi của người bán!", billId = bill.BillId });
        }

        private string GenerateBillId()
        {
            DateTime currentDate = DateTime.Now;

            string billId = $"BILL{currentDate.Day:00}{currentDate.Month:00}{currentDate.Year % 100:00}";

            var lastBill = _context.Bills
                .Where(b => b.BillId.StartsWith(billId))
                .OrderByDescending(b => b.BillId)
                .FirstOrDefault();

            if (lastBill == null)
            {
                return $"{billId}0001";
            }

            int newId = int.Parse(lastBill.BillId.Substring(10)) + 1;
            string newIdString = newId.ToString().PadLeft(4, '0');
            return $"{billId}{newIdString}";
        }

        [HttpGet("GetBillOfCustomer/{customerId}")]
        public async Task<ActionResult> GetBillOfCustomer(string customerId, BillStatus status)
        {
            try
            {
                var billInfo = await _context.Bills
                    .Where(b => b.CustomerId == customerId && b.Status == status)
                    .Select(b => new
                    {
                        b.BillId,
                        DateBill = DateTimeOffset.Parse(b.DateBill.ToString()).ToString("yyyy-MM-dd HH:mm"),
                        TotalProducts = _context.BillDetails.Where(d => d.BillId == b.BillId).Sum(d => d.Amount),
                        b.TotalBill,
                        status = b.Status
                    })
                    .ToListAsync();

                return Ok(billInfo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }

        [HttpGet("GetBillDetail/{billId}")]
        public async Task<ActionResult<BillSummaryDTO>> GetBillDetails(string billId)
        {
            var bill = await _context.Bills
                .Include(b => b.Customer)
                .Include(b => b.BillDetails)
                    .ThenInclude(bd => bd.Product)
                    .ThenInclude(p => p.ProductImages)
                .Include(b => b.BillDetails)
                    .ThenInclude(bd => bd.ColorNameNavigation)  
                .Include(b => b.BillDetails)
                    .ThenInclude(bd => bd.StorageGbNavigation)
                .FirstOrDefaultAsync(b => b.BillId == billId);

            if (bill == null)
            {
                return NotFound("Bill not found!");
            }

            var products = bill.BillDetails
                .Where(bd => bd.Product != null)
                .Select(bd => {
                    
                    var color = bd.ColorNameNavigation;
                    var storage = bd.StorageGbNavigation;
                    var product = bd.Product;

                    return new BillItemDTO
                    {
                        ProductID = product.ProductId,
                        ProductName = product.ProductName,
                        OriginalPrice = (int)(product.Price + (color?.ColorPrice ?? 0) + (storage?.StoragePrice ?? 0)),
                        DiscountedPrice = (int)bd.Price,
                        ColorName = color?.ColorName,
                        StorageGB = (int)(storage?.StorageGb),
                        Amount = (int)bd.Amount,
                        Img = bd.Product.ProductImages
                                    .Where(pi => pi.ColorName == bd.ColorName)
                                    .Select(pi => pi.ImagePath)
                                    .FirstOrDefault() ?? "default.jpg"
                    };
                }).ToList();

            var billSummary = new BillSummaryDTO
            {
                BillId = bill.BillId,
                CustomerName = bill.Customer.CustomerName,
                CustomerPhone = bill.Customer.PhoneNumber,
                DeliveryAddress = bill.DeliveryAddress,
                Note = bill.Note,
                lstProductBill = products
            };
            return Ok(billSummary);
        }
        [HttpGet("GetBillByID")]
        public async Task<ActionResult> GetBillByID(string customerId, string query)
        {
            try
            {
                var bills = await _context.Bills
                    .Where(b => b.CustomerId == customerId)
                    .SelectMany(b => b.BillDetails)
                    .Where(bd => bd.Bill.BillId.Contains(query) || bd.Product.ProductName.Contains(query))
                    .Select(bd => new
                    {
                        bd.Bill.BillId,
                        DateBill = DateTimeOffset.Parse(bd.Bill.DateBill.ToString()).ToString("yyyy-MM-dd HH:mm"),
                        TotalProducts = _context.BillDetails.Count(d => d.BillId == bd.Bill.BillId),
                        bd.Bill.TotalBill,
                        bd.Bill.Status
                    })
                    .Distinct()
                    .ToListAsync();

                if (!bills.Any())
                {
                    return NotFound("");
                }

                return Ok(bills);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

		[HttpGet("CalculateTotalBill/{billId}")]
		public async Task<ActionResult> CalculateTotalBill(string billId)
		{
			try
			{
				var totalBill = await _context.BillDetails
					.Where(bd => bd.BillId == billId)
					.SumAsync(bd => bd.Total);

				var bill = await _context.Bills.FindAsync(billId);
				if (bill == null)
				{
					return NotFound($"Không tìm thấy hóa đơn với ID: {billId}");
				}

				bill.TotalBill = totalBill;
				await _context.SaveChangesAsync();

				return Ok(new { success = true, totalBill });
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Lỗi: {ex.Message}");
			}
		}

		[HttpGet("SearchCustomerName/{searchString}")]
		public async Task<ActionResult<IEnumerable<Bill>>> SearchCustomerName(string searchString)
		{
			try
			{
				searchString = searchString.ToLower();

				var bills = await _context.Bills
					.Where(b => b.CustomerName.ToLower().Contains(searchString))
					.ToListAsync();

				if (bills.Count == 0)
				{
					// Nếu không tìm thấy kết quả, trả về 404 Not Found
					return NotFound("Không tìm thấy khách hàng phù hợp.");
				}

				return Ok(bills);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Lỗi: {ex.Message}");
			}
		}

	}
}
