using System.Diagnostics;
using System.Text.Json;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using TwelveData.Models;
using TwelveData.Providers;

namespace TwelveData.Services;

public class TwelveDataService : BackgroundService
{
    private readonly ILogger<TwelveDataService> _logger;
    private readonly IConfiguration _configuration;

    private readonly TwelveDataProvider _twelveDataProvider;

    public TwelveDataService(ILogger<TwelveDataService> logger
        , IConfiguration configuration
        , TwelveDataProvider twelveDataProvider)
    {
        _logger = logger;
        _configuration = configuration;
        _twelveDataProvider = twelveDataProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            foreach (var symbol in _configuration.GetSection("TwelveData:Symbols").Get<string[]>() ?? [])
            {
                var marketOpen = DateTimeOffset.Now > DateTimeOffset.Now.Date.AddHours(9)
                    && DateTimeOffset.Now.LocalDateTime < DateTimeOffset.Now.LocalDateTime.Date.AddHours(17);

                _logger.LogInformation($"Checking if market is open: {DateTimeOffset.Now} - {marketOpen}");

                if (marketOpen)
                {
                    var price = await _twelveDataProvider.GetPriceAsync(symbol);

                    UpdateBucket(price);
                }
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    private void UpdateBucket(TwelveDataPrice? twelveDataPrice)
    {
        var influxDBProtocol = _configuration["InfluxDBv2:Protocol"];
        var influxDBUrl = _configuration["InfluxDBv2:Url"];
        var influxDBPort = _configuration["InfluxDBv2:Port"];
        var influxDBBucket = _configuration["InfluxDBv2:Bucket"];
        var influxDBToken = Environment.GetEnvironmentVariable(_configuration["InfluxDBv2:TokenKey"] ?? "");
        var influxDBOrg = Environment.GetEnvironmentVariable(_configuration["InfluxDBv2:OrgKey"] ?? "");

        var influxDBUri = $"{influxDBProtocol}://{influxDBUrl}:{influxDBPort}";

        using var client = new InfluxDBClient(influxDBUri, influxDBToken);
        using var writeApi = client.GetWriteApi();

        _logger.LogInformation($"Saving price: {JsonSerializer.Serialize(twelveDataPrice)}");

        writeApi.WriteMeasurement(twelveDataPrice, WritePrecision.Ns, influxDBBucket, influxDBOrg);
    }
}
