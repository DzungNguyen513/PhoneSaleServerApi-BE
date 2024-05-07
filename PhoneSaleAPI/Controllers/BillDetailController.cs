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

		// GET: api/BillDetail
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


		// PUT: api/BillDetail/{billId}/{productId}/{colorName}/{storageGb}
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPut("{billId}/{productId}/{colorName}/{storageGb}")]
		public async Task<IActionResult> PutBillDetail(string billId, string productId, int storageGb, string colorName, BillDetail billDetail)
		{
			if (billId != billDetail.BillId || productId != billDetail.ProductId || storageGb != billDetail.StorageGb || colorName != billDetail.ColorName)
			{
				return BadRequest();
			}

			var existingBillDetail = await _context.BillDetails.FirstOrDefaultAsync(b => b.BillId == billId && b.ProductId == productId && b.StorageGb == storageGb && b.ColorName == colorName);

			if (existingBillDetail == null)
			{
				return NotFound();
			}

			// Cập nhật các trường của BillDetail
			existingBillDetail.Amount = billDetail.Amount;
			existingBillDetail.Price = billDetail.Price;
			existingBillDetail.Discount = billDetail.Discount;
			existingBillDetail.Total = billDetail.Total;
			existingBillDetail.UpdateAt = billDetail.UpdateAt;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!BillDetailExists(billId))
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

		[HttpPost("CreateBillDetail/{billId}/{productId}/{colorName}/{storageGb}")]
		public async Task<IActionResult> CreateBillDetail(string billId, string productId, string colorName, int storageGb, BillDetail billDetail)
		{
			if (billDetail != null)
			{
				try
				{
					// Kiểm tra xem sản phẩm đã tồn tại trong hóa đơn chưa
					var existingBillDetail = await _context.BillDetails.FirstOrDefaultAsync(b => b.BillId == billId && b.ProductId == productId && b.ColorName == colorName && b.StorageGb == storageGb);
					if (existingBillDetail != null)
					{
						return Conflict("This product has already been added to the bill.");
					}

					// Kiểm tra tính hợp lệ của billId
					var existingBill = await _context.Bills.FindAsync(billId);
					if (existingBill == null)
					{
						return BadRequest($"Bill with ID {billId} does not exist.");
					}

					// Tạo mới BillDetail
					var newBillDetail = new BillDetail
					{
						BillId = billId,
						ProductId = productId,
						ColorName = colorName,
						StorageGb = storageGb,
						Amount = billDetail.Amount ?? 0, // Kiểm tra và thêm giá trị mặc định nếu Amount là null
						Price = billDetail.Price ?? 0, // Kiểm tra và thêm giá trị mặc định nếu Price là null
						Discount = billDetail.Discount ?? 0, // Kiểm tra và thêm giá trị mặc định nếu Discount là null
						Total = billDetail.Total ?? 0, // Kiểm tra và thêm giá trị mặc định nếu Total là null
						CreateAt = billDetail.CreateAt ?? DateTime.UtcNow,
						UpdateAt = billDetail.UpdateAt ?? DateTime.UtcNow
					};

					_context.BillDetails.Add(newBillDetail);
					await _context.SaveChangesAsync();

					return Ok(new { newBillDetail.BillId });
				}
				catch (Exception ex)
				{
					return StatusCode(500, $"Internal server error: {ex.Message}");
				}
			}
			else
			{
				return BadRequest("Invalid bill detail data.");
			}
		}


		// DELETE: api/BillDetail/{billId}/{productId}/{colorName}/{storageGb}
		[HttpDelete("{billId}/{productId}/{colorName}/{storageGb}")]
		public async Task<IActionResult> DeleteBillDetail(string billId, string productId, string colorName, int storageGb)
		{
			if (billId == null || productId == null || colorName == null || storageGb <= 0)
			{
				return BadRequest();
			}

			var billDetail = await _context.BillDetails.FirstOrDefaultAsync(b =>
				b.BillId == billId &&
				b.ProductId == productId &&
				b.ColorName == colorName &&
				b.StorageGb == storageGb);

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
