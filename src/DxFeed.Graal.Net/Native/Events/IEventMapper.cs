// <copyright file="IEventMapper.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Events;

namespace DxFeed.Graal.Net.Native.Events;

/// <summary>
/// Represents an event mapper for converting to/from
/// <see cref="EventTypeNative"/> and <see cref="IEventType"/>.
/// </summary>
internal interface IEventMapper
{
    /// <summary>
    /// Converts an <see cref="EventTypeNative"/>
    /// to associated <see cref="IEventType"/> by <see cref="EventCodeNative"/>.
    /// </summary>
    /// <param name="eventType">The unsafe pointer to <see cref="EventTypeNative"/>.</param>
    /// <returns>The created <see cref="IEventType"/>.</returns>
    unsafe IEventType FromNative(EventTypeNative* eventType);

    /// <summary>
    /// Converts an <see cref="IEventType"/>
    /// to the associated <see cref="EventTypeNative"/> using the
    /// <see cref="EventCodeNative"/> contained in the attribute.
    /// This method allocates unmanaged memory, which must be released with <see cref="Release"/>.
    /// </summary>
    /// <param name="eventType">The <see cref="IEventType"/>.</param>
    /// <returns>
    /// The allocated unmanaged unsafe pointer to <see cref="EventTypeNative"/>.
    /// Must be released by <see cref="Release"/>.
    /// </returns>
    unsafe EventTypeNative* ToNative(IEventType eventType);

    /// <summary>
    /// Populates an existing managed event type instance with data from a native event type.
    /// </summary>
    /// <param name="nativeEventType">An unsafe pointer to a native <see cref="EventTypeNative"/> instance.</param>
    /// <param name="eventType">The managed <see cref="IEventType"/> instance to populate.</param>
    /// <returns>The populated <see cref="IEventType"/> instance.</returns>
    public unsafe IEventType FillFromNative(EventTypeNative* nativeEventType, IEventType eventType);

    /// <summary>
    /// Releases all associated resources.
    /// This method released unmanaged memory allocated by <see cref="ToNative"/>.
    /// </summary>
    /// <param name="eventType">The <see cref="EventTypeNative"/>.</param>
    unsafe void Release(EventTypeNative* eventType);
}
