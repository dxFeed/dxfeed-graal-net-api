// <copyright file="IEventTypeNative.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Events;

namespace DxFeed.Graal.Net.Native.Events;

/// <summary>
/// Represents a native events.
/// </summary>
/// <typeparam name="T">The implementation <see cref="IEventType"/>.</typeparam>
internal interface IEventTypeNative<out T>
    where T : IEventType
{
    /// <summary>
    /// Converts a native event to the associated <see cref="IEventType"/>.
    /// </summary>
    /// <returns>The <see cref="IEventType"/>.</returns>
    T ToEventType();
}
