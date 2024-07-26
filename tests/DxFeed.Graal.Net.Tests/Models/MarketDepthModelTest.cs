// <copyright file="MarketDepthModelTest.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Api;
using DxFeed.Graal.Net.Events;
using DxFeed.Graal.Net.Events.Market;
using DxFeed.Graal.Net.Models;
using DxFeed.Graal.Net.Native.Executors;

// ReSharper disable PossibleMultipleEnumeration

namespace DxFeed.Graal.Net.Tests.Models;

[TestFixture]
public class MarketDepthModelTest
{
    private const string Symbol = "INDEX-TEST";

    private readonly InPlaceExecutor _executor = InPlaceExecutor.Create();
    private readonly List<Order> _publishedEvents = new();
    private readonly List<Order> _buyOrders = new();
    private readonly List<Order> _sellOrders = new();
    private DXEndpoint _endpoint;
    private DXFeed _feed;
    private DXPublisher _publisher;
    private MarketDepthModel<Order> _model;

    private int _listenerCalls;
    private int _changesBuy;
    private int _changesSell;

    [SetUp]
    public void SetUp()
    {
        _endpoint = DXEndpoint.Create(DXEndpoint.Role.LocalHub);
        _endpoint.Executor(_executor);
        _feed = _endpoint.GetFeed();
        _publisher = _endpoint.GetPublisher();
        _model = CreateBuilder().Build();
    }

    [TearDown]
    public void TearDown()
    {
        _listenerCalls = 0;
        _buyOrders.Clear();
        _publishedEvents.Clear();
        _sellOrders.Clear();
        _model.Dispose();
        _endpoint.Dispose();
    }


    [OneTimeTearDown]
    public void OneTimeTearDown() =>
        _executor.Dispose();

    [Test]
    public void TestRemoveNonExistent() =>
        PublishAndProcess(false, CreateOrder(0, Side.Buy, 1, 1, EventFlags.RemoveEvent));

    [Test]
    public void TestRemoveBySizeAndByFlags()
    {
        var o1 = CreateOrder(2, Side.Buy, 3, 1, 0);
        var o2 = CreateOrder(1, Side.Buy, 2, 1, 0);
        var o3 = CreateOrder(0, Side.Buy, 1, 1, EventFlags.SnapshotBegin | EventFlags.SnapshotEnd);

        PublishAndProcess(true, o3, o2, o1);
        CheckBuySize(3);
        CheckSellSize(0);
        CheckOrder(Side.Buy, o1, 0);
        CheckOrder(Side.Buy, o2, 1);
        CheckOrder(Side.Buy, o3, 2);

        PublishAndProcess(true, CreateOrder(2, Side.Buy, 1, double.NaN, 0));
        CheckBuySize(2);
        CheckSellSize(0);
        CheckOrder(Side.Buy, o2, 0);

        PublishAndProcess(true, CreateOrder(1, Side.Buy, 1, 1, EventFlags.RemoveEvent));
        CheckBuySize(1);
        CheckSellSize(0);
        CheckOrder(Side.Buy, o3, 0);

        PublishAndProcess(true, CreateOrder(0, Side.Buy, 1, 1, EventFlags.SnapshotEnd | EventFlags.RemoveEvent));
        CheckBookSize(0);
        CheckBuySize(0);
        CheckSellSize(0);
    }

    [Test]
    public void TestOrderChangeSide()
    {
        var buy = CreateOrder(0, Side.Buy, 1, 1, EventFlags.SnapshotBegin | EventFlags.SnapshotEnd);
        PublishAndProcess(true, buy);
        CheckBookSize(1);
        CheckBuySize(1);
        CheckOrder(Side.Buy, buy, 0);

        var sell = CreateOrder(0, Side.Sell, 1, 1, 0);
        PublishAndProcess(true, sell);
        CheckBookSize(1);
        CheckSellSize(1);
        CheckOrder(Side.Sell, sell, 0);
    }

