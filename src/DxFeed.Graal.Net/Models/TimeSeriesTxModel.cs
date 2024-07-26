// <copyright file="TimeSeriesTxModel.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Collections.Generic;
using DxFeed.Graal.Net.Api;
using DxFeed.Graal.Net.Api.Osub;
using DxFeed.Graal.Net.Events;

namespace DxFeed.Graal.Net.Models;

/// <summary>
/// An incremental model for time-series events.
/// This model manages all snapshot and transaction logic, subscription handling, and listener notifications.
///
/// <p>This model is designed to handle incremental transactions. Users of this model only see the list
/// of events in a consistent state. This model delays incoming events that are part of an incomplete snapshot
/// or ongoing transaction until the snapshot is complete or the transaction has ended. This model notifies
/// the user of received transactions through an installed <see cref="TxModelListener{TE}"/>.</p>
///
/// <h3>Configuration</h3>
///
/// <p>This model must be configured using the <see cref="Builder"/> class, as most configuration
/// settings cannot be changed once the model is built. This model requires configuration
/// <see cref="AbstractTxModel{TE}.Builder{TB,TM}.WithSymbol(object)"/> and <see cref="Builder.WithFromTime"/>
/// for subscription, and it must be <see cref="AbstractTxModel{TE}.Builder{TB,TM}.WithFeed(DXFeed)"/> attached
/// to a <see cref="DXFeed"/> instance to begin operation.
/// For ease of use, some of these configurations can be changed after the model is built,
/// see <see cref="SetFromTime"/>.</p>
///
/// <p>This model only supports single symbol subscriptions; multiple symbols cannot be configured.</p>
///
/// <h3>Resource management and closed models</h3>
///
/// <p>Attached model is a potential memory leak. If the pointer to attached model is lost, then there is no way
/// to detach this model from the feed and the model will not be reclaimed by the garbage collector as long as the
/// corresponding feed is still used. Detached model can be reclaimed by the garbage collector, but detaching model
/// requires knowing the pointer to the feed at the place of the call, which is not always convenient.</p>
///
/// <p>The convenient way to detach model from the feed is to call its
/// <see cref="AbstractTxModel{TE}.Dispose()"/> method.
/// Closed model becomes permanently detached from all feeds, removes all its listeners and is guaranteed
/// to be reclaimable by the garbage collector as soon as all external references to it are cleared.</p>
///
/// <h3>Threads and locks</h3>
///
/// <p>This class is thread-safe and can be used concurrently from multiple threads without external synchronization.</p>
/// The corresponding <see cref="TxModelListener{TE}"/> to never be concurrent.
/// </summary>
/// <typeparam name="TE">The type of  time series events processed by this model.</typeparam>
public sealed class TimeSeriesTxModel<TE> : AbstractTxModel<TE>
    where TE : class, ITimeSeriesEvent
{
    private readonly object _syncRoot = new();
    private long _fromTime;

    private TimeSeriesTxModel(Builder builder)
        : base(builder)
    {
        _fromTime = builder.FromTime;
        UpdateSubscription(GetUndecoratedSymbol(), _fromTime);
    }

    /// <summary>
    /// Factory method to create a new builder for this model.
    /// </summary>
    /// <returns>A new <see cref="Builder"/> instance.</returns>
    public static Builder NewBuilder() =>
        new();

    /// <summary>
    /// Gets the time from which to subscribe for time-series,
    /// or <see cref="long.MaxValue"/> if this model is not subscribed (this is the default value).
    /// </summary>
    /// <returns>The time in milliseconds since Unix epoch of January 1, 1970.</returns>
    public long GetFromTime()
    {
        lock (_syncRoot)
        {
            return _fromTime;
        }
    }

    /// <summary>
    /// Sets the time from which to subscribe for time-series.
    /// If this time has already been set, nothing happens.
    /// </summary>
    /// <param name="fromTime">The time in milliseconds since Unix epoch of January 1, 1970.</param>
    public void SetFromTime(long fromTime)
    {
        lock (_syncRoot)
        {
            if (_fromTime == fromTime)
            {
                return;
            }

            _fromTime = fromTime;
            UpdateSubscription(GetUndecoratedSymbol(), _fromTime);
        }
    }

    private static HashSet<object> DecorateSymbol(object symbol, long fromTime) =>
        fromTime == long.MaxValue
            ? new HashSet<object>()
            : new HashSet<object> { new TimeSeriesSubscriptionSymbol(symbol, fromTime) };

    private void UpdateSubscription(object symbol, long fromTime) =>
        SetSymbols(DecorateSymbol(symbol, fromTime));

    /// <summary>
    /// A builder class for creating an instance of <see cref="TimeSeriesTxModel{TE}"/>.
    /// </summary>
    public new class Builder : Builder<Builder, TimeSeriesTxModel<TE>>
    {
        internal long FromTime { get; private set; } = long.MaxValue;

        /// <summary>
        /// Sets the time from which to subscribe for time-series.
        ///
        /// <p>This time defaults to <see cref="long.MaxValue"/>, which means that this model is not subscribed.
        /// This time can be changed later, after the model has been created,
        /// by calling <see cref="SetFromTime(long)"/>.</p>
        /// </summary>
        /// <param name="fromTime">The time in milliseconds since Unix epoch of January 1, 1970.</param>
        /// <returns><c>this</c> builder.</returns>
        public Builder WithFromTime(long fromTime)
        {
            FromTime = fromTime;
            return this;
        }

        /// <summary>
        /// Builds an instance of <see cref="TimeSeriesTxModel{TE}"/> based on the provided parameters.
        /// </summary>
        /// <returns>The created <see cref="TimeSeriesTxModel{TE}"/>.</returns>
        public override TimeSeriesTxModel<TE> Build() =>
            new(this);
    }
}
