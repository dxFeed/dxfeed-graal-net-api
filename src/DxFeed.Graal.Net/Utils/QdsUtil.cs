// <copyright file="QdsUtil.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Collections.Generic;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Native;
using DxFeed.Graal.Net.Native.ErrorHandling;
using DxFeed.Graal.Net.Native.Graal;
using DxFeed.Graal.Net.Native.Interop;

namespace DxFeed.Graal.Net.Utils;

/// <summary>
/// Provides utility methods for running qds-tools using the DxFeed Graal.NET infrastructure.
/// </summary>
public static class QdsUtil
{
    /// <summary>
    /// Executes a qds-tools with the specified arguments.
    /// </summary>
    /// <param name="args">The arguments to pass to the qds-tools. These arguments control the tool's behavior.</param>
    /// <remarks>
    /// This method converts the provided string arguments into a native format suitable for the QDS tool,
    /// handles any errors during the execution, and ensures proper resource management within the native interop context.
    /// </remarks>
    public static unsafe void RunTool(IEnumerable<string> args) =>
        ErrorCheck.SafeCall(Import.Main(
            Isolate.CurrentThread,
            ListNative<StringNative>.Create(args, s => StringNative.ValueOf(s).NativeStringPtr)));

    private static class Import
    {
        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_Tools_main")]
        public static extern unsafe int Main(
            nint thread,
            ListNative<StringNative>* role);
    }
}
