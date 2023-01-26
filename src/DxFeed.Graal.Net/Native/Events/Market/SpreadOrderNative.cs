// <copyright file="SpreadOrderNative.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Events.Market;
using DxFeed.Graal.Net.Native.Interop;

namespace DxFeed.Graal.Net.Native.Events.Market;

/// <summary>
/// The structure contains all the fields required
/// to build an <see cref="SpreadOrder"/>.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal readonly record struct SpreadOrderNative(
        OrderBaseNative OrderBase,
        StringNative SpreadSymbol)
    : IEventTypeNative<SpreadOrder>
{
    /// <inheritdoc/>
    public SpreadOrder ToEventType()
    {
        var spreadOrder = OrderBase.ToEventType<SpreadOrder>();
        spreadOrder.SpreadSymbol = SpreadSymbol.ToString();
        return spreadOrder;
    }
}
