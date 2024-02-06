# TwelveData .NET
This application connects to the TwelveData API to retrieve price for one or more symbols.

When a successful authorization is made, symbol price is returns and then written to an InfluxDB v2 instance that runs separately.  For more information on how to get InfluxDB v2 installed, please see their [installation documentation](https://docs.influxdata.com/influxdb/v2/install/?t=Docker).

## Configuration
Reference `appsettings.Development.json` and update the correct values for your environment.  You will need to update the following sections:

### - `InfluxDBv2`
  - **Protocol**: Specify either `http` or `https` depending on if your InfluxDB v2 instance requires SSL.
  - **Url**: Provide the IP address or DNS name of the InfluxDB instance.
  - **Port**: Specify the port number that InfluxDB v2 is accessible from.
  - **TokenKey**: Specify an environment variable key for the InfluxDB v2 token that allows access to your InfluxDB v2 bucket.
    - Note: Do not provide the actual token. When starting this application, an environment variable should be set with the actual token that matches the key.
  - **OrgKey**: Specify an environment variable key for the InfluxDB v2 organization that allows access to your InfluxDB v2 bucket.
    - Note: Do not provide the actual organization. When starting this application, an environment variable should be set with the actual organization that matches the key.
  - **Bucket**: specify the InfluxDB v2 bucket where you would like the TwelveData metric data to be written.

### - `TwelveData`

  - **Authorization**:
    - **ApiKey**: Specify an environment variable key for the TwelveData login.
      - Note: Do not provide the actual login . When starting this application, an environment variable should be set with the actual login that matches the key.
  - **"Symbols"**: Specify an array of the symbols for the prices desired to be captured.
    - [ "TSLA", "COF" ]
  - **"MarketOpen"**: The time expressed in military time when the market opens. Requests to TwelveData will only occur between `MarketOpen` and `MarketClose`.
  - **"MarketClose"**: The time expressed in military time when the market closes. Requests to TwelveData will only occur between `MarketOpen` and `MarketClose`.


from(bucket: "twelvedata_dotnet") |> range(start: -24h, stop: now()) |> filter(fn: (r) => r["_measurement"] == "TwelveDataPrice") |> filter(fn: (r) => r["symbol"] == "SYMBOL")   |> filter(fn: (r) => r["_field"] == "price")   |> yield(name: "mean")

from(bucket: "twelvedata_dotnet")
  |> range(start: -24h, stop: now())
  |> filter(fn: (r) => r["_measurement"] == "TwelveDataPrice")
  |> filter(fn: (r) => r["symbol"] == "SYMBOL")
  |> yield(name: "mean")
