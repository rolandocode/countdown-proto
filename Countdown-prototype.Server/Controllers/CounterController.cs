using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace Countdown_prototype.Server.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class CounterController : ControllerBase
    {

        public CounterController() { }

        private string GetIP()
        {
            string clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            return clientIp;
        }

        private static readonly object _logLock = new object();

        private void WriteLog(string requestName, DateTime now)
        {
            lock (_logLock)
            {
                try
                {
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "requestLogs.csv");

                    // FileShare.ReadWrite allows other streams/endpoints to access the file simultaneously
                    using var stream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                    using var writer = new StreamWriter(stream);

                    writer.WriteLine($"{GetIP()}|{now:yyyy-MM-dd HH:mm:ss}|'{requestName}'");
                }
                catch (IOException ex)
                {
                    // Catches temporary external locks (e.g., OneDrive or Excel holding the file)
                    // Prevents the API request from crashing
                    System.Diagnostics.Debug.WriteLine($"Log write skipped: {ex.Message}");
                }
            }
        }

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

            WriteLog("Get Generic Request", now);
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
            WriteLog("Get Day Request", now);

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


            if (now.DayOfWeek == DayOfWeek.Saturday || now.DayOfWeek == DayOfWeek.Sunday || (now.DayOfWeek == DayOfWeek.Friday && now.Hour >= 17))
            {
                var nextMonday = now.AddDays(((int)DayOfWeek.Monday - (int)now.DayOfWeek + 7) % 7);
                startDate = new DateTime(nextMonday.Year, nextMonday.Month, nextMonday.Day, 7, 0, 0);
                endDate = startDate.AddHours(10);
            }

            var totalDuration = endDate - startDate;
            var elapsed = now - startDate;

            var percentage =
                elapsed.TotalMilliseconds /
                totalDuration.TotalMilliseconds * 100;

            percentage = Math.Clamp(percentage, 0, 100);

            if (now < startDate || now > endDate || (now.DayOfWeek == DayOfWeek.Saturday || now.DayOfWeek == DayOfWeek.Sunday))
            {
                percentage = 0;
            }

            WriteLog("Get Work Request", now);

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

                var diffDays = 0;
                if (currentDayOfWeek == DayOfWeek.Saturday)
                    diffDays = (int)now.DayOfWeek - (int)DayOfWeek.Friday;

                if (currentDayOfWeek == DayOfWeek.Sunday)
                    diffDays = 2;


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

            WriteLog("Get Weekend Request", now);


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

            WriteLog("Get Year Request", now);

            return new
            {
                startDate = startDate.ToString("yyyy-MM-dd HH:mm:ss"),
                endDate = endDate.ToString("yyyy-MM-dd HH:mm:ss"),
                currentDate = now.ToString("yyyy-MM-dd HH:mm:ss"),
                percentage = percentage
            };
        }

        [HttpGet]
        [Route("hour")]
        public object GetHour()
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


            var startDate = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0);

            var endDate = startDate.AddHours(1);


            var totalDuration = endDate - startDate;
            var elapsed = now - startDate;

            var percentage =
                elapsed.TotalMilliseconds /
                totalDuration.TotalMilliseconds * 100;

            percentage = Math.Clamp(percentage, 0, 100);

            WriteLog("Get Hour Request", now);

            return new
            {
                startDate = startDate.ToString("yyyy-MM-dd HH:mm:ss"),
                endDate = endDate.ToString("yyyy-MM-dd HH:mm:ss"),
                currentDate = now.ToString("yyyy-MM-dd HH:mm:ss"),
                percentage = percentage
            };
        }

        [HttpGet]
        [Route("month")]
        public object GetMonth()
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


            var startDate = new DateTime(now.Year, now.Month, 1, 0, 0, 0);
            var endDate = startDate.AddMonths(1);


            var totalDuration = endDate - startDate;
            var elapsed = now - startDate;

            var percentage =
                elapsed.TotalMilliseconds /
                totalDuration.TotalMilliseconds * 100;

            percentage = Math.Clamp(percentage, 0, 100);

            WriteLog("Get Month Request", now);

            return new
            {
                startDate = startDate.ToString("yyyy-MM-dd HH:mm:ss"),
                endDate = endDate.ToString("yyyy-MM-dd HH:mm:ss"),
                currentDate = now.ToString("yyyy-MM-dd HH:mm:ss"),
                percentage = percentage
            };
        }


        [HttpGet]
        [Route("payroll")]
        public object GetPayroll()
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

            var startDate = new DateTime(now.Year, now.Month, 1, 0, 0, 0);
            var endDate = startDate.AddMonths(1);

            var dayOfMonth = now.Day;

            if (dayOfMonth < 15)
            {
                startDate = new DateTime(now.Year, now.Month, 1, 0, 0, 0);
                endDate = new DateTime(now.Year, now.Month, 15, 0, 0, 0);
            }
            else
            {
                startDate = new DateTime(now.Year, now.Month, 15, 0, 0, 0);
                endDate = new DateTime(now.Year, now.Month, 1, 0, 0, 0).AddMonths(1).AddSeconds(-1);
            }

            var totalDuration = endDate - startDate;
            var elapsed = now - startDate;

            var percentage =
                elapsed.TotalMilliseconds /
                totalDuration.TotalMilliseconds * 100;

            percentage = Math.Clamp(percentage, 0, 100);

            WriteLog("Get Payroll Request", now);


            return new
            {
                startDate = startDate.ToString("yyyy-MM-dd HH:mm:ss"),
                endDate = endDate.ToString("yyyy-MM-dd HH:mm:ss"),
                currentDate = now.ToString("yyyy-MM-dd HH:mm:ss"),
                percentage = percentage
            };
        }

        [HttpGet]
        [Route("logs")]
        public IActionResult GetLogs()
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "requestLogs.csv");

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Log file does not exist yet.");
            }

            // Open stream with FileShare.ReadWrite to avoid locking collisions with AppendAllText
            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            return File(stream, "text/csv", "requestLogs.csv");
        }

        [HttpGet]
        [Route("clearLogs")]
        public IActionResult ClearLogs()
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "requestLogs.csv");

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Log file does not exist.");
            }

            try
            {
                var lineCount = 0;
                var fileSizeBytes = 0L;
                lock (_logLock)
                {
                    var fileInfo = new FileInfo(filePath);
                    fileSizeBytes = fileInfo.Length; // Get size in bytes before clearing

                    lineCount = System.IO.File.ReadLines(filePath).Count();
                    // FileMode.Create overwrites the file and truncates its size to 0 bytes
                    using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                }

                return Ok(new { message = $"Log file cleared successfully. {lineCount} lines removed. {fileSizeBytes} bytes cleaned" });
            }
            catch (IOException ex)
            {
                // Triggers if another process (like Excel) holds an exclusive lock on the file
                return StatusCode(409, $"Cannot clear log file because it is locked by another process: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, $"Permission denied: {ex.Message}");
            }
        }
    }
}
