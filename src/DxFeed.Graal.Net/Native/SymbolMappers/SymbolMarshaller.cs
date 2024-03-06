// <copyright file="SymbolMarshaller.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Api.Osub;
using DxFeed.Graal.Net.Events;
using DxFeed.Graal.Net.Events.Candles;
using DxFeed.Graal.Net.Events.Market;
using DxFeed.Graal.Net.Native.ErrorHandling;
using DxFeed.Graal.Net.Native.Graal;
using DxFeed.Graal.Net.Native.Interop;

namespace DxFeed.Graal.Net.Native.SymbolMappers;

internal class SymbolMarshaller : AbstractMarshaller
{
    private static readonly Lazy<SymbolMarshaller> Instance = new();

    public static ICustomMarshaler GetInstance(string cookie) =>
        Instance.Value;

    public override object? MarshalNativeToManaged(IntPtr native)
    {
        if (native == IntPtr.Zero)
        {
            return null;
        }

        RegisterCleanUpActionsForPointer(native, CleanFromNative);
        var result = CreateAndFillManaged(native);
        return result;
    }

    public override IntPtr MarshalManagedToNative(object? managed)
    {
        unsafe
        {
            var result = (IntPtr)CreateAndFillNative(managed);
            RegisterCleanUpActionsForPointer(result, native =>
            {
                ReleaseNative((SymbolNative*)native);
            });
            return result;
        }
    }

    private static object? CreateAndFillManaged(IntPtr native)
    {
        unsafe
        {
            var symbolNative = (SymbolNative*)native;
            switch (symbolNative->SymbolCode)
            {
                case SymbolCodeNative.String:
                    return ((StringSymbolNative*)symbolNative)->Symbol.ToString();
                case SymbolCodeNative.CandleSymbol:
                    var str = ((CandleSymbolNative*)symbolNative)->Symbol.ToString();
                    return CandleSymbol.ValueOf(str);
                case SymbolCodeNative.WildcardSymbol:
                    return WildcardSymbol.All;
                case SymbolCodeNative.IndexedEventSubscriptionSymbol:
                    var indexedNative = (IndexedEventSubscriptionSymbolNative*)symbolNative;
                    var indexedSymbol = CreateAndFillManaged((IntPtr)indexedNative->Symbol);
                    if (indexedSymbol != null)
                    {
                        return new IndexedEventSubscriptionSymbol(
                            indexedSymbol,
                            new IndexedEventSource(
                                indexedNative->Source->Id,
                                indexedNative->Source->Name.ToString() ?? string.Empty));
                    }

                    break;
                case SymbolCodeNative.TimeSeriesSubscriptionSymbol:
                    var timeSeriesNative = (TimeSeriesSubscriptionSymbolNative*)symbolNative;
                    var timeSeriesSymbol = CreateAndFillManaged((IntPtr)timeSeriesNative->Symbol);
                    if (timeSeriesSymbol != null)
                    {
                        return new TimeSeriesSubscriptionSymbol(timeSeriesSymbol, timeSeriesNative->FromTime);
                    }

                    break;
                default:
                    throw new ArgumentException($"Unknown symbol type: {symbolNative->SymbolCode}");
            }
        }

        return null;
    }

    private static unsafe void ReleaseNative(SymbolNative* nativeSymbol)
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
                var iss = (SymbolMarshaller.IndexedEventSubscriptionSymbolNative*)nativeSymbol;
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
                stringNative->Symbol = StringNative.ValueOf(value);
                return (SymbolNative*)stringNative;
            case IndexedEventSubscriptionSymbol value:
                var indexedSymbol =
                    (IndexedEventSubscriptionSymbolNative*)Marshal.AllocHGlobal(
                        sizeof(IndexedEventSubscriptionSymbolNative));
                indexedSymbol->SymbolNative.SymbolCode = SymbolCodeNative.IndexedEventSubscriptionSymbol;
                indexedSymbol->Symbol = CreateAndFillNative(value.EventSymbol);
                indexedSymbol->Source = IndexedSourceMapper.CreateNative(value.Source);
                return (SymbolNative*)indexedSymbol;
            case TimeSeriesSubscriptionSymbol value:
                var timesSeriesNative = (TimeSeriesSubscriptionSymbolNative*)Marshal.AllocHGlobal(
                    sizeof(TimeSeriesSubscriptionSymbolNative));
                timesSeriesNative->SymbolNative.SymbolCode = SymbolCodeNative.TimeSeriesSubscriptionSymbol;
                timesSeriesNative->Symbol = CreateAndFillNative(value.EventSymbol);
                timesSeriesNative->FromTime = value.FromTime;
                return (SymbolNative*)timesSeriesNative;
            case CandleSymbol value:
                var candleNative = (CandleSymbolNative*)Marshal.AllocHGlobal(sizeof(CandleSymbolNative));
                candleNative->SymbolNative.SymbolCode = SymbolCodeNative.String;
                candleNative->Symbol = value.Symbol;
                return (SymbolNative*)candleNative;
            case WildcardSymbol:
                var wildcardSymbolNative =
                    (WildcardSymbolNative*)Marshal.AllocHGlobal(sizeof(WildcardSymbolNative));
                wildcardSymbolNative->SymbolNative.SymbolCode = SymbolCodeNative.WildcardSymbol;
                return (SymbolNative*)wildcardSymbolNative;
            default:
                throw new ArgumentException($"Unknown symbol type: {managed.GetType().Name}");
        }
    }

    private static void CleanFromNative(IntPtr native) =>
        ErrorCheck.SafeCall(Import.Release(Isolate.CurrentThread, native));

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

    private static class Import
    {
        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_Symbol_release")]
        public static extern int Release(nint thread, nint handle);
    }
}
