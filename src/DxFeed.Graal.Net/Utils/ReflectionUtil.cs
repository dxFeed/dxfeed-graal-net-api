// <copyright file="ReflectionUtil.cs" company="Devexperts LLC">
// Copyright © 2023 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DxFeed.Graal.Net.Utils;

/// <summary>
/// Simple utility class for working with the reflection API.
/// Used to manipulate types.
/// </summary>
public static class ReflectionUtil
{
    /// <summary>
    /// Gets all inherited types form the specified type,
    /// and creates a coma-separated string with types names.
    /// Abstract classes and interfaces ignored.
    /// </summary>
    /// <param name="type">The specified type.</param>
    /// <returns>Returns comma-separated string with inherited types.</returns>
    /// <exception cref="NullReferenceException">If cannot find specified type.</exception>
    public static string GetInheritedTypesString(Type type) =>
        CreateTypesString(GetInheritedTypes(type));

    /// <summary>
    /// Creates a coma-separated string with types names from types enumerable.
    /// </summary>
    /// <param name="types">The specified types.</param>
    /// <returns>Returns comma-separated string with inherited types.</returns>
    public static string CreateTypesString(IEnumerable<Type> types) =>
        string.Join(",", types.Select(x => x.Name));

    /// <summary>
    /// Creates a coma-separated string with types names from types dictionary.
    /// </summary>
    /// <param name="types">The specified types.</param>
    /// <returns>Returns comma-separated string with inherited types.</returns>
    public static string CreateTypesString(IDictionary<string, Type> types) =>
        string.Join(",", types.Select(x => x.Key));

    /// <summary>
    /// Gets all inherited types form the specified type,
    /// and creates a dictionary where the key is the type name and value is the type.
    /// Abstract classes and interfaces ignored.
    /// </summary>
    /// <param name="type">The specified type.</param>
    /// <returns>Returns dictionary with inherited types.</returns>
    /// <exception cref="NullReferenceException">If cannot find specified type.</exception>
    public static IDictionary<string, Type> GetInheritedTypesDictionary(Type type) =>
        CreateTypesDictionary(GetInheritedTypes(type));

    /// <summary>
    /// Creates a dictionary from specified types where the key is the type name and value is the type.
    /// </summary>
    /// <param name="types">The specified types.</param>
    /// <returns>Returns dictionary with inherited types.</returns>
    public static IDictionary<string, Type> CreateTypesDictionary(IEnumerable<Type> types) =>
        types.ToDictionary(kvp => kvp.Name, kvp => kvp);

    /// <summary>
    /// Gets all inherited types form the specified type.
    /// Abstract classes and interfaces ignored.
    /// </summary>
    /// <param name="type">The specified type.</param>
    /// <returns>Returns enumerable with inherited types.</returns>
    /// <exception cref="NullReferenceException">If cannot find specified type.</exception>
    public static IEnumerable<Type> GetInheritedTypes(Type type) =>
        Assembly.GetAssembly(type)!
            .GetTypes()
            .Where(type.IsAssignableFrom)
            .Where(t => t is { IsAbstract: false, IsClass: true });
}
