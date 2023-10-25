// <copyright file="InstrumentProfileMapper.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Ipf;

namespace DxFeed.Graal.Net.Native.Ipf;

internal static class InstrumentProfileMapper
{
    public static unsafe InstrumentProfile Convert(InstrumentProfileNative* eventType) =>
        new()
        {
            Type = eventType->Type!,
            Symbol = eventType->Symbol!,
            Description = eventType->Description!,
            LocalSymbol = eventType->LocalSymbol!,
            LocalDescription = eventType->LocalDescription!,
            Country = eventType->Country!,
            OPOL = eventType->OPOL!,
            ExchangeData = eventType->ExchangeData!,
            Exchanges = eventType->Exchanges!,
            Currency = eventType->Currency!,
            BaseCurrency = eventType->BaseCurrency!,
            CFI = eventType->CFI!,
            ISIN = eventType->ISIN!,
            SEDOL = eventType->SEDOL!,
            CUSIP = eventType->CUSIP!,
            ICB = eventType->ICB,
            SIC = eventType->SIC,
            Multiplier = eventType->Multiplier,
            Product = eventType->Product!,
            Underlying = eventType->Underlying!,
            SPC = eventType->SPC,
            AdditionalUnderlyings = eventType->AdditionalUnderlyings!,
            MMY = eventType->MMY!,
            Expiration = eventType->Expiration,
            LastTrade = eventType->LastTrade,
            Strike = eventType->Strike,
            OptionType = eventType->OptionType!,
            ExpirationStyle = eventType->ExpirationStyle!,
            SettlementStyle = eventType->SettlementStyle!,
            PriceIncrements = eventType->PriceIncrements!,
            TradingHours = eventType->TradingHours!,
        };
}
