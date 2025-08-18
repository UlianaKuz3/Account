using Hangfire;
using RabbitMQ.Client;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace AccountServices.Features
{
    public class OutboxPublisherJob(IServiceScopeFactory scopeFactory, ILogger<OutboxPublisherJob> logger, IConnection rabbitConnection)
    {

        [AutomaticRetry(Attempts = 5)]
        public async Task PublishOutboxMessages()
        {
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var messages = await db.Outbox
                .Where(x => !x.Processed)
                .OrderBy(x => x.CreatedAt)
                .Take(50)
                .ToListAsync();

            using var channel = rabbitConnection.CreateModel();
            channel.ExchangeDeclare("account.events", ExchangeType.Topic, durable: true);

            foreach (var msg in messages)
            {
                try
                {
                    var body = Encoding.UTF8.GetBytes(msg.Payload ?? throw new ArgumentNullException(nameof(msg.Payload)));
                    channel.BasicPublish("account.events", msg.RoutingKey, null, body);

                    msg.Processed = true;

                    logger.LogInformation("Outbox message {Id} published", msg.Id);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error publishing message {Id}", msg.Id);
                }
            }

            await db.SaveChangesAsync();
        }
    }

}
