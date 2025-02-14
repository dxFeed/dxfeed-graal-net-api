// <copyright file="ScheduleHandle.cs" company="Devexperts LLC">
// Copyright © 2025 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Ipf;
using DxFeed.Graal.Net.Native.Interop;
using static DxFeed.Graal.Net.Native.ErrorHandling.ErrorCheck;

namespace DxFeed.Graal.Net.Native.Schedules;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Created by marshaler")]
internal sealed class ScheduleHandle : JavaHandle
{
    private static readonly StringMarshaler _marshaler = new();

    public static ScheduleHandle GetInstance(InstrumentProfile profile)
    {
        SafeCall(Import.GetInstance(CurrentThread, profile, out var scheduleHandle));
        return scheduleHandle;
    }

    public static ScheduleHandle GetInstance(string scheduleDefinition) =>
        SafeCall(Import.GetInstance(CurrentThread, scheduleDefinition));

    public static ScheduleHandle GetInstance(InstrumentProfile profile, string venue)
    {
        SafeCall(Import.GetInstance(CurrentThread, profile, venue, out var scheduleHandle));
        return scheduleHandle;
    }

    public static unsafe List<string> GetTradingVenues(InstrumentProfile profile)
    {
        SafeCall(Import.GetTradingVenues(CurrentThread, profile, out var venues));
        return ConvertToStringList(venues);
    }

    public static void DownloadDefaults(string downloadConfig) =>
        SafeCall(Import.DownloadDefaults(CurrentThread, downloadConfig));

    public static void SetDefaults(byte[] data)
    {
        unsafe
        {
            fixed (byte* ptr = data)
            {
                var size = data.Length;
                SafeCall(Import.SetDefaults(CurrentThread, ptr, size));
            }
        }
    }

    public SessionHandle GetSessionByTime(long time) =>
        SafeCall(Import.GetSessionByTime(CurrentThread, this, time));

    public DayHandle GetDayByTime(long time) =>
        SafeCall(Import.GetDayByTime(CurrentThread, this, time));

    public DayHandle GetDayById(int dayId) =>
        SafeCall(Import.GetDayById(CurrentThread, this, dayId));

    public DayHandle GetDayByYearMonthDay(int yearMonthDay) =>
        SafeCall(Import.GetDayByYearMonthDay(CurrentThread, this, yearMonthDay));

    public SessionHandle? FindNearestSessionByTime(long time, SessionFilterHandle filter) =>
        SafeCall(Import.FindNearestSessionByTime(CurrentThread, this, time, filter));

    public string GetName() =>
        SafeCall(Import.GetName(CurrentThread, this));

    public string GetTimeZone() =>
        SafeCall(Import.GetTimeZone(CurrentThread, this));

    private static unsafe List<string> ConvertToStringList(ListNative<IntPtr>* handles)
    {
        try
        {
            var venues = new List<string>(handles->Size);
            for (var i = 0; i < handles->Size; i++)
            {
                var profile = (string)_marshaler.ConvertNativeToManaged((IntPtr)handles->Elements[i])!;
                venues.Add(profile);
            }

            return venues;
        }
        finally
        {
            SafeCall(Import.ReleaseList(CurrentThread, (IntPtr)handles));
        }
    }

    private static class Import
    {
        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_Schedule_getInstance4")]
        public static extern int GetInstance(
            nint thread,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(InstrumentProfileMarshaler))]
            InstrumentProfile profile,
            out ScheduleHandle handle);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_Schedule_getInstance2")]
        public static extern ScheduleHandle GetInstance(
            nint thread,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string scheduleDefinition);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_Schedule_getInstance5")]
        public static extern int GetInstance(
            nint thread,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(InstrumentProfileMarshaler))]
            InstrumentProfile profile,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string venue,
            out ScheduleHandle handle);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_Schedule_getTradingVenues2")]
        public static extern unsafe int GetTradingVenues(
            nint thread,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(InstrumentProfileMarshaler))]
            InstrumentProfile profile,
            out ListNative<IntPtr>* handle);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_Schedule_downloadDefaults")]
        public static extern int DownloadDefaults(
            nint thread,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string downloadConfig);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_Schedule_setDefaults")]
        public static extern unsafe int SetDefaults(
            nint thread,
            byte* data,
            int size);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_Schedule_getSessionByTime")]
        public static extern SessionHandle GetSessionByTime(
            nint thread,
            ScheduleHandle schedule,
            long time);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_Schedule_getDayByTime")]
        public static extern DayHandle GetDayByTime(
            nint thread,
            ScheduleHandle schedule,
            long time);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_Schedule_getDayById")]
        public static extern DayHandle GetDayById(
            nint thread,
            ScheduleHandle schedule,
            int dayId);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_Schedule_getDayByYearMonthDay")]
        public static extern DayHandle GetDayByYearMonthDay(
            nint thread,
            ScheduleHandle schedule,
            int yearMonthDay);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_Schedule_findNearestSessionByTime")]
        public static extern SessionHandle? FindNearestSessionByTime(
            nint thread,
            ScheduleHandle schedule,
            long time,
            SessionFilterHandle filter);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_Schedule_getName")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
        public static extern string GetName(nint thread, ScheduleHandle schedule);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_Schedule_getTimeZone")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
        public static extern string GetTimeZone(nint thread, ScheduleHandle schedule);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_CList_String_release")]
        public static extern int ReleaseList(nint thread, nint handle);
    }
}
