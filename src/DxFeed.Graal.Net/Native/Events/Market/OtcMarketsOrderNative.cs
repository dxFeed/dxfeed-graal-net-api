// <copyright file="OtcMarketsOrderNative.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Events.Market;

namespace DxFeed.Graal.Net.Native.Events.Market;

/// <summary>
/// The structure contains all the fields required
/// to build an <see cref="OtcMarketsOrder"/>.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal record struct OtcMarketsOrderNative(
    OrderNative Order,
    int QuoteAccessPayment,
    int OtcMarketsFlags);
