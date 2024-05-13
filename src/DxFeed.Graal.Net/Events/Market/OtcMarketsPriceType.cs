// <copyright file="OtcMarketsPriceType.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Utils;

namespace DxFeed.Graal.Net.Events.Market;

/// <summary>
/// Type of prices on the OTC Markets.
///
/// Please see <a href="https://downloads.dxfeed.com/specifications/OTC_Markets_Data_Display_Requirements.pdf">OTC Markets Data Display Requirements</a>
/// </summary>
public enum OtcMarketsPriceType
{
    /// <summary>
    /// Unpriced quotes are an indication of interest (IOI) in a security
    /// used when a trader does not wish to show a price or size.
    /// Unpriced, name-only quotes are also used as the other side of a one-sided, priced quote.
    /// Unpriced quotes may not have a Quote Access Payment (QAP) value.
    /// </summary>
    Unpriced,

    /// <summary>
    /// Actual (Priced) is the actual amount a trader is willing to buy or sell securities.
    /// </summary>
    Actual,

    /// <summary>
    /// Offer Wanted/Bid Wanted (OW/BW) is used to solicit sellers/buyers,
    /// without displaying actual price or size.
    /// OW/BW quotes may not have a Quote Access Payment (QAP) value.
    /// </summary>
    Wanted,
}

/// <summary>
/// Class extension for <see cref="PriceType"/> enum.
/// </summary>
internal static class OtcMarketsPriceTypeExt
{
    private static readonly OtcMarketsPriceType[] Values = EnumUtil.CreateEnumBitMaskArrayByValue(OtcMarketsPriceType.Unpriced);

    /// <summary>
    /// Returns price type by integer code bit pattern.
    /// </summary>
    /// <param name="value">The specified value.</param>
    /// <returns>The enum constant of the specified enum type with the specified value.</returns>
    public static OtcMarketsPriceType ValueOf(int value) =>
        Values[value];
}
