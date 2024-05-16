using Microsoft.Extensions.Options;
using Polly.Retry;
using Polly;
using Axpo;
using AxpoAsignacion.Services.FileStorageService;
using Serilog;

namespace AxpoAsignacion.Services.VolumeRetrieverService
{
    public class VolumeRetrieverService : IVolumeRetrieverService
    {
        private readonly IPowerService _powerService;
        private readonly IFileGeneratorService _fileGeneratorService;
        private readonly VolumeRetrieverOptions _options;

        public VolumeRetrieverService(IPowerService powerService,IFileGeneratorService fileGeneratorService, IOptions<VolumeRetrieverOptions> options)
        {
            _powerService = powerService;
            _options = options.Value;
            _fileGeneratorService = fileGeneratorService;
        }

        public void Retrieve()
        {
            var retryPolicy = initRetryPolicy();
            var date = DateTime.Now;
            Log.Information($"Executing retrieval of data at {date.ToUniversalTime():yyyy-MM-ddTHH:mm:ssZ}");
            retryPolicy.Execute(() =>
            {
                var volume = _powerService.GetTrades(date);
                _fileGeneratorService.writeFile(FlattenPowerPeriodsList(volume.ToList()), date, _options.Path);
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
        private List<PowerPeriod> FlattenPowerPeriodsList(List<PowerTrade> data)
        {
            var jointPowerTrades = data.SelectMany(x => x.Periods).ToList();

            var SumByDate = jointPowerTrades
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
