// <copyright file="SessionHandle.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Native.Interop;
using DxFeed.Graal.Net.Schedules;
using static DxFeed.Graal.Net.Native.ErrorHandling.ErrorCheck;

namespace DxFeed.Graal.Net.Native.Schedules;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Created by marshaler")]
internal class SessionHandle : JavaHandle
{
    public SessionHandle(IntPtr handle)
        : base(handle)
    {
    }

    public DayHandle GetDay() =>
        SafeCall(Import.GetDay(CurrentThread, this));

    public SessionType GetSessionType() =>
        SafeCall(Import.GetType(CurrentThread, this));

    public bool IsTrading() =>
        SafeCall(Import.IsTrading(CurrentThread, this)) != 0;

    public bool IsEmpty() =>
        SafeCall(Import.IsEmpty(CurrentThread, this)) != 0;

    public long GetStartTime() =>
        SafeCall(Import.GetStartTime(CurrentThread, this));

    public long GetEndTime() =>
        SafeCall(Import.GetEndTime(CurrentThread, this));

    public bool ContainsTime(long time) =>
        SafeCall(Import.ContainsTime(CurrentThread, this, time)) != 0;

    public SessionHandle? FindPrevSession(SessionFilterHandle filter) =>
        SafeCall(Import.FindPrevSession(CurrentThread, this, filter));

    public SessionHandle? FindNextSession(SessionFilterHandle filter) =>
        SafeCall(Import.FindNextSession(CurrentThread, this, filter));

    public int HashCode() =>
        SafeCall(Import.HashCode(CurrentThread, this));

    public bool CheckEquals(SessionHandle other) =>
        SafeCall(Import.NativeEquals(CurrentThread, this, other)) != 0;

    public new string ToString() =>
        SafeCall(Import.ToString(CurrentThread, this));

    private static class Import
    {
        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_Session_getDay")]
        public static extern DayHandle GetDay(nint thread, SessionHandle session);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_Session_getType")]
        public static extern SessionType GetType(nint thread, SessionHandle session);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_Session_isTrading")]
        public static extern int IsTrading(nint thread, SessionHandle session);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_Session_isEmpty")]
        public static extern int IsEmpty(nint thread, SessionHandle session);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_Session_getStartTime")]
        public static extern long GetStartTime(nint thread, SessionHandle session);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_Session_getEndTime")]
        public static extern long GetEndTime(nint thread, SessionHandle session);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_Session_containsTime")]
        public static extern int ContainsTime(nint thread, SessionHandle session, long time);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_Session_findPrevSession")]
        public static extern SessionHandle? FindPrevSession(
            nint thread,
            SessionHandle session,
            SessionFilterHandle filter);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_Session_findNextSession")]
        public static extern SessionHandle? FindNextSession(
            nint thread,
            SessionHandle session,
            SessionFilterHandle filter);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_Session_hashCode")]
        public static extern int HashCode(nint thread, SessionHandle session);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_Session_equals")]
        public static extern int NativeEquals(nint thread, SessionHandle obj, SessionHandle other);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_Session_toString")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
        public static extern string ToString(nint thread, SessionHandle session);
    }
}
