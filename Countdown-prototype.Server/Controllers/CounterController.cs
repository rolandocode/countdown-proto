using Microsoft.AspNetCore.Mvc;

namespace Countdown_prototype.Server.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class CounterController : ControllerBase
    {

        public CounterController() { }

        [HttpGet]
        public object Get()
        {
            var mexicoTimeZone = TimeZoneInfo.FindSystemTimeZoneById(
                OperatingSystem.IsWindows()
                    ? "Central Standard Time"
                    : "America/Monterrey"
            );

            var startDate = new DateTime(
                2026, 8, 10, 18, 0, 0,
                DateTimeKind.Unspecified
            );

            var endDate = startDate.AddMonths(9);

            var now = TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow,
                mexicoTimeZone
            ).AddHours(-1);

            var totalDuration = endDate - startDate;
            var elapsed = now - startDate;

            var percentage =
                elapsed.TotalMilliseconds /
                totalDuration.TotalMilliseconds * 100;

            percentage = Math.Clamp(percentage, 0, 100);

            return new
            {
                startDate = startDate.ToString("yyyy-MM-dd HH:mm:ss"),
                endDate = endDate.ToString("yyyy-MM-dd HH:mm:ss"),
                currentDate = now.ToString("yyyy-MM-dd HH:mm:ss"),
                percentage = percentage
            };
        }

        [HttpGet]
        [Route("day")]
        public object GetDay()
        {
            var mexicoTimeZone = TimeZoneInfo.FindSystemTimeZoneById(
                OperatingSystem.IsWindows()
                    ? "Central Standard Time"
                    : "America/Monterrey"
            );

            var now = TimeZoneInfo.ConvertTimeFromUtc(
               DateTime.UtcNow,
               mexicoTimeZone
           ).AddHours(-1);


            var startDate = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);

            var endDate = startDate.AddDays(1);

           
            var totalDuration = endDate - startDate;
            var elapsed = now - startDate;

            var percentage =
                elapsed.TotalMilliseconds /
                totalDuration.TotalMilliseconds * 100;

            percentage = Math.Clamp(percentage, 0, 100);

            return new
            {
                startDate = startDate.ToString("yyyy-MM-dd HH:mm:ss"),
                endDate = endDate.ToString("yyyy-MM-dd HH:mm:ss"),
                currentDate = now.ToString("yyyy-MM-dd HH:mm:ss"),
                percentage = percentage
            };
        }

        [HttpGet]
        [Route("work")]
        public object GetWorkDay()
        {
            var mexicoTimeZone = TimeZoneInfo.FindSystemTimeZoneById(
                OperatingSystem.IsWindows()
                    ? "Central Standard Time"
                    : "America/Monterrey"
            );

            var now = TimeZoneInfo.ConvertTimeFromUtc(
               DateTime.UtcNow,
               mexicoTimeZone
           ).AddHours(-1);


            var startDate = new DateTime(now.Year, now.Month, now.Day, 7, 0, 0);

            var endDate = startDate.AddHours(10);


            var totalDuration = endDate - startDate;
            var elapsed = now - startDate;

            var percentage =
                elapsed.TotalMilliseconds /
                totalDuration.TotalMilliseconds * 100;

            percentage = Math.Clamp(percentage, 0, 100);

            if (now < startDate || now > endDate && (now.DayOfWeek == DayOfWeek.Saturday || now.DayOfWeek == DayOfWeek.Sunday))
            {
                percentage = 0;
            }

            return new
            {
                startDate = startDate.ToString("yyyy-MM-dd HH:mm:ss"),
                endDate = endDate.ToString("yyyy-MM-dd HH:mm:ss"),
                currentDate = now.ToString("yyyy-MM-dd HH:mm:ss"),
                percentage = percentage
            };
        }
    }
}
