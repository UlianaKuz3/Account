using RabbitMQ.Client;
using Testcontainers.RabbitMq;

namespace AccountServices.Features
{
    public class RabbitMqContainerInitializer : IHostedService
    {
        private readonly RabbitMqContainer _container;

        public RabbitMqContainerInitializer(RabbitMqContainer container)
        {
            _container = container;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _container.StartAsync(cancellationToken);

            using var connection = new ConnectionFactory
            {
                HostName = _container.Hostname,
                Port = _container.GetMappedPublicPort(5672),
                UserName = "user",
                Password = "password"
            }.CreateConnection();

            using var channel = connection.CreateModel();
            channel.QueueDeclare("test-queue", durable: false, exclusive: false, autoDelete: false);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _container.DisposeAsync();
        }
    }
}
