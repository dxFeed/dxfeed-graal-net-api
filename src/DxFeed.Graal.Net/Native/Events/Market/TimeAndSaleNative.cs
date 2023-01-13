// <copyright file="TimeAndSaleNative.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Events.Market;

namespace DxFeed.Graal.Net.Native.Events.Market;

/// <summary>
/// The structure contains all the fields required
/// to build an <see cref="TimeAndSale"/>.
/// Used to exchange data with native code.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal readonly struct TimeAndSaleNative
{
    public readonly MarketEventNative MarketEvent;
    public readonly int EventFlags;
    public readonly long Index;
    public readonly int TimeNanoPart;
    public readonly char ExchangeCode;
    public readonly double Price;
    public readonly double Size;
    public readonly double BidPrice;
    public readonly double AskPrice;
    public readonly nint ExchangeSaleConditions; // A null-terminated UTF-8 string.
    public readonly int Flags;
    public readonly nint Buyer; // A null-terminated UTF-8 string.
    public readonly nint Seller; // A null-terminated UTF-8 string.
}
