using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneSaleAPI.DTO.SystemNotification;
using PhoneSaleAPI.Models;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using FirebaseAdmin.Messaging;
using PhoneSaleAPI.Firebase;

namespace PhoneSaleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemNotificationController : ControllerBase
    {
        private readonly PhoneManagementContext _context;

        public SystemNotificationController(PhoneManagementContext context)
        {
            _context = context;
        }

        // GET: api/SystemNotification
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SystemNotification>>> GetSystemNotifications()
        {
            if (_context.SystemNotifications == null)
            {
                return NotFound();
            }
            return await _context.SystemNotifications.ToListAsync();
        }

        // GET: api/SystemNotification/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SystemNotification>> GetSystemNotification(string id)
        {
            if (_context.SystemNotifications == null)
            {
                return NotFound();
            }
            var systemNotification = await _context.SystemNotifications.FindAsync(id);

            if (systemNotification == null)
            {
                return NotFound();
            }

            return systemNotification;
        }

        // PUT: api/SystemNotification/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSystemNotification(string id, SystemNotification systemNotification)
        {
            if (id != systemNotification.NotificationId)
            {
                return BadRequest();
            }

            _context.Entry(systemNotification).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SystemNotificationExists(id))
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

        // POST: api/SystemNotification
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SystemNotification>> PostSystemNotification(SystemNotification systemNotification)
        {
            if (_context.SystemNotifications == null)
            {
                return Problem("Entity set 'PhoneManagementContext.SystemNotifications'  is null.");
            }
            _context.SystemNotifications.Add(systemNotification);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (SystemNotificationExists(systemNotification.NotificationId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetSystemNotification", new { id = systemNotification.NotificationId }, systemNotification);
        }

        // DELETE: api/SystemNotification/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSystemNotification(string id)
        {
            if (_context.SystemNotifications == null)
            {
                return NotFound();
            }
            var systemNotification = await _context.SystemNotifications.FindAsync(id);
            if (systemNotification == null)
            {
                return NotFound();
            }

            _context.SystemNotifications.Remove(systemNotification);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SystemNotificationExists(string id)
        {
            return (_context.SystemNotifications?.Any(e => e.NotificationId == id)).GetValueOrDefault();
        }

        [HttpPost("CreateNotification")]
        public async Task<ActionResult<SystemNotificationResponseDTO>> CreateNotification(SystemNotificationDTO notificationDTO)
        {
            try
            {
                var notification = new SystemNotification
                {
                    NotificationId = "Noti" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                    NotificationName = notificationDTO.NotificationName,
                    Title = notificationDTO.Title,
                    Description = notificationDTO.Description,
                    Message = notificationDTO.Message,
                    NotificationType = notificationDTO.NotificationType,
                    CustomerId = notificationDTO.CustomerId,
                    IsActive = notificationDTO.IsActive,
                    CreatedAt = DateTime.Now
                };

                _context.SystemNotifications.Add(notification);
                await _context.SaveChangesAsync();

                if (string.IsNullOrEmpty(notificationDTO.CustomerId))
                {
                    await FirebaseManager.SendNotificationToTopic("com.example.phonesaleapp", notificationDTO.Title, notificationDTO.Description);
                }
                else
                {
                    var customer = await _context.Customers
                                                 .Where(c => c.CustomerId == notificationDTO.CustomerId)
                                                 .FirstOrDefaultAsync();
                    if (customer != null && !string.IsNullOrEmpty(customer.NotificationToken))
                    {
                        await FirebaseManager.SendNotificationToToken(customer.NotificationToken, notificationDTO.Title, notificationDTO.Description);
                    }
                }

                var responseDTO = new SystemNotificationResponseDTO
                {
                    NotificationID = notification.NotificationId,
                    Title = notification.Title,
                    Message = notification.Message,
                    NotificationType = notification.NotificationType,
                    IsActive = (bool)notification.IsActive,
                    CreatedAt = DateTimeOffset.Parse(notification.CreatedAt.ToString()).ToString("dd/MM/yyyy HH:mm"),
                };
                return CreatedAtAction(nameof(GetNotification), new { id = notification.NotificationId }, responseDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }


        [HttpGet("GetNotification")]
        public async Task<ActionResult<IEnumerable<SystemNotificationResponseDTO>>> GetNotification(string? customerId = null)
        {
            try
            {
                IQueryable<SystemNotification> query = _context.SystemNotifications;

                if (string.IsNullOrEmpty(customerId))
                {
                    query = query.Where(n => n.CustomerId == null);
                }
                else
                {
                    query = query.Where(n => n.CustomerId == customerId || n.CustomerId == null);
                }

                var notifications = await query
                    .Select(n => new SystemNotificationResponseDTO
                    {
                        NotificationID = n.NotificationId,
                        NotificationName = n.NotificationName,
                        Title = n.Title,
                        Description = n.Description,
                        Message = n.Message,
                        NotificationType = n.NotificationType,
                        IsActive = (bool)n.IsActive,           
                        CreatedAt = DateTimeOffset.Parse(n.CreatedAt.ToString()).ToString("dd/MM/yyyy HH:mm"),
                    })
                    .ToListAsync();

                return Ok(notifications);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }
    }
}
