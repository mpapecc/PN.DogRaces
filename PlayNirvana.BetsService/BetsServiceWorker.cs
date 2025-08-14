using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using PlayNirvana.Bll.Services;

namespace PlayNirvana.BetsService
{
    public class BetsServiceWorker : BackgroundService
    {
        private readonly ILogger<BetsServiceWorker> _logger;
        private IConnection? _connection;
        private IChannel _channel;
        private readonly IServiceScopeFactory serviceScopeFactory;

        public BetsServiceWorker(ILogger<BetsServiceWorker> logger,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost"
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.QueueDeclareAsync(queue: "bets-processor",
                                  durable: true,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);

            await base.StartAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                using var scope = serviceScopeFactory.CreateScope();
                var betService = scope.ServiceProvider.GetRequiredService<BetService>();

                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                _logger.LogInformation("Received message: {Message}", message);

                var ids = JsonSerializer.Deserialize<int[]>(message);

                if(ids is not null)
                    betService.ProcessRoundBets(ids);

                // Acknowledge message so it’s removed from queue
                await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
                _logger.LogInformation("Ack message: {Message}", message);
            };

            _channel.BasicConsumeAsync(queue: "bets-processor",
                                  autoAck: false,
                                  consumer: consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel?.CloseAsync();
            _connection?.CloseAsync();
            base.Dispose();
        }
    }
}
