// <copyright file="Program.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using DxFeed.Graal.Net.Tools.Connect;
using DxFeed.Graal.Net.Tools.Dump;
using DxFeed.Graal.Net.Tools.PerfTest;

namespace DxFeed.Graal.Net.Tools;

internal abstract class Program
{
    public static void Main(string[] args)
    {
        var cmdArgs = new ProgramArgs().ParseArgs(args);
        if (cmdArgs == null)
        {
            return;
        }

        switch (cmdArgs.Tool)
        {
            case Tools.Connect:
                ConnectTool.Run(args.AsSpan()[1..].ToArray());
                break;
            case Tools.Dump:
                DumpTool.Run(args.AsSpan()[1..].ToArray());
                break;
            case Tools.PerfTest:
                PerfTestTool.Run(args.AsSpan()[1..].ToArray());
                break;
        }
    }
}
