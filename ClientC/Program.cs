using CasCap.Services;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
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
                await foreach (var tick in streamingCall.ResponseStream.ReadAllAsync(cancellationToken: cts.Token))
                {
                    if (markets.TryGetValue(tick.Symbol, out var market))
                    {
                        var colour = market.Bid > tick.Bid ? ConsoleColor.Red : ConsoleColor.Green;
                        WriteLine($"{tick.Date.ToDateTime():HH:mm:ss.fff}\t| {tick.Symbol}\t| {tick.Bid:0.00}/{tick.Offer:0.00}", colour);
                    }
                    else
                    {
                        Console.WriteLine($"new market detected '{tick.Symbol}");
                        markets.Add(tick.Symbol, tick);
                    }
                }
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
            {
                Console.WriteLine("Stream cancelled.");
            }
        }

        static Dictionary<string, TickResponse> markets { get; set; } = new Dictionary<string, TickResponse>();

        static void WriteLine(string str, ConsoleColor colour)
        {
            var current = Console.ForegroundColor;
            Console.ForegroundColor = colour;
            Console.WriteLine(str);
            Console.ForegroundColor = current;
        }
    }
}