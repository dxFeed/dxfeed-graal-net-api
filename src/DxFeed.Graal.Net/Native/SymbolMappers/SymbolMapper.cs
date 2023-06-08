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
using DxFeed.Graal.Net.Native.Symbols;
using DxFeed.Graal.Net.Native.Symbols.Candle;
using DxFeed.Graal.Net.Native.Symbols.Indexed;
using DxFeed.Graal.Net.Native.Symbols.TimeSeries;

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

    public static SymbolNative* CreateNative(object symbol)
    {
        var nativeSymbol = symbol switch
        {
            string => (SymbolNative*)Marshal.AllocHGlobal(sizeof(StringSymbolNative)),
            CandleSymbol => (SymbolNative*)Marshal.AllocHGlobal(sizeof(CandleSymbolNative)),
            IndexedEventSubscriptionSymbol => (SymbolNative*)Marshal.AllocHGlobal(sizeof(IndexedEventSubscriptionSymbolNative)),
            TimeSeriesSubscriptionSymbol => (SymbolNative*)Marshal.AllocHGlobal(sizeof(TimeSeriesSubscriptionSymbolNative)),
            WildcardSymbol => (SymbolNative*)Marshal.AllocHGlobal(sizeof(WildcardSymbolNative)),
            _ => throw new ArgumentException($"Unknown symbol type: {symbol.GetType().Name}"),
        };
        FillNative(symbol, nativeSymbol);
        return nativeSymbol;
    }

    public static void ReleaseNative(SymbolNative* nativeSymbol)
    {
        if ((nint)nativeSymbol == 0)
        {
            return;
        }

        switch (nativeSymbol->SymbolCode)
        {
            case SymbolCodeNative.String:
                var s = (StringSymbolNative*)nativeSymbol;
                s->Symbol.Release();
                break;
            case SymbolCodeNative.CandleSymbol:
                var cs = (CandleSymbolNative*)nativeSymbol;
                cs->Symbol.Release();
                break;
            case SymbolCodeNative.IndexedEventSubscriptionSymbol:
                var iss = (IndexedEventSubscriptionSymbolNative*)nativeSymbol;
                IndexedSourceMapper.ReleaseNative(iss->Source);
                ReleaseNative(iss->Symbol);
                break;
            case SymbolCodeNative.TimeSeriesSubscriptionSymbol:
                var tss = (TimeSeriesSubscriptionSymbolNative*)nativeSymbol;
                ReleaseNative(tss->Symbol);
                break;
            case SymbolCodeNative.WildcardSymbol:
                break;
            default:
                throw new ArgumentException($"Unknown symbol type: {nativeSymbol->SymbolCode}");
        }

        Marshal.FreeHGlobal((nint)nativeSymbol);
    }

    private static void FillNative(object symbol, SymbolNative* nativeSymbol)
    {
        switch (symbol)
        {
            case string s:
                var stringSymbol = (StringSymbolNative*)nativeSymbol;
                stringSymbol->Base.SymbolCode = SymbolCodeNative.String;
                stringSymbol->Symbol = s;
                break;
            case CandleSymbol cs:
                var candleSymbol = (CandleSymbolNative*)nativeSymbol;
                candleSymbol->SymbolNative.SymbolCode = SymbolCodeNative.String;
                candleSymbol->Symbol = cs.Symbol;
                break;
            case IndexedEventSubscriptionSymbol iss:
                var indexedSymbol = (IndexedEventSubscriptionSymbolNative*)nativeSymbol;
                indexedSymbol->SymbolNative.SymbolCode = SymbolCodeNative.IndexedEventSubscriptionSymbol;
                indexedSymbol->Symbol = CreateNative(iss.EventSymbol);
                indexedSymbol->Source = IndexedSourceMapper.CreateNative(iss.Source);
                break;
            case TimeSeriesSubscriptionSymbol tss:
                var timeSeriesSymbol = (TimeSeriesSubscriptionSymbolNative*)nativeSymbol;
                timeSeriesSymbol->SymbolNative.SymbolCode = SymbolCodeNative.TimeSeriesSubscriptionSymbol;
                timeSeriesSymbol->Symbol = CreateNative(tss.EventSymbol);
                timeSeriesSymbol->FromTime = tss.FromTime;
                break;
            case WildcardSymbol:
                var wildcardSymbol = (WildcardSymbolNative*)nativeSymbol;
                wildcardSymbol->SymbolNative.SymbolCode = SymbolCodeNative.WildcardSymbol;
                break;
        }
    }
}
