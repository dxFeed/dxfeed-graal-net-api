// <copyright file="AuthOrderSource.cs" company="Devexperts LLC">
// Copyright © 2026 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using DxFeed.Graal.Net.Events.Market;
using DxFeed.Graal.Net.Native.Orcs;

namespace DxFeed.Graal.Net.Orcs;

/// <summary>
/// Represents information about available symbols to the client for the entitled collection of <see cref="OrderSource"/>.
/// </summary>
public class AuthOrderSource
{
    private readonly AuthOrderSourceHandle handle;

    private readonly object lockObject = new();
    private ConcurrentDictionary<int, ISet<string>>? symbolsByOrderSourceId;

    internal AuthOrderSource(AuthOrderSourceHandle handle) => this.handle = handle;

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public Dictionary<int, ISet<string>> GetByIds()
    {
        if (symbolsByOrderSourceId == null)
        {
            lock (lockObject)
            {
                symbolsByOrderSourceId = new ConcurrentDictionary<int, ISet<string>>(handle.GetByIds());
            }
        }

        return new Dictionary<int, ISet<string>>(symbolsByOrderSourceId);
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public Dictionary<OrderSource, ISet<string>> GetByOrderSources() =>
        GetByIds().ToDictionary(pair => OrderSource.ValueOf(pair.Key), pair => pair.Value);
}
