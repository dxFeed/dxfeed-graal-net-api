using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using DxFeed.Graal.Net.Api;
using DxFeed.Graal.Net.Events;
using DxFeed.Graal.Net.Events.Market;

namespace DxFeed.Graal.Net.Samples;

/// <summary>
/// This sample demonstrates a way to subscribe to the big world of symbols with dxFeed API, so that the events are
/// updated and cached in memory of this process, and then take snapshots of those events from memory whenever
/// they are needed. This example repeatedly reads symbol name from the console and prints a snapshot of its last
/// quote, trade, summary, and profile events.
/// </summary>
[SuppressMessage("ReSharper", "FunctionNeverReturns")]
public abstract class Program
{
    private static async Task Main()
    {
        // Permanent subscription to the world is performed with a special property named "dxfeed.qd.subscribe.ticker".
        // Its value consists of a comma-separated list of records, followed by a space, followed by a comma-separated
        // list of symbols. Record names for composite (NBBO) events are the same as the corresponding event classes
        // in API. The string below defines subscription for quote, trade, summary, and profile composite events:
        var records = "Quote,Trade,Summary,Profile";

        // Records for regional-exchange events are derived by appending "&" (ampersand) and the a single-digit
        // exchange code. Regexp-like syntax can be used instead of listing them all. For example, the commented
        // line below and to the mix a subscription on regional quotes from all potential exchange codes A-Z
        // var record  records = "Quote,Trade,Summary,Profile,Quote&[A-Z]";

        // There is an important trade-off between a resource consumption and speed of access to the last events.
        // The whole world of stocks and options from all the exchanges is very large and will consume gigabytes
        // of memory to cache. Moreover, this cache has to be constantly kept up-to-date which consumes a lot of
        // network and CPU.
        //
        // A particular application's use cases have to be studied to figure out what is optimal for this particular
        // application. If some events are known to be rarely needed and a small delay while accessing them can be
        // tolerated, then it is not worth configuring a permanent subscription for them. The code in this
        // sample works using DXFeed.GetLastEventAsync method that will request the event from the upstream data provider
        // if it is not present in the local in-memory cache.

        // There are multiple ways to specify a list of symbols. It is typically taken from an IPF file and its
        // specification consists of a URL to the file which has to contain ".ipf" in order to be recognized.
        // The string below defines subscription for all symbols that are available on the demo feed.
        var symbols = "http://dxfeed.s3.amazonaws.com/masterdata/ipf/demo/mux-demo.ipf.zip";

        // The permanent subscription property "dxfeed.qd.subscribe.ticker" can be placed directly into the
        // "dxfeed.properties" file, eliminating the need for a custom DXEndpoint instance. In this example,
        // it is explicitly specified using the DXFeedEndpoint.Builder class. Note that the "connect" method
        // is not used on DXEndpoint. This sample assumes that the "dxfeed.address" property is specified in the builder,
        // establishing the connection automatically. Alternatively, "dxfeed.address" can also
        // be specified in the "dxfeed.properties" file.
        var endpoint = DXEndpoint.NewBuilder()
            .WithProperty("dxfeed.address", "demo.dxfeed.com:7300")
            .WithProperty("dxfeed.qd.subscribe.ticker", $"{records} {symbols}")
            .Build();

        // The actual client code does not need a reference to DXEndpoint, which only contains lifecycle
        // methods like "connect" and "close". The client code needs a reference to DXFeed.
        var feed = endpoint.GetFeed();

        // Print a short help.
        Console.WriteLine("Type symbols to get their quote, trade, summary, and profile event snapshots");

        // The main loop of this sample loops forever reading symbols from console and printing events.
        while (true)
        {
            // User of this sample application can type symbols on the console. Symbol like "IBM" corresponds
            // to the stock. Symbol like "IBM&N" corresponds to the information from a specific exchange.
            // See the dxFeed Symbol guide at http://www.dxfeed.com/downloads/documentation/dxFeed_Symbol_Guide.pdf
            var symbol = Console.ReadLine();
            if (symbol == null)
            {
                continue;
            }

            // The first step is to extract tasks for all events that we are interested in. This way we
            // can get an event even if we have not previously subscribed for it.
            var quoteTask = feed.GetLastEventAsync<Quote>(symbol).ContinueWith(task => task.Result as ILastingEvent);
            var tradeTask = feed.GetLastEventAsync<Trade>(symbol).ContinueWith(task => task.Result as ILastingEvent);
            var summaryTask = feed.GetLastEventAsync<Summary>(symbol).ContinueWith(task => task.Result as ILastingEvent);

            // All tasks are put into a list for convenience.
            var tasks = new List<Task<ILastingEvent>> { quoteTask, tradeTask, summaryTask };

            // Profile events are composite-only. They are not available for regional symbols like
            // "IBM&N" and the attempt to retrieve them never completes (will timeout), so we don't even try.
            if (!MarketEventSymbols.HasExchangeCode(symbol))
            {
                var profileTask = feed.GetLastEventAsync<Profile>(symbol)
                    .ContinueWith(task => task.Result as ILastingEvent);
                tasks.Add(profileTask);
            }

            // If the events are available in the in-memory cache, then the tasks will be completed immediately.
            // Otherwise, a request to the upstream data provider is sent. Below we combine tasks using
            // Task.WhenAll in order to wait for at most 1 second for all the tasks to complete.
            // This sample prints a special message in the case of timeout.
            var timeout = Task.Delay(TimeSpan.FromSeconds(1));
            var completedTask = await Task.WhenAny(Task.WhenAll(tasks), timeout);
            if (completedTask == timeout)
            {
                Console.WriteLine("Request timed out");
            }

            // The combination above is used only to ensure a common wait of 1 second. Tasks for individual events
            // are completed independently and the corresponding events can be accessed even if some events were not
            // available for any reason and the wait above had timed out. This sample just prints all completed tasks.
            foreach (var task in tasks)
            {
                if (task.IsCompleted)
                {
                    Console.WriteLine(task.Result);
                }
            }
        }
    }
}
