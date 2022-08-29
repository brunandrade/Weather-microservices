using CloudWeather.Reports.Configs;
using CloudWeather.Reports.DataAccess;
using CloudWeather.Reports.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();
builder.Services.AddTransient<IReportAggreagator, ReportAggreagator>();
builder.Services.AddOptions();
builder.Services.Configure<ReportDataConfig>(builder.Configuration.GetSection("WeatherDataConfig"));

//Add Database Context
builder.Services.AddDbContext<ReportDbContext>(
    options =>
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
        options.UseNpgsql(builder.Configuration.GetConnectionString("AppDb"));
    }, ServiceLifetime.Transient
);

// Add services to the container.
var app = builder.Build();

//Add Service Routes
app.MapGet("/weather-report/{zip}", async (string zip, [FromQuery] int? days, IReportAggreagator reportAgg) =>
{
    if (days == null || days < 1 || days > 30)
    {
        return Results.BadRequest("Please provide a 'days' query parameter between 1 and 30");
    }

    var report = await reportAgg.BuildWeeklyReport(zip, days.Value);
    return Results.Ok(report);
});

app.Run();
