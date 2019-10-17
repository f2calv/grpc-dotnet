using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace CasCap.Services
{
    public class MarketsService : Markets.MarketsBase
    {
        readonly ILogger<MarketsService> _logger;

        public MarketsService(ILogger<MarketsService> logger) => _logger = logger;

        public override async Task GetTickStream(Empty _, IServerStreamWriter<TickResponse> responseStream, ServerCallContext context)
        {
            await Task.Delay(0);
            _logger.LogInformation(nameof(GetTickStream));

            var r = new Random();
            var utcNow = DateTime.UtcNow;

            var i = 0;
            while (!context.CancellationToken.IsCancellationRequested && i < 20)
            {
                //generate random time delay
                await Task.Delay(r.Next(0, 5) * 100);

                //pick out a random stock
                var stockIndex = r.Next(0, stocks.Count);
                var stock = stocks[stockIndex];

                //generate random stock price change
                //https://stackoverflow.com/questions/3975290/produce-a-random-number-in-a-range-using-c-sharp
                var rDiff = Math.Round((r.NextDouble() * 2) - 1.0, 1);

                //update stock prices
                stock.bid += rDiff;
                stock.offer += rDiff;
                stock.date = DateTime.UtcNow;

                var tick = new TickResponse
                {
                    Bid = stock.bid,
                    Offer = stock.offer,
                    Ticker = stock.ticker,
                    UtcNow = stock.date.ToTimestamp(),
                };

                _logger.LogInformation("Sending TickResponse response");

                await responseStream.WriteAsync(tick);
            }
        }

        public List<Stock> stocks
        {
            get
            {
                var s = new List<Stock>();
                s.Add(new Stock("msft") { bid = 130, offer = 131 });
                s.Add(new Stock("appl") { bid = 77, offer = 78 });
                s.Add(new Stock("tsco") { bid = 45, offer = 46 });
                return s;
            }
        }

        public class Stock
        {
            public Stock(string _ticker)
            {
                ticker = _ticker;
            }
            public string ticker { get; }
            public double bid { get; set; }
            public double offer { get; set; }
            public DateTime date { get; set; }
        }
    }
}