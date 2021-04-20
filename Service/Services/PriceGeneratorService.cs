using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
namespace CasCap.Services
{
    /// <summary>
    /// Generates randomised stock price movements.
    /// </summary>
    public interface IPriceGeneratorService
    {
        FixedSizedQueue<StockPrice> priceBacklog { get; set; }

        IAsyncEnumerable<StockPrice> GetPricesAsync(CancellationToken cancellationToken = default);
    }

    public class PriceGeneratorService : IPriceGeneratorService
    {
        readonly ILogger _logger;

        public PriceGeneratorService(ILogger<PriceGeneratorService> logger) => _logger = logger;

        List<StockPrice> GetPrices
        {
            get
            {
                var r = new Random();
                var limit = 1_000;
                var l = new List<StockPrice>(limit);
                for (var i = 0; i < limit; i++)
                    l.Add(GetStockPrice(r));
                return l;
            }
        }

        Channel<StockPrice> pricesChannel { get; set; } = Channel.CreateBounded<StockPrice>(
            new BoundedChannelOptions(1024)
            {
                SingleWriter = true,
                FullMode = BoundedChannelFullMode.DropOldest,
            });

        ConcurrentQueue<StockPrice> priceQueue { get; set; } = new ConcurrentQueue<StockPrice>();

        public FixedSizedQueue<StockPrice> priceBacklog { get; set; } = new FixedSizedQueue<StockPrice>(30);

        public async IAsyncEnumerable<StockPrice> GetPricesAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var r = new Random();
            _logger.LogInformation("starting {methodName}", nameof(GetPricesAsync));
            //cancellationToken.ThrowIfCancellationRequested();
            while (!cancellationToken.IsCancellationRequested)
            {
                //generate random time delay, a simulated price gap
                await Task.Delay(r.Next(0, 5) * 100, CancellationToken.None);
                var price = GetStockPrice(r);
                //lets store a limited backlog of data
                pricesChannel.Writer.TryWrite(price);
                priceQueue.Enqueue(price);
                priceBacklog.Enqueue(price);
                yield return price;
            }
        }

        StockPrice GetStockPrice(Random r)
        {
            //pick out a random stock
            var stockIndex = r.Next(0, stocks.Count);
            var stock = stocks[stockIndex];

            //generate random price change
            //https://stackoverflow.com/questions/3975290/produce-a-random-number-in-a-range-using-c-sharp
            var rDiff = Math.Round((r.NextDouble() * 2) - 1.0, 1);

            //update stock prices
            stock.bid += rDiff;
            stock.offer += rDiff;
            stock.date = DateTime.UtcNow;

            return stock;
        }

        static List<StockPrice> stocks
        {
            get
            {
                //initialise stocks with starting prices
                var s = new List<StockPrice>
                {
                    new StockPrice("MSFT") { bid = 250, offer = 250.5 },
                    new StockPrice("APPL") { bid = 110, offer = 110.5 },
                    new StockPrice("TSLA") { bid = 597.5, offer = 598 },
                    new StockPrice("GOOG") { bid = 2294.5, offer = 2295 },
                    new StockPrice("HOG") { bid = 46, offer = 46.5 },
                };
                return s;
            }
        }
    }

    public class StockPrice
    {
        public StockPrice(string symbol)
        {
            this.symbol = symbol;
        }
        public string symbol { get; }
        public double bid { get; set; }
        public double offer { get; set; }
        public DateTime date { get; set; }
    }
}