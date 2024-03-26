// <copyright file="EventListenerFunc.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Native.Events;
using DxFeed.Graal.Net.Native.Interop;

namespace DxFeed.Graal.Net.Native.Subscription;

/// <summary>
/// Function pointer to the event listener.
/// </summary>
/// <param name="thread">The pointer to a run-time data structure for the thread.</param>
/// <param name="events">The pointer-to-pointer events (array of pointers to events).</param>
/// <param name="userData">The pointer to user data.</param>
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal unsafe delegate void EventListenerFunc(nint thread, ListNative<EventTypeNative>* events, nint userData);
