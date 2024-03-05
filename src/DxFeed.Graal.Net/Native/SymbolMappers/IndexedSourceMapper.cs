// <copyright file="IndexedSourceMapper.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Events;
using DxFeed.Graal.Net.Events.Market;

namespace DxFeed.Graal.Net.Native.SymbolMappers;

internal static unsafe class IndexedSourceMapper
{
    /// <summary>
    /// Creates unsafe <see cref="SymbolMarshaller.IndexedEventSourceNative"/> pointer from <see cref="IndexedEventSource"/>.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <returns>Returns unsafe pointer. The pointer must be freed with <see cref="ReleaseNative"/>. </returns>
    public static SymbolMarshaller.IndexedEventSourceNative* CreateNative(IndexedEventSource source)
    {
        var sourceNative = (SymbolMarshaller.IndexedEventSourceNative*)Marshal.AllocHGlobal(sizeof(SymbolMarshaller.IndexedEventSourceNative));
        sourceNative->Type = SymbolMarshaller.IndexedSourceTypeNative.IndexedEventSource;
        if (source is OrderSource)
        {
            sourceNative->Type = SymbolMarshaller.IndexedSourceTypeNative.OrderEventSource;
        }

        sourceNative->Id = source.Id;
        sourceNative->Name = source.Name;

        return sourceNative;
    }

    /// <summary>
    /// Release unsafe <see cref="SymbolMarshaller.IndexedEventSourceNative"/> pointer.
    /// </summary>
    /// <param name="sourceNative">The source unsafe pointer.</param>
    public static void ReleaseNative(SymbolMarshaller.IndexedEventSourceNative* sourceNative)
    {
        if ((nint)sourceNative == 0)
        {
            return;
        }

        sourceNative->Name.Release();
        Marshal.FreeHGlobal((nint)sourceNative);
    }
}
