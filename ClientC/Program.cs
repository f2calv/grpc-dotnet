using CasCap.Services;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Threading.Tasks;
namespace CasCap;

class Program : ProgramBase
{
    static async Task Main()
    {
        await Run();
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    async static Task Run()
    {
        using var channel = GrpcChannel.ForAddress("https://localhost:5001");
        var client = new Markets.MarketsClient(channel);

        //var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));
        using var streamingCall = client.GetTickStream(new Empty()/*, cancellationToken: cts.Token*/);

        try
        {
            await foreach (var tick in streamingCall.ResponseStream.ReadAllAsync(/*cancellationToken: cts.Token*/))
                Display(tick);
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
        {
            Console.WriteLine("Stream cancelled.");
        }
    }
}