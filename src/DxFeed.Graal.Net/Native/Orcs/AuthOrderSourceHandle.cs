// <copyright file="AuthOrderSourceHandle.cs" company="Devexperts LLC">
// Copyright © 2025 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Native.ErrorHandling;
using DxFeed.Graal.Net.Native.Interop;

namespace DxFeed.Graal.Net.Native.Orcs;

internal sealed class AuthOrderSourceHandle : JavaHandle
{
    private static readonly ListMarshaler<StringMarshaler> StringListMarshaller = new();

    public Dictionary<int, ISet<string>> GetByIds()
    {
        unsafe
        {
            ErrorCheck.SafeCall(NativeGetByIds(CurrentThread, this, out var symbolsByOrderSourceIdMapEntryList));

            return ConvertToDictionary(symbolsByOrderSourceIdMapEntryList);
        }
    }

    private static unsafe Dictionary<int, ISet<string>> ConvertToDictionary(ListNative<IntPtr>* list)
    {
        try
        {
            var result = new Dictionary<int, ISet<string>>();

            for (var i = 0; i < list->Size; i++)
            {
                var native = (IntPtr)list->Elements[i];

                if (native == IntPtr.Zero)
                {
                    continue;
                }

                var entry = (SymbolsByOrderSourceIdMapEntryNative*)native;
                var strings = ((List<object>)StringListMarshaller.ConvertNativeToManaged((IntPtr)entry->Symbols))
                    .Cast<string>();

                result.Add(entry->OrderSourceId, new HashSet<string>(strings));
            }

            return result;
        }
        finally
        {
            ErrorCheck.SafeCall(ReleaseListWrapper(CurrentThread, list));
        }
    }

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "dxfg_AuthOrderSource_getByIds")]
    private static extern unsafe int NativeGetByIds(
        nint thread,
        AuthOrderSourceHandle authOrderSource,
        out ListNative<IntPtr>* symbolsByOrderSourceIdMapEntryList);

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "dxfg_symbols_by_order_source_id_map_entry_list_free")]
    private static extern unsafe int ReleaseListWrapper(
        nint thread,
        ListNative<IntPtr>* list);
}
