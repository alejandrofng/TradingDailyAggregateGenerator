using Axpo;
using System.Text;
using Serilog;

namespace AxpoAsignacion.Services.FileWriteService
{
    public class FileWriteService : IFileWriteService
    {
        public void writeFile(string filePath, string content)
        {
            File.WriteAllText(filePath, content);
        }
    }
}
