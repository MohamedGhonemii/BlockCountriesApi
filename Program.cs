using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using BlkCountriesProj.Services.Iservice;
using BlkCountriesProj.Services.Service;
using BlkCountriesProj.Background;
using BlkCountriesProj.Models;
using BlkCountriesProj.Services;

var builder = WebApplication.CreateBuilder(args);

// =========================================
// 1) Bind GeoIp config section from appsettings.json
// =========================================
builder.Services.Configure<GeoIpOptions>(builder.Configuration.GetSection("GeoIp"));

// =========================================
// 2) Add Services to DI Container
// =========================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "BlockedCountries API",
        Version = "v1",
        Description = "API for managing blocked countries and checking IP addresses"
    });
});

// 3) HttpClient for GeoIP service
builder.Services.AddHttpClient<IGeoIpService, GeoIpService>();

// 4) In-memory Repositories & Services
builder.Services.AddSingleton<IBlockedCountryRepository, InMemoryBlockedCountryRepository>();


builder.Services.AddSingleton<ILogService, LogService>();
builder.Services.AddHostedService<TemporalBlockCleanupService>();

// 5) Background Service for cleaning temporal blocks
//builder.Services.AddScoped<TemporalBlockCleanupService>();

// =========================================
// 6) Build app
// =========================================
var app = builder.Build();

// =========================================
// 7) Configure middleware
// =========================================
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "BlockedCountries API v1"));
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

// =========================================
// Helper record for config binding
// =========================================
public record GeoIpOptions
{
    public string Provider { get; init; } = "ipapi"; 
    public string ApiKey { get; init; } = "";
}
