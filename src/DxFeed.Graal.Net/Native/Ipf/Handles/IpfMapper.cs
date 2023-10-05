// <copyright file="IpfMapper.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Ipf;

namespace DxFeed.Graal.Net.Native.Ipf.Handles;

internal class IpfMapper
{
    public static unsafe InstrumentProfile Convert(IpfNative* eventType)
    {
        var ipf = new InstrumentProfile();
        ipf.Type = eventType->type;
        ipf.Symbol = eventType->symbol;
        ipf.Description = eventType->description;
        ipf.LocalSymbol = eventType->localSymbol;
        ipf.LocalDescription = eventType->localDescription;
        ipf.Country = eventType->country;
        ipf.OPOL = eventType->opol;
        ipf.ExchangeData = eventType->exchangeData;
        ipf.Exchanges = eventType->exchanges;
        ipf.Currency = eventType->currency;
        ipf.BaseCurrency = eventType->baseCurrency;
        ipf.CFI = eventType->cfi;
        ipf.ISIN = eventType->isin;
        ipf.SEDOL = eventType->sedol;
        ipf.CUSIP = eventType->cusip;
        ipf.ICB = eventType->icb;
        ipf.SIC = eventType->sic;
        ipf.Multiplier = eventType->multiplier;
        ipf.Product = eventType->product;
        ipf.Underlying = eventType->underlying;
        ipf.SPC = eventType->spc;
        ipf.AdditionalUnderlyings = eventType->additionalUnderlyings;
        ipf.MMY = eventType->mmy;
        ipf.Expiration = eventType->expiration;
        ipf.LastTrade = eventType->lastTrade;
        ipf.Strike = eventType->strike;
        ipf.OptionType = eventType->optionType;
        ipf.ExpirationStyle = eventType->expirationStyle;
        ipf.SettlementStyle = eventType->settlementStyle;
        ipf.PriceIncrements = eventType->priceIncrements;
        ipf.TradingHours = eventType->tradingHours;
        return ipf;
    }

}
