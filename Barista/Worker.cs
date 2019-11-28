using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared;

namespace Barista
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class Worker : IHostedService, IDisposable,  IConsumer<Order>
    {
        private readonly ILogger<Worker> _logger;
        private readonly MessageBus _messageBus;
        private readonly Random _random = new Random();

        public Worker(ILogger<Worker> logger, TransportSettings settings)
        {
            _logger = logger;
            _messageBus = new MessageBus(new Uri(settings.RabbitMqUrl), 
                hostCfg =>
                {
                },
                endpointCfg => endpointCfg.Instance(this),
                settings.QueueName);
        }

        public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;
        public Task StopAsync(CancellationToken cancellationToken) => _messageBus.StopAsync(cancellationToken);

        public void Dispose() => _messageBus.Dispose();

        public async Task Consume(ConsumeContext<Order> context)
        {
            if (!context.Message.IsComplete)
            {
                _logger.LogInformation($"Received order from '{context.Message.CustomerName}' for '{context.Message.Type.Name}', started working on it");
                await Task.Delay(_random.Next(500, 3000));
                context.Message.WhenCompleted = DateTime.UtcNow;
                await _messageBus.Publish(context.Message, CancellationToken.None);
            }

        }
    }
}
