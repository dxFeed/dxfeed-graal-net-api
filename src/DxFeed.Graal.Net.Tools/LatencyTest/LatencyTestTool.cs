// <copyright file="LatencyTestTool.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using DxFeed.Graal.Net.Api;
using DxFeed.Graal.Net.Events;
using DxFeed.Graal.Net.Events.Market;
using DxFeed.Graal.Net.Tools.Attributes;
using DxFeed.Graal.Net.Utils;

namespace DxFeed.Graal.Net.Tools.LatencyTest;

// ReSharper disable NonAtomicCompoundOperator
// ReSharper disable AccessToDisposedClosure

[ToolInfo(
    "LatencyTest",
    ShortDescription = "Connects to the specified address(es) and calculates latency.",
    Usage = new[] { "LatencyTest <address> <types> <symbols> [<options>]" })]
public class LatencyTestTool : AbstractTool<LatencyTestArgs>
{
    private sealed class Diagnostic : IDisposable
    {
        private static readonly string DiagnosticHeader = PlatformUtils.PlatformDiagInfo;
        private static readonly NumberFormatInfo SpaceNumFormat = new() { NumberGroupSeparator = " " };

        private readonly LatencyTestArgs args;
        private readonly List<string> ignoringExhanges = new();
        private readonly Timer _timer;

        private readonly Stopwatch _timerDiff = new();
        private readonly Stopwatch _runningDiff = new();
        private long _eventCounter;
        private long _listenerCounter;

        private double _min = double.NaN;
        private double _mean = double.NaN;
        private double _max = double.NaN;
        private double _percentile = double.NaN;
        private double _stdDev = double.NaN;
        private double _stdErr = double.NaN;
        private long _sampleSize;


        private double _minTotal = double.MaxValue;
        private double _maxTotal = double.MinValue;

        private readonly ConcurrentSet<string?> _symbols = new();
        private readonly ConcurrentBag<long> _deltaTime = new();

        private readonly TimeSpan _measurementPeriod;

        public Diagnostic(LatencyTestArgs args, TimeSpan measurementPeriod)
        {
            if (args.IgnoreExchanges != null)
            {
                ignoringExhanges = args.IgnoreExchanges.Split(',').ToList();
            }

            _timerDiff.Restart();
            _runningDiff.Restart();
            _measurementPeriod = measurementPeriod;
            _timer = new Timer(TimerCallback, null, measurementPeriod, measurementPeriod);
        }

        public void AddEventCounter(long value) =>
            Interlocked.Add(ref _eventCounter, value);

        public void AddListenerCounter(long value) =>
            Interlocked.Add(ref _listenerCounter, value);

        public void HandleEvents(IEnumerable<IEventType> value)
        {
            var time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            long validEvent = 0;
            foreach (var e in value)
            {
                long deltaTime;
                switch (e)
                {
                    case Quote quote:
                        if (ignoringExhanges.Count !=0 && (ignoringExhanges.Contains(quote.AskExchangeCode.ToString()) || ignoringExhanges.Contains(quote.BidExchangeCode.ToString())))
                        {
                            continue;
                        }

                        deltaTime = time - quote.Time;
                        ++validEvent;
                        _deltaTime.Add(deltaTime);
                        _symbols.Add(e.EventSymbol);
                        break;
                    case Trade trade:
                        if (ignoringExhanges.Count !=0 && ignoringExhanges.Contains(trade.ExchangeCode.ToString()))
                        {
                            continue;
                        }

                        deltaTime = time - trade.Time;
                        ++validEvent;
                        _deltaTime.Add(deltaTime);
                        _symbols.Add(e.EventSymbol);
                        break;
                    case TradeETH tradeETH:
                        deltaTime = time - tradeETH.Time;
                        ++validEvent;
                        _deltaTime.Add(deltaTime);
                        _symbols.Add(e.EventSymbol);
                        break;
                    case TimeAndSale timeAndSale:
                        if (ignoringExhanges.Count !=0 && (ignoringExhanges.Contains(timeAndSale.ExchangeCode.ToString())))
                        {
                            continue;
                        }
                        if (!timeAndSale.IsNew || !timeAndSale.IsValidTick)
                        {
                            continue;
                        }

                        deltaTime = time - timeAndSale.Time;
                        ++validEvent;
                        _deltaTime.Add(deltaTime);
                        _symbols.Add(e.EventSymbol);
                        break;
                }
            }

            AddEventCounter(validEvent);
        }

        public void Dispose() =>
            _timer.Dispose();

        private double GetEventsPerSec() =>
            GetAndResetEventCounter() / _timerDiff.Elapsed.TotalSeconds;

        private long GetAndResetEventCounter() =>
            Interlocked.Exchange(ref _eventCounter, 0);

        private static string FormatDouble(double value)
        {
            return (double.IsNaN(value) || value == double.MaxValue || value == double.MinValue) ? "---" : value.ToString("N2", SpaceNumFormat);
        }

