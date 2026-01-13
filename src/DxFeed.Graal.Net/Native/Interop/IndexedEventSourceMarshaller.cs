// <copyright file="IndexedEventSourceMarshaller.cs" company="Devexperts LLC">
// Copyright © 2025 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Events;
using DxFeed.Graal.Net.Events.Market;
using DxFeed.Graal.Net.Native.ErrorHandling;
using DxFeed.Graal.Net.Native.Graal;

namespace DxFeed.Graal.Net.Native.Interop;

internal sealed partial class IndexedEventSourceMarshaller : AbstractMarshaler
{
    private static readonly Lazy<IndexedEventSourceMarshaller> Instance = new();

    public static ICustomMarshaler GetInstance(string cookie) => Instance.Value;

    public static unsafe object CreateAndFillManaged(IntPtr native)
    {
        var sourceNative = (IndexedEventSourceNative*)native;

        return sourceNative->Type switch
        {
            IndexedEventSourceTypeNative.IndexedEventSource =>

                // In case there are events that have a source other than DEFAULT.
                new IndexedEventSource(
                    sourceNative->Id,
                    sourceNative->Name.ToString() ?? string.Empty),
            IndexedEventSourceTypeNative.OrderEventSource => OrderSource.ValueOf(sourceNative->Id),
            _ => throw new ArgumentException($"Unknown source type: {sourceNative->Type}"),
        };
    }

    public static unsafe IndexedEventSourceNative* CreateAndFillNative(object? managed)
    {
        if (managed == null)
        {
            return null;
        }

        var sourceNative =
            (IndexedEventSourceNative*)Marshal.AllocHGlobal(sizeof(IndexedEventSourceNative));
        sourceNative->Type = IndexedEventSourceTypeNative.IndexedEventSource;

        switch (managed)
        {
            case OrderSource value:
                sourceNative->Type = IndexedEventSourceTypeNative.OrderEventSource;
                sourceNative->Id = value.Id;
                sourceNative->Name = value.Name;
                break;
            case IndexedEventSource value:
                sourceNative->Type = IndexedEventSourceTypeNative.IndexedEventSource;
                sourceNative->Id = value.Id;
                sourceNative->Name = value.Name;
                break;
        }

        return sourceNative;
    }

    public static unsafe void ReleaseNative(IndexedEventSourceNative* sourceNative)
    {
        if ((nint)sourceNative == 0)
        {
            return;
        }

        sourceNative->Name.Release();
        Marshal.FreeHGlobal((nint)sourceNative);
    }

    public override object? ConvertNativeToManaged(IntPtr native)
    {
        if (native == IntPtr.Zero)
        {
            return null;
        }

        var result = CreateAndFillManaged(native);
        return result;
    }

    public override IntPtr ConvertManagedToNative(object? managed)
    {
        unsafe
        {
            var result = (IntPtr)CreateAndFillNative(managed);
            return result;
        }
    }

    public override void CleanUpFromManaged(IntPtr ptr)
    {
        unsafe
        {
            ReleaseNative((IndexedEventSourceNative*)ptr);
        }
    }

    public override void CleanUpFromNative(IntPtr ptr) =>
        ErrorCheck.SafeCall(Import.Release(Isolate.CurrentThread, ptr));

    // ToDo: waiting for the MDAPI-315
    public override void CleanUpListFromNative(IntPtr ptr) =>
        throw new NotImplementedException(); // ErrorCheck.SafeCall(Import.ReleaseList(Isolate.CurrentThread, ptr));

    private static class Import
    {
        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_IndexedEventSource_release")]
        public static extern int Release(nint thread, nint handle);

        // ToDo: waiting for the MDAPI-315
        // [DllImport(
        //     ImportInfo.DllName,
        //     CallingConvention = CallingConvention.Cdecl,
        //     CharSet = CharSet.Ansi,
        //     EntryPoint = "dxfg_CList_IndexedEventSource_release")]
        // public static extern int ReleaseList(nint thread, nint handle);
    }
}
