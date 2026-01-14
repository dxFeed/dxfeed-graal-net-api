// <copyright file="EventTypeNative.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Native.Graal;
using DxFeed.Graal.Net.Native.Interop;
using static DxFeed.Graal.Net.Native.ErrorHandling.ErrorCheck;

namespace DxFeed.Graal.Net.Native.Events;

/// <summary>
/// The "base" type for all native events.
/// Contains an <see cref="EventCodeNative"/> associated with one of the managed types.
/// Must be included at the beginning of every native event structure to determine its type.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal record struct EventTypeNative(
    EventCodeNative EventCode,
    StringNative EventSymbol,
    long EventTime)
{
    public static void Release(IntPtr nativeEvent)
    {
        unsafe
        {
            SafeCall(Import.Release(Isolate.CurrentThread, (EventTypeNative*)nativeEvent));
        }
    }

    public static void ReleaseList(IntPtr nativeEventList)
    {
        unsafe
        {
            SafeCall(Import.ReleaseList(Isolate.CurrentThread, (ListNative<EventTypeNative>*)nativeEventList));
        }
    }

    private static class Import
    {
        [DllImport(
            ImportInfo.DllName,
            EntryPoint = "dxfg_CList_EventType_release")]
        public static extern unsafe int ReleaseList(
            nint thread,
            ListNative<EventTypeNative>* eventList);

        [DllImport(
            ImportInfo.DllName,
            EntryPoint = "dxfg_EventType_release")]
        public static extern unsafe int Release(
            nint thread,
            EventTypeNative* nativeEvent);
    }
}
