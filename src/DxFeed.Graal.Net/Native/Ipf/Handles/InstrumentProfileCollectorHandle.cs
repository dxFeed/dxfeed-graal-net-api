// <copyright file="InstrumentProfileCollectorHandle.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Native.ErrorHandling;
using DxFeed.Graal.Net.Native.Interop;

namespace DxFeed.Graal.Net.Native.Ipf.Handles;

internal abstract class InstrumentProfileCollectorHandle : JavaSafeHandle
{
    public static InstrumentProfileCollectorHandle Create() =>
        ErrorCheck.NativeCall(CurrentThread, NativeCreate(CurrentThread));

    public long GetLastUpdateTime() =>
        ErrorCheck.NativeCall(CurrentThread, NativeGetLastUpdateTime(CurrentThread, this));

    public IterableInstrumentProfileHandle View() =>
        ErrorCheck.NativeCall(CurrentThread, NativeView(CurrentThread, this));

    public void AddUpdateListener(InstrumentProfileUpdateListenerHandle listener) =>
        ErrorCheck.NativeCall(CurrentThread, NativeAddUpdateListener(CurrentThread, this, listener));

    public void RemoveUpdateListener(InstrumentProfileUpdateListenerHandle listener) =>
        ErrorCheck.NativeCall(CurrentThread, NativeRemoveUpdateListener(CurrentThread, this, listener));

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
