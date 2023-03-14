using EventSourcing.Application;
using EventSourcing.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NATS.Client;


IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        // var configurationBuilder = new ConfigurationBuilder();
        // var config = configurationBuilder.AddJsonFile("appsettings.json", true, true).AddEnvironmentVariables().Build();
        var natsConnectionString = hostContext.Configuration.GetConnectionString("NATS");
        services.AddHostedService<Worker>();
        services.AddHostedService<MyHostedService>();
        var connectionFactory = new ConnectionFactory();
        var connection = connectionFactory.CreateConnection(natsConnectionString);
        services.AddSingleton(connection);
    })
    .Build();
host.Run();
Console.WriteLine("RUNNING");

