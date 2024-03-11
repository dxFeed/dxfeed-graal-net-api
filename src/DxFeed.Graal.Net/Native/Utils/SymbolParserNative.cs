// <copyright file="SymbolParserNative.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Native.ErrorHandling;
using DxFeed.Graal.Net.Native.Graal;
using DxFeed.Graal.Net.Native.Interop;

namespace DxFeed.Graal.Net.Native.Utils;

internal static class SymbolParserNative
{
    public static IEnumerable<string> Parse(string value) =>
        ErrorCheck.SafeCall(Import.ParseSymbols(IsolateThread.CurrentThread, value)).OfType<string>();

    private static class Import
    {
        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            BestFitMapping = false,
            EntryPoint = "dxfg_Tools_parseSymbols")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ListMarshaler<StringMarshaler>))]
        public static extern IEnumerable<object> ParseSymbols(
            nint thread,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string key);
    }
}
