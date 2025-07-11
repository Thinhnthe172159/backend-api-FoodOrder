using FoodOrder.Dtos;
using FoodOrder.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace FoodOrder.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationController(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        // 🔹 Khách gửi thông báo đến tất cả nhân viên
        [HttpPost("customer/send")]
        [Authorize(Roles = "customer")]
        public async Task<IActionResult> CustomerSendToStaff([FromBody] NotificationDto dto)
        {
            var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customerName = User.FindFirstValue(ClaimTypes.Name);

            dto.SenderId = customerId;
            dto.SenderName = customerName;

            await _hubContext.Clients.Group("Staffs")
                .SendAsync("ReceiveNotification", dto);

            return Ok("Thông báo đã gửi đến nhân viên.");
        }

        // 🔹 Nhân viên gửi phản hồi đến 1 khách hàng cụ thể
        [HttpPost("staff/reply")]
        [Authorize(Roles = "staff")]
        public async Task<IActionResult> StaffReplyToCustomer([FromBody] NotificationDto dto)
        {
            if (string.IsNullOrEmpty(dto.TargetCustomerId))
                return BadRequest("Vui lòng cung cấp ID khách hàng cần gửi thông báo.");

            var staffId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var staffName = User.FindFirstValue(ClaimTypes.Name);

            dto.SenderId = staffId;
            dto.SenderName = staffName;

            await _hubContext.Clients.User(dto.TargetCustomerId)
                .SendAsync("ReceiveNotification", dto);

            return Ok($"Đã gửi thông báo đến khách hàng {dto.TargetCustomerId}.");
        }
    }


}
