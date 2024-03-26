// <copyright file="OptionSaleMapper.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Events;
using DxFeed.Graal.Net.Events.Market;

namespace DxFeed.Graal.Net.Native.Events.Market;

internal sealed class OptionSaleMapper : EventTypeMapper<OptionSale, OptionSaleNative>
{
    public override unsafe IEventType FromNative(EventTypeNative* eventType) =>
        Convert((OptionSaleNative*)eventType);

    public override unsafe EventTypeNative* ToNative(IEventType eventType) =>
        (EventTypeNative*)Convert((OptionSale)eventType);

    public override unsafe void Release(EventTypeNative* eventType)
    {
        if (eventType == (EventTypeNative*)0)
        {
            return;
        }

        var optionSaleNative = (OptionSaleNative*)eventType;
        optionSaleNative->OptionSymbol.Release();
        optionSaleNative->ExchangeSaleConditions.Release();
        ReleaseEventType(eventType);
    }

    protected override unsafe OptionSale Convert(OptionSaleNative* eventType)
    {
        var optionSale = CreateEventType(eventType);
        optionSale.EventFlags = eventType->EventFlags;
        optionSale.Index = eventType->Index;
        optionSale.TimeSequence = eventType->TimeSequence;
        optionSale.TimeNanoPart = eventType->TimeNanoPart;
        optionSale.ExchangeCode = eventType->ExchangeCode;
        optionSale.Price = eventType->Price;
        optionSale.Size = eventType->Size;
        optionSale.BidPrice = eventType->BidPrice;
        optionSale.AskPrice = eventType->AskPrice;
        optionSale.ExchangeSaleConditions = eventType->ExchangeSaleConditions;
        optionSale.Flags = eventType->Flags;
        optionSale.UnderlyingPrice = eventType->UnderlyingPrice;
        optionSale.Volatility = eventType->Volatility;
        optionSale.Delta = eventType->Delta;
        optionSale.OptionSymbol = eventType->OptionSymbol;
        return optionSale;
    }

    protected override unsafe OptionSaleNative* Convert(OptionSale eventType)
    {
        var ptr = AllocEventType();
        *ptr = new()
        {
            EventType = CreateEventType(eventType),
            EventFlags = eventType.EventFlags,
            Index = eventType.Index,
            TimeSequence = eventType.TimeSequence,
            TimeNanoPart = eventType.TimeNanoPart,
            ExchangeCode = eventType.ExchangeCode,
            Price = eventType.Price,
            Size = eventType.Size,
            BidPrice = eventType.BidPrice,
            AskPrice = eventType.AskPrice,
            ExchangeSaleConditions = eventType.ExchangeSaleConditions,
            Flags = eventType.Flags,
            UnderlyingPrice = eventType.UnderlyingPrice,
            Volatility = eventType.Volatility,
            Delta = eventType.Delta,
            OptionSymbol = eventType.OptionSymbol,
        };
        return ptr;
    }
}
