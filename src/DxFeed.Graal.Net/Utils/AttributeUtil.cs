// <copyright file="AttributeUtil.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Reflection;

namespace DxFeed.Graal.Net.Utils;

/// <summary>
/// Provides utility methods for retrieving custom <see cref="Attribute"/>.
/// </summary>
public static class AttributeUtil
{
    /// <summary>
    /// A generic version of <see cref="Attribute.GetCustomAttribute(MemberInfo,Type,bool)"/> for <see cref="Type"/>.
    /// Retrieves a custom attribute applied to the type.
    /// Parameters specify the type with the custom attribute, the type of the custom attribute to search for.
    /// Doesn't look up the element's ancestors for custom attributes.
    /// </summary>
    /// <param name="type">The type to which the custom attribute is applied.</param>
    /// <typeparam name="T">The type, or a base type, of the custom attribute to search for.</typeparam>
    /// <returns>
    /// A reference to the single custom attribute of type <c>T</c> that is applied to element,
    /// or <c>null</c> if there is no such attribute.
    /// </returns>
    /// <exception cref="ArgumentNullException"><c>type</c> is <c>null</c>.</exception>
    /// <exception cref="AmbiguousMatchException">More than one of the requested attributes was found.</exception>
    /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded.</exception>
    public static T? GetCustomAttribute<T>(Type type)
        where T : Attribute =>
        GetCustomAttribute<T>(type, false);

    /// <summary>
    /// A generic version of <see cref="Attribute.GetCustomAttribute(MemberInfo,Type,bool)"/> for <see cref="Type"/>.
    /// Retrieves a custom attribute applied to the type.
    /// Parameters specify the type with the custom attribute, the type of the custom attribute to search for,
    /// and whether to search ancestors of the type.
    /// </summary>
    /// <param name="type">The type to which the custom attribute is applied.</param>
    /// <param name="inherit">
    /// If <c>true</c>, specifies to also search the ancestors of element for custom attributes.
    /// </param>
    /// <typeparam name="T">The type, or a base type, of the custom attribute to search for.</typeparam>
    /// <returns>
    /// A reference to the single custom attribute of type <c>T</c> that is applied to element,
    /// or <c>null</c> if there is no such attribute.
    /// </returns>
    /// <exception cref="ArgumentNullException"><c>type</c> is <c>null</c>.</exception>
    /// <exception cref="AmbiguousMatchException">More than one of the requested attributes was found.</exception>
    /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded.</exception>
    public static T? GetCustomAttribute<T>(Type type, bool inherit)
        where T : Attribute =>
        (T?)Attribute.GetCustomAttribute(type, typeof(T), inherit);
}
