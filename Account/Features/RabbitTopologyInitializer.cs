using RabbitMQ.Client;

namespace AccountServices.Features
{
    public static class RabbitTopologyInitializer
    {
        public static void Initialize(IModel channel)
        {
            channel.ExchangeDeclare("account.events", ExchangeType.Topic, durable: true);

            channel.QueueDeclare("account.crm", durable: true, exclusive: false, autoDelete: false);
            channel.QueueDeclare("account.notifications", durable: true, exclusive: false, autoDelete: false);
            channel.QueueDeclare("account.antifraud", durable: true, exclusive: false, autoDelete: false);
            channel.QueueDeclare("account.audit", durable: true, exclusive: false, autoDelete: false);

            channel.QueueBind("account.crm", "account.events", "account.*");
            channel.QueueBind("account.notifications", "account.events", "money.*");
            channel.QueueBind("account.antifraud", "account.events", "client.*");
            channel.QueueBind("account.audit", "account.events", "#");
        }

        public static void InitializeTopology(string hostName = "rabbitmq")
        {
            var factory = new ConnectionFactory { HostName = hostName };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            Initialize(channel);
        }
    }

}
