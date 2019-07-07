using CasCap.Service;
using Grpc.Net.Client;
using System;
using System.Net.Http;
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
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            using var httpClient = new HttpClient();
            // The port number(50051) must match the port of the gRPC server.
            httpClient.BaseAddress = new Uri("http://localhost:50051");
            var client = GrpcClient.Create<Greeter.GreeterClient>(httpClient);
            for (var i = 0; i < 10; i++)
            {
                var reply = await client.SayHelloAsync(new HelloRequest { Name = "GreeterClient" });
                Console.WriteLine("Greeting: " + reply.Message);
                await Task.Delay(1_000);
            }
        }
    }
}