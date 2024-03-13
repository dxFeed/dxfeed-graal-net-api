// <copyright file="CmdArgsUtil.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using DxFeed.Graal.Net.Events;

namespace DxFeed.Graal.Net.Utils;

/// <summary>
/// A collection of static helper methods for parses command-line arguments.
/// </summary>
public static class CmdArgsUtil
{
    /// <summary>
    /// Parses an input string and returns a set of symbols.
    /// </summary>
    /// <param name="symbols">The coma-separated list of symbols.</param>
    /// <returns>Returns created a set of parsed symbols.</returns>
    public static IEnumerable<string> ParseSymbols(string symbols) => SymbolParser.Parse(symbols);

    /// <summary>
    /// Parses an input string and returns a set of event types.
    /// </summary>
    /// <param name="types">The coma-separated list of event types.</param>
    /// <returns>Returns a set of parsed types.</returns>
    /// <exception cref="ArgumentException">If the passed type is not available.</exception>
    public static IEnumerable<Type> ParseTypes(string types)
    {
        var availableTypes = new Dictionary<string, Type>(
            ReflectionUtil.CreateTypesDictionary(IEventType.GetEventTypes()),
            StringComparer.OrdinalIgnoreCase);

        var result = new HashSet<Type>();
        foreach (var typeName in types.Split(',').Where(type => !string.IsNullOrWhiteSpace(type)))
        {
            if (!availableTypes.TryGetValue(typeName.Trim(), out var type))
            {
                throw new ArgumentException(
                    $"{typeName} event type is not available! List of available event types: " +
                    $"{ReflectionUtil.CreateTypesString(availableTypes)}.");
            }

            result.Add(type);
        }

        return result;
    }

    /// <inheritdoc cref="TimeFormat.Parse"/>
    public static DateTimeOffset ParseFromTime(string fromTime) => TimeFormat.Default.Parse(fromTime);

    /// <summary>
    /// Parses the input collection of strings and returns a collection of key-value properties.
    /// The input strings should look like comma-separated: "key=value".
    /// </summary>
    /// <param name="properties">The input comma-separated key-value pairs.</param>
    /// <returns>Returns collection of key-value properties.</returns>
    /// <exception cref="ArgumentException">If string has wrong format.</exception>
    public static IReadOnlyDictionary<string, string> ParseProperties(string properties)
    {
        var result = new Dictionary<string, string>();
        foreach (var property in properties.Split(',').Where(type => !string.IsNullOrWhiteSpace(type)))
        {
            var kvp = property.Split('=').Select(s => s.Trim()).ToList();
            if (kvp.Count != 2)
            {
                throw new ArgumentException(
                    $"Invalid key-value pair property: {property}");
            }

            result[kvp[0]] = kvp[1];
        }

        return result;
    }
}
