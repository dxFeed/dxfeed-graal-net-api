// <copyright file="TimePeriodNative.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Native.ErrorHandling;
using DxFeed.Graal.Net.Native.Interop;

namespace DxFeed.Graal.Net.Native.Utils;

internal sealed class TimePeriodNative : JavaHandle
{
    internal static TimePeriodNative Zero() => ErrorCheck.SafeCall(Import.Zero(CurrentThread));

    internal static TimePeriodNative Unlimited() => ErrorCheck.SafeCall(Import.Unlimited(CurrentThread));

    internal static TimePeriodNative ValueOf(long value) => ErrorCheck.SafeCall(Import.ValueOf(CurrentThread, value));

    internal static TimePeriodNative ValueOf(string value) => ErrorCheck.SafeCall(Import.ValueOf(CurrentThread, value));

    internal long GetTime() => ErrorCheck.SafeCall(Import.GetTime(CurrentThread, this));

    internal int GetSeconds() => ErrorCheck.SafeCall(Import.GetSeconds(CurrentThread, this));

    internal long GetNanos() => ErrorCheck.SafeCall(Import.GetNanos(CurrentThread, this));

    private static class Import
    {
        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_TimePeriod_ZERO")]
        public static extern TimePeriodNative Zero(nint thread);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_TimePeriod_UNLIMITED")]
        public static extern TimePeriodNative Unlimited(nint thread);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_TimePeriod_valueOf")]
        public static extern TimePeriodNative ValueOf(nint thread, long value);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            BestFitMapping = false,
            EntryPoint = "dxfg_TimePeriod_valueOf2")]
        public static extern TimePeriodNative ValueOf(
            nint thread,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string value);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_TimePeriod_getTime")]
        public static extern long GetTime(
            nint thread,
            TimePeriodNative handle);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_TimePeriod_getSeconds")]
        public static extern int GetSeconds(
            nint thread,
            TimePeriodNative handle);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_TimePeriod_getNanos")]
        public static extern long GetNanos(
            nint thread,
            TimePeriodNative handle);
    }
}
