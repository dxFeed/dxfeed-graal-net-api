// <copyright file="SymbolMapper.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Runtime.InteropServices;
using System.Text;
using DxFeed.Graal.Net.Api.Osub;
using DxFeed.Graal.Net.Native.Symbols;
using DxFeed.Graal.Net.Native.Symbols.Indexed;
using DxFeed.Graal.Net.Native.Symbols.TimeSeries;
using DxFeed.Graal.Net.Native.Utils;

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

    public static BaseSymbolNative* CreateNative(object symbol)
    {
        var nativeSymbol = symbol switch
        {
            string => (BaseSymbolNative*)Marshal.AllocHGlobal(sizeof(StringSymbolNative)),
            IndexedEventSubscriptionSymbol => (BaseSymbolNative*)Marshal.AllocHGlobal(sizeof(IndexedEventSubscriptionSymbolNative)),
            TimeSeriesSubscriptionSymbol => (BaseSymbolNative*)Marshal.AllocHGlobal(sizeof(TimeSeriesSubscriptionSymbolNative)),
            WildcardSymbol => (BaseSymbolNative*)Marshal.AllocHGlobal(sizeof(WildcardSymbolNative)),
            _ => throw new ArgumentException($"Unknown symbol type: {symbol.GetType().Name}"),
        };
        FillNative(symbol, nativeSymbol);
        return nativeSymbol;
    }

    public static void ReleaseNative(BaseSymbolNative* nativeSymbol)
    {
        if ((nint)nativeSymbol == 0)
        {
            return;
        }

        switch (nativeSymbol->Type)
        {
            case SymbolTypeNative.String:
                var s = (StringSymbolNative*)nativeSymbol;
                Marshal.FreeHGlobal(s->Symbol);
                break;
            case SymbolTypeNative.IndexedEventSymbol:
                var iss = (IndexedEventSubscriptionSymbolNative*)nativeSymbol;
                IndexedSourceMapper.ReleaseNative(iss->Source);
                ReleaseNative(iss->Symbol);
                break;
            case SymbolTypeNative.TimeSeriesSymbol:
                var tss = (TimeSeriesSubscriptionSymbolNative*)nativeSymbol;
                ReleaseNative(tss->Symbol);
                break;
            case SymbolTypeNative.Wildcard:
                break;
            default:
                throw new ArgumentException($"Unknown symbol type: {nativeSymbol->Type}");
        }

        Marshal.FreeHGlobal((nint)nativeSymbol);
    }

    private static void FillNative(object symbol, BaseSymbolNative* nativeSymbol)
    {
        switch (symbol)
        {
            case string s:
                var stringSymbol = (StringSymbolNative*)nativeSymbol;
                stringSymbol->Base.Type = SymbolTypeNative.String;
                stringSymbol->Symbol = StringUtilNative.NativeFromString(s, Encoding.UTF8);
                break;
            case IndexedEventSubscriptionSymbol iss:
                var indexedSymbol = (IndexedEventSubscriptionSymbolNative*)nativeSymbol;
                indexedSymbol->BaseSymbol.Type = SymbolTypeNative.IndexedEventSymbol;
                indexedSymbol->Symbol = CreateNative(iss.EventSymbol);
                indexedSymbol->Source = IndexedSourceMapper.CreateNative(iss.Source);
                break;
            case TimeSeriesSubscriptionSymbol tss:
                var timeSeriesSymbol = (TimeSeriesSubscriptionSymbolNative*)nativeSymbol;
                timeSeriesSymbol->BaseSymbol.Type = SymbolTypeNative.TimeSeriesSymbol;
                timeSeriesSymbol->Symbol = CreateNative(tss.EventSymbol);
                timeSeriesSymbol->FromTime = tss.FromTime;
                break;
            case WildcardSymbol:
                var wildcardSymbol = (WildcardSymbolNative*)nativeSymbol;
                wildcardSymbol->BaseSymbol.Type = SymbolTypeNative.Wildcard;
                break;
        }
    }
}
