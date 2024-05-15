using Axpo;
using System.Reflection;
using System.Text;

namespace AxpoAsignacion.Services.FileStorageService
{
    internal class FileGeneratorService : IFileGeneratorService
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

            // Save the data to the CSV file
            File.WriteAllText(filePath + "\\"+fileName, sb.ToString());
        }
    }
}
