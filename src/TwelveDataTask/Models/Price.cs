using System.Text.Json.Serialization;
using InfluxDB.Client.Core;
using Newtonsoft.Json;

namespace TwelveData.Models;

[Measurement("TwelveDataPrice")]
public class TwelveDataPrice
{
    [Column("symbol", IsTag = true)]
    public string? Symbol { get; set; }

    [Column("price")]
    [JsonPropertyName("price")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal Price { get; set; } = default!;

    [Column(IsTimestamp = true)]
    public DateTimeOffset Time { get; set; } = DateTimeOffset.Now!;
}