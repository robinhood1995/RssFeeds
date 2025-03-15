using Microsoft.Extensions.Hosting;
using RssFeeds.Repositories.Contracts;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace RssFeeds.Services
{
    public class RssFeedBackgroundService : BackgroundService
    {

        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RssFeedBackgroundService> _logger;
        private readonly int _intervalSeconds = 900; // Adjust interval as needed.
        private Timer _timer; // Add a timer.

        public RssFeedBackgroundService(IServiceProvider serviceProvider, ILogger<RssFeedBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("RSS Feed Background Service started.");
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(_intervalSeconds)); // Start timer.
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("RSS Feed Background Service stopped.");
            _timer?.Dispose(); // Dispose timer when stopping.
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // This method is no longer needed with the timer.
            // It's kept here to satisfy the BackgroundService abstract class.
            await Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            _ = ProcessRssFeeds(); // Fire and forget.
        }

        private async Task ProcessRssFeeds()
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var rssFeedProcessor = scope.ServiceProvider.GetRequiredService<IRssFeedProcessor>();
                    await rssFeedProcessor.ProcessFeeds();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing RSS feeds.");
            }
        }


    }
}
