// <copyright file="QuoteMapper.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Events;
using DxFeed.Graal.Net.Events.Market;

namespace DxFeed.Graal.Net.Native.Events.Market;

internal class QuoteMapper : EventTypeMapper<Quote, QuoteNative>
{
    public override unsafe IEventType FromNative(EventTypeNative* eventType) =>
        Convert((QuoteNative*)eventType);

    public override unsafe EventTypeNative* ToNative(IEventType eventType) =>
        (EventTypeNative*)Convert((Quote)eventType);

    public override unsafe void Release(EventTypeNative* eventType) =>
        ReleaseEventType(eventType);

    protected override unsafe Quote Convert(QuoteNative* eventType)
    {
        var quote = CreateEventType(eventType);
        quote.TimeMillisSequence = eventType->TimeMillisSequence;
        quote.TimeNanoPart = eventType->TimeNanoPart;
        quote.BidTime = eventType->BidTime;
        quote.BidExchangeCode = eventType->BidExchangeCode;
        quote.BidPrice = eventType->BidPrice;
        quote.BidSize = eventType->BidSize;
        quote.AskTime = eventType->AskTime;
        quote.AskExchangeCode = eventType->AskExchangeCode;
        quote.AskPrice = eventType->AskPrice;
        quote.AskSize = eventType->AskSize;
        return quote;
    }

    protected override unsafe QuoteNative* Convert(Quote eventType)
    {
        var ptr = AllocEventType();
        *ptr = new()
        {
            EventType = CreateEventType(eventType),
            TimeMillisSequence = eventType.TimeMillisSequence,
            TimeNanoPart = eventType.TimeNanoPart,
            BidTime = eventType.BidTime,
            BidExchangeCode = eventType.BidExchangeCode,
            BidPrice = eventType.BidPrice,
            BidSize = eventType.BidSize,
            AskTime = eventType.AskTime,
            AskExchangeCode = eventType.AskExchangeCode,
            AskPrice = eventType.AskPrice,
            AskSize = eventType.AskSize,
        };
        return ptr;
    }
}
