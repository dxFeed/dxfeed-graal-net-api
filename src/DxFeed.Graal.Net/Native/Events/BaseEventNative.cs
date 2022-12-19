// <copyright file="BaseEventNative.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;

namespace DxFeed.Graal.Net.Native.Events;

/// <summary>
/// "Base" type for all native events.
/// Must be included at the beginning of every native event structure to determine its type.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal readonly struct BaseEventNative
{
    public readonly EventCodeNative EventCode;
}
