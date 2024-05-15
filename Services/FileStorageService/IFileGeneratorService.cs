using Axpo;

namespace AxpoAsignacion.Services.FileStorageService
{
    internal interface IFileGeneratorService
    {
        public void writeFile(List<PowerPeriod> data, DateTime date, string filePath);
    }
}
