// <copyright file="InstrumentProfileUpdateListenerHandle.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Ipf.Live;
using DxFeed.Graal.Net.Native.Interop;
using static DxFeed.Graal.Net.Native.ErrorHandling.ErrorCheck;

namespace DxFeed.Graal.Net.Native.Ipf;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Created by marshaler")]
internal sealed class InstrumentProfileUpdateListenerHandle : JavaHandle
{
    private static readonly Delegate OnUpdateDelegate = new OnUpdateDelegateType(OnUpdate);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void OnUpdateDelegateType(nint thread, nint iterator, GCHandle netHandle);

    public static InstrumentProfileUpdateListenerHandle Create(InstrumentProfileUpdateListener listener) =>
        CreateAndRegisterFinalize(listener, handle => SafeCall(Import.New(CurrentThread, OnUpdateDelegate, handle)));

    private static void OnUpdate(nint thread, nint iterator, GCHandle netHandle)
    {
        if (!netHandle.IsAllocated)
        {
            return;
        }

        using var it = new IterableInstrumentProfileHandle(iterator, false);
        var listener = netHandle.Target as InstrumentProfileUpdateListener;
        listener?.Invoke(it.ToList());
    }

    private static class Import
    {
        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_InstrumentProfileUpdateListener_new")]
        public static extern InstrumentProfileUpdateListenerHandle New(
            nint thread,
            Delegate listener,
            GCHandle netHandle);
    }
}
