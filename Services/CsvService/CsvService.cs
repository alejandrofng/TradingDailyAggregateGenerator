using Axpo;
using AxpoAsignacion.Services.FileWriteService;
using AxpoAsignacion.Services.TimeZoneService;
using Serilog;
using System.Text;

namespace AxpoAsignacion.Services.CsvService
{
    public class CsvService: ICsvService
    {
        private readonly IFileWriteService _fileWriteService;
        private readonly ITimeZoneService _timeZoneService;
        public CsvService(IFileWriteService fileWriteService, ITimeZoneService timeZoneService)
        {
            _fileWriteService = fileWriteService;
            _timeZoneService = timeZoneService;
        }
        public void SaveCsv(List<PowerPeriod> data, DateTime date, string filePath)
        {
            var sb = new StringBuilder();
            // Write the header line
            sb.Append("datetime ; Volume");
            sb.AppendLine();

            // Write the data lines
            foreach (var item in data)
            {
                var periodDate = DateTime.SpecifyKind(date.Date.AddHours(item.Period - 1),DateTimeKind.Unspecified);
                var periodDateOffSet = new DateTimeOffset(periodDate, _timeZoneService.getOffSet(periodDate)).UtcDateTime;
                sb.Append($"{periodDateOffSet.ToString("yyyy-MM-ddTHH:mm:ssZ")} ; ");
                sb.Append(item.Volume);
                sb.AppendLine();
            }
            string fileName = date.Date.ToString("yyyyMMdd") + "_" + date.AddDays(-1).ToString("yyyyMMddHHmm") + ".csv";
            Log.Information($"Saving file at {filePath} {DateTime.Now:yyyy-MM-ddTHH:mm:ssZ}");
            // Save the data to the CSV file
            _fileWriteService.writeFile(filePath + "\\" + fileName, sb.ToString());
        }

    }
}
