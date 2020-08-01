using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using MassTransit;
using Session04.MassTest.Contracts;
using Session04.MassTest.Components.Consumers;
namespace Session04.MassTest.ComponentConsumer
{
    class Program
    {
        public static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                sbc.Host("rabbitmq://localhost");

                sbc.ReceiveEndpoint("test_queue", ep =>
                {
                    ep.Handler<OrderRegistered>(context =>
                    {
                        var res = new OrderRegisteredHandler().Consume(context);
                        return res;
                        //return Console.Out.WriteLineAsync($"Received: {context.Message.Text}");
                    });
                });
            });

            await bus.StartAsync();

            Console.WriteLine("Press any key to exit");
            await Task.Run(() => Console.ReadKey());

            await bus.StopAsync();
        }
    }
}
