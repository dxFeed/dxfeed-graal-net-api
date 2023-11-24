// <copyright file="InstrumentProfileUpdateListenerHandle.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Ipf.Live;
using DxFeed.Graal.Net.Native.Interop;
using static DxFeed.Graal.Net.Native.ErrorHandling.ErrorCheck;

namespace DxFeed.Graal.Net.Native.Ipf.Handles;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Created by marshaller")]
internal sealed unsafe class InstrumentProfileUpdateListenerHandle : JavaHandle
{
    public static InstrumentProfileUpdateListenerHandle Create(InstrumentProfileUpdateListener listener) =>
        CreateAndRegisterFinalize(listener, handle => SafeCall(Import.New(CurrentThread, &OnUpdate, handle)));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
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
            delegate* unmanaged[Cdecl]<nint, nint, GCHandle, void> listener,
            GCHandle netHandle);
    }
}
