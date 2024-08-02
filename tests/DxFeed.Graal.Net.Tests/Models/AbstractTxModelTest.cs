// <copyright file="AbstractTxModelTest.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Api;
using DxFeed.Graal.Net.Events;
using DxFeed.Graal.Net.Models;
using DxFeed.Graal.Net.Native.Executors;
using static DxFeed.Graal.Net.Events.EventFlags;

namespace DxFeed.Graal.Net.Tests.Models;

public abstract class AbstractTxModelTest<TE, TM, TB>
    where TE : class, IIndexedEvent
    where TM : AbstractTxModel<TE>
    where TB : AbstractTxModel<TE>.Builder<TB, TM>
{
    protected const string TestSymbol = "TEST-SYMBOL";

    internal InPlaceExecutor Executor { get; } = InPlaceExecutor.Create();

    private readonly List<TE> PublishedEvents = new();

    private DXEndpoint Endpoint;

    protected int ListenerNotificationCounter { get; set; }
    protected int SnapshotNotificationCounter { get; set; }

    protected static IEnumerable<TestCaseData> Params()
    {
        for (var i = 0; i < 4; i++)
        {
            var isBatchProcessing = (i & 1) != 0;
            var isSnapshotProcessing = (i & 2) != 0;
            yield return new TestCaseData(isBatchProcessing, isSnapshotProcessing)
                .SetName(
                    $"TestCase {i} - isBatchProcessing:{isBatchProcessing}; isSnapshotProcessing:{isSnapshotProcessing}");
        }
    }

    [SetUp]
    public void SetUp()
    {
        Endpoint = DXEndpoint.Create(DXEndpoint.Role.LocalHub);
        Endpoint.Executor(Executor);
        Feed = Endpoint.GetFeed();
        Publisher = Endpoint.GetPublisher();
    }

    [TearDown]
    public void TearDown()
    {
        SnapshotNotificationCounter = 0;
        ListenerNotificationCounter = 0;
        ReceivedEvents.Clear();
        PublishedEvents.Clear();
        Model?.Dispose();
        Endpoint.Dispose();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown() =>
        Executor.Dispose();

    [Test, TestCaseSource(nameof(Params))]
    public void TestSnapshotAndUpdate(bool isBatchProcessing, bool isSnapshotProcessing)
    {
        Model = Builder()
            .WithBatchProcessing(isBatchProcessing)
            .WithSnapshotProcessing(isSnapshotProcessing)
            .Build();

        AddToPublish(1, 1, SnapshotBegin);
        AddToPublish(0, 2, SnapshotEnd);
        AddToPublish(1, 3);
        AddToPublish(3, 4);
        AddToPublish(2, 5);
        PublishDeferred(false);
        AssertSnapshotNotification(1); // only one snapshot
        AssertListenerNotification(Model.IsBatchProcessing ? 2 : 4);
        AssertReceivedEventCount(5);
        AssertEvent(1, 1, Model.IsSnapshotProcessing ? 0 : SnapshotBegin);
        AssertEvent(0, 2, Model.IsSnapshotProcessing ? 0 : SnapshotEnd);
        AssertEvent(1, 3, 0); // event with index 1 is not merged
        AssertEvent(3, 4, 0);
        AssertEvent(2, 5, 0);
    }

    [Test, TestCaseSource(nameof(Params))]
    public void TestEmptySnapshot(bool isBatchProcessing, bool isSnapshotProcessing)
    {
        Model = Builder()
            .WithBatchProcessing(isBatchProcessing)
            .WithSnapshotProcessing(isSnapshotProcessing)
            .Build();

        AddToPublish(0, 1, SnapshotBegin | SnapshotEnd | RemoveEvent);
        PublishDeferred(false);
        AssertSnapshotNotification(1); // only one snapshot
        if (Model.IsSnapshotProcessing)
        {
            AssertReceivedEventCount(0); // event with RemoveEvent flag was removed inside the snapshot
        }
        else
        {
            AssertReceivedEventCount(1); // event with RemoveEvent flag is saved
            AssertEvent(0, 1, SnapshotBegin | SnapshotEnd | RemoveEvent);
        }
    }

    [Test, TestCaseSource(nameof(Params))]
    public void TestSnapshotWithPending(bool isBatchProcessing, bool isSnapshotProcessing)
    {
        Model = Builder()
            .WithBatchProcessing(isBatchProcessing)
            .WithSnapshotProcessing(isSnapshotProcessing)
            .Build();

        AddToPublish(1, 1, SnapshotBegin);
        PublishDeferred(false);
        AssertIsChanged(false); // not processed yet
        AssertReceivedEventCount(0);

        AddToPublish(0, 2, SnapshotEnd | TxPending);
        PublishDeferred(false);
        AssertIsChanged(false); // not processed yet, because the pending flag is set

        AddToPublish(1, 3); // event without pending
        PublishDeferred(false);
        AssertListenerNotification(1); // since the transaction ended here
        AssertSnapshotNotification(1); // and it's all one snapshot
        if (Model.IsSnapshotProcessing)
        {
            AssertReceivedEventCount(2); // the same indices within a snapshot were merged
            AssertEvent(1, 3, 0);
            AssertEvent(0, 2, 0);
        }
        else
        {
            AssertReceivedEventCount(3);
            AssertEvent(1, 1, SnapshotBegin);
            AssertEvent(0, 2, SnapshotEnd | TxPending);
            AssertEvent(1, 3, 0);
        }
    }

    [Test, TestCaseSource(nameof(Params))]
    public void TestMultipleSnapshot(bool isBatchProcessing, bool isSnapshotProcessing)
    {
        Model = Builder()
            .WithBatchProcessing(isBatchProcessing)
            .WithSnapshotProcessing(isSnapshotProcessing)
            .Build();

        AddToPublish(1, 1, SnapshotBegin);
        PublishDeferred(false);
        AssertIsChanged(false); // not processed yet
        AssertReceivedEventCount(0);

        AddToPublish(0, 2, SnapshotEnd);
        AddToPublish(2, 3, SnapshotBegin);
        PublishDeferred(false);
        AssertListenerNotification(1);
        AssertSnapshotNotification(1); // only one snapshot so far, beginning of the second one is in the buffer
        AssertReceivedEventCount(2);
        AssertEvent(1, 1, Model.IsSnapshotProcessing ? 0 : SnapshotBegin);
        AssertEvent(0, 2, Model.IsSnapshotProcessing ? 0 : SnapshotEnd);

        AddToPublish(0, 4, SnapshotEnd); // end of second snapshot
        AddToPublish(3, 5); // update after second snapshot
        PublishDeferred(false);
        AssertSnapshotNotification(1);
        AssertListenerNotification(2);
        AssertReceivedEventCount(3);
        AssertEvent(2, 3, Model.IsSnapshotProcessing ? 0 : SnapshotBegin);
        AssertEvent(0, 4, Model.IsSnapshotProcessing ? 0 : SnapshotEnd);
        AssertEvent(3, 5, 0);
    }

    [Test, TestCaseSource(nameof(Params))]
    public void TestMultipleSnapshotInOneBatch(bool isBatchProcessing, bool isSnapshotProcessing)
    {
        Model = Builder()
            .WithBatchProcessing(isBatchProcessing)
            .WithSnapshotProcessing(isSnapshotProcessing)
            .Build();

        AddToPublish(0, 1, SnapshotBegin | SnapshotEnd);
        AddToPublish(0, 2, SnapshotBegin | SnapshotSnip);
        AddToPublish(0, 3, SnapshotBegin | RemoveEvent | SnapshotSnip | SnapshotEnd);
        PublishDeferred(false);
        AssertListenerNotification(3);
        AssertSnapshotNotification(3);
        if (Model.IsSnapshotProcessing)
        {
            AssertReceivedEventCount(2); // no event with RemoveEvent flag
            AssertEvent(0, 1, 0);
            AssertEvent(0, 2, 0);
        }
        else
        {
            AssertReceivedEventCount(3);
            AssertEvent(0, 1, SnapshotBegin | SnapshotEnd);
            AssertEvent(0, 2, SnapshotBegin | SnapshotSnip);
            AssertEvent(0, 3, SnapshotBegin | RemoveEvent | SnapshotSnip | SnapshotEnd);
        }
    }

    [Test, TestCaseSource(nameof(Params))]
    public void TestIncompleteSnapshot(bool isBatchProcessing, bool isSnapshotProcessing)
    {
        Model = Builder()
            .WithBatchProcessing(isBatchProcessing)
            .WithSnapshotProcessing(isSnapshotProcessing)
            .Build();

        AddToPublish(1, 1, SnapshotBegin);
        PublishDeferred(false);
        AssertIsChanged(false); // not processed yet
        AssertReceivedEventCount(0);

        AddToPublish(2, 2, SnapshotBegin); // yet another snapshot begins
        AddToPublish(3, 3); // event part of a snapshot
        PublishDeferred(false);
        AssertIsChanged(false); // not processed yet
        AssertReceivedEventCount(0);

        AddToPublish(4, 4, SnapshotBegin); // start new snapshot
        PublishDeferred(false);
        AssertIsChanged(false);
        AssertReceivedEventCount(0);

        AddToPublish(0, 5, SnapshotEnd); // full snapshot
        AddToPublish(5, 6); // update event after the snapshot end in the same batch
        AddToPublish(6, 7); // yet another update event after the snapshot end in the same batch
        PublishDeferred(false);
        AssertListenerNotification(Model.IsBatchProcessing ? 2 : 3);
        AssertSnapshotNotification(1); // of which one snapshot
        AssertReceivedEventCount(4); // chunks of previous snapshots have been deleted
        AssertEvent(4, 4, Model.IsSnapshotProcessing ? 0 : SnapshotBegin);
        AssertEvent(0, 5, Model.IsSnapshotProcessing ? 0 : SnapshotEnd);
        AssertEvent(5, 6, 0);
        AssertEvent(6, 7, 0);

        AddToPublish(7, 4, SnapshotBegin); // the snapshot hasn't ended yet
        PublishDeferred(false);
        AssertIsChanged(false); // not processed yet
    }

    [Test, TestCaseSource(nameof(Params))]
    public void TestPending(bool isBatchProcessing, bool isSnapshotProcessing)
    {
        Model = Builder()
            .WithBatchProcessing(isBatchProcessing)
            .WithSnapshotProcessing(isSnapshotProcessing)
            .Build();

        AddToPublish(0, 1, SnapshotBegin | SnapshotEnd);
        PublishDeferred(false);
        AssertIsChanged(true);
        AssertIsSnapshot(true);
        AssertReceivedEventCount(1);

        AddToPublish(1, 2, TxPending); // publish pending event
        AddToPublish(2, 3, TxPending); // publish pending event, same index as the previous one
        PublishDeferred(false);
        AssertIsChanged(false); // not processed yet

        AddToPublish(3, 4, 0); // publish without pending
        AddToPublish(4, 5, 0); // publish without pending
        PublishDeferred(false);
        AssertListenerNotification(Model.IsBatchProcessing ? 1 : 2);
        AssertIsSnapshot(false);
        AssertReceivedEventCount(5); // all published events, without merge
        AssertEvent(0, 1, Model.IsSnapshotProcessing ? 0 : SnapshotBegin | SnapshotEnd);
        AssertEvent(1, 2, TxPending);
        AssertEvent(2, 3, TxPending);
        AssertEvent(3, 4, 0);
        AssertEvent(4, 5, 0);
    }

    [Test, TestCaseSource(nameof(Params))]
    public void TestEventsWithoutSnapshot(bool isBatchProcessing, bool isSnapshotProcessing)
    {
        Model = Builder()
            .WithBatchProcessing(isBatchProcessing)
            .WithSnapshotProcessing(isSnapshotProcessing)
            .Build();

        AddToPublish(2, 1);
        AddToPublish(3, 2);
        AddToPublish(1, 3);
        AddToPublish(1, 4); // same index as the previous one
        AddToPublish(0, 5);
        PublishDeferred(false);
        AssertIsChanged(true); // bypass all events without snapshot
        AssertReceivedEventCount(5);
        AssertEvent(2, 1, 0);
        AssertEvent(3, 2, 0);
        AssertEvent(1, 3, 0);
        AssertEvent(1, 4, 0);
        AssertEvent(0, 5, 0);

        AddToPublish(0, 1, SnapshotBegin | SnapshotEnd);
        AddToPublish(1, 2);
        PublishDeferred(false);
        AssertIsChanged(true);
        AssertReceivedEventCount(2); // after receiving a snapshot, all events are received
        AssertEvent(0, 1, Model.IsSnapshotProcessing ? 0 : SnapshotBegin | SnapshotEnd);
        AssertEvent(1, 2, 0);
    }

    [Test, TestCaseSource(nameof(Params))]
    public void TestSnapshotWithRemoveAndPending(bool isBatchProcessing, bool isSnapshotProcessing)
    {
        Model = Builder()
            .WithBatchProcessing(isBatchProcessing)
            .WithSnapshotProcessing(isSnapshotProcessing)
            .Build();

        AddToPublish(7, 1, SnapshotBegin);
        AddToPublish(6, 2);
        AddToPublish(5, 3, RemoveEvent);
        AddToPublish(4, 4);
        AddToPublish(3, 5);
        AddToPublish(2, 6, TxPending);
        AddToPublish(2, 7);
        AddToPublish(1, 8);
        AddToPublish(0, double.NaN, SnapshotEnd | TxPending | RemoveEvent);
        AddToPublish(1, 9);
        PublishDeferred(false);
        if (Model.IsSnapshotProcessing)
        {
            AssertReceivedEventCount(6);
            AssertEvent(7, 1, 0);
            AssertEvent(6, 2, 0);
            AssertEvent(4, 4, 0);
            AssertEvent(3, 5, 0);
            AssertEvent(2, 7, 0);
            AssertEvent(1, 9, 0);
        }
        else
        {
            AssertReceivedEventCount(10);
            AssertEvent(7, 1, SnapshotBegin);
            AssertEvent(6, 2, 0);
            AssertEvent(5, 3, RemoveEvent);
            AssertEvent(4, 4, 0);
            AssertEvent(3, 5, 0);
            AssertEvent(2, 6, TxPending);
            AssertEvent(2, 7, 0);
            AssertEvent(1, 8, 0);
            AssertEvent(0, double.NaN, SnapshotEnd | TxPending | RemoveEvent);
            AssertEvent(1, 9, 0);
        }
    }

    [Test, TestCaseSource(nameof(Params))]
    public void TestCloseAbruptly(bool isBatchProcessing, bool isSnapshotProcessing)
    {
        Model = Builder()
            .WithBatchProcessing(isBatchProcessing)
            .WithSnapshotProcessing(isSnapshotProcessing)
            .Build();

        AddToPublish(0, 12.34, SnapshotBegin | SnapshotEnd);
        PublishDeferred(true);
        AssertIsChanged(true);
        AssertIsSnapshot(true);

        Model.Dispose();

        AddToPublish(2, 56.78, 0); // emulate stale events processing
        PublishDeferred(true);
        AssertIsChanged(false); // no change after close
    }


    protected DXFeed Feed { get; private set; }
    protected DXPublisher Publisher { get; private set; }
    protected TM? Model { get; set; }
    protected Queue<TE> ReceivedEvents { get; } = new();

    protected void AssertIsChanged(bool isChanged)
    {
        Assert.That(isChanged ? ListenerNotificationCounter > 0 : ListenerNotificationCounter == 0);
        ListenerNotificationCounter = 0;
    }

    protected void AssertSnapshotNotification(int count)
    {
        Assert.That(count, Is.EqualTo(SnapshotNotificationCounter));
        SnapshotNotificationCounter = 0;
    }

    protected void AssertReceivedEventCount(int count) =>
        Assert.That(count, Is.EqualTo(ReceivedEvents.Count));

    protected abstract void AssertEvent(int index, double size, int eventFlags);

    protected abstract TE CreateEvent(int index, double size, int eventFlags);

    protected abstract TB Builder();

    private void AssertIsSnapshot(bool isSnapshot)
    {
        Assert.That(isSnapshot ? SnapshotNotificationCounter > 0 : SnapshotNotificationCounter == 0);
        SnapshotNotificationCounter = 0;
    }

    private void AssertListenerNotification(int count)
    {
        Assert.That(count, Is.EqualTo(ListenerNotificationCounter));
        ListenerNotificationCounter = 0;
    }

    private void AddToPublish(int index, double size) =>
        AddToPublish(index, size, 0);

    private void AddToPublish(int index, double size, int eventFlags)
    {
        var e = CreateEvent(index, size, eventFlags);
        PublishedEvents.Add(e);
    }

    private void PublishDeferred(bool withPublisher)
    {
        if (withPublisher)
        {
            Publisher.PublishEvents(PublishedEvents);
            Executor.ProcessAllPendingTasks();
        }
        else
        {
            Model?.ProcessEvents(PublishedEvents);
        }

        PublishedEvents.Clear();
    }
}
