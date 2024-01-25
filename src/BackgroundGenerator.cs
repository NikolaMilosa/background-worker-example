using System.Threading.Channels;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

class BackgroundGenerator<T> : IHostedService
{
    private readonly ILogger<BackgroundGenerator<T>> _logger;
    private readonly ChannelReader<T> _requests;
    private readonly int _maxBatchSize;

    public BackgroundGenerator(ILogger<BackgroundGenerator<T>> logger, ChannelReader<T> requests, int maxBatchSize)
    {
        _logger = logger;
        _requests = requests;
        _maxBatchSize = maxBatchSize;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(async () => await RunThread(cancellationToken));
    }

    public async Task RunThread(CancellationToken cancellationToken) {
        // Normal work, until the API is running
        _logger.LogInformation("Starting execution of background service...");

        while(await _requests.WaitToReadAsync(cancellationToken)) {
            _logger.LogInformation("Received data, creating a batch...");
            var batch = new List<T>();
            while(_requests.TryRead(out T item)){
                batch.Add(item);
                if (batch.Count == _maxBatchSize) break;
            }
            await ExecuteLogic(batch);
        }
    } 

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received stop signal, waiting for all batches to process...");
        // Either save the status of Queue in some file so when the service restarts if the work is important
        // Or just exit
        _logger.LogInformation("Stopping generator...");
        return Task.CompletedTask;
    }

    private async Task ExecuteLogic(List<T> batch) {
        foreach(var item in batch) {
            _logger.LogInformation($"Processing item [{item}]");
            await Task.Delay(1000);
        }
    }
}