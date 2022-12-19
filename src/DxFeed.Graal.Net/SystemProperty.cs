// <copyright file="SystemProperty.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Collections.Generic;
using DxFeed.Graal.Net.Native;

namespace DxFeed.Graal.Net;

/// <inheritdoc cref="SystemPropertyNative"/>
public static class SystemProperty
{
    /// <inheritdoc cref="SystemPropertyNative.SetProperty"/>
    public static void SetProperty(string key, string value)
        => SystemPropertyNative.SetProperty(key, value);

    /// <summary>
    /// Sets the system properties from the provided key-value collection.
    /// </summary>
    /// <param name="properties">The key-value collection.</param>
    public static void SetProperties(IReadOnlyDictionary<string, string> properties)
    {
        foreach (var property in properties)
        {
            SetProperty(property.Key, property.Value);
        }
    }

    /// <inheritdoc cref="SystemPropertyNative.GetProperty"/>
    public static string? GetProperty(string key)
        => SystemPropertyNative.GetProperty(key);
}
