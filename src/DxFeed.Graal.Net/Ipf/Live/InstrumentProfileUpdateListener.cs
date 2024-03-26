// <copyright file="InstrumentProfileUpdateListener.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Collections.Generic;

namespace DxFeed.Graal.Net.Ipf.Live;

/// <summary>
/// Notifies about instrument profile changes.
/// </summary>
/// <param name="profiles">The list of instruments profile.</param>
public delegate void InstrumentProfileUpdateListener(List<InstrumentProfile> profiles);
