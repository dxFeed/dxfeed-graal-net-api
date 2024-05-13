// <copyright file="DXEndpointTest.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Api;
using DxFeed.Graal.Net.Api.Osub;
using DxFeed.Graal.Net.Events;
using DxFeed.Graal.Net.Events.Candles;
using DxFeed.Graal.Net.Events.Market;
using static DxFeed.Graal.Net.Api.DXEndpoint;
using static DxFeed.Graal.Net.Api.DXEndpoint.Role;

namespace DxFeed.Graal.Net.Tests.Api;

[TestFixture]
public class DXEndpointTest
{
    [Test]
    public void MultipleDisposeNotThrowException()
    {
        var endpoint = Create();
        Assert.DoesNotThrow(() => endpoint.Dispose());
        Assert.DoesNotThrow(() => endpoint.Dispose());
    }

    [Test]
    public void DefaultEndpointRoleIsFeed() =>
        Assert.Multiple(() =>
        {
            Assert.That(GetInstance().GetRole(), Is.EqualTo(Feed));
            Assert.That(Create().GetRole(), Is.EqualTo(Feed));
        });

    [Test]
    public void CreateMethodBuildCorrectEndpointRole() =>
        Assert.Multiple(() =>
        {
            Assert.That(Create(Feed).GetRole(), Is.EqualTo(Feed));
            Assert.That(Create(OnDemandFeed).GetRole(), Is.EqualTo(OnDemandFeed));
            Assert.That(Create(StreamFeed).GetRole(), Is.EqualTo(StreamFeed));
            Assert.That(Create(Publisher).GetRole(), Is.EqualTo(Publisher));
            Assert.That(Create(StreamPublisher).GetRole(), Is.EqualTo(StreamPublisher));
            Assert.That(Create(LocalHub).GetRole(), Is.EqualTo(LocalHub));
        });

    [Test]
    public void UnsupportedRoleThrowException() =>
        Assert.Throws<ArgumentException>(() => Create((Role)100500));

    [Test]
    public void GetInstanceReturnsSameObject()
    {
        foreach (var role in (Role[])Enum.GetValues(typeof(Role)))
        {
            var endpoint = GetInstance(role);
            Assert.That(GetInstance(role), Is.EqualTo(endpoint));
        }
    }

    [Test]
    public void GetFeedReturnsSameObject()
    {
        var feed = GetInstance().GetFeed();
        Assert.That(feed, Is.EqualTo(GetInstance().GetFeed()));
    }

    [Test]
    public void GetPublisherReturnsSameObject()
    {
        var publisher = GetInstance().GetPublisher();
        Assert.That(publisher, Is.EqualTo(GetInstance().GetPublisher()));
    }

    [Test]
    public void CheckDisposeCallClose()
    {
        var countdownEvent = new CountdownEvent(1);
        var endpoint = Create(Feed);
        var currentState = endpoint.GetState();
        endpoint.AddStateChangeListener((_, newState) =>
        {
            currentState = newState;
            countdownEvent.Signal();
        });

        endpoint.Dispose();

        Assert.Multiple(() =>
        {
            Assert.That(countdownEvent.Wait(new TimeSpan(0, 0, 3)), Is.True);
            Assert.That(currentState, Is.EqualTo(State.Closed));
        });
    }

    [Test]
    public void CheckAddSymbols()
    {
        using var endpoint = Create(Feed);

        var subscription = endpoint.GetFeed().CreateSubscription(typeof(Candle));
        var symbols = new List<object>
        {
            "AAPL_TEST",
            "AAPL_TEST{=d}",
            // WildcardSymbol.All, temporarily disabled
            CandleSymbol.ValueOf("AAPL0", CandlePeriod.Day),
            new TimeSeriesSubscriptionSymbol("AAPL2", 1),
            new IndexedEventSubscriptionSymbol("AAPL1", IndexedEventSource.DEFAULT),
            new IndexedEventSubscriptionSymbol("AAPL3", OrderSource.ntv),
            new IndexedEventSubscriptionSymbol("AAPL4", OrderSource.ValueOf(1))
        };
        subscription.SetSymbols(symbols);
        var resultSymbols = subscription.GetSymbols();
        Assert.That(new HashSet<object>(symbols).SetEquals(resultSymbols));
        subscription.Clear();
        Assert.That(subscription.GetSymbols().Any(), Is.False);
        subscription.SetSymbols(symbols.ToArray());
        Assert.That(new HashSet<object>(symbols).SetEquals(resultSymbols));
        subscription.AddSymbols(symbols);
        resultSymbols = subscription.GetSymbols();
        Assert.That(new HashSet<object>(symbols).SetEquals(resultSymbols));

        subscription.Clear();
        var tempList = new List<object>();
        foreach (var symbol in symbols)
        {
            tempList.Add(symbol);
            subscription.AddSymbols(symbol);
            Assert.That(new HashSet<object>(tempList).SetEquals(subscription.GetSymbols()));
        }
    }

