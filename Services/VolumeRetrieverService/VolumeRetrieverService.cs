using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly.Retry;
using Polly;
using Axpo;
using AxpoAsignacion.Services.FileStorageService;
using System.Timers;

namespace AxpoAsignacion.Services.VolumeRetrieverService
{
    internal class VolumeRetrieverService : IVolumeRetrieverService
    {
        private readonly IPowerService _powerService;
        private readonly IFileGeneratorService _fileGeneratorService;
        private readonly VolumeRetrieverOptions _options;
        private readonly ILogger _logger;   
        public VolumeRetrieverService(IPowerService powerService,IFileGeneratorService fileGeneratorService, ILogger<VolumeRetrieverService> logger, IOptions<VolumeRetrieverOptions> options)
        {
            _powerService = powerService;
            _logger = logger;
            _options = options.Value;
            _fileGeneratorService = fileGeneratorService;
        }

        public void Retrieve(Object source, ElapsedEventArgs e)
        {
            Console.WriteLine("Ejecutando ");
            var retryPolicy = initRetryPolicy();
            var date = DateTime.Now;
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
                       Console.WriteLine($"Reintento {attempt} después de {sleepDuration.Seconds} segundos debido a: {exception.Message}");
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
