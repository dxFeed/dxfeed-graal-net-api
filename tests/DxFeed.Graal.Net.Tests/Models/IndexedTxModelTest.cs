// <copyright file="IndexedTxModelTest.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Api;
using DxFeed.Graal.Net.Events;
using DxFeed.Graal.Net.Events.Market;
using DxFeed.Graal.Net.Models;

namespace DxFeed.Graal.Net.Tests.Models;

[TestFixture]
public class IndexedTxModelTest : AbstractTxModelTest<Order, IndexedTxModel<Order>, IndexedTxModel<Order>.Builder>
{
    [Test]
    public void TestInitialState()
    {
        var builder = IndexedTxModel<Order>.NewBuilder();
        Assert.Throws<InvalidOperationException>(() => builder.Build()); // if the symbol and feed is not set

        Model = builder.WithFeed(DXFeed.GetInstance()).WithSymbol(TestSymbol).Build();
        Assert.Multiple(() =>
        {
            Assert.That(Model.IsBatchProcessing, Is.True);
            Assert.That(Model.IsSnapshotProcessing, Is.False);
            Assert.That(Model.GetSources(), Is.Empty);
        });
    }

    [Test]
    public void TestChangeSource()
    {
        var dexOrder = CreateOrder(OrderSource.DEX, 1);
        var ntvOrder = CreateOrder(OrderSource.ntv, 2);
        var ntvOrderUpperCase = CreateOrder(OrderSource.NTV, 3);

        Model = Builder()
            .WithSources(OrderSource.AGGREGATE_ASK, OrderSource.AGGREGATE_BID) // add two sources
            .WithSources(OrderSource.ntv, OrderSource.NTV) // override previous sources
            .Build();

        var sources = Model.GetSources();
        Assert.Multiple(() =>
        {
            Assert.That(sources, Has.Count.EqualTo(2));
            Assert.That(!sources.Except(new HashSet<OrderSource> { OrderSource.ntv, OrderSource.NTV }).Any());
        });

        Publish(dexOrder); // publish an unsubscribed source
        AssertIsChanged(false);

        Publish(ntvOrder, ntvOrderUpperCase, dexOrder); // publish two subscribed and unsubscribed sources
        AssertIsChanged(true);
        AssertSnapshotNotification(2);
        AssertReceivedEventCount(2);

        Model.SetSources(OrderSource.DEX); // change source
        sources = Model.GetSources();
        Assert.Multiple(() =>
        {
            Assert.That(sources, Has.Count.EqualTo(1));
            Assert.That(sources, Does.Contain(OrderSource.DEX));
        });

        Publish(dexOrder); // publish a subscribed source
        AssertIsChanged(true);
        AssertSnapshotNotification(1);
        AssertReceivedEventCount(3);
    }

    [Test]
    public void TestEmptySource()
    {
        var dexOrder = IndexedTxModelTest.CreateOrder(OrderSource.DEX, 1);
        var ntvOrder = IndexedTxModelTest.CreateOrder(OrderSource.ntv, 2);
        var ntvOrderUpperCase = IndexedTxModelTest.CreateOrder(OrderSource.NTV, 3);

        Model = Builder().Build();
        var sources = Model.GetSources();
        Assert.That(sources, Is.Empty);

        Publish(ntvOrder, ntvOrderUpperCase, dexOrder); // empty sources means subscribing to all available sources
        AssertIsChanged(true);
        AssertSnapshotNotification(3);
        AssertReceivedEventCount(3);
    }

    /// <summary>
    /// Asserts the properties of an event.
    /// </summary>
    /// <param name="index">Index of the event.</param>
    /// <param name="size">Size of the event.</param>
    /// <param name="eventFlags">Flags associated with the event.</param>
    protected override void AssertEvent(int index, double size, int eventFlags)
    {
        var order = ReceivedEvents.Dequeue();
        Assert.Multiple(() =>
        {
            Assert.That(order.EventSymbol, Is.EqualTo(TestSymbol));
            Assert.That(index, Is.EqualTo(order.Index));
            Assert.That(size, Is.EqualTo(order.Size));
            Assert.That(eventFlags, Is.EqualTo(order.EventFlags));
        });
    }

    /// <summary>
    /// Creates an event.
    /// </summary>
    /// <param name="index">Index of the event.</param>
    /// <param name="size">Size of the event.</param>
    /// <param name="eventFlags">Flags associated with the event.</param>
    /// <returns>A new instance of <see cref="Order"/>.</returns>
    protected override Order CreateEvent(int index, double size, int eventFlags) =>
        new(TestSymbol)
        {
            Index = index,
            EventSource = OrderSource.DEFAULT,
            Size = size,
            EventFlags = eventFlags,
            OrderSide = Side.Buy
        };

    /// <summary>
    /// Creates a builder for the <see cref="IndexedTxModel{Order}"/>.
    /// </summary>
    /// <returns>A new instance of <see cref="IndexedTxModel{Order}.Builder"/>.</returns>
    protected override IndexedTxModel<Order>.Builder Builder() =>
        IndexedTxModel<Order>.NewBuilder()
            .WithFeed(Feed)
            .WithSymbol(TestSymbol)
            .WithListener((_, events, isSnapshot) =>
            {
                ++ListenerNotificationCounter;
                if (isSnapshot)
                {
                    ++SnapshotNotificationCounter;
                }

                foreach (var e in events)
                {
                    ReceivedEvents.Enqueue(e);
                }
            });

    /// <summary>
    /// Creates an order with the specified source and size.
    /// </summary>
    /// <param name="source">Source of the order.</param>
    /// <param name="size">Size of the order.</param>
    /// <returns>A new instance of <see cref="Order"/>.</returns>
    private static Order CreateOrder(OrderSource source, double size) =>
        new(TestSymbol)
        {
            Index = 0,
            EventSource = source,
            Size = size,
            EventFlags = EventFlags.SnapshotBegin | EventFlags.SnapshotEnd,
            OrderSide = Side.Buy
        };

    /// <summary>
    /// Publishes the specified orders.
    /// </summary>
    /// <param name="orders">Orders to publish.</param>
    private void Publish(params IEventType[] orders)
    {
        Publisher.PublishEvents(orders);
        Executor.ProcessAllPendingTasks();
    }
}
