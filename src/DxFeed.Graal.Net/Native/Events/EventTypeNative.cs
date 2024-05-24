// <copyright file="EventTypeNative.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Native.Interop;

namespace DxFeed.Graal.Net.Native.Events;

/// <summary>
/// The "base" type for all native events.
/// Contains an <see cref="EventCodeNative"/> associated with one of the managed types.
/// Must be included at the beginning of every native event structure to determine its type.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal record struct EventTypeNative(
    EventCodeNative EventCode,
    StringNative EventSymbol,
    long EventTime);
