using System.Collections.Specialized;
using System.Text.Json;
using System.Web;
using TwelveData.Models;

namespace TwelveData.Providers;

public class TwelveDataProvider
{
    private readonly ILogger<TwelveDataProvider> _logger;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public TwelveDataProvider(ILogger<TwelveDataProvider> logger
        , IConfiguration configuration
        , HttpClient httpClient
        )
    {
        _logger = logger;
        _configuration = configuration;
        _httpClient = httpClient;
    }

    public async Task<TwelveDataPrice?> GetPriceAsync(string? symbol)
    {
        NameValueCollection queryString = new()
        {
            { "symbol", symbol }
        };

        var response = await _httpClient.GetAsync($"price?{ConstructQueryString(queryString)}");

        var content = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            var price = JsonSerializer.Deserialize<TwelveDataPrice>(content) ?? throw new ArgumentException("Seven Hells!");

            price.Symbol = symbol;

            return price;
        }

        return null;
    }

    public static String ConstructQueryString(NameValueCollection parameters, String delimiter = "&", Boolean omitEmpty = true)
    {
        Char equals = '=';
        List<String> items = [];

        for (int i = 0; i < parameters.Count; i++)
        {
            foreach (String value in parameters.GetValues(i))
            {
                Boolean addValue = (omitEmpty) ? !String.IsNullOrEmpty(value) : true;
                if (addValue)
                    items.Add(String.Concat(parameters.GetKey(i), equals, HttpUtility.UrlEncode(value)));
            }
        }

        return String.Join(delimiter, items.ToArray());
    }
}