    [Test]
    public void TestOrderPriorityAfterUpdate()
    {
        var b1 = CreateOrder(0, Side.Buy, 100, 1, EventFlags.SnapshotBegin | EventFlags.SnapshotEnd);
        var b2 = CreateOrder(1, Side.Buy, 150, 1, 0);
        var s1 = CreateOrder(3, Side.Sell, 150, 1, 0);
        var s2 = CreateOrder(2, Side.Sell, 100, 1, 0);

        PublishAndProcess(b1);
        CheckOrder(Side.Buy, b1, 0);
        PublishAndProcess(b2);
        CheckOrder(Side.Buy, b2, 0);
        CheckOrder(Side.Buy, b1, 1);

        PublishAndProcess(s1);
        CheckOrder(Side.Sell, s1, 0);
        PublishAndProcess(s2);
        CheckOrder(Side.Sell, s2, 0);
        CheckOrder(Side.Sell, s1, 1);
    }

    [Test]
    public void TestMultipleUpdatesWithMixedSides()
    {
        var buyLowPrice = CreateOrder(0, Side.Buy, 100, 1, EventFlags.SnapshotBegin | EventFlags.SnapshotEnd);
        var buyHighPrice = CreateOrder(1, Side.Buy, 200, 1, 0);
        var sellLowPrice = CreateOrder(2, Side.Sell, 150, 1, 0);
        var sellHighPrice = CreateOrder(3, Side.Sell, 250, 1, 0);

        PublishAndProcess(buyLowPrice, sellHighPrice, buyHighPrice, sellLowPrice);
        CheckBookSize(4);

        CheckOrder(Side.Buy, buyHighPrice, 0);
        CheckOrder(Side.Sell, sellLowPrice, 0);
    }

    [Test]
    public void TestDuplicateOrderIndexUpdatesExistingOrder()
    {
        var originalIndexOrder = CreateOrder(0, Side.Buy, 100, 1, EventFlags.SnapshotBegin | EventFlags.SnapshotEnd);
        var duplicateIndexOrder = CreateOrder(0, Side.Buy, 150, 1, 0);

        PublishAndProcess(originalIndexOrder, duplicateIndexOrder);
        CheckBookSize(1);
        CheckOrder(Side.Buy, duplicateIndexOrder, 0);
    }

    [Test]
    public void TestEnforceEntryLimit()
    {
        _model.Dispose();
        _model = CreateBuilder().WithDepthLimit(3).Build();
        PublishAndProcess(true, CreateOrder(0, Side.Buy, 5, 1, EventFlags.SnapshotBegin | EventFlags.SnapshotEnd));
        PublishAndProcess(true, CreateOrder(1, Side.Buy, 4, 1, 0));
        PublishAndProcess(true, CreateOrder(2, Side.Buy, 3, 1, 0));

        PublishAndProcess(false, CreateOrder(3, Side.Buy, 2, 1, 0)); // outside limit
        PublishAndProcess(false, CreateOrder(4, Side.Buy, 1, 1, 0)); // outside limit
        PublishAndProcess(false, CreateOrder(4, Side.Buy, 1, 2, 0)); // modify outside limit
        PublishAndProcess(false, CreateOrder(3, Side.Buy, 1, double.NaN, 0)); // remove outside limit
        PublishAndProcess(true, CreateOrder(2, Side.Buy, 3, 2, 0)); // update in limit
        PublishAndProcess(true, CreateOrder(1, Side.Buy, 3, double.NaN, 0)); // remove in limit

        PublishAndProcess(true, CreateOrder(4, Side.Sell, 1, 1, 0));
        PublishAndProcess(true, CreateOrder(5, Side.Sell, 2, 1, 0));
        PublishAndProcess(true, CreateOrder(6, Side.Sell, 3, 1, 0));

        PublishAndProcess(false, CreateOrder(7, Side.Sell, 4, 1, 0)); // outside limit
        CheckChanged(false);
        PublishAndProcess(false, CreateOrder(8, Side.Sell, 5, 1, 0)); // outside limit
        CheckChanged(false);
        PublishAndProcess(false, CreateOrder(8, Side.Sell, 5, 2, 0)); // modify outside limit
        CheckChanged(false);
        PublishAndProcess(false, CreateOrder(8, Side.Sell, 5, double.NaN, 0)); // remove outside limit
        CheckChanged(false);
        PublishAndProcess(true, CreateOrder(6, Side.Sell, 4, 2, 0)); // update in limit
        PublishAndProcess(true, CreateOrder(5, Side.Sell, 2, double.NaN, 0)); // remove in limit

        _model.SetDepthLimit(0); // disable limit
        PublishAndProcess(true, CreateOrder(4, Side.Buy, 1, 3, 0));
        PublishAndProcess(true, CreateOrder(8, Side.Sell, 1, 3, 0));

        _model.SetDepthLimit(1);
        PublishAndProcess(true, CreateOrder(0, Side.Buy, 2, 1, 0));
        PublishAndProcess(false, CreateOrder(1, Side.Buy, 2, 1, 0));
    }

