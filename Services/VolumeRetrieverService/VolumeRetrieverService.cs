using Microsoft.Extensions.Options;
using Polly.Retry;
using Polly;
using Axpo;
using AxpoAsignacion.Services.FileWriteService;
using Serilog;
using System.Runtime.CompilerServices;
using AxpoAsignacion.Services.CsvService;
[assembly: InternalsVisibleTo("AxpoAsignacionTests")]

namespace AxpoAsignacion.Services.VolumeRetrieverService
{
    public class VolumeRetrieverService : IVolumeRetrieverService
    {
        private readonly IPowerService _powerService;
        private readonly ICsvService _csvService;
        private readonly VolumeRetrieverOptions _options;

        public VolumeRetrieverService(IPowerService powerService, ICsvService csvService, IOptions<VolumeRetrieverOptions> options)
        {
            _powerService = powerService;
            _options = options.Value;
            _csvService = csvService;
        }

        public void Retrieve()
        {
            var retryPolicy = initRetryPolicy();
            var date = DateTime.SpecifyKind(DateTime.Now.AddDays(1), DateTimeKind.Utc);
            Log.Information($"Executing retrieval of data at {DateTime.Now:yyyy-MM-ddTHH:mm:ssZ}");
            retryPolicy.Execute(() =>
            {
                var volume = _powerService.GetTrades(date);
                var dataToSaveInFile = SumPowerPeriods(volume.ToList());
                _csvService.SaveCsv(dataToSaveInFile, date, _options.Path);
            });
        }
        private RetryPolicy initRetryPolicy()
        {
            return Policy
               .Handle<Exception>()
               .WaitAndRetry(
                   retryCount: 59,
                   sleepDurationProvider: attempt => TimeSpan.FromSeconds(1),
                   onRetry: (exception, sleepDuration, attempt, context) =>
                   {
                       Log.Information($"Retrying attempt {attempt} after {sleepDuration.Seconds} seconds due to exception: {exception.Message}");
                   });
        }
        internal List<PowerPeriod> SumPowerPeriods(List<PowerTrade> data)
        {
            var flattenedPowerPeriods = data.SelectMany(x => x.Periods).ToList();
            var SumByDate = flattenedPowerPeriods
                .GroupBy(pair => pair.Period)
                .Select(grupo => {
                    var period = new PowerPeriod(grupo.Key);
                    period.SetVolume(grupo.Sum(pair => pair.Volume));
                    return period;
                }
                ).ToList();
            return SumByDate;
        }
    }
}
