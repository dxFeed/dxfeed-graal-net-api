// <copyright file="ScheduleHandle.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Ipf;
using DxFeed.Graal.Net.Native.Interop;
using static DxFeed.Graal.Net.Native.ErrorHandling.ErrorCheck;

namespace DxFeed.Graal.Net.Native.Schedules;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Created by marshaler")]
internal class ScheduleHandle : JavaHandle
{
    public static ScheduleHandle GetInstance(InstrumentProfile profile) =>
        SafeCall(Import.GetInstance(CurrentThread, profile));

    public static ScheduleHandle GetInstance(string scheduleDefinition) =>
        SafeCall(Import.GetInstance(CurrentThread, scheduleDefinition));

    public static ScheduleHandle GetInstance(InstrumentProfile profile, string venue) =>
        SafeCall(Import.GetInstance(CurrentThread, profile, venue));

    public static List<string> GetTradingVenues(InstrumentProfile profile) =>
        SafeCall(Import.GetTradingVenues(CurrentThread, profile));

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

    private static class Import
    {
        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_Schedule_getInstance")]
        public static extern ScheduleHandle GetInstance(
            nint thread,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(InstrumentProfileMarshaler))]
            InstrumentProfile profile);

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
            EntryPoint = "dxfg_Schedule_getInstance3")]
        public static extern ScheduleHandle GetInstance(
            nint thread,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(InstrumentProfileMarshaler))]
            InstrumentProfile profile,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string venue);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_Schedule_getTradingVenues")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ListMarshaler<StringMarshaler>))]
        public static extern List<string> GetTradingVenues(
            nint thread,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(InstrumentProfileMarshaler))]
            InstrumentProfile profile);

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
    }
}