    [Test]
    public void TestStressBuySellOrders()
    {
        PublishAndProcess(false,
            CreateOrder(0, Side.Buy, double.NaN, double.NaN,
                EventFlags.SnapshotBegin | EventFlags.SnapshotEnd | EventFlags.RemoveEvent));
        _listenerCalls = 0;
        _buyOrders.Clear();
        _publishedEvents.Clear();
        _sellOrders.Clear();
        _listenerCalls = 0;
        _changesBuy = 0;
        _changesSell = 0;

        var rnd = new Random(1);
        const int bookSize = 100;
        var book = new Order[bookSize];
        var expectedBuy = 0;
        var expectedSell = 0;
        for (var i = 0; i < 10000; i++)
        {
            var index = rnd.Next(bookSize);
            var order = CreateOrder(Scope.Order, rnd.Next(2) != 0 ? Side.Buy : Side.Sell, index, rnd.Next(10), '\0',
                null);
            var old = book[index];
            book[index] = order;
            var deltaBuy = OneIfBuy(order) - OneIfBuy(old);
            var deltaSell = OneIfSell(order) - OneIfSell(old);
            expectedBuy += deltaBuy;
            expectedSell += deltaSell;
            PublishAndProcess(order);
            switch (order.OrderSide)
            {
                case Side.Buy:
                    CheckChangesBuy(deltaBuy != 0 || (!Same(order, old) && old.OrderSide == Side.Buy) ? 1 : 0);
                    CheckChangesSell(OneIfSell(old));
                    break;
                case Side.Sell:
                    CheckChangesSell(deltaSell != 0 || (!Same(order, old) && old.OrderSide == Side.Sell) ? 1 : 0);
                    CheckChangesBuy(OneIfBuy(old));
                    break;
                default:
                    Assert.Fail();
                    break;
            }

            Assert.Multiple(() =>
            {
                Assert.That(_buyOrders, Has.Count.EqualTo(expectedBuy));
                Assert.That(_sellOrders, Has.Count.EqualTo(expectedSell));
            });
        }

        var snapshotOrder = CreateOrder(Scope.Order, Side.Undefined, 0, 0, '\0', null);
        snapshotOrder.EventFlags = EventFlags.SnapshotBegin | EventFlags.SnapshotEnd | EventFlags.RemoveEvent;
        _publisher.PublishEvents(new List<Order> { snapshotOrder });
        _executor.ProcessAllPendingTasks();
        CheckChangesBuy(expectedBuy > 0 ? 1 : 0);
        CheckChangesSell(expectedSell > 0 ? 1 : 0);
        Assert.Multiple(() =>
        {
            Assert.That(_buyOrders, Is.Empty);
            Assert.That(_sellOrders, Is.Empty);
        });
    }

    [Test]
    public void TestNegativeAggregationPeriod()
    {
        _model = CreateBuilder().WithAggregationPeriod(-1).Build();
        Assert.That(_model.GetAggregationPeriod(), Is.EqualTo(0));
        _model.SetAggregationPeriod(0);
        Assert.That(_model.GetAggregationPeriod(), Is.EqualTo(0));
        _model.SetAggregationPeriod(-1);
        Assert.That(_model.GetAggregationPeriod(), Is.EqualTo(0));
    }

    [Test]
    public void TestNegativeDepthLimit()
    {
        _model = CreateBuilder().WithDepthLimit(-1).Build();
        Assert.That(_model.GetDepthLimit(), Is.EqualTo(0));
        _model.SetDepthLimit(0);
        Assert.That(_model.GetDepthLimit(), Is.EqualTo(0));
        _model.SetDepthLimit(int.MaxValue);
        Assert.That(_model.GetDepthLimit(), Is.EqualTo(int.MaxValue));
        _model.SetDepthLimit(-1);
        Assert.That(_model.GetDepthLimit(), Is.EqualTo(0));
        _model.SetDepthLimit(int.MinValue);
        Assert.That(_model.GetDepthLimit(), Is.EqualTo(0));
    }

