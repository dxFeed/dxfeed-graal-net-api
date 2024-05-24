// <copyright file="HandleMap.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace DxFeed.Graal.Net.Native.Interop;

internal sealed class HandleMap<TListener, THandle>
    where TListener : notnull
    where THandle : JavaHandle
{
    private readonly ConcurrentDictionary<TListener, ConcurrentBag<THandle>> listeners = new();
    private readonly Func<TListener, THandle> createHandle;
    private readonly object syncRoot = new();

    public HandleMap(Func<TListener, THandle> createHandle) =>
        this.createHandle = createHandle;

    public THandle Add(TListener listener)
    {
        lock (syncRoot)
        {
            var handles = listeners.GetOrAdd(listener, _ => new ConcurrentBag<THandle>());
            var handle = createHandle(listener);
            handles.Add(handle);
            return handle;
        }
    }

    public bool TryRemove(TListener listener, out THandle handle)
    {
        handle = null!;
        lock (syncRoot)
        {
            if (listeners.TryGetValue(listener, out var handles) && handles.TryTake(out handle))
            {
                if (handles.IsEmpty)
                {
                    listeners.TryRemove(listener, out _);
                }

                return true;
            }
        }

        return false;
    }
}
