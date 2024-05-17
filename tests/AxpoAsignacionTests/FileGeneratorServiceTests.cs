
using Axpo;
using AxpoAsignacion.Services.CsvService;
using AxpoAsignacion.Services.FileWriteService;
using AxpoAsignacion.Services.TimeZoneService;
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
            var mockTimeZone = new Mock<ITimeZoneService>();

            var service = new CsvService(mockFileWriter.Object,mockTimeZone.Object);
            mockFileWriter.Setup(
                e => e.writeFile(It.IsAny<string>(), It.IsAny<string>())
                ).Verifiable();

            mockTimeZone.Setup(
                e => e.getOffSet(It.IsAny<DateTime>())
                ).Returns(new TimeSpan(2,0,0));

            var period1 = new PowerPeriod(1);
            period1.SetVolume(5);
            var period2 = new PowerPeriod(2);
            period2.SetVolume(10);

            var path = "dummyPath";
            var data = new List<PowerPeriod>() { period1,period2};
            DateTime date = new DateTime(2023, 7, 1, 14, 30, 0);
            // Act
            service.SaveCsv(data, date, path);

            // Assert
            var expectedName = path+"\\20230701_202306301430.csv";
            var expectedContent = "datetime ; Volume\r\n2023-06-30T22:00:00Z ; 5\r\n2023-06-30T23:00:00Z ; 10\r\n";
            mockFileWriter.Verify(fw => fw.writeFile(expectedName, expectedContent), Times.Once);
        }

        [Fact]
        public void SaveCsv_WithSaveDaylight_ShoudlReturnAppropiateResult()
        {
            // Arrange
            var mockFileWriter = new Mock<IFileWriteService>();
            var timeZoneService = new TimeZoneService("Europe/Berlin");

            var service = new CsvService(mockFileWriter.Object, timeZoneService);
            mockFileWriter.Setup(
                e => e.writeFile(It.IsAny<string>(), It.IsAny<string>())
                ).Verifiable();

            var data = new List<PowerPeriod>();

            for (int i = 0; i <24; i++)
            {
                var period = new PowerPeriod(i + 1);
                period.SetVolume(i);
                data.Add(period);
            }

            var path = "dummyPath";

            DateTime date = new DateTime(2024, 10, 27, 0, 30, 0);
            // Act
            service.SaveCsv(data, date, path);

            // Assert
            var expectedName = path + "\\20241027_202410260030.csv";
            var expectedContent = "datetime ; Volume\r\n2024-10-26T22:00:00Z ; 0\r\n2024-10-26T23:00:00Z ; 1\r\n2024-10-27T01:00:00Z ; 2\r\n2024-10-27T02:00:00Z ; 3\r\n2024-10-27T03:00:00Z ; 4\r\n2024-10-27T04:00:00Z ; 5\r\n2024-10-27T05:00:00Z ; 6\r\n2024-10-27T06:00:00Z ; 7\r\n2024-10-27T07:00:00Z ; 8\r\n2024-10-27T08:00:00Z ; 9\r\n2024-10-27T09:00:00Z ; 10\r\n2024-10-27T10:00:00Z ; 11\r\n2024-10-27T11:00:00Z ; 12\r\n2024-10-27T12:00:00Z ; 13\r\n2024-10-27T13:00:00Z ; 14\r\n2024-10-27T14:00:00Z ; 15\r\n2024-10-27T15:00:00Z ; 16\r\n2024-10-27T16:00:00Z ; 17\r\n2024-10-27T17:00:00Z ; 18\r\n2024-10-27T18:00:00Z ; 19\r\n2024-10-27T19:00:00Z ; 20\r\n2024-10-27T20:00:00Z ; 21\r\n2024-10-27T21:00:00Z ; 22\r\n2024-10-27T22:00:00Z ; 23\r\n";
            mockFileWriter.Verify(fw => fw.writeFile(expectedName, expectedContent), Times.Once);
        }
    }
}
