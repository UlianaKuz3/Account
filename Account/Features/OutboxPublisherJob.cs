using Hangfire;
using RabbitMQ.Client;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace AccountServices.Features
{
    public class OutboxPublisherJob
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<OutboxPublisherJob> _logger;
        private readonly IConnection _rabbitConnection;

        public OutboxPublisherJob(IServiceScopeFactory scopeFactory, ILogger<OutboxPublisherJob> logger, IConnection rabbitConnection)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _rabbitConnection = rabbitConnection;
        }

        [AutomaticRetry(Attempts = 5)]
        public async Task PublishOutboxMessages()
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var messages = await db.Outbox
                .Where(x => !x.Processed)
                .OrderBy(x => x.CreatedAt)
                .Take(50)
                .ToListAsync();

            using var channel = _rabbitConnection.CreateModel();
            channel.ExchangeDeclare("account.events", ExchangeType.Topic, durable: true);

            foreach (var msg in messages)
            {
                try
                {
                    var body = Encoding.UTF8.GetBytes(msg.Payload);
                    channel.BasicPublish("account.events", msg.RoutingKey, null, body);

                    msg.Processed = true;

                    _logger.LogInformation("Outbox message {Id} published", msg.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error publishing message {Id}", msg.Id);
                }
            }

            await db.SaveChangesAsync();
        }
    }

}
