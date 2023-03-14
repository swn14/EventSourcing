using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
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
        // DynamoDb
        var clientConfig = new AmazonDynamoDBConfig
        {
            ServiceURL = hostContext.Configuration.GetConnectionString("DYNAMO")
        };
        // var dynamoConfig = hostContext.Configuration.GetSection(nameof(DynamoConfig))
        //     .Get<DynamoConfig>();
        var dynamoDbClient = new AmazonDynamoDBClient(clientConfig);
        var dynamoDbContext = new DynamoDBContext(
            dynamoDbClient,
            new DynamoDBContextConfig
            {
                TableNamePrefix = "test_",
                Conversion = DynamoDBEntryConversion.V2
            }
        );

        // var dynamoSetup = new DynamoSetup(dynamoDbClient, dynamoConfig, dynamoDbContext, environment);

        services.AddSingleton<IAmazonDynamoDB>(dynamoDbClient);
        services.AddSingleton<IDynamoDBContext>(dynamoDbContext);
        // services.AddSingleton<IDynamoSetup>(dynamoSetup);

    })
    .Build();
host.Run();
Console.WriteLine("RUNNING");

