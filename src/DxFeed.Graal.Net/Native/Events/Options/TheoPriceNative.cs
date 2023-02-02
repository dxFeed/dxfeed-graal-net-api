// <copyright file="TheoPriceNative.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Events.Options;

namespace DxFeed.Graal.Net.Native.Events.Options;

/// <summary>
/// The structure contains all the fields required
/// to build an <see cref="TheoPrice"/>.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal readonly record struct TheoPriceNative(
    EventTypeNative EventType,
    int EventFlags,
    long Index,
    double Price,
    double UnderlyingPrice,
    double Delta,
    double Gamma,
    double Dividend,
    double Interest);
