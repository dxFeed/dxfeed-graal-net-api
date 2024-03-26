// <copyright file="DayHandle.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Native.Interop;
using static DxFeed.Graal.Net.Native.ErrorHandling.ErrorCheck;

namespace DxFeed.Graal.Net.Native.Schedules;

internal sealed class DayHandle : JavaHandle
{
    public int GetDayId() =>
        SafeCall(Import.GetDayId(CurrentThread, this));

    public int GetYearMonthDay() =>
        SafeCall(Import.GetYearMonthDay(CurrentThread, this));

    public int GetYear() =>
        SafeCall(Import.GetYear(CurrentThread, this));

    public int GetMonthOfYear() =>
        SafeCall(Import.GetMonthOfYear(CurrentThread, this));

    public int GetDayOfMonth() =>
        SafeCall(Import.GetDayOfMonth(CurrentThread, this));

    public int GetDayOfWeek() =>
        SafeCall(Import.GetDayOfWeek(CurrentThread, this));

    public bool IsHoliday() =>
        SafeCall(Import.IsHoliday(CurrentThread, this)) != 0;

    public bool IsShortDay() =>
        SafeCall(Import.IsShortDay(CurrentThread, this)) != 0;

    public bool IsTrading() =>
        SafeCall(Import.IsTrading(CurrentThread, this)) != 0;

    public long GetStartTime() =>
        SafeCall(Import.GetStartTime(CurrentThread, this));

    public long GetEndTime() =>
        SafeCall(Import.GetEndTime(CurrentThread, this));

    public long GetResetTime() =>
        SafeCall(Import.GetResetTime(CurrentThread, this));

    public bool ContainsTime(long time) =>
        SafeCall(Import.ContainsTime(CurrentThread, this, time)) != 0;

    public unsafe List<SessionHandle> GetSessions()
    {
        var ptr = SafeCall(Import.GetSessions(CurrentThread, this));
        var handles = new List<SessionHandle>();
        for (var i = 0; i < ptr->Size; i++)
        {
            handles.Add(new SessionHandle((IntPtr)ptr->Elements[i]));
        }

        return handles;
    }

    public SessionHandle GetSessionByTime(long time) =>
        SafeCall(Import.GetSessionByTime(CurrentThread, this, time));

    public SessionHandle? FindFirstSession(SessionFilterHandle filter) =>
        SafeCall(Import.FindFirstSession(CurrentThread, this, filter));

    public SessionHandle? FindLastSession(SessionFilterHandle filter) =>
        SafeCall(Import.FindLastSession(CurrentThread, this, filter));

    public DayHandle? FindPrevDay(DayFilterHandle dayFilter) =>
        SafeCall(Import.FindPrevDay(CurrentThread, this, dayFilter));

    public DayHandle? FindNextDay(DayFilterHandle dayFilter) =>
        SafeCall(Import.FindNextDay(CurrentThread, this, dayFilter));

    public int HashCode() =>
        SafeCall(Import.HashCode(CurrentThread, this));

    public bool CheckEquals(DayHandle other) =>
        SafeCall(Import.NativeEquals(CurrentThread, this, other)) != 0;

    public override string ToString() =>
        SafeCall(Import.ToString(CurrentThread, this));

    private static class Import
    {
        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_Day_getDayId")]
        public static extern int GetDayId(nint thread, DayHandle day);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_Day_getYearMonthDay")]
        public static extern int GetYearMonthDay(nint thread, DayHandle day);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_Day_getYear")]
        public static extern int GetYear(nint thread, DayHandle day);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_Day_getMonthOfYear")]
        public static extern int GetMonthOfYear(nint thread, DayHandle day);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_Day_getDayOfMonth")]
        public static extern int GetDayOfMonth(nint thread, DayHandle day);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_Day_getDayOfWeek")]
        public static extern int GetDayOfWeek(nint thread, DayHandle day);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_Day_isHoliday")]
        public static extern int IsHoliday(nint thread, DayHandle day);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_Day_isShortDay")]
        public static extern int IsShortDay(nint thread, DayHandle day);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_Day_isTrading")]
        public static extern int IsTrading(nint thread, DayHandle day);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_Day_getStartTime")]
        public static extern long GetStartTime(nint thread, DayHandle day);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_Day_getEndTime")]
        public static extern long GetEndTime(nint thread, DayHandle day);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_Day_getResetTime")]
        public static extern long GetResetTime(nint thread, DayHandle day);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_Day_containsTime")]
        public static extern int ContainsTime(nint thread, DayHandle day, long time);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_Day_getSessions")]
        public static extern unsafe ListNative<IntPtr>* GetSessions(nint thread, DayHandle day);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_Day_getSessionByTime")]
        public static extern SessionHandle GetSessionByTime(nint thread, DayHandle day, long time);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_Day_findFirstSession")]
        public static extern SessionHandle? FindFirstSession(nint thread, DayHandle day, SessionFilterHandle filter);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_Day_findLastSession")]
        public static extern SessionHandle? FindLastSession(nint thread, DayHandle day, SessionFilterHandle filter);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_Day_findPrevDay")]
        public static extern DayHandle? FindPrevDay(nint thread, DayHandle day, DayFilterHandle dayFilter);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_Day_findNextDay")]
        public static extern DayHandle? FindNextDay(nint thread, DayHandle day, DayFilterHandle dayFilter);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_Day_hashCode")]
        public static extern int HashCode(nint thread, DayHandle day);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_Day_equals")]
        public static extern int NativeEquals(nint thread, DayHandle day, DayHandle otherDay);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_Day_toString")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
        public static extern string ToString(nint thread, DayHandle day);
    }
}
