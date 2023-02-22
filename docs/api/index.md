# API Documentation

With dxFeed .NET API you can start receiving your market events writing only several lines of code:

```csharp
using DxFeed.Graal.Net.Api;
using DxFeed.Graal.Net.Events.Market;

using var endpoint = DXEndpoint.Create().Connect("demo.dxfeed.com:7300");
using var subscription = endpoint.GetFeed().CreateSubscription(typeof(Quote));
subscription.AddEventListener(events =>
{
    foreach (var e in events)
    {
        Console.WriteLine(e);
    }
});
subscription.AddSymbols("AAPL");
Console.ReadKey();
```
