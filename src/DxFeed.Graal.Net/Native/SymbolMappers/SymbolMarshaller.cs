// <copyright file="SymbolMarshaller.cs" company="Devexperts LLC">
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

internal class SymbolMarshaller : AbstractMarshaller
{
    private static readonly Lazy<SymbolMarshaller> Instance = new();

    public static ICustomMarshaler GetInstance(string cookie) =>
        Instance.Value;

    public override object? MarshalNativeToManaged(IntPtr native) => throw new NotImplementedException();

    public override IntPtr MarshalManagedToNative(object? managed)
    {
        unsafe
        {
            return (IntPtr)CreateAndFillNative(managed);
        }
    }

    private unsafe SymbolNative* CreateAndFillNative(object? managed)
    {
        if (managed == null)
        {
            return null;
        }

        switch (managed)
        {
            case string value:
                var stringNative = (StringSymbolNative*)Marshal.AllocHGlobal(sizeof(StringNative));
                stringNative->Base.SymbolCode = SymbolCodeNative.String;
                stringNative->Symbol = new StringNative() { NativeStringPtr = Marshal.StringToCoTaskMemUTF8(value) };
                RegisterCleanUpActionsForPointer((IntPtr)stringNative, native =>
                {
                    var s = (StringSymbolNative*)native;
                    Marshal.ZeroFreeCoTaskMemUTF8(s->Symbol.NativeStringPtr);
                });
                return (SymbolNative*)stringNative;
                break;
            case IndexedEventSubscriptionSymbol value:
                var indexedSymbol =
                    (IndexedEventSubscriptionSymbolNative*)Marshal.AllocHGlobal(
                        sizeof(IndexedEventSubscriptionSymbolNative));
                indexedSymbol->SymbolNative.SymbolCode = SymbolCodeNative.IndexedEventSubscriptionSymbol;
                indexedSymbol->Symbol = (SymbolNative*)CreateAndFillNative(value.EventSymbol);
                indexedSymbol->Source = IndexedSourceMapper.CreateNative(value.Source);
                RegisterCleanUpActionsForPointer((IntPtr)indexedSymbol, native =>
                {
                    var s = (IndexedEventSubscriptionSymbolNative*)native;
                    IndexedSourceMapper.ReleaseNative(s->Source);
                });
                return (SymbolNative*)indexedSymbol;
                break;
            case TimeSeriesSubscriptionSymbol value:
                var timesSeriesNative = (TimeSeriesSubscriptionSymbolNative*)Marshal.AllocHGlobal(
                    sizeof(TimeSeriesSubscriptionSymbolNative));
                timesSeriesNative->SymbolNative.SymbolCode = SymbolCodeNative.TimeSeriesSubscriptionSymbol;
                timesSeriesNative->Symbol = (SymbolNative*)CreateAndFillNative(value.EventSymbol);
                timesSeriesNative->FromTime = value.FromTime;
                return (SymbolNative*)timesSeriesNative;
                break;
            case CandleSymbol value:
                var candleNative = (CandleSymbolNative*)Marshal.AllocHGlobal(sizeof(CandleSymbolNative));
                candleNative->SymbolNative.SymbolCode = SymbolCodeNative.String;
                candleNative->Symbol = value.Symbol;
                RegisterCleanUpActionsForPointer((IntPtr)candleNative, native =>
                {
                    var s = (StringSymbolNative*)native;
                    s->Symbol.Release();
                });
                return (SymbolNative*)candleNative;
                break;
            case WildcardSymbol:
                var wildcardSymbolNative =
                    (WildcardSymbolNative*)Marshal.AllocHGlobal(sizeof(WildcardSymbolNative));
                wildcardSymbolNative->SymbolNative.SymbolCode = SymbolCodeNative.WildcardSymbol;
                return (SymbolNative*)wildcardSymbolNative;
                break;
            default:
                throw new ArgumentException($"Unknown symbol type: {managed.GetType().Name}");
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct SymbolNative
    {
        public SymbolCodeNative SymbolCode;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct IndexedEventSourceNative
    {
        public IndexedSourceTypeNative Type;
        public int Id;
        public StringNative Name;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct IndexedEventSubscriptionSymbolNative
    {
        public SymbolNative SymbolNative;
        public SymbolNative* Symbol; // Can be any allowed symbol (String, Wildcard, etc.).
        public IndexedEventSourceNative* Source;
    }

    internal enum IndexedSourceTypeNative
    {
        /// <summary>
        /// Represent <see cref="Net.Events.IndexedEventSource"/> type.
        /// </summary>
        IndexedEventSource,

        /// <summary>
        /// Represent <see cref="OrderSource"/> type.
        /// </summary>
        OrderEventSource,
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct TimeSeriesSubscriptionSymbolNative
    {
        public SymbolNative SymbolNative;
        public SymbolNative* Symbol; // Can be any allowed symbol (String, Wildcard, etc.).
        public long FromTime;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct StringSymbolNative
    {
        public SymbolNative Base;
        public StringNative Symbol; // A null-terminated UTF-8 string.
    }

    internal enum SymbolCodeNative
    {
        /// <summary>
        /// Symbol as <see cref="string"/>.
        /// </summary>
        String,

        /// <summary>
        /// Symbol as <see cref="Net.Events.Candles.CandleSymbol"/>.
        /// </summary>
        CandleSymbol,

        /// <summary>
        /// Symbol as <see cref="Api.Osub.WildcardSymbol"/>.
        /// </summary>
        WildcardSymbol,

        /// <summary>
        /// Symbols as <see cref="Api.Osub.IndexedEventSubscriptionSymbol"/>.
        /// </summary>
        IndexedEventSubscriptionSymbol,

        /// <summary>
        /// Symbols as <see cref="Api.Osub.TimeSeriesSubscriptionSymbol"/>.
        /// </summary>
        TimeSeriesSubscriptionSymbol,
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct WildcardSymbolNative
    {
        public SymbolNative SymbolNative;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct CandleSymbolNative
    {
        public SymbolNative SymbolNative;
        public StringNative Symbol; // A null-terminated UTF-8 string.
    }
}
