using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using RestSharp;
using Shared;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new RestClient("http://localhost:5000"); //the default address of Cashier node API

            var mre = new ManualResetEventSlim();
            var random = new Random();
            var drinkTypes = Builder<BeverageType>.CreateListOfSize(10)
                .All()
                    .With(x => x.Name = Pick<string>.RandomItemFrom(
                        new []
                        {
                            "Frappuccino", 
                            "Americano", 
                            "Mocha",
                            "Iced Tea",
                            "Latte",
                            "Machiato",
                            "Hot Chocolate"
                        }))
                    .With(x => x.Price = random.Next(2,12))
                .Build();

            var orders = 
                Builder<Order>.CreateListOfSize(100)
                    .All()
                        .With(x => x.CustomerName = Faker.Name.First())
                        .With(x => x.Type = Pick<BeverageType>.RandomItemFrom(drinkTypes))
                    .Build();

            Task.Run(() =>
            {
                while (!mre.IsSet)
                {
                    var request = new RestRequest("/orders", Method.POST);
                    request.AddJsonBody(Pick<Order>.RandomItemFrom(orders));

                    client.Execute(request);

                    Thread.Sleep(random.Next(100,1500));
                }
            });

            Console.ReadKey();
            mre.Set();
        }
    }
}
