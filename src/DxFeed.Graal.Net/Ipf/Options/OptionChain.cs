// <copyright file="OptionChain.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;

namespace DxFeed.Graal.Net.Ipf.Options;

/// <summary>
/// Represents a set of option series for a single product or underlying symbol.
///
/// <h3>Threads and locks</h3>
///
/// This class is <b>NOT</b> thread-safe and cannot be used from multiple threads without external synchronization.
/// </summary>
/// <typeparam name="T">The type of option instrument instances.</typeparam>
public sealed class OptionChain<T> : ICloneable
{
    private readonly SortedDictionary<OptionSeries<T>, OptionSeries<T>> _seriesMap = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="OptionChain{T}"/> class with the specified symbol.
    /// </summary>
    /// <param name="symbol">The symbol (product or underlying) of this option chain.</param>
    internal OptionChain(string symbol) =>
        Symbol = symbol;

    /// <summary>
    /// Gets the symbol (product or underlying) of this option chain.
    /// </summary>
    public string Symbol { get; }

    /// <summary>
    /// Returns a sorted set of option series in this option chain.
    /// </summary>
    /// <returns>A sorted set of option series in this option chain.</returns>
    public SortedSet<OptionSeries<T>> GetSeries() =>
        new(_seriesMap.Keys);

    /// <summary>
    /// Returns a shallow copy of this option chain.
    /// All series are copied (cloned) themselves, but option instrument instances are shared with the original.
    /// </summary>
    /// <returns>A shallow copy of this option chain.</returns>
    public object Clone()
    {
        var clone = new OptionChain<T>(Symbol);
        foreach (var series in _seriesMap.Values)
        {
            var seriesClone = (OptionSeries<T>)series.Clone();
            clone._seriesMap.Add(seriesClone, seriesClone);
        }

        return clone;
    }

    /// <summary>
    /// Adds an option to the specified series in this option chain.
    /// If the series does not exist, it is created.
    /// </summary>
    /// <param name="series">The option series to which the option will be added.</param>
    /// <param name="isCall">Indicates whether the option is a call option.</param>
    /// <param name="strike">The strike price of the option.</param>
    /// <param name="option">The option to add.</param>
    internal void AddOption(OptionSeries<T> series, bool isCall, double strike, T option)
    {
        if (!_seriesMap.TryGetValue(series, out var os))
        {
            os = new OptionSeries<T>(series);
            _seriesMap[os] = os;
        }

        os.AddOption(isCall, strike, option);
    }
}
