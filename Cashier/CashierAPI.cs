using System;
using System.Collections.Concurrent;
using System.Linq;
using Nancy;
using Nancy.ModelBinding;
using Shared;
// ReSharper disable VirtualMemberCallInConstructor
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace Cashier
{
    public class CashierAPI : NancyModule
    {
        public CashierAPI(BlockingCollection<Order> ordersQueue)
        {
            Get("/orders", _ => Response.AsJson(ordersQueue.ToList()));
            Post("/orders", args =>
            {
                try
                {
                    ordersQueue.Add(this.Bind<Order>());
                    return Response.AsJson(new { Message = "OK" });
                }
                catch (Exception e)
                {
                    return Response.AsJson(new { e.Message, Exception = e.ToString()},
                        HttpStatusCode.BadRequest);
                }
            });
        }
    }
}
