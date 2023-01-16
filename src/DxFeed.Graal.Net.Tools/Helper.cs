// <copyright file="Helper.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using DxFeed.Graal.Net.Api.Osub;
using DxFeed.Graal.Net.Events;

namespace DxFeed.Graal.Net.Tools;

public static class Helper
{
    /// <summary>
    /// Parses the input collection of strings and returns a collection of key-value properties.
    /// The input strings should look like: "key=value".
    /// </summary>
    /// <param name="properties">The input collection of strings.</param>
    /// <returns>Returns collection of key-value properties.</returns>
    public static IReadOnlyDictionary<string, string> ParseProperties(IEnumerable<string> properties) =>
        properties.ToDictionary(kvp => kvp.Split('=')[0], kvp => kvp.Split('=')[1]);

    /// <summary>
    /// Parses an input string and returns a set of event types.
    /// If eventTypeNames is "feed", that returns all available events types.
    /// </summary>
    /// <param name="eventTypeNames">The coma-separated list of event types.</param>
    /// <returns>Returns a set of parsed types.</returns>
    /// <exception cref="ArgumentException">If the passed type is not available.</exception>
    public static IEnumerable<Type> ParseEventTypes(string eventTypeNames)
    {
        if (eventTypeNames.Equals("feed", StringComparison.OrdinalIgnoreCase))
        {
            return IEventType.GetEventTypes();
        }

        var availableTypesDictionary = IEventType.GetEventTypes().ToDictionary(kvp => kvp.Name, kvp => kvp);
        var setTypes = new HashSet<Type>();
        foreach (var typeName in eventTypeNames.Split(','))
        {
            if (!availableTypesDictionary.TryGetValue(typeName, out var type))
            {
                var availableTypesStr = string.Join(", ", availableTypesDictionary.Select(x => x.Key));
                throw new ArgumentException(
                    $"{typeName} event type is not available! List of available event types: {availableTypesStr}.");
            }

            setTypes.Add(type);
        }

        return setTypes;
    }

    /// <summary>
    /// Parses an input string and returns a set of symbols.
    /// The "all" symbol converted to <see cref="WildcardSymbol"/>.
    /// </summary>
    /// <param name="symbols">The coma-separated list of symbols.</param>
    /// <returns>Returns a set of parsed symbols.</returns>
    public static IEnumerable<object> ParseSymbols(string symbols)
    {
        var setSymbols = new HashSet<object>();
        setSymbols.UnionWith(symbols.Split(','));

        // If string contains "all", remove "all" and add WildcardSymbol.
        const string wildcard = "all";
        if (setSymbols.Contains(wildcard))
        {
            setSymbols.Remove(wildcard);
            setSymbols.Add(WildcardSymbol.All);
        }

        return setSymbols;
    }
}
