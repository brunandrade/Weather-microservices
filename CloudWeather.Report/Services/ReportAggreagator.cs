using CloudWeather.Reports.Configs;
using CloudWeather.Reports.DataAccess;
using CloudWeather.Reports.Models;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace CloudWeather.Reports.Services
{
    public interface IReportAggreagator
    {
        public Task<Report> BuildWeeklyReport(string zip, int days);
    }
    public class ReportAggreagator : IReportAggreagator
    {
        private readonly IHttpClientFactory _http;
        private readonly ILogger<ReportAggreagator> _logger;
        private readonly ReportDbContext _db;
        private readonly IOptions<ReportDataConfig> _config;

        public ReportAggreagator(IHttpClientFactory http, ILogger<ReportAggreagator> logger, ReportDbContext db, IOptions<ReportDataConfig> config)
        {
            _http = http;
            _logger = logger;
            _db = db;
            _config = config;
        }

        public async Task<Report> BuildWeeklyReport(string zip, int days)
        {
            var httpClient = _http.CreateClient();

            var precipData = await FetchPrecipitationData(httpClient, zip, days);
            var totalSnow = GetTotalSnow(precipData);
            var totalRain = GetTotalRain(precipData);

            var tempData = await FetchTemperatureData(httpClient, zip, days);
            var averageHighTemp = tempData.Average(t => t.TempHigh);
            var averageLowTemp = tempData.Average(t => t.TempLow);

            var weeklyReport = new Report
            {
                AverageHigh = Math.Round(averageHighTemp, 1),
                AverageLow = Math.Round(averageLowTemp, 1),
                RainfallTotalInches = totalRain,
                SnowTotalInches = totalSnow,
                ZipCode = zip,
                CreatedOn = DateTime.UtcNow,
            };

            _db.Add(weeklyReport);
            await _db.SaveChangesAsync();

            return weeklyReport;
        }

        private async Task<List<TemperatureModel>> FetchTemperatureData(HttpClient httpClient, string zip, int days)
        {
            var endpoint = BuildTemperatureServiceEnpoint(zip, days);
            var temparatureRecords = await httpClient.GetAsync(endpoint);
            var temperatureData = await temparatureRecords
                .Content
                .ReadFromJsonAsync<List<TemperatureModel>>();

            return temperatureData ?? new List<TemperatureModel>();
        }

        private string BuildTemperatureServiceEnpoint(string zip, int days)
        {
            var tempServiceProtocol = _config.Value.TempDataProtocol;
            var tempServiceHost = _config.Value.TempDataHost;
            var tempServicePort = _config.Value.TempDataPort;

            return $"{tempServiceProtocol}://{tempServiceHost}:{tempServicePort}/observation/{zip}?days={days}";
        }

        private async Task<List<PrecipitationModel>> FetchPrecipitationData(HttpClient httpClient, string zip, int days)
        {
            var endpoint = BuildPrecipitationServiceEnpoint(zip, days);
            var precipRecords = await httpClient.GetAsync(endpoint);
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

            var precipitationData = await precipRecords
               .Content
               .ReadFromJsonAsync<List<PrecipitationModel>>();

            return precipitationData ?? new List<PrecipitationModel>();
        }

        private string BuildPrecipitationServiceEnpoint(string zip, int days)
        {
            var precipServiceProtocol = _config.Value.PrecipDataProtocol;
            var precipServiceHost = _config.Value.PrecipDataHost;
            var precipServicePort = _config.Value.PrecipDataPort;

            return $"{precipServiceProtocol}://{precipServiceHost}:{precipServicePort}/observation/{zip}?days={days}";
        }

        private static decimal GetTotalSnow(List<PrecipitationModel> precipData)
        {
            var totalSnow = precipData
                 .Where(p => p.WeatherType == "snow")
                 .Sum(p => p.AmountInches);

            return Math.Round(totalSnow, 1);
        }

        private static decimal GetTotalRain(List<PrecipitationModel> precipData)
        {
            var totalRain = precipData
                 .Where(p => p.WeatherType == "rain")
                 .Sum(p => p.AmountInches);

            return Math.Round(totalRain, 1);
        }
    }
}
