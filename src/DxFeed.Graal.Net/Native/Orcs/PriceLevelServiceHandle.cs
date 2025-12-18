// <copyright file="PriceLevelServiceHandle.cs" company="Devexperts LLC">
// Copyright © 2025 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Native.ErrorHandling;
using DxFeed.Graal.Net.Native.Interop;
using DxFeed.Graal.Net.Native.SymbolMappers;

namespace DxFeed.Graal.Net.Native.Orcs;

internal sealed class PriceLevelServiceHandle : JavaHandle
{
    public static PriceLevelServiceHandle Create(string address)
    {
        ErrorCheck.SafeCall(NativeCreate(CurrentThread, address, out var service));

        return service;
    }

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        ExactSpelling = true,
        BestFitMapping = false,
        ThrowOnUnmappableChar = true,
        EntryPoint = "dxfg_PriceLevelService_new")]
    private static extern int NativeCreate(
        nint thread,
        [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
        string address,
        out PriceLevelServiceHandle service);

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        ExactSpelling = true,
        BestFitMapping = false,
        ThrowOnUnmappableChar = true,
        EntryPoint = "dxfg_PriceLevelService_getOrders")]
    private static extern unsafe int NativeGetOrders(
        nint thread,
        PriceLevelServiceHandle service,
        [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SymbolMarshaler))]
        object candleSymbol,
        SymbolMarshaler.IndexedEventSourceNative* orderSource,
        long from,
        long to,
        [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
        string caller,
        out IntPtr /* ListNative<EventTypeNative>* */ orders);

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        ExactSpelling = true,
        BestFitMapping = false,
        ThrowOnUnmappableChar = true,
        EntryPoint = "dxfg_PriceLevelService_getOrders2")]
    private static extern unsafe int NativeGetOrders(
        nint thread,
        PriceLevelServiceHandle service,
        [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SymbolMarshaler))]
        object candleSymbol,
        SymbolMarshaler.IndexedEventSourceNative* orderSource,
        long from,
        long to,
        out IntPtr /* ListNative<EventTypeNative>* */ orders);

    // ToDo: add dxfg_PriceLevelService_getAuthOrderSource
}
