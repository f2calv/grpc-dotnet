using CasCap.Service;
using Grpc.Net.Client;
using System;
using System.Threading.Tasks;
namespace CasCap
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Run();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        async static Task Run()
        {
            //AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            // The port number(5001) must match the port of the gRPC server.
            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Greeter.GreeterClient(channel);
            for (var i = 0; i < 10; i++)
            {
                var reply = await client.SayHelloAsync(new HelloRequest { Name = "GreeterClient" });
                Console.WriteLine("Greeting: " + reply.Message);
                await Task.Delay(1_000);
            }
        }
    }
}