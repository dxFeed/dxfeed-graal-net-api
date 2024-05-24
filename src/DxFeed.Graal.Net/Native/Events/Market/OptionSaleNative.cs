// <copyright file="OptionSaleNative.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Native.Interop;

namespace DxFeed.Graal.Net.Native.Events.Market;

/// <summary>
/// The structure contains all the fields required
/// to build an <see cref="OptionSaleNative"/>.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal record struct OptionSaleNative(
    EventTypeNative EventType,
    int EventFlags,
    long Index,
    long TimeSequence,
    int TimeNanoPart,
    char ExchangeCode,
    double Price,
    double Size,
    double BidPrice,
    double AskPrice,
    StringNative ExchangeSaleConditions,
    int Flags,
    double UnderlyingPrice,
    double Volatility,
    double Delta,
    StringNative OptionSymbol);
