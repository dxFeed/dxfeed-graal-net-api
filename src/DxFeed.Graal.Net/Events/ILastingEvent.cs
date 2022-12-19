// <copyright file="ILastingEvent.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Events.Market;

namespace DxFeed.Graal.Net.Events;

/// <summary>
/// Represents up-to-date information about some
/// condition or state of an external entity that updates in real-time.
/// For example, a <see cref="Quote"/> is an up-to-date information about best bid and best offer for a specific symbol.
/// <br/>
/// For more details see <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/event/LastingEvent.html">Javadoc</a>.
/// </summary>
/// <typeparam name="T">Type of the event symbol for this event type.</typeparam>
public interface ILastingEvent<out T> : IEventType<T>
{
}
