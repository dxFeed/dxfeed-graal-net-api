// <copyright file="InstrumentProfileCollector.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Collections.Generic;
using DxFeed.Graal.Net.Native.Ipf;

namespace DxFeed.Graal.Net.Ipf.Live;

/// <summary>
/// Collects instrument profile updates and provides the live list of instrument profiles.
/// This class contains a map that keeps a unique instrument profile per symbol.
/// This class is intended to be used with InstrumentProfileConnection as a repository
/// that keeps profiles of all known instruments.
/// As set of instrument profiles stored in this collector can be accessed with view method.
/// A snapshot plus a live stream of updates can be accessed with addUpdateListener method.
/// Removal of instrument profile is represented by an InstrumentProfile instance
/// with a type equal to InstrumentProfileType.REMOVED.
/// </summary>
public class InstrumentProfileCollector
{
    private readonly InstrumentProfileCollectorHandle handle = InstrumentProfileCollectorHandle.Create();

    /// <summary>
    /// Gets last modification time (in milliseconds) of instrument profiles or zero if it is unknown.
    /// Note, that while the time is represented in milliseconds, the actual granularity of time here is a second.
    /// </summary>
    /// <returns>Last modification time (in milliseconds) of instrument profiles or zero if it is unknown.</returns>
    public long GetLastUpdateTime() =>
        handle.GetLastUpdateTime();

    /// <summary>
    /// Gets a concurrent view of the set of instrument profiles.
    /// Note, that removal of instrument profile is represented by an <see cref="InstrumentProfile"/> instance with a
    /// <see cref="InstrumentProfileType"/> equal to
    /// <c>InstrumentProfileType.REMOVED</c>
    /// Normally, this view exposes only non-removed profiles. However, if iteration is concurrent with removal,
    /// then a removed instrument profile (with a removed type) can be exposed by this view.
    /// </summary>
    /// <returns>A concurrent view of the set of instrument profiles.</returns>
    public IEnumerable<InstrumentProfile> View() =>
        handle.View();

    /// <summary>
    /// Adds listener that is notified about any updates in the set of instrument profiles.
    /// If a set of instrument profiles is not empty, then this listener will be immediately notified.
    /// </summary>
    /// <param name="listener">The profile update listener.</param>
    public void AddUpdateListener(InstrumentProfileUpdateListener listener) =>
        handle.AddUpdateListener(listener);

    /// <summary>
    /// Removes listener that is notified about any updates in the set of instrument profiles.
    /// </summary>
    /// <param name="listener">The profile update listener.</param>
    public void RemoveUpdateListener(InstrumentProfileUpdateListener listener) =>
        handle.RemoveUpdateListener(listener);

    internal InstrumentProfileCollectorHandle GetHandle() =>
        handle;
}
