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

        [HttpGet]
        [Route("weekend")]
        public object GetWeekend()
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

            //TEST FOR SATURDAY
            // var now = TimeZoneInfo.ConvertTimeFromUtc(
            //    new DateTime(2026, 8, 15, 12, 0, 0, DateTimeKind.Utc),
            //    mexicoTimeZone
            //).AddHours(-1);

            //TEST FOR TUESDAY
            // var now = TimeZoneInfo.ConvertTimeFromUtc(
            //    new DateTime(2026, 8, 17, 12, 0, 0, DateTimeKind.Utc),
            //    mexicoTimeZone
            //).AddHours(-1);

            //TEST FOR FRIDAY
            // var now = TimeZoneInfo.ConvertTimeFromUtc(
            //    new DateTime(2026, 8, 14, 12, 0, 0, DateTimeKind.Utc),
            //    mexicoTimeZone
            //).AddHours(-1);


            var currentDayOfWeek = now.DayOfWeek;
            var startDate = now;
            var fridayStartDate = now;

            if (currentDayOfWeek == DayOfWeek.Saturday || currentDayOfWeek == DayOfWeek.Sunday)
            {

                var diffDays = (int)now.DayOfWeek - (int)DayOfWeek.Friday;
                fridayStartDate = now.AddDays(-diffDays);
                startDate = new DateTime(fridayStartDate.Year, fridayStartDate.Month, fridayStartDate.Day, 17, 0, 0);
            }
            else
            {
                var diffDays = ((int)DayOfWeek.Friday) - (int)now.DayOfWeek;
                fridayStartDate = now.AddDays(diffDays);
                startDate = new DateTime(fridayStartDate.Year, fridayStartDate.Month, fridayStartDate.Day, 17, 0, 0);
            }

            var nextMonday = startDate.AddDays(3);

            var endDate = new DateTime(nextMonday.Year, nextMonday.Month, nextMonday.Day, 7, 0, 0);


            var totalDuration = endDate - startDate;
            var elapsed = now - startDate;

            var percentage =
                elapsed.TotalMilliseconds /
                totalDuration.TotalMilliseconds * 100;

            percentage = Math.Clamp(percentage, 0, 100);

            //if (now < startDate || now > endDate && (now.DayOfWeek == DayOfWeek.Saturday || now.DayOfWeek == DayOfWeek.Sunday))
            //{
            //    percentage = 0;
            //}

            return new
            {
                startDate = startDate.ToString("yyyy-MM-dd HH:mm:ss"),
                endDate = endDate.ToString("yyyy-MM-dd HH:mm:ss"),
                currentDate = now.ToString("yyyy-MM-dd HH:mm:ss"),
                percentage = percentage
            };
        }

        [HttpGet]
        [Route("year")]
        public object GetYear()
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


            var startDate = new DateTime(now.Year, 1, 1, 0, 0, 0);

            var endDate = new DateTime(now.Year + 1, 1, 1, 0, 0, 0);


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
    }
}
