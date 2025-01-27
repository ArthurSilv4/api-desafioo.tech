namespace api_desafioo.tech.Helpers
{
    public class DateTimeHelper
    {
        public static DateTime GetBrasiliaTime()
        {
            var brasiliaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brasiliaTimeZone);
        }
    }
}
