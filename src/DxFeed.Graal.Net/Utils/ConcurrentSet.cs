// <copyright file="ConcurrentSet.cs" company="Devexperts LLC">
// Copyright © 2023 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DxFeed.Graal.Net.Utils;

/// <summary>
/// This is a simple wrapper over <see cref="ConcurrentDictionary{TKey,TValue}"/>,
/// to provide an interface as a <c>Set</c> (.NET does not provide a built-in concurrent hashset type)
/// for more consistent.
/// </summary>
/// <typeparam name="T">The type of elements in the set.</typeparam>
public class ConcurrentSet<T> : ICollection<T>
    where T : notnull
{
    private readonly ConcurrentDictionary<T, int> _inner;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConcurrentSet{T}"/> class.
    /// </summary>
    public ConcurrentSet() =>
        _inner = new ConcurrentDictionary<T, int>();

    /// <inheritdoc/>
    public int Count =>
        _inner.Count;

    /// <inheritdoc/>
    public bool IsReadOnly =>
        false;

    /// <summary>
    /// Adds the specified element to a set.
    /// </summary>
    /// <param name="item">The element to add to the set.</param>
    /// <returns>
    /// <c>true</c> if the element is added to the <see cref="ConcurrentSet{T}"/> object;
    /// <c>false</c> if the element is already present.
    /// </returns>
    public bool Add(T item) =>
        _inner.TryAdd(item, 0);

    /// <inheritdoc/>
    public void Clear() =>
        _inner.Clear();

    /// <inheritdoc/>
    public bool Contains(T item) =>
        _inner.ContainsKey(item);

    /// <inheritdoc/>
    public void CopyTo(T[] array, int arrayIndex) =>
        _inner.Keys.CopyTo(array, arrayIndex);

    /// <inheritdoc/>
    public bool Remove(T item) =>
        _inner.TryRemove(item, out _);

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() =>
        _inner.Keys.GetEnumerator();

    /// <inheritdoc/>
    void ICollection<T>.Add(T item) =>
        Add(item);

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