    [Test]
    public void CheckOtcMarketsOrder()
    {
        const string SYMBOL1 = "AAPL_TEST1";
        const string SYMBOL2 = "AAPL_TEST2";

        var order1 = new OtcMarketsOrder(SYMBOL1)
        {
            OrderSide = Side.Buy,
            MarketMaker = "MM1",
            Scope = Scope.Order,
            Price = 10.0,
            Size = 1,
            Index = 1,
            QuoteAccessPayment = -30,
            IsOpen = true,
            IsUnsolicited = true,
            OtcMarketsPriceType = OtcMarketsPriceType.Actual,
            IsSaturated = true,
            IsAutoExecution = true,
            IsNmsConditional = true
        };
        var order2 = new OtcMarketsOrder(SYMBOL2)
        {
            OrderSide = Side.Buy,
            MarketMaker = "MM2",
            Scope = Scope.Order,
            Price = 10.0,
            Size = 1,
            Index = 1,
            QuoteAccessPayment = -30,
            IsOpen = true,
            IsUnsolicited = false,
            OtcMarketsPriceType = OtcMarketsPriceType.Wanted,
            IsSaturated = true,
            IsAutoExecution = false,
            IsNmsConditional = false
        };
        var resultList = new List<OtcMarketsOrder>();

        var endpoint = DXEndpoint.Create(LocalHub);
        var feed = endpoint.GetFeed();

        var publisher = endpoint.GetPublisher();
        var sub = feed.CreateSubscription(typeof(OtcMarketsOrder));
        var cde = new CountdownEvent(2);
        sub.AddEventListener(events =>
        {
            var eventTypes = events as IEventType[] ?? events.ToArray();
            resultList.AddRange(eventTypes.OfType<OtcMarketsOrder>());
            cde.Signal(eventTypes.Length);
        });
        sub.AddSymbols(SYMBOL1, SYMBOL2);
        publisher.PublishEvents(order1);
        publisher.PublishEvents(order2);

        cde.Wait(3000);

        foreach (var eventType in resultList)
        {
            var received = eventType;
            switch (eventType.EventSymbol)
            {
                case SYMBOL1:
                    Assert.Multiple(() =>
                    {
                        Assert.That(received.EventSymbol, Is.EqualTo(SYMBOL1));
                        Assert.That(received.OrderSide, Is.EqualTo(Side.Buy));
                        Assert.That(received.Scope, Is.EqualTo(Scope.Order));
                        Assert.That(Math.Abs(10.0 - received.Price), Is.LessThan(0.01));
                        Assert.That(Math.Abs(1 - received.Size), Is.LessThan(0.01));
                        Assert.That(received.MarketMaker, Is.EqualTo("MM1"));
                        Assert.That(received.QuoteAccessPayment, Is.EqualTo(-30));
                        Assert.That(received.IsOpen, Is.True);
                        Assert.That(received.IsUnsolicited, Is.True);
                        Assert.That(received.OtcMarketsPriceType, Is.EqualTo(OtcMarketsPriceType.Actual));
                        Assert.That(received.IsSaturated, Is.True);
                        Assert.That(received.IsAutoExecution, Is.True);
                        Assert.That(received.IsNmsConditional, Is.True);
                    });
                    break;
                case SYMBOL2:
                    Assert.Multiple(() =>
                    {
                        Assert.That(received.EventSymbol, Is.EqualTo(SYMBOL2));
                        Assert.That(received.OrderSide, Is.EqualTo(Side.Buy));
                        Assert.That(received.Scope, Is.EqualTo(Scope.Order));
                        Assert.That(Math.Abs(10.0 - received.Price), Is.LessThan(0.01));
                        Assert.That(Math.Abs(1 - received.Size), Is.LessThan(0.01));
                        Assert.That(received.MarketMaker, Is.EqualTo("MM2"));
                        Assert.That(received.QuoteAccessPayment, Is.EqualTo(-30));
                        Assert.That(received.IsOpen, Is.True);
                        Assert.That(received.IsUnsolicited, Is.False);
                        Assert.That(received.OtcMarketsPriceType, Is.EqualTo(OtcMarketsPriceType.Wanted));
                        Assert.That(received.IsSaturated, Is.True);
                        Assert.That(received.IsAutoExecution, Is.False);
                        Assert.That(received.IsNmsConditional, Is.False);
                    });
                    break;
                default:
                    Assert.Fail($"Undefined symbol {eventType.EventSymbol}");
                    break;
            }
        }
    }
}
