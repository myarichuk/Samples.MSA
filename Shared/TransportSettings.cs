// ReSharper disable ClassNeverInstantiated.Global
namespace Shared
{
    public class TransportSettings
    {
        public string RabbitMqUrl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string QueueName { get; set; }
    }
}
