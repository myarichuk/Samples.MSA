using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared;

namespace PickUpCounter
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Worker : IHostedService,  IConsumer<Order>
    {
        private readonly ILogger<Worker> _logger;
        private readonly MessageBus _messageBus;
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


        public Task Consume(ConsumeContext<Order> context)
        {
            if (context.Message.IsComplete)
            {
                _logger.LogInformation($"Announcement: Order of {context.Message.CustomerName} for {context.Message.Type.Name} is complete!");
            }
            return Task.CompletedTask;
        }
    }
}
