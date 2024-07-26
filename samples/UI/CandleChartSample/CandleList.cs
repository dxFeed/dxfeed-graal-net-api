// <copyright file="CandleList.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using DxFeed.Graal.Net.Events;
using DxFeed.Graal.Net.Events.Candles;
using ScottPlot;

namespace CandleChartSample;

/// <summary>
/// The CandleList class manages a list of OHLC (Open, High, Low, Close) objects for candle chart visualization.
/// This class provides methods for updating the list based on incoming candle events,
/// supporting both snapshot and incremental updates.
/// </summary>
public class CandleList : List<OHLC>
{
    /// <summary>
    /// Updates the candle list with a new set of candles.
    /// Depending on whether the update is a snapshot or incremental, it will replace or update the existing list.
    /// </summary>
    /// <param name="candles">The collection of candles to update the list with.</param>
    /// <param name="isSnapshot">Indicates whether the update is a snapshot (true) or incremental (false).</param>
    public void Update(IEnumerable<Candle> candles, bool isSnapshot)
    {
        // Sort candles by their index. This is stable sort.
        var sortedCandles = candles.OrderBy(c => c.Index);

        if (isSnapshot)
        {
            // Handle snapshot update
            UpdateSnapshot(sortedCandles);
        }
        else
        {
            // Handle incremental update
            UpdateIncremental(sortedCandles);
        }
    }

    /// <summary>
    /// Updates the list with a snapshot of candles.
    /// Clears the existing list and adds the new set of candles, ensuring to remove any that should be removed.
    /// </summary>
    /// <param name="candles">The snapshot of candles to update the list with.</param>
    private void UpdateSnapshot(IEnumerable<Candle> candles)
    {
        // Clear the current list of OHLC objects
        Clear();
        foreach (var candle in candles)
        {
            // Check if the candle should be removed
            if (!ShouldRemove(candle))
            {
                // Convert the candle to OHLC and add to the list
                Add(candle.ToOHLC());
            }
        }
    }

    /// <summary>
    /// Updates the list incrementally with a set of candles.
    /// Adds, updates, or removes candles as necessary based on the provided list.
    /// </summary>
    /// <param name="candles">The incremental set of candles to update the list with.</param>
    private void UpdateIncremental(IEnumerable<Candle> candles)
    {
        foreach (var candle in candles)
        {
            // Check if the candle should be removed.
            if (ShouldRemove(candle))
            {
                // Remove the corresponding OHLC object.
                RemoveCandle(candle);
                continue;
            }

            // Convert the candle to OHLC.
            var ohlc = candle.ToOHLC();
            // Gets last OHLC.
            var lastOhlc = LastOrDefault();

            // Compare the date and time of the OHLC object with the last one in the list
            switch (DateTime.Compare(ohlc.DateTime, lastOhlc.DateTime))
            {
                case < 0:
                    // If the new OHLC is older, insert or update it in the correct position.
                    InsertOrUpdate(ohlc);
                    break;
                case 0:
                    // If the new OHLC has the same date and time, update the last one.
                    AddOrUpdateLast(ohlc);
                    break;
                case > 0:
                    // If the new OHLC is newer, add it to the list.
                    Add(ohlc);
                    break;
            }
        }
    }

    /// <summary>
    /// Adds a new OHLC object or updates the last one if it exists.
    /// </summary>
    /// <param name="ohlc">The OHLC object to add or update.</param>
    private void AddOrUpdateLast(OHLC ohlc)
    {
        if (IsEmpty())
        {
            // Add the OHLC if the list is empty.
            Add(ohlc);
        }
        else
        {
            // Update the last OHLC object.
            this[Count - 1] = ohlc;
        }
    }

    /// <summary>
    /// Inserts a new OHLC object into the list or updates an existing one based on its date and time.
    /// </summary>
    /// <param name="ohlc">The OHLC object to insert or update.</param>
    private void InsertOrUpdate(OHLC ohlc)
    {
        // Find the index of the OHLC object to insert or update.
        var index = FindIndex(o => DateTime.Compare(ohlc.DateTime, o.DateTime) <= 0);
        if (index >= 0 && this[index].DateTime.Equals(ohlc.DateTime))
        {
            // Update the existing OHLC object if the date and time match.
            this[index] = ohlc;
        }
        else
        {
            // Insert the new OHLC object at the correct position.
            Insert(index >= 0 ? index : 0, ohlc);
        }
    }

    /// <summary>
    /// Removes a candle from the list based on its date and time.
    /// </summary>
    /// <param name="candle">The candle to remove.</param>
    private void RemoveCandle(Candle candle) =>
        RemoveAll(ohlc => candle.ToOHLC().DateTime.Equals(ohlc.DateTime));

    /// <summary>
    /// Checks if the list is empty.
    /// </summary>
    /// <returns><c>true</c> if the list is empty; otherwise, <c>false</c>.</returns>
    private bool IsEmpty() =>
        Count == 0;

    /// <summary>
    /// Returns the last OHLC object in the list or a new OHLC object if the list is empty.
    /// </summary>
    /// <returns>The last OHLC object or a new OHLC object if the list is empty.</returns>
    private OHLC LastOrDefault() =>
        IsEmpty() ? new OHLC() : this[Count - 1];

    /// <summary>
    /// Determines whether a candle should be removed.
    /// </summary>
    /// <param name="candle">The candle to check.</param>
    /// <returns><c>true</c> if the candle should be removed; otherwise, <c>false</c>.</returns>
    private static bool ShouldRemove(Candle candle) =>
        EventFlags.IsRemove(candle) ||
        (double.IsNaN(candle.Open) &&
         double.IsNaN(candle.High) &&
         double.IsNaN(candle.Low) &&
         double.IsNaN(candle.Close));
}
