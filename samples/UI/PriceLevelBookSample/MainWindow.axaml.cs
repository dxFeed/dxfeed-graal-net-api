// <copyright file="MainWindow.axaml.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using DxFeed.Graal.Net.Api;
using DxFeed.Graal.Net.Events;
using DxFeed.Graal.Net.Events.Market;

namespace PriceLevelBookSample;

/// <summary>
/// The MainWindow class represents the main window of the PriceLevelBookSample application.
/// This window is responsible for setting up the price level book, handling user input,
/// and updating the UI with price level data.
/// </summary>
public partial class MainWindow : Window
{
    private readonly PriceLevels _priceLevels = new();
    private readonly PriceLevelBook<Order>.Builder _builder;
    private string _symbol = string.Empty;
    private string _source = string.Empty;
    private PriceLevelBook<Order>? _model;

    /// <summary>
    /// Initializes a new instance of the MainWindow class.
    /// Sets up the market feed address, initializes components,
    /// and configures the price level book builder.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
        BuyTable.Source = _priceLevels.BuyPriceLevels;
        SellTable.Source = _priceLevels.SellPriceLevels;

        _builder = PriceLevelBook<Order>.NewBuilder()
            .WithFeed(DXFeed.GetInstance())
            .WithListener((buy, sell) =>
            {
                Dispatcher.UIThread.Post(() =>
                {
                    // Update UI.
                    _priceLevels.UpdateBuy(buy);
                    _priceLevels.UpdateSell(sell);
                }, DispatcherPriority.Normal);
            });

        HandleTextChanged(SymbolTextBox);
    }

    /// <summary>
    /// Disposes the current price level book when the window is closed.
    /// </summary>
    /// <param name="e">Event arguments.</param>
    protected override void OnClosed(EventArgs e) =>
        _model?.Dispose();

    /// <summary>
    /// Handles the KeyDown event for the TextBox controls.
    /// Updates the price level book when the Enter key is pressed.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Event arguments.</param>
    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            HandleTextChanged(sender as TextBox);
        }
    }

    /// <summary>
    /// Handles the LostFocus event for the TextBox controls.
    /// Updates the price level book when the focus is lost.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Event arguments.</param>
    private void OnLostFocus(object sender, RoutedEventArgs e) =>
        HandleTextChanged(sender as TextBox);

    /// <summary>
    /// Handles the text changed event for the TextBox controls.
    /// Determines which TextBox triggered the event and calls the appropriate handler.
    /// </summary>
    /// <param name="textBox">The TextBox that triggered the event.</param>
    private void HandleTextChanged(TextBox? textBox)
    {
        if (textBox == null)
        {
            return;
        }

        switch (textBox.Name)
        {
            case nameof(SymbolTextBox):
            case nameof(SourceTextBox):
                OnSymbolTextChanged();
                break;
            case nameof(DepthLimitTextBox):
                OnDepthLimitTextChanged();
                break;
            case nameof(PeriodTextBox):
                OnAggregationPeriodTextChanged();
                break;
        }
    }

    /// <summary>
    /// Updates the price level book when the symbol or sources text changes.
    /// Disposes the old model and creates a new one with the updated parameters.
    /// </summary>
    private void OnSymbolTextChanged()
    {
        if (_symbol.Equals(SymbolTextBox.Text, StringComparison.Ordinal) &&
            _source.Equals(SourceTextBox.Text, StringComparison.Ordinal))
        {
            return;
        }

        _symbol = SymbolTextBox.Text ?? string.Empty;
        _source = SourceTextBox.Text ?? string.Empty;

        _model?.Dispose();
        try
        {
            _model = _builder
                .WithSymbol(GetSymbol())
                .WithSource(GetSource()!)
                .WithDepthLimit(GetDepthLimit())
                .WithAggregationPeriod(GetAggregationPeriod())
                .Build();
        }
        catch(Exception e)
        {
            Console.WriteLine(e);
        }
    }

    /// <summary>
    /// Updates the depth limit of the price level book when the depth limit text changes.
    /// </summary>
    private void OnDepthLimitTextChanged() =>
        _model?.SetDepthLimit(GetDepthLimit());

    /// <summary>
    /// Updates the aggregation period of the price level book when the period text changes.
    /// </summary>
    private void OnAggregationPeriodTextChanged() =>
        _model?.SetAggregationPeriod(GetAggregationPeriod());

    /// <summary>
    /// Gets the current symbol from the SymbolTextBox.
    /// </summary>
    /// <returns>The current symbol.</returns>
    private string GetSymbol() =>
        SymbolTextBox.Text ?? string.Empty;

    /// <summary>
    /// Gets the list of sources from the SourcesTextBox.
    /// </summary>
    /// <returns>A list of order sources.</returns>
    private IndexedEventSource? GetSource()
    {
        var sourceList = new List<IndexedEventSource>();
        if (!string.IsNullOrWhiteSpace(SourceTextBox.Text))
        {
            foreach (var source in SourceTextBox.Text.Split(","))
            {
                sourceList.Add(OrderSource.ValueOf(source));
            }
        }

        return sourceList.FirstOrDefault();
    }

    /// <summary>
    /// Gets the depth limit from the DepthLimitTextBox.
    /// </summary>
    /// <returns>The depth limit as an integer.</returns>
    private int GetDepthLimit()
    {
        try
        {
            return string.IsNullOrWhiteSpace(DepthLimitTextBox.Text)
                ? 0
                : int.Parse(DepthLimitTextBox.Text, CultureInfo.InvariantCulture);
        }
        catch
        {
            return 0;
        }
    }

    /// <summary>
    /// Gets the aggregation period from the PeriodTextBox.
    /// </summary>
    /// <returns>The aggregation period as an integer.</returns>
    private int GetAggregationPeriod()
    {
        try
        {
            return string.IsNullOrWhiteSpace(PeriodTextBox.Text)
                ? 0
                : int.Parse(PeriodTextBox.Text, CultureInfo.InvariantCulture);
        }
        catch
        {
            return 0;
        }
    }
}
