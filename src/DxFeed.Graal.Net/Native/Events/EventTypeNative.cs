// <copyright file="EventTypeNative.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Events;
using DxFeed.Graal.Net.Native.Interop;

namespace DxFeed.Graal.Net.Native.Events;

/// <summary>
/// The "base" type for all native events.
/// Contains an <see cref="EventCodeNative"/> associated with one of the managed types.
/// Must be included at the beginning of every native event structure to determine its type.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal readonly record struct EventTypeNative(
    EventCodeNative EventCode,
    StringNative EventSymbol,
    long EventTime)
{
    /// <summary>
    /// Converts a native event to the specified <see cref="IEventType"/>.
    /// This method fills only <see cref="IEventType.EventSymbol"/> and <see cref="IEventType.EventTime"/>.
    /// </summary>
    /// <typeparam name="T">The specified <see cref="IEventType"/>.</typeparam>
    /// <returns>The <see cref="IEventType"/>.</returns>
    public T ToEventType<T>()
        where T : IEventType, new() =>
        new() { EventSymbol = EventSymbol.ToString(), EventTime = EventTime, };
}
