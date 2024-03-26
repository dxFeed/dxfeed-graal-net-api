// <copyright file="SessionType.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

namespace DxFeed.Graal.Net.Schedules;

// ReSharper disable InconsistentNaming

/// <summary>
/// Defines type of a session - what kind of trading activity is allowed (if any),
/// what rules are used, what impact on daily trading statistics it has, etc..
/// The <see cref="NO_TRADING"/> session type is used for non-trading sessions.
/// <p/>
/// Some exchanges support all session types defined here, others do not.
/// <p/>
/// Some sessions may have zero duration - e.g. indices that post value once a day.
/// Such sessions can be of any appropriate type, trading or non-trading.
/// </summary>
public enum SessionType
{
    /// <summary>
    /// Non-trading session type is used to mark periods of time during which trading is not allowed.
    /// </summary>
    NO_TRADING,

    /// <summary>
    /// Pre-market session type marks extended trading session before regular trading hours.
    /// </summary>
    PRE_MARKET,

    /// <summary>
    /// Regular session type marks regular trading hours session.
    /// </summary>
    REGULAR,

    /// <summary>
    /// After-market session type marks extended trading session after regular trading hours.
    /// </summary>
    AFTER_MARKET,
}
