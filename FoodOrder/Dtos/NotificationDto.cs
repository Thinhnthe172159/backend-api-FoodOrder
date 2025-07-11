namespace FoodOrder.Dtos
{
    public class NotificationDto
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string SenderId { get; set; }
        public string SenderName { get; set; }
        public string? TargetCustomerId { get; set; }
    }
}
