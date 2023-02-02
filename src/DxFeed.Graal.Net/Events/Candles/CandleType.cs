// <copyright file="CandleType.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Concurrent;
using System.Linq;

namespace DxFeed.Graal.Net.Events.Candles;

/// <summary>
/// Type of the candle aggregation period constitutes <see cref="CandlePeriod"/> type together
/// its actual <see cref="CandlePeriod.Value"/>.
/// For more details see <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/event/candle/CandleType.html">Javadoc</a>.
/// </summary>
public class CandleType
{
    // Elements Must Be Ordered By Access. Ignored, it is necessary to observe the order of initialization.
    // For avoid creating static ctor.
#pragma warning disable SA1202
    /// <summary>
    /// A dictionary containing the matching string representation
    /// of the candle type (<see cref="ToString"/>) and the <see cref="CandleType"/> instance.
    /// </summary>
    private static readonly ConcurrentDictionary<string, CandleType> ByValue = new();

    /// <summary>
    /// A dictionary containing the matching id
    /// of the candle type (<see cref="Id"/>) and the <see cref="CandleType"/> instance.
    /// </summary>
    private static readonly ConcurrentDictionary<CandleTypeId, CandleType> ById = new();

    /// <summary>
    /// Certain number of ticks.
    /// </summary>
    public static readonly CandleType Tick = new(CandleTypeId.Tick, "t", 0);

    /// <summary>
    /// Certain number of seconds.
    /// </summary>
    public static readonly CandleType Second = new(CandleTypeId.Second, "s", 1000L);

    /// <summary>
    /// Certain number of minutes.
    /// </summary>
    public static readonly CandleType Minute = new(CandleTypeId.Minute, "m", 60 * 1000L);

    /// <summary>
    /// Certain number of hours.
    /// </summary>
    public static readonly CandleType Hour = new(CandleTypeId.Hour, "h", 60 * 60 * 1000L);

    /// <summary>
    /// Certain number of days.
    /// </summary>
    public static readonly CandleType Day = new(CandleTypeId.Day, "d", 24 * 60 * 60 * 1000L);

    /// <summary>
    /// Certain number of weeks.
    /// </summary>
    public static readonly CandleType Week = new(CandleTypeId.Week, "w", 7 * 24 * 60 * 60 * 1000L);

    /// <summary>
    /// Certain number of months.
    /// </summary>
    public static readonly CandleType Month = new(CandleTypeId.Month, "mo", 30 * 24 * 60 * 60 * 1000L);

    /// <summary>
    /// Certain number of option expirations.
    /// </summary>
    public static readonly CandleType OptExp = new(CandleTypeId.OptExp, "o", 30 * 24 * 60 * 60 * 1000L);

    /// <summary>
    /// Certain number of years.
    /// </summary>
    public static readonly CandleType Year = new(CandleTypeId.Year, "y", 365 * 24 * 60 * 60 * 1000L);

    /// <summary>
    /// Certain volume of trades.
    /// </summary>
    public static readonly CandleType Volume = new(CandleTypeId.Volume, "v", 0);

    /// <summary>
    /// Certain price change, calculated according to the following rules:
    /// <ul>
    ///     <li>high(n) - low(n) = price range</li>
    ///     <li>close(n) = high(n) or close(n) = low(n)</li>
    ///     <li>open(n+1) = close(n)</li>
    /// </ul>
    /// where n is the number of the bar.
    /// </summary>
    public static readonly CandleType Price = new(CandleTypeId.Price, "p", 0);

    /// <summary>
    /// Certain price change, calculated according to the following rules:
    /// <ul>
    ///     <li>high(n) - low(n) = price range</li>
    ///     <li>close(n) = high(n) or close(n) = low(n)</li>
    ///     <li>open(n+1) = close(n) + tick size, if close(n) = high(n)</li>
    ///     <li>open(n+1) = close(n) - tick size, if close(n) = low(n)</li>
    /// </ul>
    /// where n is the number of the bar.
    /// </summary>
    public static readonly CandleType PriceMomentum = new(CandleTypeId.PriceMomentum, "pm", 0);

    /// <summary>
    /// Certain price change, calculated according to the following rules:
    /// <ul>
    ///     <li>high(n+1) - high(n) = price range or low(n) - low(n+1) = price range</li>
    ///     <li>close(n) = high(n) or close(n) = low(n)</li>
    ///     <li>open(n+1) = high(n), if high(n+1) - high(n) = price range</li>
    ///     <li>open(n+1) = low(n), if low(n) - low(n+1) = price range</li>
    /// </ul>
    /// where n is the number of the bar.
    /// </summary>
    public static readonly CandleType PriceRenko = new(CandleTypeId.PriceRenko, "pr", 0);
#pragma warning restore SA1202

    private CandleType(CandleTypeId id, string value, long periodIntervalMillis)
    {
        Id = id;
        Name = id.ToString();
        Value = value;
        PeriodIntervalMillis = periodIntervalMillis;

        if (!ByValue.TryAdd(value, this))
        {
            throw new ArgumentException($"Duplicate value: {value}", nameof(value));
        }

        if (!ById.TryAdd(id, this))
        {
            throw new ArgumentException($"Duplicate id: {id}", nameof(id));
        }
    }

    /// <summary>
    /// List of ids <see cref="CandleType"/>.
    /// </summary>
    public enum CandleTypeId
    {
        /// <summary>
        /// Id associated with
        /// <see cref="CandleType"/>.<see cref="CandleType.Tick"/>.
        /// </summary>
        Tick,

