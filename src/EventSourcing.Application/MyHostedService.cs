using Microsoft.Extensions.Hosting;
using NATS.Client;

namespace EventSourcing.Application;

internal class MyHostedService : IHostedService
{
    private readonly IHostApplicationLifetime _appLifetime;
    private readonly IConnection _natsConnection;

    public MyHostedService(IHostApplicationLifetime appLifetime, IConnection natsConnection)
    {
        _appLifetime = appLifetime;
        _natsConnection = natsConnection;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(1000); //your scheduled work
        var messageData = System.Text.Encoding.UTF8.GetBytes("Hello, subscriber!");
        var message = new Msg("my.subscription", messageData);
        _natsConnection.Publish(message);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
