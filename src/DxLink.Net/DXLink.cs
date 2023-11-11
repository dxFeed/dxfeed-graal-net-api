// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Text.Json;
using Websocket.Client;

namespace DxLink.Net;

public static class DXLink
{
    public const int QUOTE_CHANNEL = 1;
    public const int GREEKS_CHANNEL = 3;
    public const int TRADE_CHANNEL = 5;
    public const int DEFAULT_CHANNEL = 0;
    private const int SLEEP_MS = 2000;
    public static void StartConnection(WebsocketClient client) => SetupConnection(client);

    private static void SetupConnection(WebsocketClient client)
    {
        var setup = new SetupObject
        {
            type = "SETUP",
            channel = DEFAULT_CHANNEL,
            keepaliveTimeout = 60,
            acceptKeepaliveTimeout = 60,
            version = "0.1-js/1.0.0"
        };

        client.Send(GetSerializeString(setup));
        Thread.Sleep(SLEEP_MS);
    }

    public static void AuthorizeConnection(WebsocketClient client, string token)
    {
        var auth = new DXLinkAuthObject
        {
            type = "AUTH",
            channel = DEFAULT_CHANNEL,
            token = token,
        };

        client.Send(GetSerializeString(auth));
        Thread.Sleep(SLEEP_MS);

    }

    public static void SendKeepAlive(WebsocketClient client)
    {
        if (client != null)
        {
            var keepalive = new KeepAliveObject { };

            client.Send(GetSerializeString(keepalive));
        }
    }
    public static void ResetChannel(WebsocketClient client)
    {
        if (client != null)
        {
            var reset1 = new ResetChannelObject { channel = QUOTE_CHANNEL };
            client.Send(GetSerializeString(reset1));
            Thread.Sleep(SLEEP_MS);

            var reset2 = new ResetChannelObject { channel = GREEKS_CHANNEL };
            client.Send(GetSerializeString(reset2));
            Thread.Sleep(SLEEP_MS);

            var reset3 = new ResetChannelObject { channel = TRADE_CHANNEL };
            client.Send(GetSerializeString(reset3));
            Thread.Sleep(SLEEP_MS);

            var cancel1 = new CancelChannelObject { channel = QUOTE_CHANNEL };
            client.Send(GetSerializeString(cancel1));
            Thread.Sleep(SLEEP_MS);

            var cancel2 = new CancelChannelObject { channel = GREEKS_CHANNEL };
            client.Send(GetSerializeString(cancel2));
            Thread.Sleep(SLEEP_MS);

            var cancel3 = new CancelChannelObject { channel = TRADE_CHANNEL };
            client.Send(GetSerializeString(cancel3));
            Thread.Sleep(SLEEP_MS);
        }
    }

    public static void OpenChannels(WebsocketClient client)
    {
        var quoteChannel = new ChannelRequestObject
        {
            channel = QUOTE_CHANNEL,
        };

        client.Send(GetSerializeString(quoteChannel));
        Thread.Sleep(SLEEP_MS);

        var greekChannel = new ChannelRequestObject
        {
            channel = GREEKS_CHANNEL,
        };

        client.Send(GetSerializeString(greekChannel));
        Thread.Sleep(SLEEP_MS);

        var tradeChannel = new ChannelRequestObject
        {
            channel = TRADE_CHANNEL,
        };

        client.Send(GetSerializeString(tradeChannel));
        Thread.Sleep(SLEEP_MS);
    }

    public static void AddQuotes(WebsocketClient client, List<string> quotes)
    {
        var addItems = new FeedSubscriptionObject
        {
            channel = QUOTE_CHANNEL,

        };
        foreach (var item in quotes)
        {
            addItems.add.Add(new AddItem()
            {
                type = "Quote",
                symbol = item
            });
        }
        client.Send(GetSerializeString(addItems));
    }

    public static void AddGreeks(WebsocketClient client, List<string> quotes)
    {
        var addItems = new FeedSubscriptionObject
        {
            channel = GREEKS_CHANNEL,

        };
        foreach (var item in quotes)
        {
            addItems.add.Add(new AddItem()
            {
                type = "Greeks",
                symbol = item
            });
        }
        client.Send(GetSerializeString(addItems));
    }

    public static void AddTrades(WebsocketClient client, List<string> quotes)
    {
        var addItems = new FeedSubscriptionObject
        {
            channel = TRADE_CHANNEL,

        };
        foreach (var item in quotes)
        {
            addItems.add.Add(new AddItem()
            {
                type = "Trade",
                symbol = item
            });

        }
        var serString = JsonSerializer.Serialize(addItems);
        client.Send(serString);
    }

    private static string GetSerializeString(object serObject)
    {
        var serString = JsonSerializer.Serialize(serObject);
        return serString;
    }
}



public class SetupObject
{
    public string type { get; set; }
    public int channel { get; set; }
    public int keepaliveTimeout { get; set; }
    public int acceptKeepaliveTimeout { get; set; }
    public string version { get; set; }
}

public class DXLinkAuthObject
{
    public string type { get; set; }
    public string token { get; set; }
    public int channel { get; set; } = 0;
}

public class ChannelRequestObject
{
    public string type { get; set; } = "CHANNEL_REQUEST";
    public int channel { get; set; } = 0;
    public string service { get; set; } = "FEED";
    public Parameters parameters { get; set; } = new Parameters();
}

public class Parameters
{
    public string contract { get; set; } = "AUTO";
}

public class FeedSubscriptionObject
{
    public string type { get; set; } = "FEED_SUBSCRIPTION";
    public int channel { get; set; }
    public List<AddItem> add { get; set; } = new List<AddItem>();
}

public class KeepAliveObject
{
    public string type { get; set; } = "KEEPALIVE";
    public int channel { get; set; } = DXLink.DEFAULT_CHANNEL;
}

public class ResponseData
{
    public string type { get; set; }
    public int channel { get; set; }

    public object data { get; set; }
}

public class ResetChannelObject
{
    public string type { get; set; } = "FEED_SUBSCRIPTION";
    public bool reset { get; set; } = true;
    public int channel { get; set; }
}

public class CancelChannelObject
{
    public string type { get; set; } = "CHANNEL_CANCEL";
    public int channel { get; set; }
}

public class TradeResponse
{
    public string eventSymbol { get; set; }
    public object price { get; set; }
}

public class AddItem
{
    public string symbol { get; set; }
    public string type { get; set; }
}
