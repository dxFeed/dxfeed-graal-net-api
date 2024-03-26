// <copyright file="TimeFormatNative.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Native.ErrorHandling;
using DxFeed.Graal.Net.Native.Interop;

namespace DxFeed.Graal.Net.Native.Utils;

internal sealed class TimeFormatNative : JavaHandle
{
    internal static TimeFormatNative Default() => ErrorCheck.SafeCall(Import.TimeFormatDefault(CurrentThread));

    internal static TimeFormatNative GMT() => ErrorCheck.SafeCall(Import.TimeFormatGMT(CurrentThread));

    internal static TimeFormatNative WithTimeZone(string timeZoneId)
    {
        var nativeTimeZone = TimeZoneNative.Create(timeZoneId);
        return ErrorCheck.SafeCall(Import.TimeFormatWithTimeZone(CurrentThread, nativeTimeZone));
    }

    internal TimeFormatNative WithMillis() => ErrorCheck.SafeCall(Import.TimeFormatWithMillis(CurrentThread, this));

    internal TimeFormatNative AsFullIso() => ErrorCheck.SafeCall(Import.TimeFormatAsFullIso(CurrentThread, this));

    internal DateTimeOffset Parse(string value)
    {
        var result = ErrorCheck.SafeCall(Import.Parse(CurrentThread, this, value));
        return DateTimeOffset.FromUnixTimeMilliseconds(result);
    }

    internal string Format(long value)
    {
        return ErrorCheck.SafeCall(Import.Format(CurrentThread, this, value));
    }

    private static class Import
    {
        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_TimeFormat_DEFAULT")]
        public static extern TimeFormatNative TimeFormatDefault(nint thread);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_TimeFormat_GMT")]
        public static extern TimeFormatNative TimeFormatGMT(nint thread);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_TimeFormat_getInstance")]
        public static extern TimeFormatNative TimeFormatWithTimeZone(
            nint thread,
            TimeZoneNative timeZone);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_TimeFormat_withMillis")]
        public static extern TimeFormatNative TimeFormatWithMillis(
            nint thread,
            TimeFormatNative timeFormat);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_TimeFormat_asFullIso")]
        public static extern TimeFormatNative TimeFormatAsFullIso(
            nint thread,
            TimeFormatNative timeFormat);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            BestFitMapping = false,
            EntryPoint = "dxfg_TimeFormat_parse")]
        public static extern long Parse(
            nint thread,
            TimeFormatNative handle,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string key);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            BestFitMapping = false,
            EntryPoint = "dxfg_TimeFormat_format")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
        public static extern string Format(
            nint thread,
            TimeFormatNative handle,
            long value);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_TimeZone_getDefault")]
        public static extern TimeZoneNative TimeZoneDefault(nint thread);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            BestFitMapping = false,
            EntryPoint = "dxfg_TimeZone_getTimeZone")]
        public static extern TimeZoneNative TimeZoneWithID(
            nint thread,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string key);
    }

    private sealed class TimeZoneNative : JavaHandle
    {
        internal static TimeZoneNative Default()
        {
            return ErrorCheck.SafeCall(Import.TimeZoneDefault(CurrentThread));
        }

        internal static TimeZoneNative Create(string timeZoneId)
        {
            return ErrorCheck.SafeCall(Import.TimeZoneWithID(CurrentThread, timeZoneId));
        }
    }
}
