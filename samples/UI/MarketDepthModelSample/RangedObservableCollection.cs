// <copyright file="RangedObservableCollection.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace MarketDepthModelSample;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

/// <summary>
/// The RangedObservableCollection class extends <see cref="ObservableCollection{T}"/>
/// to provide a method for replacing the entire collection with a new range of items.
/// </summary>
/// <typeparam name="T">The type of elements in the collection.</typeparam>
public class RangedObservableCollection<T> : ObservableCollection<T>
{
    /// <summary>
    /// Replaces the current items in the collection with the provided collection.
    /// </summary>
    /// <param name="collection">The collection of items to replace the current items.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provided collection is null.</exception>
    public void ReplaceRange(IEnumerable<T> collection)
    {
        if (collection == null)
        {
            throw new ArgumentNullException(nameof(collection));
        }

        Items.Clear();
        foreach (var i in collection)
        {
            Items.Add(i);
        }

        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }
}
