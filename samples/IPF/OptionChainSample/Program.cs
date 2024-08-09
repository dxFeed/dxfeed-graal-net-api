using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DxFeed.Graal.Net.Api;
using DxFeed.Graal.Net.Events.Market;
using DxFeed.Graal.Net.Ipf;
using DxFeed.Graal.Net.Ipf.Options;

namespace DxFeed.Graal.Net.Samples;

/// <summary>
/// A simple sample that shows how to build option chains, and prints quotes for nearby option strikes.
/// </summary>
[SuppressMessage("ReSharper", "MethodSupportsCancellation")]
internal abstract class Program
{
    public static async Task Main(string[] args)
    {
        if (args.Length != 5)
        {
            Console.WriteLine("usage: <address> <ipf-file> <symbol> <nStrikes> <nMonths>");
            Console.WriteLine("       <address>  is endpoint address");
            Console.WriteLine("       <ipf-file> is name of instrument profiles file");
            Console.WriteLine("       <symbol>   is the product or underlying symbol");
            Console.WriteLine("       <nStrikes> number of strikes to print for each series");
            Console.WriteLine("       <nMonths>  number of months to print");
            return;
        }

        var argAddress = args[0];
        var argIpfFile = args[1];
        var argSymbol = args[2];
        var nStrikes = int.Parse(args[3], CultureInfo.InvariantCulture);
        var nMonths = int.Parse(args[4], CultureInfo.InvariantCulture);

        var feed = DXEndpoint.Create().Connect(argAddress).GetFeed();

        // Subscribe to trade to learn instrument last price.
        Console.WriteLine($"Waiting for price of {argSymbol} ...");
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(1));
        var trade = await feed.GetLastEventAsync<Trade>(argSymbol, cts.Token);

        var price = trade.Price;
        Console.WriteLine($"Price of {argSymbol} is {price.ToString(CultureInfo.InvariantCulture)}");

        Console.WriteLine($"Reading instruments from {argIpfFile} ...");
        var instruments = new InstrumentProfileReader().ReadFromFile(argIpfFile).ToList();

        Console.WriteLine("Building option chains ...");
        var chains = OptionChainsBuilder<InstrumentProfile>.Build(instruments).Chains;
        if (!chains.TryGetValue(argSymbol, out var chain))
        {
            Console.WriteLine($"No chain found for symbol {argSymbol}");
            return;
        }

        nMonths = Math.Min(nMonths, chain.GetSeries().Count);
        var seriesList = chain.GetSeries().Take(nMonths).ToList();

        Console.WriteLine("Requesting option quotes ...");
        var quotes = new Dictionary<InstrumentProfile, Task<Quote>>();
        foreach (var series in seriesList)
        {
            var strikes = series.GetNStrikesAround(nStrikes, price);
            foreach (var strike in strikes)
            {
                if (series.Calls.TryGetValue(strike, out var call))
                {
                    quotes[call] = feed.GetLastEventAsync<Quote>(call.Symbol);
                }

                if (series.Puts.TryGetValue(strike, out var put))
                {
                    quotes[put] = feed.GetLastEventAsync<Quote>(put.Symbol);
                }
            }
        }

        // Ignore timeout and continue to print retrieved quotes even on timeout.
        await Task.WhenAny(Task.WhenAll(quotes.Values), Task.Delay(TimeSpan.FromSeconds(1)));

        Console.WriteLine("Printing option series ...");
        foreach (var series in seriesList)
        {
            Console.WriteLine($"Option series {series}");
            var strikes = series.GetNStrikesAround(nStrikes, price);
            Console.WriteLine($"{"C.BID",10} {"C.ASK",10} {"STRIKE",10} {"P.BID",10} {"P.ASK",10}");
            foreach (var strike in strikes)
            {
                series.Calls.TryGetValue(strike, out var call);
                series.Puts.TryGetValue(strike, out var put);
                var callQuote = new Quote();
                if (call != null && quotes.TryGetValue(call, out var callTask) && callTask.IsCompleted)
                {
                    callQuote = callTask.Result;
                }
                var putQuote = new Quote();
                if (put != null && quotes.TryGetValue(put, out var putTask) && putTask.IsCompleted)
                {
                    putQuote = putTask.Result;
                }
                Console.WriteLine(
                    $"{callQuote.BidPrice,10:F3} {callQuote.AskPrice,10:F3} {strike,10:F3} {putQuote.BidPrice,10:F3} {putQuote.AskPrice,10:F3}");
            }
        }
    }
}
