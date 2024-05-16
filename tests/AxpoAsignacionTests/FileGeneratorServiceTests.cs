
using Axpo;
using AxpoAsignacion.Services.CsvService;
using AxpoAsignacion.Services.FileWriteService;
using Moq;

namespace AxpoAsignacionTests
{
    public class CsvServiceTests
    {
        [Fact]
        public void SaveCsv_ShouldCallFileWriterWithCorrectContent()
        {
            // Arrange
            var mockFileWriter = new Mock<IFileWriteService>();
            var service = new CsvService(mockFileWriter.Object);
            mockFileWriter.Setup(
                e => e.writeFile(It.IsAny<string>(), It.IsAny<string>())
                ).Verifiable();

            var period1 = new PowerPeriod();
            period1.SetVolume(5);
            var period2 = new PowerPeriod();
            period2.SetVolume(10);

            var path = "dummyPath";
            var data = new List<PowerPeriod>() { period1,period2};
            DateTime date = new DateTime(2023, 7, 1, 14, 30, 0);
            // Act
            service.SaveCsv(data, date, path);

            // Assert
            var expectedName = path+"\\20230702_202307011430.csv";
            var expectedContent = "datetime ; Volume\r\n2023-07-01T12:30:00Z;5\r\n2023-07-01T13:30:00Z;10\r\n";
            mockFileWriter.Verify(fw => fw.writeFile(expectedName, expectedContent), Times.Once);
        }
    }
}
