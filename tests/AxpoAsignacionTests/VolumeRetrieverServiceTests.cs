using Moq;
using AxpoAsignacion.Services.FileStorageService;
using Axpo;
using AxpoAsignacion.Services.VolumeRetrieverService;
using Microsoft.Extensions.Options;

namespace AxpoAsignacionTests
{
    public class VolumeRetrieverServiceTests
    {

        [Fact]
        public void powerServiceThrows_RequestIsRetried()
        {
            //Arrange
            var fileGeneratorService = new Mock<IFileGeneratorService>();
            var powerService = new Mock<IPowerService>();
            IOptions<VolumeRetrieverOptions> options = new Mock<IOptions<VolumeRetrieverOptions>>().Object;

            var powerTrade1 = PowerTrade.Create(DateTime.Today, 24);
            var powerTrade2 = PowerTrade.Create(DateTime.Today, 24);
            List<PowerTrade> powerTrades = new List<PowerTrade> { powerTrade1, powerTrade2 };
            var setupSequence = powerService.SetupSequence(
                e => e.GetTrades(It.IsAny<DateTime>())
                )
                .Throws(new Exception())
                .Returns(powerTrades);

            VolumeRetrieverService _sut = new(powerService.Object, fileGeneratorService.Object, options);
            _sut.Retrieve();

        }
    }
}