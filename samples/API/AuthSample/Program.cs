using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DxFeed.Graal.Net.Api;
using DxFeed.Graal.Net.Events.Market;

namespace DxFeed.Graal.Net.Samples;

internal abstract class Program
{
    private const string Address = "demo.dxfeed.com:7300";
    private static readonly Random Rand = new();

    /// <summary>
    /// Demonstrates how to connect to endpoint requires authentication token, subscribe to market data events,
    /// and handle periodic token updates.
    /// </summary>
    public static async Task Main(string[] args)
    {
        // Add a listener for state changes to the default application-wide singleton instance of DXEndpoint.
        DXEndpoint.GetInstance().AddStateChangeListener((oldState, newState) =>
        {
            Console.WriteLine($"Connection state changed: {oldState}->{newState}");
        });

        // Set up a timer to periodically update the token and reconnect every 10 seconds.
        // The first connection will be made immediately.
        // After reconnection, all existing subscriptions will be re-subscribed automatically.
        await using var updateTokenTimer = new Timer(_ => { UpdateTokenAndReconnect(); }, null, 0, 10000);

        // Create a subscription for Quote events.
        var subscription = DXFeed.GetInstance().CreateSubscription(typeof(Quote));
        subscription.AddEventListener(events =>
        {
            // Event listener that prints each received event.
            foreach (var e in events)
            {
                Console.WriteLine(e);
            }
        });

        // Add the specified symbol to the subscription.
        subscription.AddSymbols("ETH/USD:GDAX");

        // Keep the application running indefinitely.
        await Task.Delay(Timeout.Infinite);
    }

    /// <summary>
    /// Updates the token and reconnects to the endpoint.
    /// </summary>
    private static void UpdateTokenAndReconnect() =>
        DXEndpoint.GetInstance().Connect($"{Address}[login=entitle:{GenerateToken()}]");

    /// <summary>
    /// Generates a random token.
    /// This is not a real token and is generated for demonstration purposes only.
    /// </summary>
    /// <returns>A new token string.</returns>
    private static string GenerateToken()
    {
        var length = Rand.Next(4, 10);
        var sb = new StringBuilder(length);

        for (var i = 0; i < length; i++)
        {
            var randValue = Rand.Next(0, 26);
            var letter = Convert.ToChar(randValue + 65);
            sb.Append(letter);
        }

        return sb.ToString();
    }
}
