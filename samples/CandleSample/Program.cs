using System;
using System.Globalization;
using System.Text.RegularExpressions;
using DxFeed.Graal.Net.Api;
using DxFeed.Graal.Net.Api.Osub;
using DxFeed.Graal.Net.Events.Candles;
using DxFeed.Graal.Net.Utils;
using DxFeed.Graal.Net.Utils.Time;

namespace DxFeed.Graal.Net.Samples;

/// <summary>
///     This sample class demonstrates subscription to candle events.
///     The sample configures via command line, subscribes to candle events and prints received data.
/// </summary>
internal abstract class Program
{
    private const int HostIndex = 0;
    private const int SymbolIndex = 1;

    private static bool TryParseDateTimeParam(string stringParam, InputParam<DateTimeOffset?> param)
    {
        try
        {
            param.Value = DXTimeFormat.Default().Parse(stringParam);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private static void WriteHelp() =>
        Console.WriteLine(
            "Usage: dxf_candle_sample <host:port>|<path> <base symbol> [<date>] [-T <token>] [-p] [<attributes> ...] \n" +
            "where\n" +
            "    host:port   - The address of dxfeed server (demo.dxfeed.com:7300)\n" +
            "    path        - The path to file with candle data (tape or non zipped Candle Web Service output)\n" +
            "    base symbol - The base market symbol without attributes\n" +
            "    date        - The date of Candle in the format YYYY-MM-DD (may be empty)\n" +
            "    attributes  - The candle attributes\n\n" +
            "attributes must use the style of \"name=value\" and may be as follows:\n" +
            "    exchange  - The exchange code letter (may be empty)\n" +
            "    period    - The aggregation period of this symbol (may be empty, default: 1)\n" +
            "    type      - The type of the candle aggregation period (use \"t\" for TICK, \"s\"\n" +
            "                for SECOND, \"m\" for MINUTE, \"h\" for HOUR, \"d\" for DAY, \"w\" for\n" +
            "                WEEK, \"mo\" for MONTH, \"o\" for OPTEXP, \"y\" for YEAR, \"v\" for\n" +
            "                VOLUME, \"p\" for PRICE, \"pm\" for PRICE_MOMENTUM, \"pr\" for\n" +
            "                PRICE_RENKO or may be empty, default: TICK\n" +
            "    price     - Defines price that is used to build the candle (use \"last\" for\n" +
            "                LAST, \"ask\" for ASK, \"mark\" for MARK, \"s\" for SETTLEMENT or may\n" +
            "                be empty, default: LAST\n" +
            "    session   - The session attribute defines trading that is used to build the\n" +
            "                candles. (use \"false\" for ANY, \"true\" for REGULAR or may be\n" +
            "                empty, default: ANY\n" +
            "    alignment - The alignment defines how candle are aligned with respect to time\n" +
            "                (use \"m\" for MIDNIGHT, \"s\" for SESSION or may be empty,\n" +
            "    priceLevel - The candle price level\n" +
            "                 default: NaN\n\n" +
            "All missed attributes values will be set to defaults\n" +
            "examples: \n" +
            "    demo.dxfeed.com:7300 AAPL 2016-06-18 exchange=A period=1 type=d price=last session=true alignment=m\n" +
            "    demo.dxfeed.com:7300 \"AAPL&A{=d,tho=true}\" 2016-06-18\n"
        );

    private static void Main(string[] args)
    {
        if (args.Length < 2)
        {
            WriteHelp();
            return;
        }

        try
        {
            var address = args[HostIndex];
            var baseSymbol = args[SymbolIndex];
            var dateTime = new InputParam<DateTimeOffset?>(new DateTime(1970, 1, 1, 0, 0, 0, 0));
            var exchange = CandleExchange.Default;
            var periodValue = 1.0;
            var period = CandlePeriod.Default;
            var price = CandlePrice.Default;
            var session = CandleSession.Default;
            var alignment = CandleAlignment.Default;
            var priceLevel = CandlePriceLevel.Default;
            var logDataTransferFlag = false;

            var attributesAreSet = false;
            for (var i = SymbolIndex + 1; i < args.Length; i++)
            {
                if (!dateTime.IsSet && TryParseDateTimeParam(args[i], dateTime))
                {
                    continue;
                }


                if (!logDataTransferFlag && args[i].Equals("-p", StringComparison.Ordinal))
                {
                    logDataTransferFlag = true;
                    i++;

                    continue;
                }

                const string KEY_VALUE_REGEX = @"([a-z]+)(=)([a-z]+|\d+\.?\d*)";
                var match = Regex.Match(args[i], KEY_VALUE_REGEX, RegexOptions.IgnoreCase);

                if (match.Groups.Count < 4 || !match.Success)
                {
                    Console.WriteLine("Invalid Attributes");
                    WriteHelp();
                    return;
                }

                if (match.Groups[1].Value.Equals("exchange", StringComparison.Ordinal))
                {
                    if (match.Groups[3].Length == 1 && char.IsLetter(match.Groups[3].Value[0]))
                    {
                        exchange = CandleExchange.ValueOf(match.Groups[3].Value[0]);
                        attributesAreSet = true;
                    }
                }
                else if (match.Groups[1].Value.Equals("period", StringComparison.Ordinal))
                {
                    periodValue = double.Parse(match.Groups[3].Value, new CultureInfo("en-US"));
                    attributesAreSet = true;
                }
                else if (match.Groups[1].Value.Equals("type", StringComparison.Ordinal))
                {
                    period = CandlePeriod.ValueOf(periodValue, CandleType.Parse(match.Groups[3].Value));
                    attributesAreSet = true;
                }
                else if (match.Groups[1].Value.Equals("price", StringComparison.Ordinal))
                {
                    price = CandlePrice.Parse(match.Groups[3].Value);
                    attributesAreSet = true;
                }
                else if (match.Groups[1].Value.Equals("session", StringComparison.Ordinal))
                {
                    session = CandleSession.Parse(match.Groups[3].Value);
                    attributesAreSet = true;
                }
                else if (match.Groups[1].Value.Equals("alignment", StringComparison.Ordinal))
                {
                    alignment = CandleAlignment.Parse(match.Groups[3].Value);
                    attributesAreSet = true;
                }
                else if (match.Groups[1].Value.Equals("priceLevel", StringComparison.Ordinal))
                {
                    priceLevel = CandlePriceLevel.Parse(match.Groups[3].Value);
                    attributesAreSet = true;
                }
            }

            var symbol = attributesAreSet
                ? CandleSymbol.ValueOf(baseSymbol, exchange, period, price, session, alignment, priceLevel)
                : CandleSymbol.ValueOf(baseSymbol);

            var endpoint = DXEndpoint.Create().Connect(address);
            var sub = endpoint.GetFeed().CreateSubscription(typeof(Candle));
            sub.AddEventListener(events =>
            {
                foreach (var candle in events)
                {
                    Console.WriteLine(candle);
                }
            });
            sub.AddSymbols(new TimeSeriesSubscriptionSymbol(symbol, (DateTimeOffset)dateTime.Value!));
            Console.WriteLine("Press enter to stop");
            Console.ReadLine();
        }
        catch (Exception exc)
        {
            Console.WriteLine($"Exception occurred: {exc.GetType()}, message: {exc.Message}");
        }
    }

    private class InputParam<T>
    {
        private T _value;

        public InputParam(T defaultValue) =>
            _value = defaultValue;

        public bool IsSet { get; private set; }

        public T Value
        {
            get => _value;
            set
            {
                _value = value;
                IsSet = true;
            }
        }
    }
}
