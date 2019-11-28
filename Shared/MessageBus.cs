using MassTransit;
using System;
using System.Threading;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit.RabbitMqTransport;

namespace Shared
{
    public class MessageBus : IDisposable
    {
        private readonly IBusControl _busControl;
        private Task<BusHandle> _startupTask;

        public MessageBus(
            Uri rabbitMqHost, 
            Action<IRabbitMqHostConfigurator> hostConfigurator,
            Action<IReceiveEndpointConfigurator> endpointConfigurator = null,
            string queueName = null)
        {
            _busControl = Bus.Factory.CreateUsingRabbitMq(
                cfg =>
                {
                    cfg.Host(rabbitMqHost, hostConfigurator);
                    cfg.UseMessageRetry(rc => rc.Incremental(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(15)));

                    if (endpointConfigurator != null)
                        cfg.ReceiveEndpoint(queueName ?? Guid.NewGuid().ToString(), endpointConfigurator);
                });

            _startupTask = _busControl.StartAsync();
        }

        public async Task Publish<TMessage>(TMessage message, CancellationToken token = default)
            where TMessage : class
        {
            await _startupTask; //make sure the bus is started
            await _busControl.Publish(message, token);
        }

        public Task StopAsync(CancellationToken token = default) => _busControl.StopAsync(token);
        public void Dispose() => _busControl.Stop();
    }
}
