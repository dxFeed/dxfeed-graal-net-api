// <copyright file="DXFeedEventListener.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Collections.Generic;
using DxFeed.Graal.Net.Events;

namespace DxFeed.Graal.Net.Api;

/// <summary>
/// The listener delegate for receiving events.
/// Invoked when events of type are received.
/// </summary>
/// <param name="events">The collection of received events.</param>
// ToDo Add IDXFeedEventListener interface.
public delegate void DXFeedEventListener(IEnumerable<IEventType> events);
