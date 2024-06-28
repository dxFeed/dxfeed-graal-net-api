// <copyright file="TimeSeriesMap.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using DxFeed.Graal.Net.Events;
using DxFeed.Graal.Net.Utils;

namespace DxFeed.Graal.Net.Samples;

/// <summary>
/// Abstract base class for mapping <see cref="ITimeSeriesEvent"/> to CSV using <see cref="CsvHelper"/>.
/// Provides common functionality for parsing event flags and custom type converters.
/// </summary>
/// <typeparam name="T">The type of time series event to map.</typeparam>
internal abstract class AbstractTimeSeriesMap<T> : ClassMap<T>
    where T : ITimeSeriesEvent
{
    /// <summary>
    /// Parses the <see cref="IIndexedEvent.EventFlags"/> field from the last column in the CSV row.
    /// This field may not be represented, and is not contained in the CSV header.
    /// </summary>
    /// <param name="row">The CSV row being processed.</param>
    /// <returns>An integer representing the combined <see cref="IIndexedEvent.EventFlags"/>.</returns>
    protected static int ParseEventFlags(IReaderRow row)
    {
        var text = row.Context.Parser?.Record?.Last();
        if (string.IsNullOrEmpty(text) || !text.Contains("EventFlags="))
        {
            return 0;
        }

        var i = text.IndexOf("=", StringComparison.Ordinal);
        var textFlags = text.Substring(i + 1).Split(',');
        var flags = 0;
        foreach (var textFlag in textFlags)
        {
            switch (textFlag)
            {
                case "TX_PENDING":
                    flags |= EventFlags.TxPending;
                    break;
                case "REMOVE_EVENT":
                    flags |= EventFlags.RemoveEvent;
                    break;
                case "SNAPSHOT_BEGIN":
                    flags |= EventFlags.SnapshotBegin;
                    break;
                case "SNAPSHOT_END":
                    flags |= EventFlags.SnapshotEnd;
                    break;
                case "SNAPSHOT_SNIP":
                    flags |= EventFlags.SnapshotSnip;
                    break;
            }
        }

        return flags;
    }

    /// <summary>
    /// Custom type converter to handle string fields, converting "\NULL" to null.
    /// </summary>
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Created by CsvHelper")]
    protected class StringConverter : DefaultTypeConverter
    {
        /// <summary>
        /// Converts a string field to its corresponding object representation.
        /// </summary>
        /// <param name="text">The text to convert.</param>
        /// <param name="row">The CSV row being processed.</param>
        /// <param name="memberMapData">The member map data for the field.</param>
        /// <returns>The converted object, or null if the text is "\NULL".</returns>
        public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData) =>
            text == null || text.Equals("\\NULL", StringComparison.Ordinal) ? null : text;
    }

    /// <summary>
    /// Custom type converter to handle event time fields, converting them to Unix time in milliseconds.
    /// </summary>
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Created by CsvHelper")]
    protected class EventTimeConverter : DefaultTypeConverter
    {
        /// <summary>
        /// Converts a string representation of event time to Unix time in milliseconds.
        /// </summary>
        /// <param name="text">The text to convert.</param>
        /// <param name="row">The CSV row being processed.</param>
        /// <param name="memberMapData">The member map data for the field.</param>
        /// <returns>The Unix time in milliseconds.</returns>
        public override object ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData) =>
            string.IsNullOrEmpty(text) ? 0 : TimeFormat.Default.Parse(text).ToUnixTimeMilliseconds();
    }

    /// <summary>
    /// Custom type converter to handle fields representing both time and sequence.
    /// </summary>
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Created by CsvHelper")]
    protected class TimeAndSequenceConverter : DefaultTypeConverter
    {
        /// <summary>
        /// Converts a string representation of time and sequence to a combined Index value.
        /// </summary>
        /// <param name="text">The text to convert.</param>
        /// <param name="row">The CSV row being processed.</param>
        /// <param name="memberMapData">The member map data for the field.</param>
        /// <returns>The combined Index value of time and sequence.</returns>
        public override object ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData) =>
            (ParseTime(text) << 32) | (ParseSequence(row.GetField("Sequence")) & 0xFFFFFFFFL);

        /// <summary>
        /// Parses the time component from the string representation.
        /// </summary>
        /// <param name="time">The time string to parse.</param>
        /// <returns>The parsed time as a long value representing Unix time in seconds.</returns>
        private static long ParseTime(string? time)
        {
            if (string.IsNullOrEmpty(time) || time.Equals("0", StringComparison.Ordinal))
            {
                return 0;
            }

            return TimeFormat.Default.Parse(time).ToUnixTimeSeconds();
        }

        /// <summary>
        /// Parses the sequence and millis component from the string representation.
        /// </summary>
        /// <param name="sequence">The sequence string to parse.</param>
        /// <returns>The parsed sequence and millis as an integer value.</returns>
        private static int ParseSequence(string? sequence)
        {
            if (string.IsNullOrEmpty(sequence) || sequence.Equals("0", StringComparison.Ordinal))
            {
                return 0;
            }

            var i = sequence.IndexOf(':');
            return i >= 0
                ? (int.Parse(sequence.Substring(0, i), CultureInfo.InvariantCulture) << 22) +
                  int.Parse(sequence.Substring(i + 1), CultureInfo.InvariantCulture)
                : int.Parse(sequence, CultureInfo.InvariantCulture);
        }
    }
}
