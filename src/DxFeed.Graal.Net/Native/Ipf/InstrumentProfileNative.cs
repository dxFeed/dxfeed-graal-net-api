// <copyright file="InstrumentProfileNative.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Ipf;
using DxFeed.Graal.Net.Native.Interop;

namespace DxFeed.Graal.Net.Native.Ipf;

[StructLayout(LayoutKind.Sequential)]
internal readonly record struct InstrumentProfileNative(
    StringNative Type,
    StringNative Symbol,
    StringNative Description,
    StringNative LocalSymbol,
    StringNative LocalDescription,
    StringNative Country,
    StringNative OPOL,
    StringNative ExchangeData,
    StringNative Exchanges,
    StringNative Currency,
    StringNative BaseCurrency,
    StringNative CFI,
    StringNative ISIN,
    StringNative SEDOL,
    StringNative CUSIP,
    int ICB,
    int SIC,
    double Multiplier,
    StringNative Product,
    StringNative Underlying,
    double SPC,
    StringNative AdditionalUnderlyings,
    StringNative MMY,
    int Expiration,
    int LastTrade,
    double Strike,
    StringNative OptionType,
    StringNative ExpirationStyle,
    StringNative SettlementStyle,
    StringNative PriceIncrements,
    StringNative TradingHours);
