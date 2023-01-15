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
    /// <summary>
    /// A simple sample that shows how to subscribe to quotes for one instruments,
    /// and print all received quotes to the console.
    /// Use default DXFeed instance for that data feed address is defined by "dxfeed.properties" file.
    /// The properties file is copied to the build output directory from the project directory.
    /// </summary>
    public static async Task Main(string[] args)
    {
        // Specified instrument name, fo example AAPL, IBM, MSFT, etc.
        var symbol = args[0];

        // Creates a subscription attached to a default DXFeed with a Quote event type.
        // The endpoint address to use is stored in the "dxfeed.properties" file.
        var sub = DXFeed.Instance.CreateSubscription(typeof(Quote));

        // Listener must be attached before symbols are added.
        sub.AddEventListener(events =>
        {
            // Prints all received events.
            foreach (var quote in events)
            {
                Console.WriteLine(quote);
            }
        });

        // Adds specified symbol.
        sub.AddSymbols(symbol);

        await Task.Delay(Timeout.Infinite);
    }
}
