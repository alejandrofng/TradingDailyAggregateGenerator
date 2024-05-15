using AxpoAsignacion.Services.VolumeRetrieverService;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Axpo;
using Serilog;
using AxpoAsignacion.Services.FileStorageService;
using System.Timers;
using System.Runtime.CompilerServices;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Configuration.Sources.Clear();

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.Configure<VolumeRetrieverOptions>(builder.Configuration.GetSection(nameof(VolumeRetrieverOptions)));

builder.Services.AddSingleton<IPowerService, PowerService>();
builder.Services.AddSingleton<IFileGeneratorService, FileGeneratorService>();
builder.Services.AddSingleton<IVolumeRetrieverService, VolumeRetrieverService>();

var serviceProvider = builder.Services.BuildServiceProvider();
var volumeRetriever = serviceProvider.GetRequiredService<IVolumeRetrieverService>();

// Configurar Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File("logs/log.txt",
    rollingInterval: RollingInterval.Day)
    .CreateLogger();

var timeInterval = builder.Configuration.GetValue<int>("intervalInMs");

//var autoEvent = new AutoResetEvent(true);

//TimerCallback callback = new(volumeRetriever.Retrieve);
//Timer timer = new(callback, autoEvent,0,timeInterval);
//autoEvent.WaitOne();

//autoEvent.WaitOne();

System.Timers.Timer aTimer = new (timeInterval);
aTimer.Elapsed += volumeRetriever.Retrieve;
aTimer.AutoReset = true;
aTimer.Enabled = true;

Console.WriteLine("initialized services");
Console.WriteLine("\nPress the Enter key to exit the application...\n");
Console.WriteLine("The application started at {0:HH:mm:ss.fff}", DateTime.Now);
Console.ReadLine();
aTimer.Stop();
aTimer.Dispose();