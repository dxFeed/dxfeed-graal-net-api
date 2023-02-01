// <copyright file="EventTypeMapper.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Events;

namespace DxFeed.Graal.Net.Native.Events;

/// <summary>
/// Abstract helper class for converting to/from TEventType and TEventTypeNative>.
/// </summary>
/// <typeparam name="TEventType">
/// The specific type implementing <see cref="IEventType"/>.
/// </typeparam>
/// <typeparam name="TEventTypeNative">
/// The specific native type, that includes  <see cref="EventTypeNative"/>.
/// </typeparam>
internal abstract class EventTypeMapper<TEventType, TEventTypeNative> : IEventMapper
    where TEventType : IEventType, new()
    where TEventTypeNative : unmanaged
{
    /// <inheritdoc/>
    public abstract unsafe IEventType FromNative(EventTypeNative* eventType);

    /// <inheritdoc/>
    public abstract unsafe EventTypeNative* ToNative(IEventType eventType);

    /// <inheritdoc/>
    public abstract unsafe void Release(EventTypeNative* eventType);

    /// <summary>
    /// Creates the <see cref="EventTypeNative"/> with <see cref="EventCodeNative"/> contained in attribute
    /// from TEventType.
    /// </summary>
    /// <param name="eventType">The EventType.</param>
    /// <returns>The created <see cref="EventTypeNative"/>.</returns>
    protected static EventTypeNative CreateEventType(TEventType eventType) =>
        CreateEventType(EventCodeAttribute.GetEventCode(eventType.GetType()), eventType);

    /// <summary>
    /// Creates the <see cref="EventTypeNative"/> with specified <see cref="EventCodeNative"/>
    /// from TEventType.
    /// </summary>
    /// <param name="eventCode">The <see cref="EventCodeNative"/>.</param>
    /// <param name="eventType">The TEventType.</param>
    /// <returns>The created <see cref="EventTypeNative"/>.</returns>
    protected static EventTypeNative CreateEventType(EventCodeNative eventCode, TEventType eventType) =>
        new() { EventCode = eventCode, EventSymbol = eventType.EventSymbol, EventTime = eventType.EventTime, };

    /// <inheritdoc cref="CreateEventType(EventTypeNative*)"/>
    protected static unsafe TEventType CreateEventType(TEventTypeNative* eventType) =>
        CreateEventType((EventTypeNative*)eventType);

    /// <summary>
    /// Creates the specified TEventType from <see cref="EventTypeNative"/>.
    /// Populates only the fields contained in <see cref="IEventType"/>.
    /// </summary>
    /// <param name="eventType">The <see cref="EventTypeNative"/>.</param>
    /// <returns>The created TEventType.</returns>
    protected static unsafe TEventType CreateEventType(EventTypeNative* eventType) =>
        new() { EventSymbol = eventType->EventSymbol, EventTime = eventType->EventTime, };

    /// <summary>
    /// Allocates an unmanaged pointer with type and size TEventTypeNative.
    /// The allocated memory is not fill, each specific implementation correctly fills in all fields.
    /// </summary>
    /// <returns>The unmanaged pointer.</returns>
    protected static unsafe TEventTypeNative* AllocEventType() =>
        (TEventTypeNative*)Marshal.AllocHGlobal(sizeof(TEventTypeNative));

    /// <summary>
    /// Releases resources associated with <see cref="EventTypeNative"/>.
    /// After calling this method, the use of <see cref="EventTypeNative"/> is forbidden.
    /// </summary>
    /// <param name="eventType">The unsafe pointer to <see cref="EventTypeNative"/>.</param>
    protected unsafe void ReleaseEventType(EventTypeNative* eventType)
    {
        if (eventType == (EventTypeNative*)0)
        {
            return;
        }

        eventType->EventSymbol.Release();
        Marshal.Release((nint)eventType);
    }

    /// <summary>
    /// Converts TEventTypeNative to TEventType.
    /// </summary>
    /// <param name="eventType">The unsafe pointer to TEventTypeNative.</param>
    /// <returns>The created TEventType.</returns>
    protected abstract unsafe TEventType Convert(TEventTypeNative* eventType);

    /// <summary>
    /// Converts TEventType to TEventTypeNative.
    /// This method allocates unmanaged memory, which must be released with <see cref="ReleaseEventType"/>.
    /// Each specific implementation correctly fills in all fields.
    /// </summary>
    /// <param name="eventType">The TEventType.</param>
    /// <returns>
    /// The allocated unmanaged unsafe pointer to TEventTypeNative.
    /// Must be released by <see cref="ReleaseEventType"/>.
    /// </returns>
    protected abstract unsafe TEventTypeNative* Convert(TEventType eventType);
}
