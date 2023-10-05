// <copyright file="IpfNative.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Native.Events.Market;
using DxFeed.Graal.Net.Native.Interop;

namespace DxFeed.Graal.Net.Native.Ipf.Handles;

[StructLayout(LayoutKind.Sequential)]
internal readonly record struct IpfNative(
    StringNative type,
    StringNative symbol,
    StringNative description,
    StringNative localSymbol,
    StringNative localDescription,
    StringNative country,
    StringNative opol,
    StringNative exchangeData,
    StringNative exchanges,
    StringNative currency,
    StringNative baseCurrency,
    StringNative cfi,
    StringNative isin,
    StringNative sedol,
    StringNative cusip,
    int icb,
    int sic,
    double multiplier,
    StringNative product,
    StringNative underlying,
    double spc,
    StringNative additionalUnderlyings,
    StringNative mmy,
    int expiration,
    int lastTrade,
    double strike,
    StringNative optionType,
    StringNative expirationStyle,
    StringNative settlementStyle,
    StringNative priceIncrements,
    StringNative tradingHours);