        private void TimerCallback(object? _)
        {
            var eventsPerSec = GetEventsPerSec();

            if (!_deltaTime.IsEmpty)
            {
                var deltas = _deltaTime.ToList();
                _deltaTime.Clear();
                _min = CalcMin(deltas);
                _mean = CalcMean(deltas);
                _max = CalcMax(deltas);
                _percentile = CalcPercentile(deltas.ToArray(), 0.99);
                _stdDev = CalcStdDev(deltas);
                _stdErr = CalcStdErr(deltas, _stdDev);
                _sampleSize = deltas.Count;
                _minTotal = Math.Min(_min, _minTotal);
                _maxTotal = Math.Max(_max, _maxTotal);
            }

            var uniqueSymbols = _symbols.Count;
            _symbols.Clear();

            Console.WriteLine();
            Console.WriteLine(DiagnosticHeader);
            Console.WriteLine(@"----------------------------------------------");
            Console.WriteLine(@$"  Rate of events (avg)      : {FormatDouble(eventsPerSec)} (events/s)");
            Console.WriteLine(@$"  Rate of unique symbols    : {uniqueSymbols} (symbols/interval)");
            Console.WriteLine(@$"  Min current               : {FormatDouble(_min)} (ms)");
            Console.WriteLine(@$"  Max current               : {FormatDouble(_max)} (ms)");
            Console.WriteLine(@$"  Min total                 : {FormatDouble(_minTotal)} (ms)");
            Console.WriteLine(@$"  Max total                 : {FormatDouble(_maxTotal)} (ms)");
            Console.WriteLine(@$"  99th percentile           : {FormatDouble(_percentile)} (ms)");
            Console.WriteLine(@$"  Mean                      : {FormatDouble(_mean)} (ms)");
            Console.WriteLine(@$"  StdDev                    : {FormatDouble(_stdDev)} (ms)");
            Console.WriteLine(@$"  Error                     : {FormatDouble(_stdErr)} (ms)");
            Console.WriteLine(@$"  Sample size (N)           : {_sampleSize} (events)");
            Console.WriteLine(@$"  Measurement interval      : {_measurementPeriod.Seconds} (s)");
            Console.WriteLine(@$"  Running time              : {_runningDiff.Elapsed}");
            Console.WriteLine(@$"  Timestamp                 : {TimeFormat.Default.WithMillis().Format(DateTimeOffset.Now.ToUnixTimeMilliseconds())}");

            _min = double.NaN;
            _mean = double.NaN;
            _max = double.NaN;
            _percentile = double.NaN;
            _stdDev = double.NaN;
            _stdErr = double.NaN;
            _sampleSize = 0;

            _timerDiff.Restart();
        }

        private static double CalcPercentile(long[] sequence, double excelPercentile)
        {
            Array.Sort(sequence);
            var N = sequence.Length;
            var n = ((N - 1) * excelPercentile) + 1;
            if (n.Equals(1d))
            {
                return sequence[0];
            }

            if (n.Equals(N))
            {
                return sequence[N - 1];
            }

            var k = (int)n;
            var d = n - k;
            return sequence[k - 1] + (d * (sequence[k] - sequence[k - 1]));
        }

        private static double CalcMin(List<long> values) =>
            values.Min();

        private static double CalcMean(List<long> values) =>
            values.Average();

        private static double CalcMax(List<long> values) =>
            values.Max();

        private static double CalcStdDev(List<long> values)
        {
            double stdDev = 0;
            var count = values.Count;
            if (count <= 1)
            {
                return stdDev;
            }

            count -= 1;

            var avg = values.Average();
            var sum = values.Sum(d => (d - avg) * (d - avg));
            stdDev = Math.Sqrt(sum / count);
            return stdDev;
        }

        private static double CalcStdErr(IEnumerable<long> values, double stdDev)
        {
            var count = values.Count();
            return stdDev / Math.Sqrt(count);
        }
    }

    public override void Run(LatencyTestArgs args)
    {
        SystemProperty.SetProperties(ParseProperties(args.Properties));
        using var endpoint = DXEndpoint
            .NewBuilder()
            .WithRole(args.ForceStream ? DXEndpoint.Role.StreamFeed : DXEndpoint.Role.Feed)
            .WithProperty(DXEndpoint.DXFeedWildcardEnableProperty, "true") // Enabled by default.
            .WithProperties(ParseProperties(args.Properties))
            .WithName(nameof(LatencyTestTool))
            .Build();

        using var sub = endpoint
            .GetFeed()
            .CreateSubscription(ParseEventTypes(args.Types!));

        var measurementPeriod = new TimeSpan(0, 0, args.Interval);
        using var diagnostic = new Diagnostic(args, measurementPeriod);

        sub.AddEventListener(events =>
        {
            diagnostic.AddListenerCounter(1);
            diagnostic.HandleEvents(events);
        });

        sub.AddSymbols(ParseSymbols(args.Symbols!).ToList());

        endpoint.Connect(args.Address);

        endpoint.AwaitNotConnected();
        endpoint.CloseAndAwaitTermination();
    }
}
