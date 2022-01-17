using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
namespace CasCap.Services;

public class MarketsService : Markets.MarketsBase
{
    readonly ILogger<MarketsService> _logger;
    readonly IPriceGeneratorService _generatorSvc;

    public MarketsService(ILogger<MarketsService> logger, IPriceGeneratorService generatorSvc)
    {
        _logger = logger;
        _generatorSvc = generatorSvc;
    }

    public override async Task GetTickStream(Empty _, IServerStreamWriter<TickResponse> responseStream, ServerCallContext context)
    {
        _logger.LogInformation(nameof(GetTickStream));
        var count = 1;
        while (!context.CancellationToken.IsCancellationRequested)
        {
            var cts = new CancellationTokenSource(500);
            await foreach (var price in _generatorSvc.GetPricesAsync(cancellationToken: cts.Token))
            {
                if (count % 10 == 0)
                    _logger.LogInformation("streaming {objectName} #{count}", nameof(TickResponse), count);
                await responseStream.WriteAsync(new TickResponse(price));
                count++;
            }
        }
    }

    public override Task<TicksResponse> GetAllTicks(Empty _, ServerCallContext context)
    {
        _logger.LogInformation(nameof(GetAllTicks));
        var ticks = new List<TickResponse>();
        //await foreach (var tick in _generatorSvc.ticksChannel.Reader.ReadAllAsync())//no good because the channel never actually completes...
        //while (_generatorSvc.tickQueue.TryDequeue(out var tick))
        //    ticks.Add(tick);
        var res = new TicksResponse
        {
            Ticks = { _generatorSvc.priceBacklog.OrderByDescending(p => p.date).Select(p => new TickResponse(p)).ToList() }
        };
        return Task.FromResult(res);
    }

    //public override async Task GetAllTicksStream(Empty _, IServerStreamWriter<TickResponse> responseStream, ServerCallContext context)
    //{
    //    _logger.LogInformation(nameof(GetTickStream));
    //    var count = 1;
    //    while (!context.CancellationToken.IsCancellationRequested)
    //    {
    //        await foreach (var tick in _generatorSvc.ticksChannel.Reader.ReadAllAsync())
    //        {
    //            if (count % 10 == 0)
    //                _logger.LogInformation($"streaming {nameof(TickResponse)} #{count}");
    //            await responseStream.WriteAsync(tick);
    //            count++;
    //        }
    //    }
    //}
}

public partial class TickResponse
{
    public TickResponse(StockPrice price)
    {
        Bid = price.bid;
        Offer = price.offer;
        Symbol = price.symbol;
        Date = price.date.ToTimestamp();
    }
}