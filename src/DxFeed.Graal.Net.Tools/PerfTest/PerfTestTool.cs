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

namespace DxFeed.Graal.Net.Tools.PerfTest;

internal abstract class PerfTestTool
{
    private static readonly StreamWriter Output = new(new MemoryStream());
    private static volatile int _blackHoleHashCode;

    private sealed class Diagnostic : IDisposable
    {
        private readonly bool _cpuUsageByCore;
        private readonly string _os;
        private readonly string _arch;
        private readonly Timer _timer;
        private readonly Process _currentProcess;
        private readonly NumberFormatInfo _numberFormatInfo = new() { NumberGroupSeparator = " " };
        private readonly Stopwatch _timerDiff = new();
        private readonly Stopwatch _runningDiff = new();
        private readonly double _cpuCoeff;
        private TimeSpan _cpuStartTime;
        private long _eventCounter;
        private double _peakMemoryUsage;
        private double _peakCpuUsage;

        public Diagnostic(Process currentProcess, int periodSec, bool cpuUsageByCore)
        {
            _cpuUsageByCore = cpuUsageByCore;
            _os = GetOsNameAndVersion();
            _arch = GetArch().ToString();
            _currentProcess = currentProcess;
            _timerDiff.Restart();
            _runningDiff.Restart();
            _cpuStartTime = _currentProcess.TotalProcessorTime;
            _cpuCoeff = GetCpuCoeff();
            _timer = new Timer(TimerCallback, null, periodSec * 1000, periodSec * 1000);
        }

        public void AddEventCounter(long value) =>
            Interlocked.Add(ref _eventCounter, value);

        public void Dispose()
        {
            _currentProcess.Dispose();
            _timer.Dispose();
        }

        private static Architecture GetArch() =>
            RuntimeInformation.ProcessArchitecture;

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

        private static double GetCpuCoeff()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) && GetArch() == Architecture.Arm64)
            {
                // Seems like a weird error in macOS with M1 processor.
                // EndProcessorTime - StartProcessorTime returns a very small value.
                // For example, if a process maxes out one core for 2 seconds,
                // EndProcessorTime - StartProcessorTime should be ~2000ms but it is ~42ms.
                // This factor temporarily solves the problem.
                // There has already been an error associated with this counter
                // https://github.com/dotnet/runtime/issues/29527
                return 41.6;
            }

            return 1;
        }

        private double GetEventsPerSec() =>
            GetAndResetEventCounter() / _timerDiff.Elapsed.TotalSeconds;

        private double GetMemoryUsage()
        {
            _currentProcess.Refresh();
            return _currentProcess.WorkingSet64 / 1024.0 / 1024.0;
        }

        private double GetCpuUsage()
        {
            _currentProcess.Refresh();
            var cpuEndTime = _currentProcess.TotalProcessorTime;
            var processorDiff = (cpuEndTime - _cpuStartTime) * _cpuCoeff;
            _cpuStartTime = cpuEndTime;
            return processorDiff / (_timerDiff.Elapsed * (!_cpuUsageByCore ? Environment.ProcessorCount : 1));
        }

        private long GetAndResetEventCounter() =>
            Interlocked.Exchange(ref _eventCounter, 0);

        private void TimerCallback(object? _)
        {
            var eventsPerSec = GetEventsPerSec();

            var currentMemoryUsage = GetMemoryUsage();
            _peakMemoryUsage = currentMemoryUsage > _peakMemoryUsage ? currentMemoryUsage : _peakMemoryUsage;

            var currentCpuUsage = GetCpuUsage();
            _peakCpuUsage = currentCpuUsage > _peakCpuUsage ? currentCpuUsage : _peakCpuUsage;

            Console.WriteLine();
            Console.WriteLine($"{_os} {_arch} ({Environment.ProcessorCount} core)");
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine($"  Events               : {eventsPerSec.ToString("N2", _numberFormatInfo)} (per/sec)");
            Console.WriteLine($"  Current Memory Usage : {currentMemoryUsage} (Mbyte)");
            Console.WriteLine($"  Peak Memory Usage    : {_peakMemoryUsage} (Mbyte)");
            Console.WriteLine($"  Current CPU Usage    : {currentCpuUsage:P2} (%)");
            Console.WriteLine($"  Peak CPU Usage       : {_peakCpuUsage:P2} (%)");
            Console.WriteLine($"  Running time         : {_runningDiff.Elapsed}");

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

        const int periodSec = 2;
        using var diagnostic = new Diagnostic(Process.GetCurrentProcess(), periodSec, cmdArgs.CpuUsageByCore);

        if (!cmdArgs.DetachListener)
        {
            sub.AddEventListener(events =>
            {
                var eventTypes = events as IEventType[] ?? events.ToArray();
                // ReSharper disable once AccessToDisposedClosure
                diagnostic.AddEventCounter(eventTypes.Length);
                foreach (var e in eventTypes)
                {
                    // ReSharper disable once NonAtomicCompoundOperator
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
