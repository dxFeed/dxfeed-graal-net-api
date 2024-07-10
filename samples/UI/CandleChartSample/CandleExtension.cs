// <copyright file="CandleExtension.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using DxFeed.Graal.Net.Events.Candles;
using ScottPlot;

namespace CandleChartSample;

/// <summary>
/// Provides extension methods for converting <see cref="Candle"/> objects to <see cref="OHLC"/> objects.
/// </summary>
public static class CandleExtension
{
    /// <summary>
    /// Converts a <see cref="Candle"/> object to an <see cref="OHLC"/> object.
    /// </summary>
    /// <param name="candle">The candle to convert.</param>
    /// <returns>An <see cref="OHLC"/> object representing the candle.</returns>
    public static OHLC ToOHLC(this Candle candle)
    {
        var open = GetValueWithPriority(candle.Open, candle.High, candle.Low, candle.Close);
        var high = GetValueWithPriority(candle.High, candle.Open, candle.Low, candle.Close);
        var low = GetValueWithPriority(candle.Low, candle.Close, candle.Open, candle.High);
        var close = GetValueWithPriority(candle.Close, candle.Low, candle.Open, candle.High);

        return new OHLC
        {
            Open = open,
            High = high,
            Low = low,
            Close = close,
            DateTime = DateTimeOffset.FromUnixTimeMilliseconds(candle.Time).DateTime.ToLocalTime(),
            TimeSpan = TimeSpan.FromMilliseconds(candle.CandleSymbol!.Period!.PeriodIntervalMillis)
        };
    }

    /// <summary>
    /// Returns the first non-NaN value from the provided list of values.
    /// </summary>
    /// <param name="values">An array of double values to check.</param>
    /// <returns>The first non-NaN value, or NaN if all values are NaN.</returns>
    private static double GetValueWithPriority(params double[] values)
    {
        foreach (var value in values)
        {
            if (!double.IsNaN(value))
            {
                return value;
            }
        }
        return double.NaN;
    }
}
