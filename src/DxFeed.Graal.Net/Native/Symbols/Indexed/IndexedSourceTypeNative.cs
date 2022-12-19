// <copyright file="IndexedSourceTypeNative.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Events.Market;

namespace DxFeed.Graal.Net.Native.Symbols.Indexed;

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
