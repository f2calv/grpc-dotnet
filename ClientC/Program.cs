using CasCap.Services;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Threading;
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
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Markets.MarketsClient(channel);

            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));
            using var streamingCall = client.GetTickStream(new Empty(), cancellationToken: cts.Token);

            try
            {
                await foreach (var tickData in streamingCall.ResponseStream.ReadAllAsync(cancellationToken: cts.Token))
                {
                    Console.WriteLine($"{tickData.UtcNow.ToDateTime():s}\t| {tickData.Ticker}\t| {tickData.Bid:0.00}/{tickData.Offer:0.00}");
                }
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
            {
                Console.WriteLine("Stream cancelled.");
            }
        }
    }
}