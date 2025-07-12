namespace FoodOrder.Dtos
{
    public class NotificationWithDataDto : NotificationDto
    {
        public string OrderId { get; set; }
        public string TableId { get; set; }
    }
}
