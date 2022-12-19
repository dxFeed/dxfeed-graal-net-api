// <copyright file="Program.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Threading;
using System.Threading.Tasks;
using DxFeed.Graal.Net.Api;
using DxFeed.Graal.Net.Events.Market;

namespace DxFeed.Graal.Net.Samples;

internal abstract class Program
{
    public static async Task Main(string[] args)
    {
        var symbol = args[0];
        // Use default DXFeed instance for that data feed address is defined by dxfeed.properties file.
        // The properties file is copied to the build output directory from the project directory.
        var sub = DXFeed.Instance.CreateSubscription(typeof(Quote));
        sub.AddEventListener(events =>
        {
            foreach (var quote in events)
            {
                Console.WriteLine(quote);
            }
        });
        sub.AddSymbols(symbol);

        await Task.Delay(Timeout.Infinite);
    }
}
