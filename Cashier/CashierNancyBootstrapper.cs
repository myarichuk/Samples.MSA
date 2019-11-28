using System.Collections.Concurrent;
using Nancy;
using Nancy.TinyIoc;
using Shared;

namespace Cashier
{
    public class CashierNancyBootstrapper : DefaultNancyBootstrapper
    {
        private readonly BlockingCollection<Order> _ordersQueue;

        public CashierNancyBootstrapper(BlockingCollection<Order> ordersQueue)
        {
            _ordersQueue = ordersQueue;
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            container.Register(_ordersQueue);
            base.ConfigureApplicationContainer(container);
        }
    }
}
