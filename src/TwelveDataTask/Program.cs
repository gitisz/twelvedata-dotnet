using TwelveData.Providers;
using TwelveData.Services;
using Serilog;

Log.Information("Staring up logging");

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHttpClient<TwelveDataProvider>(client =>
    {
        var apiKey = Environment.GetEnvironmentVariable(builder.Configuration["TwelveData:Authorization:ApiKey"] ?? string.Empty);
        client.BaseAddress = new Uri("https://api.twelvedata.com/");
        client.DefaultRequestHeaders.Add("Authorization", $"apikey {apiKey}");
    });
builder.Services.AddHostedService<TwelveDataService>();
builder.Logging.ClearProviders();
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
builder.Logging.AddSerilog();


var host = builder.Build();
host.Run();
