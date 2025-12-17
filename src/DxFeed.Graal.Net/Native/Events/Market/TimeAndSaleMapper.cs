// <copyright file="TimeAndSaleMapper.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Events;
using DxFeed.Graal.Net.Events.Market;

namespace DxFeed.Graal.Net.Native.Events.Market;

internal sealed class TimeAndSaleMapper : EventTypeMapper<TimeAndSale, TimeAndSaleNative>
{
    public override unsafe IEventType FromNative(EventTypeNative* eventType) =>
        Convert((TimeAndSaleNative*)eventType);

    public override unsafe EventTypeNative* ToNative(IEventType eventType) =>
        (EventTypeNative*)Convert((TimeAndSale)eventType);

    public override unsafe IEventType FillFromNative(EventTypeNative* nativeEventType, IEventType eventType) =>
        Fill((TimeAndSaleNative*)nativeEventType, (TimeAndSale)eventType);

    public override unsafe void Release(EventTypeNative* eventType)
    {
        if (eventType == (EventTypeNative*)0)
        {
            return;
        }

        var timeAndSaleNative = (TimeAndSaleNative*)eventType;
        timeAndSaleNative->Seller.Release();
        timeAndSaleNative->Buyer.Release();
        timeAndSaleNative->ExchangeSaleConditions.Release();
        ReleaseEventType(eventType);
    }

    protected override unsafe TimeAndSale Convert(TimeAndSaleNative* eventType)
    {
        var timeAndSale = new TimeAndSale();
        Fill(eventType, timeAndSale);
        return timeAndSale;
    }

    protected override unsafe TimeAndSaleNative* Convert(TimeAndSale eventType)
    {
        var ptr = AllocEventType();
        *ptr = new()
        {
            EventType = CreateEventType(eventType),
            EventFlags = eventType.EventFlags,
            Index = eventType.Index,
            TimeNanoPart = eventType.TimeNanoPart,
            ExchangeCode = eventType.ExchangeCode,
            Price = eventType.Price,
            Size = eventType.Size,
            BidPrice = eventType.BidPrice,
            AskPrice = eventType.AskPrice,
            ExchangeSaleConditions = eventType.ExchangeSaleConditions,
            Flags = eventType.Flags,
            Buyer = eventType.Buyer,
            Seller = eventType.Seller,
            TradeId = eventType.TradeId,
        };
        return ptr;
    }

    private static unsafe TimeAndSale Fill(TimeAndSaleNative* eventType, TimeAndSale timeAndSale)
    {
        AssignEventType((EventTypeNative*)eventType, timeAndSale);
        timeAndSale.EventFlags = eventType->EventFlags;
        timeAndSale.Index = eventType->Index;
        timeAndSale.TimeNanoPart = eventType->TimeNanoPart;
        timeAndSale.ExchangeCode = eventType->ExchangeCode;
        timeAndSale.Price = eventType->Price;
        timeAndSale.Size = eventType->Size;
        timeAndSale.BidPrice = eventType->BidPrice;
        timeAndSale.AskPrice = eventType->AskPrice;
        timeAndSale.ExchangeSaleConditions = eventType->ExchangeSaleConditions;
        timeAndSale.Flags = eventType->Flags;
        timeAndSale.Buyer = eventType->Buyer;
        timeAndSale.Seller = eventType->Seller;
        timeAndSale.TradeId = eventType->TradeId;

        return timeAndSale;
    }
}
