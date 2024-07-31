// <copyright file="PriceLevels.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using DxFeed.Graal.Net.Events;
using DxFeed.Graal.Net.Samples;

namespace PriceLevelBookSample;

/// <summary>
/// The PriceLevels class is responsible for managing and updating collections of buy and sell price levels.
/// It provides data sources for binding to UI elements and methods to update these collections.
/// </summary>
public class PriceLevels
{
    private readonly RangedObservableCollection<PriceLevel> buyPriceLevels = new();
    private readonly RangedObservableCollection<PriceLevel> sellPriceLevels = new();

    /// <summary>
    /// Initializes a new instance of the PriceLevels class.
    /// Sets up the data grid sources for buy and sell price levels.
    /// </summary>
    public PriceLevels()
    {
        BuyPriceLevels = CreateDataGridSource(buyPriceLevels);
        SellPriceLevels = CreateDataGridSource(sellPriceLevels);
    }

    /// <summary>
    /// Gets the data source for buy price levels.
    /// </summary>
    public ITreeDataGridSource<PriceLevel> BuyPriceLevels { get; }

    /// <summary>
    /// Gets the data source for sell price levels.
    /// </summary>
    public ITreeDataGridSource<PriceLevel> SellPriceLevels { get; }

    /// <summary>
    /// Updates the buy price levels collection with a new set of price levels.
    /// </summary>
    /// <param name="priceLevel">The new set of buy price levels.</param>
    public void UpdateBuy(IEnumerable<PriceLevel> priceLevel) =>
        buyPriceLevels.ReplaceRange(priceLevel);

    /// <summary>
    /// Updates the sell price levels collection with a new set of price levels.
    /// </summary>
    /// <param name="priceLevel">The new set of sell price levels.</param>
    public void UpdateSell(IEnumerable<PriceLevel> priceLevel) =>
        sellPriceLevels.ReplaceRange(priceLevel);

    /// <summary>
    /// Creates a data grid source from an observable collection of price levels.
    /// Configures the columns to display price levels details.
    /// </summary>
    /// <param name="collection">The observable collection of price levels.</param>
    /// <returns>A configured FlatTreeDataGridSource for the given collection.</returns>
    private static FlatTreeDataGridSource<PriceLevel> CreateDataGridSource(ObservableCollection<PriceLevel> collection) =>
        new(collection)
        {
            Columns =
            {
                new TextColumn<PriceLevel, int>("№", o => GetRowNumber(collection, o)),
                new TextColumn<PriceLevel, string>("Symbol", o => o.EventSymbol),
                new TextColumn<PriceLevel, IndexedEventSource>("Source", o => o.EventSource),
                new TextColumn<PriceLevel, double>("Price", o => o.Price),
                new TextColumn<PriceLevel, double>("Size", o => o.Size),
            }
        };

    /// <summary>
    /// Gets the row number of an price levels in the collection.
    /// </summary>
    /// <param name="collection">The collection of price levels.</param>
    /// <param name="priceLevel">The price level whose row number is to be retrieved.</param>
    /// <returns>The row number of the given price level.</returns>
    private static int GetRowNumber(ObservableCollection<PriceLevel> collection, PriceLevel priceLevel) =>
        collection.IndexOf(priceLevel) + 1;
}
