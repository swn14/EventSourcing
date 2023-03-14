using NATS.Client;

namespace EventSourcing.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConnection _natsConnection;

    public Worker(ILogger<Worker> logger, IConnection natsConnection)
    {
        _logger = logger;
        _natsConnection = natsConnection;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        var mySubscription = _natsConnection.SubscribeAsync(
            "my.subscription",
            "my.queue",
            (_, args) =>
            {
                _logger.LogInformation($"Subscriber listening for subject {args.Message.Subject} hit!");
                _natsConnection.Flush();
            }
        );

        if (stoppingToken.IsCancellationRequested)
        {
            await mySubscription.DrainAsync();
            _natsConnection.Close();
        }


        while (!stoppingToken.IsCancellationRequested)
        {
            // _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }
    }
}
