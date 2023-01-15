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
        private readonly string _os;
        private readonly string _arch;
        private readonly Timer _timer;
        private readonly Process _currentProcess;
        private readonly Stopwatch _timerTimeElapsed = new();
        private readonly Stopwatch _runningTimeElapsed = new();
        private readonly NumberFormatInfo _numberFormatInfo = new() { NumberGroupSeparator = " " };
        private long _eventCounter;
        private double _peakMemoryUsage;

        public Diagnostic(Process currentProcess, int dueTimeSec, int periodSec)
        {
            _os = GetOsNameAndVersion();
            _arch = RuntimeInformation.ProcessArchitecture.ToString();
            _currentProcess = currentProcess;
            _timerTimeElapsed.Restart();
            _runningTimeElapsed.Restart();
            _timer = new Timer(TimerCallback, null, dueTimeSec * 1000, periodSec * 1000);
        }

        public void AddEventCounter(long value) =>
            Interlocked.Add(ref _eventCounter, value);

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

        private long GetAndResetEventCounter() =>
            Interlocked.Exchange(ref _eventCounter, 0);

        private void TimerCallback(object? _)
        {
            var totalEvents = GetAndResetEventCounter();
            var elapsedSec = _timerTimeElapsed.Elapsed.TotalSeconds;
            _timerTimeElapsed.Restart();
            _currentProcess.Refresh();

            var eventsPerSec = totalEvents / elapsedSec;
            var currentMemoryUsage = _currentProcess.WorkingSet64 / 1024.0 / 1024.0;
            _peakMemoryUsage = currentMemoryUsage > _peakMemoryUsage ? currentMemoryUsage : _peakMemoryUsage;

            Console.WriteLine();
            Console.WriteLine($"{_os} {_arch}");
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine($"  Events               : {eventsPerSec.ToString("N2", _numberFormatInfo)} (per/sec)");
            Console.WriteLine($"  Current Memory Usage : {currentMemoryUsage} (Mbyte)");
            Console.WriteLine($"  Peak Memory Usage    : {_peakMemoryUsage} (Mbyte)");
            Console.WriteLine($"  Running time         : {_runningTimeElapsed.Elapsed}");
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

        const int dueTimeSec = 2;
        const int periodSec = 2;
        using var diagnostic = new Diagnostic(Process.GetCurrentProcess(), dueTimeSec, periodSec);

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
