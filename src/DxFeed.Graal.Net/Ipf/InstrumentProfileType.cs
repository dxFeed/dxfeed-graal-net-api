// <copyright file="InstrumentProfileType.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Concurrent;

namespace DxFeed.Graal.Net.Ipf;

/// <summary>
/// Defines standard types of <see cref="InstrumentProfile"/>. Note that other (unknown) types
/// can be used without listing in this class - use it for convenience only.
/// Please see <b>Instrument Profile Format</b> documentation for complete description.
/// </summary>
public class InstrumentProfileType
{
    private static readonly ConcurrentDictionary<string, InstrumentProfileType> ByName = new();

    // Elements Must Be Ordered By Access. Ignored, it is necessary to observe the order of initialization.
    // For avoid creating static ctor.
#pragma warning disable SA1202
    /// <summary>
    /// The currency type.
    /// </summary>
    public static readonly InstrumentProfileType CURRENCY = new("CURRENCY");

    /// <summary>
    /// Foreign exchange market or cryptocurrency.
    /// </summary>
    public static readonly InstrumentProfileType FOREX = new("FOREX");

    /// <summary>
    /// Debt instruments, excluding money market funds.
    /// </summary>
    public static readonly InstrumentProfileType BOND = new("BOND");

    /// <summary>
    /// Non-tradable market performance indicators.
    /// </summary>
    public static readonly InstrumentProfileType INDEX = new("INDEX");

    /// <summary>
    /// Tradable equities, excluding ETFs and mutual funds.
    /// </summary>
    public static readonly InstrumentProfileType STOCK = new("STOCK");

    /// <summary>
    /// Exchange-traded fund.
    /// </summary>
    public static readonly InstrumentProfileType ETF = new("ETF");

    /// <summary>
    /// Investment funds, excluding ETFs and money market funds.
    /// </summary>
    public static readonly InstrumentProfileType MUTUAL_FUND = new("MUTUAL_FUND");

    /// <summary>
    /// Funds that invest in short-term debt instruments.
    /// </summary>
    public static readonly InstrumentProfileType MONEY_MARKET_FUND = new("MONEY_MARKET_FUND");

    /// <summary>
    /// Grouping instrument for futures, aka futures product.
    /// </summary>
    public static readonly InstrumentProfileType PRODUCT = new("PRODUCT");

    /// <summary>
    /// Futures contract, derivative instrument.
    /// </summary>
    public static readonly InstrumentProfileType FUTURE = new("FUTURE");

    /// <summary>
    /// Option contract, derivative instrument.
    /// </summary>
    public static readonly InstrumentProfileType OPTION = new("OPTION");

    /// <summary>
    /// Derivative that gives the right, but not the obligation,
    /// to buy or sell a security at a certain price before expiration.
    /// </summary>
    public static readonly InstrumentProfileType WARRANT = new("WARRANT");

    /// <summary>
    /// Contract for differences, an arrangement where the differences
    /// in the settlement between the open and closing trade prices are cash-settled.
    /// </summary>
    public static readonly InstrumentProfileType CFD = new("CFD");

    /// <summary>
    /// Composite virtual instrument consisting of two or several individual instruments that represent multileg order.
    /// </summary>
    public static readonly InstrumentProfileType SPREAD = new("SPREAD");

    /// <summary>
    /// Non-tradable miscellaneous instruments.
    /// </summary>
    public static readonly InstrumentProfileType OTHER = new("OTHER");

    /// <summary>
    /// Special instrument type indicating instrument removal.
    /// </summary>
    public static readonly InstrumentProfileType REMOVED = new("REMOVED");
#pragma warning restore SA1202

    private InstrumentProfileType(string name)
    {
        Name = name;
        if (!ByName.TryAdd(Name, this))
        {
            throw new ArgumentException($"Duplicate value: {Name}", nameof(name));
        }
    }

    /// <summary>
    /// Gets full name this <see cref="InstrumentProfileType"/> instance.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Retrieves the corresponding <see cref="InstrumentProfileType"/> for the given name.
    /// </summary>
    /// <param name="name">The name of the <see cref="InstrumentProfileType"/> to be retrieved.</param>
    /// <returns>
    /// The associated <see cref="InstrumentProfileType"/> if found, otherwise returns <c>null</c>.
    /// </returns>
    public static InstrumentProfileType? Find(string name) =>
        ByName.TryGetValue(name, out var value) ? value : null;

    /// <summary>
    /// Compares two specified instrument profile types for order.
    /// </summary>
    /// <remarks>
    /// This method returns a negative integer if the first type is less than the second, zero if they are equal,
    /// or a positive integer if the first type is greater than the second.
    /// <p/>
    /// Unlike the natural ordering of the <see cref="InstrumentProfile"/> class itself, this method supports
    /// unknown types, ordering them alphabetically after the recognized types. It's designed primarily for
    /// arranging data representation in a file and is not intended for business logic evaluations.
    /// </remarks>
    /// <param name="type1">The first instrument profile type to compare.</param>
    /// <param name="type2">The second instrument profile type to compare.</param>
    /// <returns>
    /// A negative integer if <paramref name="type1"/> is less than <paramref name="type2"/>,
    /// zero if they are equal, or a positive integer if <paramref name="type1"/> is greater than <paramref name="type2"/>.
    /// If an unknown type is encountered, it's ordered alphabetically after recognized types.
    /// </returns>
    public static int CompareTypes(string type1, string type2)
    {
        var t1 = Find(type1);
        var t2 = Find(type2);

        if (t1 == null && t2 == null)
        {
            return string.Compare(type1, type2, StringComparison.Ordinal);
        }

        if (t1 == null)
        {
            return 1;
        }

        if (t2 == null)
        {
            return -1;
        }

        return string.Compare(t1.Name, t2.Name, StringComparison.Ordinal);
    }
}
