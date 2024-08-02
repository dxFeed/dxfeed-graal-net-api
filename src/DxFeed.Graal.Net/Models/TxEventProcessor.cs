// <copyright file="TxEventProcessor.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using DxFeed.Graal.Net.Events;
using DxFeed.Graal.Net.Events.Market;

namespace DxFeed.Graal.Net.Models;

internal class TxEventProcessor<TE>
    where TE : class, IIndexedEvent
{
    private readonly List<TE> _pendingEvents = new();
    private readonly ITransactionProcessor<TE> _transactionProcessor;
    private readonly ISnapshotProcessor<TE> _snapshotProcessor;
    private bool _isPartialSnapshot;
    private bool _isCompleteSnapshot;

    public TxEventProcessor(bool isBatchProcessing, bool isSnapshotProcessing, Listener<TE> listener)
    {
        _transactionProcessor = isBatchProcessing
            ? new BatchTransactionProcessor(listener)
            : new NotifyTransactionProcessor(listener);
        _snapshotProcessor = isSnapshotProcessing
            ? new ProcessingSnapshotProcessor(listener)
            : new NotifySnapshotProcessor(listener);
    }

    public delegate void Listener<in TE>(IEnumerable<TE> events, bool isSnapshot);

    private interface ITransactionProcessor<TE>
    {
        void ProcessTransaction(List<TE> events);

        void ProcessingBatch();
    }

    private interface ISnapshotProcessor<TE>
    {
        void ProcessSnapshot(List<TE> events);
    }

    public bool ProcessEvent(TE e)
    {
        if (EventFlags.IsSnapshotBegin(e))
        {
            _isPartialSnapshot = true;
            _isCompleteSnapshot = false;

            // Remove any unprocessed leftovers on new snapshot.
            _pendingEvents.Clear();
        }

        if (_isPartialSnapshot && EventFlags.IsSnapshotEndOrSnip(e))
        {
            _isPartialSnapshot = false;
            _isCompleteSnapshot = true;
        }

        // Defer processing of this event while snapshot in progress or tx pending.
        _pendingEvents.Add(e);
        if (EventFlags.IsPending(e) || _isPartialSnapshot)
        {
            // Waiting for the end of snapshot or transaction.
            return false;
        }

        // We have completed transaction or snapshot.
        if (_isCompleteSnapshot)
        {
            // Completed snapshot.
            _snapshotProcessor.ProcessSnapshot(_pendingEvents);
            _isCompleteSnapshot = false;
        }
        else
        {
            // Completed transaction.
            _transactionProcessor.ProcessTransaction(_pendingEvents);
        }

        _pendingEvents.Clear();
        return true;
    }

    public void ReceiveAllEventsInBatch() =>
        _transactionProcessor.ProcessingBatch();

    private sealed class BatchTransactionProcessor : ITransactionProcessor<TE>
    {
        private readonly Listener<TE> _listener;
        private readonly List<TE> _transactions = new();

        public BatchTransactionProcessor(Listener<TE> listener) =>
            _listener = listener;

        public void ProcessTransaction(List<TE> events) =>
            _transactions.AddRange(events);

        public void ProcessingBatch()
        {
            if (_transactions.Count == 0)
            {
                return;
            }

            _listener.Invoke(_transactions, false);
            _transactions.Clear();
        }
    }

    private sealed class NotifyTransactionProcessor : ITransactionProcessor<TE>
    {
        private readonly Listener<TE> _listener;

        public NotifyTransactionProcessor(Listener<TE> listener) =>
            _listener = listener;

        public void ProcessTransaction(List<TE> events) =>
            _listener.Invoke(events, false);

        public void ProcessingBatch()
        {
            // nothing to do
        }
    }

    private sealed class NotifySnapshotProcessor : ISnapshotProcessor<TE>
    {
        private readonly Listener<TE> _listener;

        public NotifySnapshotProcessor(Listener<TE> listener) =>
            _listener = listener;

        public void ProcessSnapshot(List<TE> events) =>
            _listener.Invoke(events, true);
    }

    private sealed class ProcessingSnapshotProcessor : ISnapshotProcessor<TE>
    {
        private readonly Listener<TE> _listener;
        private readonly OrderedDictionary _snapshot = new();

        public ProcessingSnapshotProcessor(Listener<TE> listener) =>
            _listener = listener;

        public void ProcessSnapshot(List<TE> events)
        {
            foreach (var e in events)
            {
                if (IsRemove(e))
                {
                    _snapshot.Remove(e.Index);
                }
                else
                {
                    e.EventFlags = 0;
                    _snapshot[e.Index] = e;
                }
            }

            _listener.Invoke(_snapshot.Values.OfType<TE>(), true);
            _snapshot.Clear();
        }

        private static bool IsRemove(TE e)
        {
            if ((e.EventFlags & EventFlags.RemoveEvent) != 0)
            {
                return true;
            }

            if (e is OrderBase order)
            {
                return !order.HasSize;
            }

            return false;
        }
    }
}
