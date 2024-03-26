// <copyright file="SingleThreadTaskScheduler.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DxFeed.Graal.Net.Utils;

/// <summary>
/// Provides a task scheduler that ensures a single thread execution for all scheduled tasks.
/// </summary>
internal sealed class SingleThreadTaskScheduler : TaskScheduler, IDisposable
{
    private readonly BlockingCollection<Task> taskQueue = new();
    private readonly CancellationTokenSource cts = new();
    private readonly Thread thread;
    private volatile bool disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="SingleThreadTaskScheduler"/> class.
    /// </summary>
    public SingleThreadTaskScheduler()
    {
        thread = new Thread(Run) { IsBackground = true, };
        thread.Start();
    }

    /// <summary>
    /// Gets the maximum concurrency level supported by this scheduler, which is 1 for single-threaded execution.
    /// </summary>
    public override int MaximumConcurrencyLevel => 1;

    /// <inheritdoc/>
    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;
        cts.Cancel();
        thread.Join();
        cts.Dispose();
        taskQueue.Dispose();
    }

    /// <inheritdoc/>
    protected override void QueueTask(Task task) =>
        taskQueue.Add(task);

    /// <inheritdoc/>
    // Intentionally do not support executing tasks on the calling thread.
    protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued) =>
        false;

    /// <inheritdoc/>
    protected override IEnumerable<Task> GetScheduledTasks() =>
        taskQueue.ToArray();

    private void Run()
    {
        while (!disposed)
        {
            try
            {
                var task = taskQueue.Take(cts.Token);
                TryExecuteTask(task);
            }
            catch (OperationCanceledException)
            {
                // Handle cancellation due to Dispose.
                break;
            }
            catch (Exception ex)
            {
                // ToDo Add log entry.
                Console.WriteLine($"Exception in task execution: {ex}");
            }
        }
    }
}
