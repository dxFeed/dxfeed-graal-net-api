// <copyright file="IndexedEventSource.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace DxFeed.Graal.Net.Events;

/// <summary>
/// Source identifier for <see cref="IIndexedEvent"/>.
/// <br/>
/// For more details see <a href="https://docs.dxfeed.com/dxfeed/api/com/dxfeed/event/IndexedEventSource.html">Javadoc</a>.
/// </summary>
/// <seealso cref="IIndexedEvent.EventSource"/>
public class IndexedEventSource
{
    /// <summary>
    /// The default source with zero identifier for all events that do not support multiple sources.
    /// </summary>
    public static readonly IndexedEventSource DEFAULT = new(0, "DEFAULT");

    /// <summary>
    /// Initializes a new instance of the <see cref="IndexedEventSource"/> class.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="name">The name of identifier.</param>
    public IndexedEventSource(int id, string name)
    {
        Id = id;
        Name = name;
    }

    /// <summary>
    /// Gets a source identifier. Source identifier is non-negative.
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// Gets a name of identifier.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Returns a string representation of the object.
    /// </summary>
    /// <returns>A string representation of the object.</returns>
    public override string ToString() => Name;

    /// <summary>
    /// Indicates whether some other indexed event source has the same id.
    /// </summary>
    /// <param name="obj"> The object to compare with the current object.</param>
    /// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj) =>
        obj == this || (obj is IndexedEventSource source && Id == source.Id);

    /// <summary>
    /// Returns a hash code value for this object.
    /// The result of this method is equal to <see cref="Id"/>.
    /// </summary>
    /// <returns>A hash code value for this object.</returns>
    public override int GetHashCode() => Id;
}
