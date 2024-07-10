// <copyright file="MainWindow.axaml.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using DxFeed.Graal.Net;
using DxFeed.Graal.Net.Api;
using DxFeed.Graal.Net.Events;
using DxFeed.Graal.Net.Events.Candles;
using DxFeed.Graal.Net.Models;
using DxFeed.Graal.Net.Utils;

namespace CandleChartSample;

/// <summary>
/// The MainWindow class represents the main window of the CandleChartSample application.
/// This window is responsible for setting up the candle chart model, handling user input,
/// and updating the UI with candle data.
/// </summary>
public partial class MainWindow : Window
{
    private readonly CandleList _candles = new();
    private readonly TimeSeriesTxModel<Candle>.Builder _modelBuilder = new();
    private TimeSeriesTxModel<Candle>? _model;
    private string _symbol = string.Empty;

    /// <summary>
    /// Initializes a new instance of the MainWindow class.
    /// Sets up the market feed address, initializes components, plot, and model builder,
    /// and configures initial user input handling.
    /// </summary>
    public MainWindow()
    {
        SystemProperty.SetProperty("dxfeed.address", "demo.dxfeed.com:7300");
        InitializeComponent();
        InitializePlot();
        InitializeModelBuilder();
        HandleTextChanged(SymbolTextBox);
    }

    /// <summary>
    /// Initializes the plot for displaying candle data.
    /// Configures axes and sets the initial time range for data.
    /// </summary>
    private void InitializePlot()
    {
        AvaPlot.Plot.Axes.DateTimeTicksBottom();
        var candlePlot = AvaPlot.Plot.Add.Candlestick(_candles);
        candlePlot.Axes.YAxis = AvaPlot.Plot.Axes.Right;
        candlePlot.Axes.YAxis.Label.Text = "Price";
        FromTimeTextBox.Text = DateTimeOffset.Now.AddMonths(-6).ToString("yyyyMMdd", CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Initializes the model builder for creating a time series model of candles.
    /// Configures the feed and listener for receiving candle events.
    /// </summary>
    private void InitializeModelBuilder() =>
        _modelBuilder
            .WithFeed(DXFeed.GetInstance())
            .WithSnapshotProcessing(true)
            .WithListener(OnCandleEventsReceived);

    /// <summary>
    /// Disposes the current time series model when the window is closed.
    /// </summary>
    /// <param name="e">Event arguments.</param>
    protected override void OnClosed(EventArgs e) =>
        _model?.Dispose();

    /// <summary>
    /// Handles the receipt of candle events.
    /// Updates the candle list and refreshes the plot UI.
    /// </summary>
    /// <param name="source">The source of the indexed event.</param>
    /// <param name="events">The collection of candle events.</param>
    /// <param name="isSnapshot">Indicates if the events are part of a snapshot.</param>
    private void OnCandleEventsReceived(IndexedEventSource source, IEnumerable<Candle> events, bool isSnapshot)
    {
        _candles.Update(events, isSnapshot);
        Dispatcher.UIThread.Invoke(() =>
        {
            if (isSnapshot)
            {
                AvaPlot.Plot.Axes.AutoScale();
            }

            AvaPlot.Refresh();
        });
    }

    /// <summary>
    /// Handles the KeyDown event for the TextBox controls.
    /// Updates the model when the Enter key is pressed.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Event arguments.</param>
    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            HandleTextChanged(sender as TextBox);
        }
    }

    /// <summary>
    /// Handles the LostFocus event for the TextBox controls.
    /// Updates the model when the focus is lost.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Event arguments.</param>
    private void OnLostFocus(object? sender, RoutedEventArgs e) =>
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
                OnSymbolTextChanged();
                break;
            case nameof(FromTimeTextBox):
                OnFromTimeTextChanged();
                break;
        }
    }

    /// <summary>
    /// Updates the model when the symbol text changes.
    /// Disposes the old model and creates a new one with the updated symbol.
    /// </summary>
    private void OnSymbolTextChanged()
    {
        if (_symbol.Equals(GetSymbol(), StringComparison.Ordinal))
        {
            return;
        }

        _symbol = GetSymbol();
        _model?.Dispose();
        _model = _modelBuilder
            .WithSymbol(CandleSymbol.ValueOf(_symbol))
            .WithFromTime(GetFromTime())
            .Build();
    }

    /// <summary>
    /// Updates the model's from time when the from-time text changes.
    /// </summary>
    private void OnFromTimeTextChanged()
    {
        if (_model == null)
        {
            OnSymbolTextChanged();
        }

        _model?.SetFromTime(GetFromTime());
    }

    /// <summary>
    /// Gets the current symbol from the SymbolTextBox.
    /// </summary>
    /// <returns>The current symbol.</returns>
    private string GetSymbol() =>
        SymbolTextBox.Text ?? string.Empty;

    /// <summary>
    /// Gets the from-time in Unix time milliseconds from the FromTimeTextBox.
    /// </summary>
    /// <returns>The from-time in Unix time milliseconds.</returns>
    private long GetFromTime()
    {
        try
        {
            return string.IsNullOrWhiteSpace(FromTimeTextBox.Text)
                ? 0
                : TimeFormat.Default.Parse(FromTimeTextBox.Text).ToUnixTimeMilliseconds();
        }
        catch (Exception)
        {
            return 0;
        }
    }
}
