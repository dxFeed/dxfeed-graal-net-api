# dxFeed Graal .NET API

.NET API for accessing [dxFeed market data](https://dxfeed.com/market-data/), built as a wrapper over
the [dxFeed Graal Native](https://www.nuget.org/packages/DxFeed.Graal.Native/) library
(compiled with [GraalVM Native Image](https://www.graalvm.org/latest/reference-manual/native-image/) from
the [dxFeed Java API](https://docs.dxfeed.com/dxfeed/api/overview-summary.html)).

> If you're currently using the [legacy dxFeed .NET API](https://github.com/dxFeed/dxfeed-net-api), please see
> the [migration guide](https://github.com/dxFeed/dxfeed-graal-net-api#migration).

## Platforms

- **Windows** x64 (Windows 8+, Server 2012+)
- **Linux** x64 (glibc 2.17+, or musl with `gcompat`)
- **macOS** x64 (10.15+) and Arm64 (11+)

## Frameworks

`netstandard2.0`, `net6.0`, `net7.0`, `net8.0`, `net9.0`

## Installation

```bash
dotnet add package DxFeed.Graal.Net
```

## Quick start

```csharp
using DxFeed.Graal.Net.Api;
using DxFeed.Graal.Net.Events.Market;

using var endpoint = DXEndpoint.Create().Connect("demo.dxfeed.com:7300");
using var subscription = endpoint.GetFeed().CreateSubscription(typeof(Quote));

subscription.AddEventListener(events =>
{
    foreach (var e in events)
        Console.WriteLine(e);
});

subscription.AddSymbols("AAPL");
Console.ReadKey();
```

To connect via dxLink (WebSocket) instead of QD, see
the [dxLink example](https://github.com/dxFeed/dxfeed-graal-net-api#how-to-connect-to-dxlink).

## Documentation & resources

- 📖 [Full README](https://github.com/dxFeed/dxfeed-graal-net-api) — overview, migration guide, samples, current state
- 📚 [API documentation](https://dxfeed.github.io/dxfeed-graal-net-api/)
- 🧪 [Samples](https://github.com/dxFeed/dxfeed-graal-net-api/tree/main/samples) — API, Candle, IPF, Model, Schedule, UI
- 🛠 [Tools](https://github.com/dxFeed/dxfeed-graal-net-api/releases) — Connect, Dump, PerfTest, LatencyTest, Qds
- 💡 [dxFeed Knowledge Base](https://kb.dxfeed.com/index.html?lang=en)

## License

[MPL-2.0](https://github.com/dxFeed/dxfeed-graal-net-api/blob/master/LICENSE)
