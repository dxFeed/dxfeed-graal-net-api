// <copyright file="PriceLevelService.cs" company="Devexperts LLC">
// Copyright © 2025 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using DxFeed.Graal.Net.Native.Orcs;

namespace DxFeed.Graal.Net.Orcs;

/// <summary>
///
/// </summary>
public class PriceLevelService : IDisposable
{
    private readonly PriceLevelServiceHandle handle;

    /// <summary>
    ///
    /// </summary>
    /// <param name="address"></param>
    public PriceLevelService(string address) => handle = PriceLevelServiceHandle.Create(address);

    /// <summary>
    ///
    /// </summary>
    public void Close() => handle.Close();

    /// <inheritdoc />
    public void Dispose() => Close();
}
