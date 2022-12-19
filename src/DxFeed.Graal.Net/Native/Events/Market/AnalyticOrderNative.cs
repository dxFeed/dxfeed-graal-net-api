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
/// Used to exchange data with native code.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal readonly struct AnalyticOrderNative
{
    public readonly OrderBaseNative OrderBase;
    public readonly double IcebergPeakSize;
    public readonly double IcebergHiddenSize;
    public readonly double IcebergExecutedSize;
    public readonly int IcebergFlags;
}
