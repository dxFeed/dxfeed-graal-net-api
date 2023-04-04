// <copyright file="IcebergType.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Utils;
using static DxFeed.Graal.Net.Events.Market.IcebergType;

namespace DxFeed.Graal.Net.Events.Market;

/// <summary>
/// Type of an iceberg order.
/// </summary>
public enum IcebergType
{
    /// <summary>
    /// Iceberg type is undefined, unknown or inapplicable.
    /// </summary>
    Undefined,

    /// <summary>
    /// Represents native (exchange-managed) iceberg type.
    /// </summary>
    Native,

    /// <summary>
    /// Represents synthetic (managed outside of the exchange) iceberg type.
    /// </summary>
    Synthetic,
}

/// <summary>
/// Class extension for <see cref="IcebergType"/> enum.
/// </summary>
internal static class IcebergTypeExt
{
    private static readonly IcebergType[] Values = EnumUtil.CreateEnumBitMaskArrayByValue(Undefined);

    /// <summary>
    /// Returns an enum constant of the <see cref="IcebergType"/> by integer code bit pattern.
    /// </summary>
    /// <param name="value">The specified value.</param>
    /// <returns>The enum constant of the specified enum type with the specified value.</returns>
    public static IcebergType ValueOf(int value) =>
        Values[value];
}
