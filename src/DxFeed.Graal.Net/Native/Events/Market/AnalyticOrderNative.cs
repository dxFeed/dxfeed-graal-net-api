// <copyright file="AnalyticOrderNative.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Events.Market;

namespace DxFeed.Graal.Net.Native.Events.Market;

/// <summary>
/// The structure contains all the fields required
/// to build an <see cref="AnalyticOrder"/>.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal readonly record struct AnalyticOrderNative(
        OrderBaseNative OrderBase,
        double IcebergPeakSize,
        double IcebergHiddenSize,
        double IcebergExecutedSize,
        int IcebergFlags)
    : IEventTypeNative<AnalyticOrder>
{
    /// <inheritdoc/>
    public AnalyticOrder ToEventType()
    {
        var analyticOrder = OrderBase.ToEventType<AnalyticOrder>();
        analyticOrder.IcebergPeakSize = IcebergPeakSize;
        analyticOrder.IcebergHiddenSize = IcebergHiddenSize;
        analyticOrder.IcebergExecutedSize = IcebergExecutedSize;
        analyticOrder.IcebergFlags = IcebergFlags;
        return analyticOrder;
    }
}
