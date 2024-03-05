// <copyright file="SymbolMapper.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Api.Osub;
using DxFeed.Graal.Net.Events.Candles;
using DxFeed.Graal.Net.Native.Interop;

namespace DxFeed.Graal.Net.Native.SymbolMappers;

// ToDo Simplify implementation.
// Todo Add release method for IEnumerable.
internal static unsafe class SymbolMapper
{
    public static ListNative<T>* CreateNative<T>(object[] symbols)
        where T : unmanaged
    {
        var listNative = (ListNative<T>*)Marshal.AllocHGlobal(sizeof(ListNative<T>));
        listNative->Size = symbols.Length;
        listNative->Elements = (T**)Marshal.AllocHGlobal(sizeof(T*) * listNative->Size);
        for (var i = 0; i < symbols.Length; ++i)
        {
            listNative->Elements[i] = (T*)CreateNative(symbols[i]);
        }

        return listNative;
    }

    public static SymbolMarshaller.SymbolNative* CreateNative(object symbol)
    {
        var nativeSymbol = symbol switch
        {
            string => (SymbolMarshaller.SymbolNative*)Marshal.AllocHGlobal(sizeof(SymbolMarshaller.StringSymbolNative)),
            CandleSymbol => (SymbolMarshaller.SymbolNative*)Marshal.AllocHGlobal(sizeof(SymbolMarshaller.CandleSymbolNative)),
            IndexedEventSubscriptionSymbol => (SymbolMarshaller.SymbolNative*)Marshal.AllocHGlobal(sizeof(SymbolMarshaller.IndexedEventSubscriptionSymbolNative)),
            TimeSeriesSubscriptionSymbol => (SymbolMarshaller.SymbolNative*)Marshal.AllocHGlobal(sizeof(SymbolMarshaller.TimeSeriesSubscriptionSymbolNative)),
            WildcardSymbol => (SymbolMarshaller.SymbolNative*)Marshal.AllocHGlobal(sizeof(SymbolMarshaller.WildcardSymbolNative)),
            _ => throw new ArgumentException($"Unknown symbol type: {symbol.GetType().Name}"),
        };
        FillNative(symbol, nativeSymbol);
        return nativeSymbol;
    }

    public static void ReleaseNative(SymbolMarshaller.SymbolNative* nativeSymbol)
    {
        if ((nint)nativeSymbol == 0)
        {
            return;
        }

        switch (nativeSymbol->SymbolCode)
        {
            case SymbolMarshaller.SymbolCodeNative.String:
                var s = (SymbolMarshaller.StringSymbolNative*)nativeSymbol;
                s->Symbol.Release();
                break;
            case SymbolMarshaller.SymbolCodeNative.CandleSymbol:
                var cs = (SymbolMarshaller.CandleSymbolNative*)nativeSymbol;
                cs->Symbol.Release();
                break;
            case SymbolMarshaller.SymbolCodeNative.IndexedEventSubscriptionSymbol:
                var iss = (SymbolMarshaller.IndexedEventSubscriptionSymbolNative*)nativeSymbol;
                IndexedSourceMapper.ReleaseNative(iss->Source);
                ReleaseNative(iss->Symbol);
                break;
            case SymbolMarshaller.SymbolCodeNative.TimeSeriesSubscriptionSymbol:
                var tss = (SymbolMarshaller.TimeSeriesSubscriptionSymbolNative*)nativeSymbol;
                ReleaseNative(tss->Symbol);
                break;
            case SymbolMarshaller.SymbolCodeNative.WildcardSymbol:
                break;
            default:
                throw new ArgumentException($"Unknown symbol type: {nativeSymbol->SymbolCode}");
        }

        Marshal.FreeHGlobal((nint)nativeSymbol);
    }

    private static void FillNative(object symbol, SymbolMarshaller.SymbolNative* nativeSymbol)
    {
        switch (symbol)
        {
            case string s:
                var stringSymbol = (SymbolMarshaller.StringSymbolNative*)nativeSymbol;
                stringSymbol->Base.SymbolCode = SymbolMarshaller.SymbolCodeNative.String;
                stringSymbol->Symbol = s;
                break;
            case CandleSymbol cs:
                var candleSymbol = (SymbolMarshaller.CandleSymbolNative*)nativeSymbol;
                candleSymbol->SymbolNative.SymbolCode = SymbolMarshaller.SymbolCodeNative.String;
                candleSymbol->Symbol = cs.Symbol;
                break;
            case IndexedEventSubscriptionSymbol iss:
                var indexedSymbol = (SymbolMarshaller.IndexedEventSubscriptionSymbolNative*)nativeSymbol;
                indexedSymbol->SymbolNative.SymbolCode = SymbolMarshaller.SymbolCodeNative.IndexedEventSubscriptionSymbol;
                indexedSymbol->Symbol = CreateNative(iss.EventSymbol);
                indexedSymbol->Source = IndexedSourceMapper.CreateNative(iss.Source);
                break;
            case TimeSeriesSubscriptionSymbol tss:
                var timeSeriesSymbol = (SymbolMarshaller.TimeSeriesSubscriptionSymbolNative*)nativeSymbol;
                timeSeriesSymbol->SymbolNative.SymbolCode = SymbolMarshaller.SymbolCodeNative.TimeSeriesSubscriptionSymbol;
                timeSeriesSymbol->Symbol = CreateNative(tss.EventSymbol);
                timeSeriesSymbol->FromTime = tss.FromTime;
                break;
            case WildcardSymbol:
                var wildcardSymbol = (SymbolMarshaller.WildcardSymbolNative*)nativeSymbol;
                wildcardSymbol->SymbolNative.SymbolCode = SymbolMarshaller.SymbolCodeNative.WildcardSymbol;
                break;
        }
    }
}
