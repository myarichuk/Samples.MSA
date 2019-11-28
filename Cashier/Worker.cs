using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared;

namespace Cashier
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly BlockingCollection<Order> _orderQueue;
        private readonly MessageBus _messageBus;
        public Worker(ILogger<Worker> logger, TransportSettings settings, BlockingCollection<Order> orderQueue)
        {
            _logger = logger; //TODO: logging is omitted for clarity, but is obviously needed
            _orderQueue = orderQueue;
            
            //cashier will only be sending messages, so no need for "receiving endpoint" config
            _messageBus = new MessageBus(new Uri(settings.RabbitMqUrl), 
                hostCfg =>
                {
                });
        }

        public override Task StopAsync(CancellationToken cancellationToken) => _messageBus.StopAsync(cancellationToken);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
                if (_orderQueue.TryTake(out var order, 512))
                {
                    _logger.LogInformation($"Received order from {order.CustomerName}, drink ordered: {order.Type.Name}, received money: {order.Type.Price}");
                    order.WhenReceived = DateTime.UtcNow;
                    await _messageBus.Publish(order, stoppingToken);
                }
                else
                    await Task.Delay(1000, stoppingToken);
                    
        }
    }
}
