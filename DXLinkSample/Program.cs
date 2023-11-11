// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Text.Json;
using DxLink.Net;
using Websocket.Client;

Console.WriteLine("Starting connection!");

var uri = new Uri("wss://demo.dxfeed.com/dxlink-ws");

var client = new WebsocketClient(uri);


var options = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true
};

client.MessageReceived.Subscribe(msg =>
{
    if (msg.MessageType == System.Net.WebSockets.WebSocketMessageType.Text)
    {

        Console.WriteLine($"Message received: {msg}");

        var authResponseObject = JsonSerializer.Deserialize<ResponseData>(msg.Text.ToString());

        if (msg.Text.ToString().Contains("\"type\":\"SETUP\""))
        {
            Console.WriteLine($"SETUP received: {msg}");
            // DXLink.AuthorizeConnection(client, ""); // Add your Auth token here if needed.
        }
        if (msg.Text.ToString().Contains("\"type\":\"AUTH_STATE\"") && msg.Text.ToString().Contains("AUTHORIZED") && !msg.Text.ToString().Contains("UNAUTHORIZED"))
        {
            Console.WriteLine($"AUTH received: {msg}");
            DXLink.OpenChannels(client);
        }
        if (msg.Text.ToString().Contains("\"type\":\"CHANNEL_OPENED\"") && msg.Text.ToString().Contains("\"channel\":5"))
        {
            DXLink.AddQuotes(client, new List<string> { "AAPL" });
            DXLink.AddQuotes(client, new List<string> { "MSFT" });
        }
        if (msg.Text.ToString().Contains("ERROR") && msg.Text.ToString().Contains("SETUP step missing"))
        {
            Console.WriteLine($"PRIMARY ERROR received: {msg}");
            DXLink.StartConnection(client);
            return;
        }
        if (authResponseObject != null && authResponseObject.data != null && authResponseObject != null)
        {
            try
            {
                var cleanText = authResponseObject.data.ToString().Replace("\"NaN\"", "0.0");
                Console.WriteLine($"Data received: {msg}");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR received: {msg}");
            }

        }
    }
    else
    {
        Console.WriteLine($"Non Text Message received: {msg}");
    }

}

);
await client.Start();

DXLink.StartConnection(client);

for (int i = 0; i < 20; i++)
{
    System.Threading.Thread.Sleep(1000);
}

Console.WriteLine("Sample run Complete");
