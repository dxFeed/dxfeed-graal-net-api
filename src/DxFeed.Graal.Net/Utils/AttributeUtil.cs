// <copyright file="AttributeUtil.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Reflection;

namespace DxFeed.Graal.Net.Utils;

/// <summary>
/// Provides utility methods for manipulating an <see cref="Attribute"/>.
/// </summary>
public static class AttributeUtil
{
    /// <summary>
    /// Generic version of <see cref="Attribute.GetCustomAttribute(Assembly,Type)"/>.
    /// Retrieves a custom attribute applied to a member of a type.
    /// Parameters specify the member, and the type of the custom attribute to search for.
    /// </summary>
    /// <param name="member">
    /// An object derived from the <see cref="Type"/> class that describes
    /// a constructor, event, field, method, or property member of a class.
    /// </param>
    /// <typeparam name="T">The type, or a base type, of the custom attribute to search for.</typeparam>
    /// <returns>
    /// A reference to the single custom attribute of type T that is applied to element,
    /// or <c>null</c> if there is no such attribute.
    /// </returns>
    public static T? GetCustomAttribute<T>(Type member)
        where T : Attribute =>
        (T?)Attribute.GetCustomAttribute(member, typeof(T));
}
