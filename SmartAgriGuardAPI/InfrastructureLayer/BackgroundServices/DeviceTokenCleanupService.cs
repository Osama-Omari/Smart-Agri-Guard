using DataAccessLayer.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureLayer.BackgroundServices
{
    public class DeviceTokenCleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<DeviceTokenCleanupService> _logger;
        public DeviceTokenCleanupService(IServiceScopeFactory serviceScopeFactory, ILogger<DeviceTokenCleanupService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                await CleanOldTokens();

                await Task.Delay(TimeSpan.FromDays(1),stoppingToken);
            }
        }

        private async Task CleanOldTokens()
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IDeviceTokenRepository>();

            var cutoff = DateTime.UtcNow.AddMonths(-6);

            var oldTokenIds = await repo.GetOldTokenIdsAsync(cutoff);
            if(oldTokenIds.Any())
            {
                await repo.DeleteTokensAsync(oldTokenIds.ToArray());
                _logger.LogInformation($"Deleted {oldTokenIds.Count} old device tokens.");
            }
            else
            {
                _logger.LogInformation("No old device tokens to delete.");
            }
        }
    }
}
