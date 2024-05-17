using Axpo;
using AxpoAsignacion.Services.CsvService;
using AxpoAsignacion.Services.FileWriteService;
using AxpoAsignacion.Services.TimeZoneService;
using Moq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AxpoAsignacionTests
{
    public class TimeZoneServiceTests
    {
        [Fact]
        public void InvalidTimeZoneId_ThrowsException()
        {
            // Arrange
            string TimeZoneId = "Europe/Caracas";
            Assert.Throws<TimeZoneNotFoundException>(() =>
            {
                new TimeZoneService(TimeZoneId);
            });
        }
        [Fact]
        public void ValidTimeZone_OffSetIsCalculatedProperly()
        {
            // Arrange
            string TimeZoneId = "Europe/Berlin";
            var sut = new TimeZoneService(TimeZoneId);
            DateTime date = new DateTime(2023, 7, 1, 14, 30, 0);
            var offset = sut.getOffSet(date).Hours;

            Assert.Equal(2, offset);
        }

        [Fact]
        public void TimeZoneSaveDaylight_Is_HandledProperly()
        {
            // Arrange
            string TimeZoneId = "Europe/Berlin";
            var sut = new TimeZoneService(TimeZoneId);
            DateTime date = new DateTime(2024, 10, 27, 3, 0, 0);
            var offset = sut.getOffSet(date).Hours;

            Assert.Equal(1, offset);
        }
    }
}
