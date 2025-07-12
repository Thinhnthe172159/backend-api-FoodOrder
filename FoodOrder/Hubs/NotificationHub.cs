using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace FoodOrder.Hubs
{
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var role = Context.User?.FindFirst(ClaimTypes.Role)?.Value;

            if (role?.ToLower() == "staff")
                await Groups.AddToGroupAsync(Context.ConnectionId, "Staffs");
            else if (role?.ToLower() == "customer")
                await Groups.AddToGroupAsync(Context.ConnectionId, "Customers");

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var role = Context.User?.FindFirst(ClaimTypes.Role)?.Value;

            if (role?.ToLower() == "staff")
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Staffs");
            else if (role?.ToLower() == "customer")
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Customers");

            await base.OnDisconnectedAsync(exception);
        }

        // Khách gửi cho tất cả nhân viên
        public async Task SendToAllStaff(string title, string message)
        {
            var role = Context.User?.FindFirst(ClaimTypes.Role)?.Value;
            var senderId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var senderName = Context.User?.FindFirst(ClaimTypes.Name)?.Value;

            if (role?.ToLower() != "customer") return;

            await Clients.Group("Staffs").SendAsync("ReceiveNotification", title, message, senderId, senderName);
        }

        // Nhân viên phản hồi lại cho 1 khách
        public async Task ReplyToCustomer(string customerUserId, string title, string message)
        {
            var role = Context.User?.FindFirst(ClaimTypes.Role)?.Value;
            var senderId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var senderName = Context.User?.FindFirst(ClaimTypes.Name)?.Value;

            if (role?.ToLower() != "staff") return;

            await Clients.User(customerUserId).SendAsync("ReceiveNotification", title, message, senderId, senderName);
        }

        public async Task SendToAllStaffWithData(string title, string message, string orderId, string tableId)
        {
            var role = Context.User?.FindFirst(ClaimTypes.Role)?.Value;
            var senderId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var senderName = Context.User?.FindFirst(ClaimTypes.Name)?.Value;

            if (role?.ToLower() != "customer") return;

            await Clients.Group("Staffs").SendAsync("ReceiveNotificationWithData", title, message, senderId, senderName, orderId, tableId);
        }
    }
}
