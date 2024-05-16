using Moq;
using AxpoAsignacion.Services.FileWriteService;
using Axpo;
using AxpoAsignacion.Services.VolumeRetrieverService;
using Microsoft.Extensions.Options;
using AxpoAsignacion.Services.CsvService;

namespace AxpoAsignacionTests
{
    public class VolumeRetrieverServiceTests
    {
        private VolumeRetrieverOptions options = new VolumeRetrieverOptions
        {
            Path = "dummyPath"
        };

        [Fact]
        public void powerServiceThrows_RequestIsRetried()
        {
            //Arrange
            var CsvService = new Mock<ICsvService>();
            var powerService = new Mock<IPowerService>();
            var mockOptions = new Mock<IOptions<VolumeRetrieverOptions>>();
            mockOptions.Setup(o=>o.Value).Returns(options);
            var powerTrade1 = PowerTrade.Create(DateTime.Today, 24);
            var powerTrade2 = PowerTrade.Create(DateTime.Today, 24);
            List<PowerTrade> powerTrades = new List<PowerTrade> { powerTrade1, powerTrade2 };
            var setupSequence = powerService.SetupSequence(
                e => e.GetTrades(It.IsAny<DateTime>())
                )
                .Throws(new Exception())
                .Returns(powerTrades);
            CsvService.Setup(
                e => e.SaveCsv(It.IsAny<List<PowerPeriod>>(), It.IsAny<DateTime>(), It.IsAny<string>())
                ).Verifiable();

            VolumeRetrieverService _sut = new(powerService.Object, CsvService.Object, mockOptions.Object);
            //Act
            _sut.Retrieve();
            //Assert
            powerService.Verify(
                e => e.GetTrades(It.IsAny<DateTime>()),
                Times.Exactly(2)
                );
            CsvService.Verify(
                e => e.SaveCsv(It.IsAny<List<PowerPeriod>>(), It.IsAny<DateTime>(), It.IsAny<string>()),
                Times.Exactly(1)
                );
        }
        [Fact]
        public void SumFunctionGeneratesResultCorrectly()
        {
            //Arrange
            var CsvService = new Mock<ICsvService>();
            var powerService = new Mock<IPowerService>();
            IOptions<VolumeRetrieverOptions> options = new Mock<IOptions<VolumeRetrieverOptions>>().Object;

            var powerTrade1 = PowerTrade.Create(DateTime.Today, 1);
            var powerTrade2 = PowerTrade.Create(DateTime.Today, 1);

            powerTrade1.Periods[0].SetVolume(4);
            powerTrade2.Periods[0].SetVolume(9);

            List<PowerTrade> powerTrades = new List<PowerTrade> { powerTrade1, powerTrade2 };

            VolumeRetrieverService _sut = new(powerService.Object, CsvService.Object, options);
            //Act
            var result = _sut.SumPowerPeriods(powerTrades);
            Assert.Equal(13,result.First().Volume);
        }
    }
}