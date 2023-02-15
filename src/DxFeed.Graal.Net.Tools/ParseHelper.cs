// <copyright file="ParseHelper.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using DxFeed.Graal.Net.Api.Osub;
using DxFeed.Graal.Net.Events;
using DxFeed.Graal.Net.Utils;

namespace DxFeed.Graal.Net.Tools;

public static class ParseHelper
{
    /// <summary>
    /// Parses an input string and returns a set of symbols.
    /// The "all" symbol converted to <see cref="WildcardSymbol"/>.
    /// </summary>
    /// <param name="symbols">The coma-separated list of symbols.</param>
    /// <returns>Returns a set of parsed symbols.</returns>
    public static IEnumerable<object> ParseSymbols(string symbols)
    {
        symbols = symbols.Trim().Trim(',');

        return symbols.Equals("all", StringComparison.OrdinalIgnoreCase)
            ? new[] { WildcardSymbol.All }
            : CmdArgsUtil.ParseSymbols(symbols);
    }

    /// <summary>
    /// Parses an input string and returns a set of event types.
    /// If types is "feed", that returns all available events types.
    /// </summary>
    /// <param name="types">The coma-separated list of event types.</param>
    /// <returns>Returns a set of parsed types.</returns>
    /// <exception cref="ArgumentException">If the passed type is not available.</exception>
    public static IEnumerable<Type> ParseEventTypes(string types)
    {
        types = types.Trim().Trim(',');

        return types.Equals("feed", StringComparison.OrdinalIgnoreCase)
            ? IEventType.GetEventTypes()
            : CmdArgsUtil.ParseTypes(types);
    }

    /// <summary>
    /// Parses the input collection of strings and returns a collection of key-value properties.
    /// The input strings should look like: "key=value".
    /// </summary>
    /// <param name="properties">The input collection of strings.</param>
    /// <returns>Returns collection of key-value properties.</returns>
    /// <exception cref="ArgumentException">If string has wrong format.</exception>
    public static IReadOnlyDictionary<string, string> ParseProperties(string? properties) =>
        string.IsNullOrWhiteSpace(properties)
            ? new Dictionary<string, string>()
            : CmdArgsUtil.ParseProperties(properties);
}
