namespace FoodOrder.Extentions
{
    public class TimeZoneChange
    {
        public static DateTime ConvertToTimeZone(DateTime dateTime, string timeZoneId)
        {
            if (dateTime.Kind == DateTimeKind.Unspecified)
            {
                dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
            }
            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return TimeZoneInfo.ConvertTime(dateTime, timeZone);
        }

    }
}