    private void CheckChanged(bool expected)
    {
        Assert.That(expected ? _listenerCalls > 0 : _listenerCalls == 0);
        _listenerCalls = 0;
    }

    private void CheckBookSize(int size) =>
        Assert.That(_buyOrders.Count + _sellOrders.Count, Is.EqualTo(size));

    private void CheckBuySize(int size) =>
        Assert.That(_buyOrders, Has.Count.EqualTo(size));

    private void CheckSellSize(int size) =>
        Assert.That(_sellOrders, Has.Count.EqualTo(size));

    private void CheckChangesBuy(int n)
    {
        Assert.That(_changesBuy, Is.EqualTo(n));
        _changesBuy = 0;
    }

    private void CheckChangesSell(int n)
    {
        Assert.That(_changesSell, Is.EqualTo(n));
        _changesSell = 0;
    }

    private void CheckOrder(Side side, Order order, int pos)
    {
        var orders = side == Side.Buy ? _buyOrders : _sellOrders;
        Assert.Multiple(() =>
        {
            Assert.That(orders, Has.Count.GreaterThan(pos));
            Assert.That(Same(order, orders[pos]));
        });
    }

    private void Publish(Order order) =>
        _publishedEvents.Add(order);

    private void Publish(params Order[] orders) =>
        _publishedEvents.AddRange(orders);

    private void Process()
    {
        _publisher.PublishEvents(_publishedEvents);
        _executor.ProcessAllPendingTasks();
        _publishedEvents.Clear();
    }

    private void PublishAndProcess(Order order)
    {
        Publish(order);
        Process();
    }

    private void PublishAndProcess(bool expected, Order order)
    {
        Publish(order);
        Process();
        CheckChanged(expected);
    }

    private void PublishAndProcess(params Order[] orders)
    {
        Publish(orders);
        Process();
    }

    private void PublishAndProcess(bool expected, params Order[] orders)
    {
        Publish(orders);
        Process();
        CheckChanged(expected);
    }

    private static int OneIfBuy(Order? order) =>
        order is { OrderSide: Side.Buy } and not { Size: 0 } ? 1 : 0;

    private static int OneIfSell(Order? order) =>
        order is { OrderSide: Side.Sell } and not { Size: 0 } ? 1 : 0;

    private static bool Same(Order order, Order old)
    {
        if (order is { Size: 0 })
        {
            return true; // order with zero size is the same as null (missing)
        }

        // Check just relevant attributes
        return order.Scope == old.Scope &&
               order.OrderSide == old.OrderSide &&
               order.Index == old.Index &&
               order.Size.Equals(old.Size) &&
               Equals(order.EventSource, old.EventSource);
    }

    private static Order CreateOrder(int index, Side side, double price, double size, int eventFlags) =>
        new(Symbol)
        {
            Index = index,
            Price = price,
            Size = size,
            EventFlags = eventFlags,
            OrderSide = side
        };

    private static Order CreateOrder(Scope scope, Side side, long index, int value, char exchange, string? mmid)
    {
        var order = new Order(Symbol);
        order.Scope = scope;
        order.Index = index;
        order.OrderSide = side;
        order.Price = value * 10;
        order.Size = value;
        order.ExchangeCode = exchange;
        order.MarketMaker = mmid;
        return order;
    }

    private MarketDepthModel<Order>.Builder CreateBuilder() =>
        MarketDepthModel<Order>.NewBuilder()
            .WithFeed(_feed)
            .WithSymbol(Symbol)
            .WithSources(OrderSource.DEFAULT)
            .WithListener((buyOrders, sellOrders) =>
            {
                _listenerCalls++;
                if (!_buyOrders.SequenceEqual(buyOrders))
                {
                    _changesBuy++;
                }

                _buyOrders.Clear();
                _buyOrders.AddRange(buyOrders);

                if (!_sellOrders.SequenceEqual(sellOrders))
                {
                    _changesSell++;
                }

                _sellOrders.Clear();
                _sellOrders.AddRange(sellOrders);
            });
}
