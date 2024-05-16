using AxpoAsignacion.Services.VolumeRetrieverService;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Axpo;
using Serilog;
using AxpoAsignacion.Services.FileWriteService;
using System.Timers;
using AxpoAsignacion.Services.CsvService;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Configuration.Sources.Clear();

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.Configure<VolumeRetrieverOptions>(builder.Configuration.GetSection(nameof(VolumeRetrieverOptions)));

builder.Services.AddSingleton<IPowerService, PowerService>();
builder.Services.AddSingleton<ICsvService, CsvService>();
builder.Services.AddSingleton<IFileWriteService, FileWriteService>();
builder.Services.AddSingleton<IVolumeRetrieverService, VolumeRetrieverService>();

var serviceProvider = builder.Services.BuildServiceProvider();
var volumeRetriever = serviceProvider.GetRequiredService<IVolumeRetrieverService>();

// Configurar Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt",
    rollingInterval: RollingInterval.Day)
    .CreateLogger();

var timeInterval = builder.Configuration.GetValue<int>("intervalInMs");

Log.Information("Configured time interval in ms: {0}", timeInterval);
volumeRetriever.Retrieve();


System.Timers.Timer aTimer = new (timeInterval);
aTimer.Elapsed += Task;
aTimer.AutoReset = true;
aTimer.Start();
Log.Information($"Timer started at {DateTime.Now.ToUniversalTime():yyyy-MM-ddTHH:mm:ssZ}");

Console.WriteLine("\nPress the Enter key to exit the application...\n");
Console.ReadLine();
aTimer.Stop();
aTimer.Dispose();


void Task(Object source, ElapsedEventArgs e)
{
    volumeRetriever.Retrieve();
}