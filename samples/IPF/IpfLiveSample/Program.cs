// <copyright file="Program.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using DxFeed.Graal.Net.Ipf;
using DxFeed.Graal.Net.Ipf.Live;

namespace DxFeed.Graal.Net.Samples;

internal abstract class Program
{
    private static readonly string DXFEED_IPF_URL = "https://demo:demo@tools.dxfeed.com/ipf";

    public static async Task Main(string[] args)
    {
        if (args.Length > 1) {
            Console.WriteLine("usage: DXFeedLiveIpfSample [<ipf-url>]");
            Console.WriteLine("where: <ipf-url>  is URL for the instruments profiles, default: " + DXFEED_IPF_URL);
            return;
        }

        var url = args.Length > 0 ? args[0] : DXFEED_IPF_URL;

        var collector = new InstrumentProfileCollector();
        var connection = InstrumentProfileConnection.CreateConnection(url, collector);
        // Update period can be used to re-read IPF files, not needed for services supporting IPF "live-update"
        connection.SetUpdatePeriod(60_000L);
        connection.Start();

        // Data model to keep all instrument profiles mapped by their ticker symbol
        var profiles = new ConcurrentDictionary<string, InstrumentProfile>();

        // It is possible to add listener after connection is started - updates will not be missed in this case
        collector.AddUpdateListener(instruments =>
        {
            Console.WriteLine("\nInstrument Profiles:");
            // We can observe REMOVED elements - need to add necessary filtering
            // See javadoc for InstrumentProfileCollector for more details

            // (1) We can either process instrument profile updates manually
            instruments.ForEach(profile =>
            {
                if (InstrumentProfileType.REMOVED.Name.Equals(profile.GetType().Name, StringComparison.Ordinal)) {
                    // Profile was removed - remove it from our data model
                    profiles.TryRemove(profile.Symbol, out _);
                } else {
                    // Profile was updated - collector only notifies us if profile was changed
                    profiles.TryAdd(profile.Symbol, profile);
                }
            });
            Console.WriteLine("Total number of profiles (1): " + profiles.Count);
            Console.WriteLine("Last modified: " + DateTimeOffset.FromUnixTimeMilliseconds(collector.GetLastUpdateTime()));
        });


        await Task.Delay(Timeout.Infinite);
    }
}
