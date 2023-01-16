// <copyright file="PerfTestTool.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using DxFeed.Graal.Net.Api;
using DxFeed.Graal.Net.Events;

// ReSharper disable NonAtomicCompoundOperator
// ReSharper disable AccessToDisposedClosure

namespace DxFeed.Graal.Net.Tools.PerfTest;

internal abstract class PerfTestTool
{
    private static readonly StreamWriter Output = new(new MemoryStream());
    private static volatile int _blackHoleHashCode;

    private sealed class Diagnostic : IDisposable
    {
        private static readonly string OsName = GetOsNameAndVersion();
        private static readonly string ArchName = GetArch().ToString();
        private static readonly int CoreCount = GetCoreCount();
        private static readonly double CpuCoeff = GetCpuCoeff();
        private static readonly string DiagnosticHeader = $"{OsName} {ArchName}({CoreCount} core)";
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

        private static string GetOsNameAndVersion()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return $"macOS({Environment.OSVersion.Version})";
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return $"Linux({Environment.OSVersion.Version})";
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return $"Windows({Environment.OSVersion.Version})";
            }

            return Environment.OSVersion.ToString();
        }

        private static Architecture GetArch() =>
            RuntimeInformation.ProcessArchitecture;

        private static int GetCoreCount() =>
            Environment.ProcessorCount;

        private static double GetCpuCoeff()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) && GetArch() == Architecture.Arm64)
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
            return cpuDiff / (_timerDiff.Elapsed * (!_showCpuUsageByCore ? Environment.ProcessorCount : 1));
        }

        private long GetAndResetEventCounter() =>
            Interlocked.Exchange(ref _eventCounter, 0);

        private long GetAndResetListenerCounter() =>
            Interlocked.Exchange(ref _listenerCounter, 0);

        private static string FormatDouble(double value) =>
            double.IsNaN(value) ? "0" : value.ToString("N2", SpaceNumFormat);

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
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine($"  Events                   : {FormatDouble(eventsPerSec)} (per/sec)");
            Console.WriteLine($"  Listener Calls           : {FormatDouble(listenerCallsPerSec)} (per/sec)");
            Console.WriteLine($"  Average Number of Events : {FormatDouble(eventsPerSec / listenerCallsPerSec)}");
            Console.WriteLine($"  Current Memory Usage     : {currentMemoryUsage} (Mbyte)");
            Console.WriteLine($"  Peak Memory Usage        : {_peakMemoryUsage} (Mbyte)");
            Console.WriteLine($"  Current CPU Usage        : {currentCpuUsage:P2}");
            Console.WriteLine($"  Peak CPU Usage           : {_peakCpuUsage:P2}");
            Console.WriteLine($"  Running Time             : {_runningDiff.Elapsed}");

            _timerDiff.Restart();
        }
    }

    public static void Run(IEnumerable<string> args)
    {
        var cmdArgs = new PerfTestArgs().ParseArgs(args);
        if (cmdArgs == null)
        {
            return;
        }

        using var endpoint = DXEndpoint
            .NewBuilder()
            .WithRole(cmdArgs.ForceStream ? DXEndpoint.Role.StreamFeed : DXEndpoint.Role.Feed)
            .WithProperty(DXEndpoint.DXFeedWildcardEnableProperty, "true") // Enabled by default.
            .WithProperties(Helper.ParseProperties(cmdArgs.Properties))
            .WithName(nameof(PerfTestTool))
            .Build();

        using var sub = endpoint
            .GetFeed()
            .CreateSubscription(Helper.ParseEventTypes(cmdArgs.Types!));

        var measurementPeriod = new TimeSpan(0, 0, 2);
        using var diagnostic =
            new Diagnostic(Process.GetCurrentProcess(), measurementPeriod, cmdArgs.ShowCpuUsageByCore);

        if (!cmdArgs.DetachListener)
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

        sub.AddSymbols(Helper.ParseSymbols(cmdArgs.Symbols!).ToList());

        endpoint.Connect(cmdArgs.Address);

        endpoint.AwaitNotConnected();
        endpoint.CloseAndAwaitTermination();

        Output.WriteLine(_blackHoleHashCode);
    }
}
