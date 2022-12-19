// <copyright file="MarketEvent.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;

// ReSharper disable MemberCanBePrivate.Global
namespace DxFeed.Graal.Net.Native.Events.Market;

/// <summary>
/// "Base" type for all market events.
/// Must be included at the beginning of every market event structure.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal readonly struct MarketEvent
{
    public readonly BaseEventNative Base;
    public readonly nint EventSymbol; // A null-terminated UTF-8 string.
    public readonly long EventTime;
}
