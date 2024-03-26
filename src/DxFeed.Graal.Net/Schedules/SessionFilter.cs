// <copyright file="SessionFilter.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Native.Schedules;

namespace DxFeed.Graal.Net.Schedules;

/// <summary>
/// A filter for sessions used by various search methods.
/// This class provides predefined filters for certain Session attributes,
/// although users can create their own filters to suit their needs.
/// <p/>
/// Please note that sessions can be either trading or non-trading, and this distinction can be
/// either based on rules (e.g. weekends) or dictated by special occasions (e.g. holidays).
/// Different filters treat this distinction differently - some accept only trading sessions,
/// some only non-trading, and some ignore type of session altogether.
/// </summary>
public class SessionFilter
{
    /// <summary>
    /// Accepts any session - useful for pure schedule navigation.
    /// </summary>
    public static readonly SessionFilter ANY = new(0, null);

    /// <summary>
    /// Accepts trading sessions only - those with <see cref="Session"/>.<see cref="Session.IsTrading"/> == <c>true</c>.
    /// </summary>
    public static readonly SessionFilter TRADING = new(1, null);

    /// <summary>
    /// Accepts non-trading sessions only - those with  <see cref="Session"/>.<see cref="Session.IsTrading"/> == <c>false</c>.
    /// </summary>
    public static readonly SessionFilter NON_TRADING = new(2, null);

    /// <summary>
    /// Accepts any session with type <see cref="SessionType.NO_TRADING"/>.
    /// </summary>
    public static readonly SessionFilter NO_TRADING = new(3, SessionType.NO_TRADING);

    /// <summary>
    /// Accepts any session with type <see cref="SessionType.PRE_MARKET"/>.
    /// </summary>
    public static readonly SessionFilter PRE_MARKET = new(4, SessionType.PRE_MARKET);

    /// <summary>
    /// Accepts any session with type <see cref="SessionType.REGULAR"/>.
    /// </summary>
    public static readonly SessionFilter REGULAR = new(5, SessionType.REGULAR);

    /// <summary>
    /// Accepts any session with type <see cref="SessionType.AFTER_MARKET"/>.
    /// </summary>
    public static readonly SessionFilter AFTER_MARKET = new(6, SessionType.AFTER_MARKET);

    private SessionFilter(int id, SessionType? type)
    {
        Handle = SessionFilterHandle.GetInstance(id);
        Type = type;
    }

    /// <summary>
    /// Gets session type.
    /// </summary>
    public SessionType? Type { get; }

    internal SessionFilterHandle Handle { get; }
}
