// <copyright file="IndexedEventSourceTypeNative.cs" company="Devexperts LLC">
// Copyright © 2026 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Events.Market;

namespace DxFeed.Graal.Net.Native.Interop;

/// <summary>
/// A wrapper enum for the dxfg_indexed_event_source_type_t Graal Native SDK enum.
/// </summary>
public enum IndexedEventSourceTypeNative
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
