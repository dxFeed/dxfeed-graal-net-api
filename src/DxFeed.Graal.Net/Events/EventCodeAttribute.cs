// <copyright file="EventCodeAttribute.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using DxFeed.Graal.Net.Native.Events;
using DxFeed.Graal.Net.Utils;

namespace DxFeed.Graal.Net.Events;

/// <summary>
/// Indicates that the attributed class contains event code native.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class EventCodeAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EventCodeAttribute"/> class.
    /// </summary>
    /// <param name="eventCode">The native event code.</param>
    public EventCodeAttribute(EventCodeNative eventCode) =>
        EventCode = eventCode;

    /// <summary>
    /// Gets native event code.
    /// </summary>
    public EventCodeNative EventCode { get; }

    /// <summary>
    /// Gets native event code from specified type.
    /// </summary>
    /// <param name="type">The specified type.</param>
    /// <returns>Returns native event code.</returns>
    /// <exception cref="ArgumentException">If specified type has no <see cref="EventCodeAttribute"/>.</exception>
    public static EventCodeNative GetEventCode(Type type)
    {
        var attribute = AttributeUtil.GetCustomAttribute<EventCodeAttribute>(type);
        return attribute?.EventCode
               ?? throw new ArgumentException($"{type.Name} has no {typeof(EventCodeAttribute)}", nameof(type));
    }

    /// <summary>
    /// Gets native event codes from specified types.
    /// </summary>
    /// <param name="types">The specified types.</param>
    /// <returns>Returns set containing native event codes.</returns>
    /// <exception cref="ArgumentException">
    /// If one on the specified type has no <see cref="EventCodeAttribute"/>.
    /// </exception>
    public static IEnumerable<EventCodeNative> GetEventCodes(params Type[] types)
    {
        var eventCodes = new HashSet<EventCodeNative>();
        foreach (var type in types)
        {
            eventCodes.Add(GetEventCode(type));
        }

        return eventCodes;
    }
}
