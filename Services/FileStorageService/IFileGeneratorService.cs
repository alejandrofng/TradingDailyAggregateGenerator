using Axpo;

namespace AxpoAsignacion.Services.FileStorageService
{
    public interface IFileGeneratorService
    {
        public void writeFile(List<PowerPeriod> data, DateTime date, string filePath);
    }
}
