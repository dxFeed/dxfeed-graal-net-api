using System;
using System.Threading;
using System.Threading.Tasks;
using DxFeed.Graal.Net.Api;
using DxFeed.Graal.Net.Events.Market;

namespace DxFeed.Graal.Net.Samples;

internal abstract class Program
{
    /// <summary>
    /// Demonstrates how to connect to an endpoint, subscribe to market data events,
    /// handle reconnections and re-subscribing.
    /// </summary>
    public static async Task Main(string[] args)
    {
        const string address = "demo.dxfeed.com:7300"; // The address of the DxFeed endpoint.
        const string symbol = "ETH/USD:GDAX"; // The symbol for which we want to receive quotes.

        // Create new endpoint and add a listener for state changes.
        var endpoint = DXEndpoint.Create();
        endpoint.AddStateChangeListener((oldState, newState) =>
        {
            Console.WriteLine($"Connection state changed: {oldState}->{newState}");
        });

        // Connect to the endpoint using the specified address.
        endpoint.Connect(address);

        // Create a subscription for Quote events.
        var sub = endpoint.GetFeed().CreateSubscription(typeof(Quote));
        sub.AddEventListener(events =>
        {
            // Event listener that prints each received event.
            foreach (var e in events)
            {
                Console.WriteLine(e);
            }
        });

        // Add the specified symbol to the subscription.
        sub.AddSymbols(symbol);

        // Wait for five seconds to allow some quotes to be received.
        Thread.Sleep(5000);

        // Disconnect from the endpoint.
        endpoint.Disconnect();

        // Wait for another five seconds to ensure quotes stop coming in.
        Thread.Sleep(5000);

        // Reconnect to the endpoint.
        // The subscription is automatically re-subscribed, and quotes start coming into the listener again.
        // Another address can also be passed on.
        endpoint.Connect(address);

        // Keep the application running indefinitely.
        await Task.Delay(Timeout.Infinite);
    }
}
