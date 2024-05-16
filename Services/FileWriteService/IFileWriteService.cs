using Axpo;
using System.Text;

namespace AxpoAsignacion.Services.FileWriteService
{
    public interface IFileWriteService
    {
        public void writeFile(string filePath, string content);
    }
}
