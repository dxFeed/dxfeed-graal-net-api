// <copyright file="PerfTestTool.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using DxFeed.Graal.Net.Api;
using DxFeed.Graal.Net.Events;
using DxFeed.Graal.Net.Tools.Attributes;
using DxFeed.Graal.Net.Utils;

// ReSharper disable NonAtomicCompoundOperator
// ReSharper disable AccessToDisposedClosure

namespace DxFeed.Graal.Net.Tools.PerfTest;

[ToolInfo(
    "PerfTest",
    ShortDescription = "Connects to specified address and calculates performance counters.",
    Description = """
    Connects to the specified address(es) and calculates performance counters (events per second, cpu usage, etc).
    """,
    Usage = new[] { "PerfTest <address> <types> <symbols> [<options>]" })]
public class PerfTestTool : AbstractTool<PerfTestArgs>
{
    private static readonly StreamWriter Output = new(new MemoryStream());
    private static volatile int _blackHoleHashCode;

    public override void Run(PerfTestArgs args)
    {
        using var endpoint = DXEndpoint
            .NewBuilder()
            .WithRole(args.ForceStream ? DXEndpoint.Role.StreamFeed : DXEndpoint.Role.Feed)
            .WithProperty(DXEndpoint.DXFeedWildcardEnableProperty, "true") // Enabled by default.
            .WithProperties(ParseProperties(args.Properties))
            .WithName(nameof(PerfTestTool))
            .Build();

        using var sub = endpoint
            .GetFeed()
            .CreateSubscription(ParseEventTypes(args.Types!));

        var measurementPeriod = new TimeSpan(0, 0, 2);
        using var diagnostic =
            new Diagnostic(Process.GetCurrentProcess(), measurementPeriod, args.ShowCpuUsageByCore);

        if (!args.DetachListener)
        {
            sub.AddEventListener(events =>
            {
                var eventTypes = events as IEventType[] ?? events.ToArray();
                diagnostic.AddListenerCounter(1);
                diagnostic.AddEventCounter(eventTypes.Length);
                foreach (var e in eventTypes)
                {
                    _blackHoleHashCode += e.GetHashCode();
                }
            });
        }

        sub.AddSymbols(ParseSymbols(args.Symbols!).ToList());

        endpoint.Connect(args.Address);

        endpoint.AwaitNotConnected();
        endpoint.CloseAndAwaitTermination();

        Output.WriteLine(_blackHoleHashCode);
    }

    private sealed class Diagnostic : IDisposable
    {
        private static readonly double CpuCoeff = GetCpuCoeff();
        private static readonly string DiagnosticHeader = PlatformUtils.PlatformDiagInfo;
        private static readonly NumberFormatInfo SpaceNumFormat = new() { NumberGroupSeparator = " " };

        private readonly Timer _timer;
        private readonly Process _currentProcess;

        private readonly bool _showCpuUsageByCore;

        private readonly Stopwatch _timerDiff = new();
        private readonly Stopwatch _runningDiff = new();
        private TimeSpan _cpuStartTime;
        private long _eventCounter;
        private long _listenerCounter;
        private double _peakMemoryUsage;
        private double _peakCpuUsage;

        public Diagnostic(Process currentProcess, TimeSpan measurementPeriod, bool showCpuUsageByCore)
        {
            _showCpuUsageByCore = showCpuUsageByCore;
            _currentProcess = currentProcess;
            _timerDiff.Restart();
            _runningDiff.Restart();
            _cpuStartTime = _currentProcess.TotalProcessorTime;
            _timer = new Timer(TimerCallback, null, measurementPeriod, measurementPeriod);
        }

        public void AddEventCounter(long value) =>
            Interlocked.Add(ref _eventCounter, value);

        public void AddListenerCounter(long value) =>
            Interlocked.Add(ref _listenerCounter, value);

        public void Dispose()
        {
            _currentProcess.Dispose();
            _timer.Dispose();
        }

        private static double GetCpuCoeff()
        {
            if (PlatformUtils.IsAppleSilicon)
            {
                // Seems like a weird error in macOS with M1 processor.
                // EndProcessorTime - StartProcessorTime returns a very small value.
                // For example, if a process maxes out one core for 2 seconds,
                // EndProcessorTime - StartProcessorTime should be ≈2000ms but it is ≈48ms.
                // Test output:
                // =======timer elapsed=======
                // total: 00:00:00.2414159
                // system: 00:00:00.2402822
                // user: 00:00:00.0011358
                // timeDiff: 2001.158 ms
                // cpuUsageDiff: 48.1079 ms        # must be ≈2000ms
                // =======timer elapsed end=======
                // This factor temporarily solves the problem (2000ms / 48ms ≈ 41.6).
                // There has already been an error associated with this counter
                // https://github.com/dotnet/runtime/issues/29527
                return 41.6;
            }

            return 1;
        }

        private static string FormatDouble(double value) =>
            double.IsNaN(value) ? "0" : value.ToString("N2", SpaceNumFormat);

        private double GetEventsPerSec() =>
            GetAndResetEventCounter() / _timerDiff.Elapsed.TotalSeconds;

        private double GetListenerCallsPerSec() =>
            GetAndResetListenerCounter() / _timerDiff.Elapsed.TotalSeconds;

        private double GetMemoryUsage()
        {
            _currentProcess.Refresh();
            return _currentProcess.WorkingSet64 / 1024.0 / 1024.0;
        }

        private double GetCpuUsage()
        {
            _currentProcess.Refresh();
            var cpuEndTime = _currentProcess.TotalProcessorTime;
            var cpuDiff = (cpuEndTime - _cpuStartTime) * CpuCoeff;
            _cpuStartTime = cpuEndTime;
            return cpuDiff / (_timerDiff.Elapsed * (!_showCpuUsageByCore ? PlatformUtils.LogicalCoreCount : 1));
        }

        private long GetAndResetEventCounter() =>
            Interlocked.Exchange(ref _eventCounter, 0);

        private long GetAndResetListenerCounter() =>
            Interlocked.Exchange(ref _listenerCounter, 0);

        private void TimerCallback(object? _)
        {
            var eventsPerSec = GetEventsPerSec();
            var listenerCallsPerSec = GetListenerCallsPerSec();

            var currentMemoryUsage = GetMemoryUsage();
            _peakMemoryUsage = currentMemoryUsage > _peakMemoryUsage ? currentMemoryUsage : _peakMemoryUsage;

            var currentCpuUsage = GetCpuUsage();
            _peakCpuUsage = currentCpuUsage > _peakCpuUsage ? currentCpuUsage : _peakCpuUsage;

            Console.WriteLine();
            Console.WriteLine(DiagnosticHeader);
            Console.WriteLine(@"----------------------------------------------");
            Console.WriteLine(@$"  Events                   : {FormatDouble(eventsPerSec)} (per/sec)");
            Console.WriteLine(@$"  Listener Calls           : {FormatDouble(listenerCallsPerSec)} (per/sec)");
            Console.WriteLine(@$"  Average Number of Events : {FormatDouble(eventsPerSec / listenerCallsPerSec)}");
            Console.WriteLine(@$"  Current Memory Usage     : {currentMemoryUsage} (Mbyte)");
            Console.WriteLine(@$"  Peak Memory Usage        : {_peakMemoryUsage} (Mbyte)");
            Console.WriteLine(@$"  Current CPU Usage        : {currentCpuUsage:P2}");
            Console.WriteLine(@$"  Peak CPU Usage           : {_peakCpuUsage:P2}");
            Console.WriteLine(@$"  Running Time             : {_runningDiff.Elapsed}");

            _timerDiff.Restart();
        }
    }
}
