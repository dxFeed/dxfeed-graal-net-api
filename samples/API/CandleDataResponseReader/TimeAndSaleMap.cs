// <copyright file="TimeAndSaleMap.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using CsvHelper.Configuration;
using DxFeed.Graal.Net.Events.Market;

namespace DxFeed.Graal.Net.Samples;

/// <summary>
/// A class map for the <see cref="TimeAndSale"/> class, used to map CSV data to <see cref="TimeAndSale"/> objects.
/// </summary>
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Created by CsvHelper")]
internal sealed class TimeAndSaleMap : AbstractTimeSeriesMap<TimeAndSale>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TimeAndSaleMap"/> class.
    /// This constructor configures the mapping rules for the <see cref="TimeAndSale"/> class.
    /// </summary>
    public TimeAndSaleMap()
    {
        // Automatically map all fields using the given CSV configuration.
        AutoMap(new CsvConfiguration(CultureInfo.InvariantCulture) { IgnoreReferences = true });
        Map(m => m.EventTime).TypeConverter<EventTimeConverter>();
        Map(m => m.Index).Name("Time").TypeConverter<TimeAndSequenceConverter>();
        Map(m => m.Time).Ignore(); // Time is processed in Index using TimeAndSequenceConverter.
        Map(m => m.Sequence).Ignore(); // Sequence is processed in Index using TimeAndSequenceConverter.
        Map(m => m.ExchangeSaleConditions).Name("SaleConditions");
        Map(m => m.Buyer).TypeConverter<StringConverter>();
        Map(m => m.Seller).TypeConverter<StringConverter>();
        Map(m => m.EventFlags).Optional().Convert(args => ParseEventFlags(args.Row));
    }
}
