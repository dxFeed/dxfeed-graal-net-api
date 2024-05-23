// <copyright file="Program.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DxFeed.Graal.Net.Api;
using DxFeed.Graal.Net.Ipf;
using DxFeed.Graal.Net.Utils;

namespace DxFeed.Graal.Net.Samples;

/// <summary>
/// An sample that demonstrates a subscription using InstrumentProfile.
/// Use default DXFeed instance for that data feed address is defined by "dxfeed.properties" file.
/// The properties file is copied to the build output directory from the project directory.
/// </summary>
internal abstract class Program
{
    public static async Task Main(string[] args)
    {
        if (args.Length != 2)
        {
            var eventTypeNames = ReflectionUtil.CreateTypesString(DXEndpoint.GetEventTypes());
            Console.WriteLine("usage: DXFeedIpfConnect <type> <ipf-file>");
            Console.WriteLine("where: <type>     is dxfeed event type (" + eventTypeNames + ")");
            Console.WriteLine("       <ipf-file> is name of instrument profiles file");
            return;
        }

        var type = args[0];
        var ipfFile = args[1];

        var eventType = CmdArgsUtil.ParseTypes(type);
        var sub = DXFeed.GetInstance().CreateSubscription(eventType);
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
        sub.AddSymbols(GetSymbols(ipfFile));

        await Task.Delay(Timeout.Infinite);
    }

    private static List<string> GetSymbols(string filename)
    {
        Console.WriteLine($"Reading instruments from {filename}");
        var profiles = new InstrumentProfileReader().ReadFromFile(filename);
        // This is just a sample, any arbitrary filtering may go here.
        var filter = (InstrumentProfile profile) =>
            profile.Type.Equals("STOCK", StringComparison.Ordinal); // stocks
        var result = new List<string>();
        Console.WriteLine("Selected symbols are:");
        foreach (var profile in profiles.Where(profile => filter(profile)))
        {
            result.Add(profile.Symbol);
            Console.WriteLine($"{profile.Symbol} ({profile.Description})");
        }

        return result;
    }
}
