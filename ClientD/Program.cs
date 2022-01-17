using CasCap.Services;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using System;
using System.Linq;
using System.Threading.Tasks;
namespace CasCap
{
    class Program : ProgramBase
    {
        static async Task Main()
        {
            await Task.Delay(2_000);
            while (true)
            {                
                await Run();
                await Task.Delay(2_000);
                Console.Clear();
            }
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        async static Task Run()
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Markets.MarketsClient(channel);

            //var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));
            var ticks = await client.GetAllTicksAsync(new Empty()/*, cancellationToken: cts.Token*/);

            if (ticks.Ticks.Count > 0)
            {
                foreach (var tick in ticks.Ticks)
                    Display(tick);
                Console.WriteLine($"Count={ticks.Ticks.Count}, Min={ticks.Ticks.Min(p => p.Date):HH:mm:ss}, Max={ticks.Ticks.Max(p => p.Date):HH:mm:ss}");
            }
        }
    }
}