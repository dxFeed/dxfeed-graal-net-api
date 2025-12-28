// <copyright file="PriceLevelServiceHandle.cs" company="Devexperts LLC">
// Copyright © 2025 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Events;
using DxFeed.Graal.Net.Events.Candles;
using DxFeed.Graal.Net.Events.Market;
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

    public List<Order> getOrders(CandleSymbol candleSymbol, OrderSource orderSource, TimeSpan from, TimeSpan to, string caller)
    {
        // ToDo: implement

        return new List<Order>();
    }

    public List<Order> getOrders(CandleSymbol candleSymbol, OrderSource orderSource, TimeSpan from, TimeSpan to)
    {
        // ToDo: implement

        return new List<Order>();
    }

    public void Close() => ErrorCheck.SafeCall(NativeClose(CurrentThread, this));

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
    private static extern int NativeGetOrders(
        nint thread,
        PriceLevelServiceHandle service,
        [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SymbolMarshaler))]
        object candleSymbol,
        [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(IndexedEventSourceMarshaller))]
        IndexedEventSource orderSource,
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
    private static extern int NativeGetOrders(
        nint thread,
        PriceLevelServiceHandle service,
        [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SymbolMarshaler))]
        object candleSymbol,
        [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(IndexedEventSourceMarshaller))]
        IndexedEventSource orderSource,
        long from,
        long to,
        out IntPtr /* ListNative<EventTypeNative>* */ orders);

    // ToDo: add dxfg_PriceLevelService_getAuthOrderSource

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        ExactSpelling = true,
        BestFitMapping = false,
        ThrowOnUnmappableChar = true,
        EntryPoint = "dxfg_PriceLevelService_getQuotes")]
    private static extern int NativeGetQuotes(
        nint thread,
        PriceLevelServiceHandle service,
        [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SymbolMarshaler))]
        object candleSymbol,
        long from,
        long to,
        [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
        string caller,
        out IntPtr /* ListNative<EventTypeNative>* */ quotes);

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        ExactSpelling = true,
        BestFitMapping = false,
        ThrowOnUnmappableChar = true,
        EntryPoint = "dxfg_PriceLevelService_getQuotes2")]
    private static extern int NativeGetQuotes(
        nint thread,
        PriceLevelServiceHandle service,
        [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SymbolMarshaler))]
        object candleSymbol,
        long from,
        long to,
        out IntPtr /* ListNative<EventTypeNative>* */ quotes);

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        ExactSpelling = true,
        BestFitMapping = false,
        ThrowOnUnmappableChar = true,
        EntryPoint = "dxfg_PriceLevelService_close")]
    private static extern int NativeClose(
        nint thread,
        PriceLevelServiceHandle service);
}
