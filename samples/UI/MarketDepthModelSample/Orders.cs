// <copyright file="Orders.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using DxFeed.Graal.Net.Events.Market;
using DxFeed.Graal.Net.Utils;

namespace MarketDepthModelSample;

/// <summary>
/// The Orders class is responsible for managing and updating collections of buy and sell orders.
/// It provides data sources for binding to UI elements and methods to update these collections.
/// </summary>
public class Orders
{
    private readonly RangedObservableCollection<Order> buyOrders = new();
    private readonly RangedObservableCollection<Order> sellOrders = new();

    /// <summary>
    /// Initializes a new instance of the Orders class.
    /// Sets up the data grid sources for buy and sell orders.
    /// </summary>
    public Orders()
    {
        BuyOrders = CreateDataGridSource(buyOrders);
        SellOrders = CreateDataGridSource(sellOrders);
    }

    /// <summary>
    /// Gets the data source for buy orders.
    /// </summary>
    public ITreeDataGridSource<Order> BuyOrders { get; }

    /// <summary>
    /// Gets the data source for sell orders.
    /// </summary>
    public ITreeDataGridSource<Order> SellOrders { get; }

    /// <summary>
    /// Updates the buy orders collection with a new set of orders.
    /// </summary>
    /// <param name="orders">The new set of buy orders.</param>
    public void UpdateBuy(IEnumerable<Order> orders) =>
        buyOrders.ReplaceRange(orders);

    /// <summary>
    /// Updates the sell orders collection with a new set of orders.
    /// </summary>
    /// <param name="orders">The new set of sell orders.</param>
    public void UpdateSell(IEnumerable<Order> orders) =>
        sellOrders.ReplaceRange(orders);

    /// <summary>
    /// Creates a data grid source from an observable collection of orders.
    /// Configures the columns to display order details.
    /// </summary>
    /// <param name="collection">The observable collection of orders.</param>
    /// <returns>A configured FlatTreeDataGridSource for the given collection.</returns>
    private static FlatTreeDataGridSource<Order> CreateDataGridSource(ObservableCollection<Order> collection) =>
        new(collection)
        {
            Columns =
            {
                new TextColumn<Order, int>("№", o => GetRowNumber(collection, o)),
                new TextColumn<Order, string>("Symbol", o => o.EventSymbol),
                new TextColumn<Order, string>("DateTime", o => TimeFormat.Default.WithMillis().Format(o.Time)),
                new TextColumn<Order, string>("EX", o => StringUtil.EncodeChar(o.ExchangeCode)),
                new TextColumn<Order, OrderSource>("Source", o => o.EventSource),
                new TextColumn<Order, Scope>("Scope", o => o.Scope),
                new TextColumn<Order, string>("MM", o => o.MarketMaker),
                new TextColumn<Order, double>("Price", o => o.Price),
                new TextColumn<Order, double>("Size", o => o.Size),
            }
        };

    /// <summary>
    /// Gets the row number of an order in the collection.
    /// </summary>
    /// <param name="collection">The collection of orders.</param>
    /// <param name="order">The order whose row number is to be retrieved.</param>
    /// <returns>The row number of the given order.</returns>
    private static int GetRowNumber(ObservableCollection<Order> collection, Order order) =>
        collection.IndexOf(order) + 1;
}
