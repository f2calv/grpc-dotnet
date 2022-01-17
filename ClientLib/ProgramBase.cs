using CasCap.Services;
using System;
using System.Collections.Generic;
namespace CasCap;

public abstract class ProgramBase
{
    static Dictionary<string, TickResponse> markets { get; set; } = new Dictionary<string, TickResponse>();

    static void WriteLine(string str, ConsoleColor colour)
    {
        var current = Console.ForegroundColor;
        Console.ForegroundColor = colour;
        Console.WriteLine(str);
        Console.ForegroundColor = current;
    }

    protected static void Display(TickResponse tick)
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
