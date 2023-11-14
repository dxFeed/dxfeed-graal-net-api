// <copyright file="InstrumentProfileCollectorHandle.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Ipf;
using DxFeed.Graal.Net.Ipf.Live;
using DxFeed.Graal.Net.Native.ErrorHandling;
using DxFeed.Graal.Net.Native.Interop;
using DxFeed.Graal.Net.Native.Ipf.Handles;

namespace DxFeed.Graal.Net.Native.Ipf;

internal sealed class InstrumentProfileCollectorHandle : JavaHandle
{
    private static readonly
        ConcurrentDictionary<InstrumentProfileUpdateListener, InstrumentProfileUpdateListenerHandle> Listeners = new();

    public static InstrumentProfileCollectorHandle Create() =>
        ErrorCheck.SafeCall(NativeCreate(CurrentThread));

    public long GetLastUpdateTime() =>
        ErrorCheck.SafeCall(NativeGetLastUpdateTime(CurrentThread, this));

    public IEnumerable<InstrumentProfile> View()
    {
        using var it = ErrorCheck.SafeCall(NativeView(CurrentThread, this));
        return it.ToList();
    }

    public void AddUpdateListener(InstrumentProfileUpdateListener listener)
    {
        if (Listeners.ContainsKey(listener))
        {
            return;
        }

        var l = InstrumentProfileUpdateListenerHandle.Create(listener);
        Listeners.TryAdd(listener, l);
        ErrorCheck.SafeCall(NativeAddUpdateListener(CurrentThread, this, l));
    }

    public void RemoveUpdateListener(InstrumentProfileUpdateListener listener)
    {
        if (Listeners.TryRemove(listener, out var l))
        {
            ErrorCheck.SafeCall(NativeRemoveUpdateListener(CurrentThread, this, l));
        }
    }

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        EntryPoint = "dxfg_InstrumentProfileCollector_new")]
    private static extern InstrumentProfileCollectorHandle NativeCreate(nint thread);

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        EntryPoint = "dxfg_InstrumentProfileCollector_getLastUpdateTime")]
    private static extern long NativeGetLastUpdateTime(
        nint thread,
        InstrumentProfileCollectorHandle collector);

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        EntryPoint = "dxfg_InstrumentProfileCollector_view")]
    private static extern IterableInstrumentProfileHandle NativeView(
        nint thread,
        InstrumentProfileCollectorHandle collector);

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        EntryPoint = "dxfg_InstrumentProfileCollector_addUpdateListener")]
    private static extern int NativeAddUpdateListener(
        nint thread,
        InstrumentProfileCollectorHandle collector,
        InstrumentProfileUpdateListenerHandle listener);

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        EntryPoint = "dxfg_InstrumentProfileCollector_removeUpdateListener")]
    private static extern int NativeRemoveUpdateListener(
        nint thread,
        InstrumentProfileCollectorHandle collector,
        InstrumentProfileUpdateListenerHandle listener);
}
