using System.Collections.Concurrent;
using System.Threading.Tasks;
using MassTransit.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nancy.Hosting.Kestrel;
using Shared;
// ReSharper disable InconsistentNaming

namespace Cashier
{
    public static class Program
    {
        private static readonly BlockingCollection<Order> _ordersQueue = new BlockingCollection<Order>();
        public static void Main(string[] args)
        {
            Task.WaitAll(CreateHostBuilder(args).Build().RunAsync(),
                         CreateWebHostBuilder().Build().RunAsync());

        }

        //host the API
        public static IWebHostBuilder CreateWebHostBuilder() =>
            new WebHostBuilder()
                .UseKestrel()
                .UseStartup<Startup>();

        //host the micro-service
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    var configuration = hostContext.Configuration;
                    var settings = configuration.GetSection("Transport")
                                                .Get<TransportSettings>();
                    services.AddSingleton(_ordersQueue);
                    services.AddSingleton(settings);
                    services.AddHostedService<Worker>();
                });

      
        internal class Startup
        {
            public void Configure(IApplicationBuilder app) => app.UseNancy(opt => 
            {
                opt.Bootstrapper = new CashierNancyBootstrapper(_ordersQueue);
            });
        }
    }

}
