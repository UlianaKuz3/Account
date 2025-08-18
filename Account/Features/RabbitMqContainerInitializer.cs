using RabbitMQ.Client;
using Testcontainers.RabbitMq;

namespace AccountServices.Features
{
    // ReSharper disable once UnusedMember.Global
    public class RabbitMqContainerInitializer(RabbitMqContainer container) : IHostedService
    {

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await container.StartAsync(cancellationToken);

            using var connection = new ConnectionFactory
            {
                HostName = container.Hostname,
                Port = container.GetMappedPublicPort(5672),
                UserName = "user",
                Password = "password"
            }.CreateConnection();

            using var channel = connection.CreateModel();
            channel.QueueDeclare("test-queue", durable: false, exclusive: false, autoDelete: false);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await container.DisposeAsync();
        }
    }
}