        /// <summary>
        /// Id associated with
        /// <see cref="CandleType"/>.<see cref="CandleType.Second"/>.
        /// </summary>
        Second,

        /// <summary>
        /// Id associated with
        /// <see cref="CandleType"/>.<see cref="CandleType.Minute"/>.
        /// </summary>
        Minute,

        /// <summary>
        /// Id associated with
        /// <see cref="CandleType"/>.<see cref="CandleType.Hour"/>.
        /// </summary>
        Hour,

        /// <summary>
        /// Id associated with
        /// <see cref="CandleType"/>.<see cref="CandleType.Day"/>.
        /// </summary>
        Day,

        /// <summary>
        /// Id associated with
        /// <see cref="CandleType"/>.<see cref="CandleType.Week"/>.
        /// </summary>
        Week,

        /// <summary>
        /// Id associated with
        /// <see cref="CandleType"/>.<see cref="CandleType.Month"/>.
        /// </summary>
        Month,

        /// <summary>
        /// Id associated with
        /// <see cref="CandleType"/>.<see cref="CandleType.OptExp"/>.
        /// </summary>
        OptExp,

        /// <summary>
        /// Id associated with
        /// <see cref="CandleType"/>.<see cref="CandleType.Year"/>.
        /// </summary>
        Year,

        /// <summary>
        /// Id associated with
        /// <see cref="CandleType"/>.<see cref="CandleType.Volume"/>.
        /// </summary>
        Volume,

        /// <summary>
        /// Id associated with
        /// <see cref="CandleType"/>.<see cref="CandleType.Price"/>.
        /// </summary>
        Price,

        /// <summary>
        /// Id associated with
        /// <see cref="CandleType"/>.<see cref="CandleType.PriceMomentum"/>.
        /// </summary>
        PriceMomentum,

        /// <summary>
        /// Id associated with
        /// <see cref="CandleType"/>.<see cref="CandleType.PriceRenko"/>.
        /// </summary>
        PriceRenko,
    }

    /// <summary>
    /// Gets <see cref="CandleTypeId"/> associated with this instance.
    /// </summary>
    public CandleTypeId Id { get; }

    /// <summary>
    /// Gets full name this <see cref="CandleType"/> instance.
    /// For example,
    /// <see cref="Tick"/> (<see cref="Value"/> == <c>"t"</c>) returns <c>"Tick"</c>,
    /// <see cref="Month"/> (<see cref="Value"/> == <c>"mo"</c>) returns <c>"Month"</c>.
    /// </summary>
    public string Name { get; }

    /// <inheritdoc cref="ToString"/>
    public string Value { get; }

    /// <summary>
    /// Gets candle type period in milliseconds (aggregation period) as closely as possible.
    /// Certain types like <see cref="Second"/> and
    /// <see cref="Day"/> span a specific number of milliseconds.
    /// <see cref="Month"/>, <see cref="OptExp"/> and <see cref="Year"/>
    /// are approximate. Candle type period of
    /// <see cref="Tick"/>, <see cref="Volume"/>, <see cref="Price"/>,
    /// <see cref="PriceMomentum"/> and <see cref="PriceRenko"/>
    /// is not defined and this method returns <c>0</c>.
    /// </summary>
    public long PeriodIntervalMillis { get; }

    /// <summary>
    /// Gets <see cref="CandleType"/> associated with the specified <see cref="CandleTypeId"/>.
    /// </summary>
    /// <param name="id">The candle type id.</param>
    /// <returns>The candle type.</returns>
    /// <exception cref="ArgumentException">If candle type id not exist.</exception>
    public static CandleType GetById(CandleTypeId id)
    {
        if (ById.TryGetValue(id, out var candleType))
        {
            return candleType;
        }

        throw new ArgumentException($"Unknown candle type id: {id}", nameof(id));
    }

    /// <summary>
    /// Parses string representation of candle type into object.
    /// Any string that is a prefix of candle type <see cref="Name"/> can be parsed
    /// (including the one that was returned by <see cref="ToString"/>)
    /// and case is ignored for parsing.
    /// </summary>
    /// <param name="s">The string representation of candle type.</param>
    /// <returns>The candle type.</returns>
    /// <exception cref="ArgumentException">If the string representation is invalid.</exception>
    public static CandleType Parse(string s)
    {
        var n = s.Length;
        if (n == 0)
        {
            throw new ArgumentException("Missing candle type", nameof(s));
        }

        // Fast path to reverse ToString result.
        if (ByValue.TryGetValue(s, out var result))
        {
            return result;
        }

        // Slow path for for everything else.
        try
        {
            return ByValue.Values.First(type =>
            {
                var name = type.Name;
                if (name.Length >= n && name[..n].Equals(s, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                // Ticks, Minutes, Seconds, etc.
                return s.EndsWith("s", StringComparison.OrdinalIgnoreCase) &&
                       name.Equals(s[..(n - 1)], StringComparison.OrdinalIgnoreCase);
            });
        }
        catch
        {
            throw new ArgumentException($"Unknown candle type: {s}", nameof(s));
        }
    }

    /// <summary>
    /// Returns string representation of this candle type.
    /// The string representation of candle type is the shortest unique prefix of the
    /// lower case string that corresponds to its <see cref="Name"/>. For example,
    /// <see cref="Tick"/> is represented as <c>"t"</c>, while <see cref="Month"/> is represented as
    /// <c>"mo"</c> to distinguish it from <see cref="Minute"/> that is represented as <c>"m"</c>.
    /// </summary>
    /// <returns>The string representation.</returns>
    public override string ToString() =>
        Value;
}
