using Axpo;
using System.Text;
using Serilog;

namespace AxpoAsignacion.Services.FileStorageService
{
    public class FileGeneratorService : IFileGeneratorService
    {
        public void writeFile(List<PowerPeriod> data, DateTime date, string filePath)
        {
            
            var sb = new StringBuilder();
            // Write the header line
            sb.Append("datetime ; Volume");
            sb.AppendLine();

            // Write the data lines
            int counter = 0;
            foreach (var item in data)
            {
                sb.Append(date.AddHours(counter).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ") +";");
                sb.Append(item.Volume);
                sb.AppendLine();
                counter++;
            }

            string fileName = date.AddDays(1).Date.ToString("yyyyMMdd") + "_" + date.ToString("yyyyMMddHHmm") + ".csv";
            Log.Information($"Saving file at {filePath} {date.ToUniversalTime():yyyy-MM-ddTHH:mm:ssZ}");
            // Save the data to the CSV file
            File.WriteAllText(filePath + "\\"+fileName, sb.ToString());
        }
    }
}
