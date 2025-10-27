
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BlkCountriesProj.Services.Iservice;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
namespace BlkCountriesProj.Background
{
    public class TemporalBlockCleanupService : BackgroundService
    {
       
        private readonly ILogger<TemporalBlockCleanupService> _logger;
        private readonly IBlockedCountryRepository _repo;
        private readonly TimeSpan _interval = TimeSpan.FromSeconds(30);

        public TemporalBlockCleanupService(ILogger<TemporalBlockCleanupService> logger, IBlockedCountryRepository repo)
        {
            _logger = logger;
            _repo = repo;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var expired = _repo.GetExpiredTemporal(DateTime.UtcNow).Select(c => c.CountryCode).ToList();
                    if (expired.Any())
                    {
                        _repo.RemoveExpired(expired);
                        _logger.LogInformation("Removed expired temporal blocks: {codes}", string.Join(",", expired));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while cleaning temporal blocks");
                }

                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}
