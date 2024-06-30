// <copyright file="Program.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using DxFeed.Graal.Net.Auth;
using DxFeed.Graal.Net.Events;
using DxFeed.Graal.Net.Events.Candles;
using DxFeed.Graal.Net.Events.Market;
using static System.Globalization.CultureInfo;

namespace DxFeed.Graal.Net.Samples;

/// <summary>
/// Demonstrates how to parse response from CandleData service.
/// Candlewebservice provides Candle and TimeAndSale data for particular time period
/// in the past with from-to period via REST-like API.
/// For more details see <a href="https://kb.dxfeed.com/en/data-services/aggregated-data-services/candlewebservice.html">KB</a>.
/// </summary>
internal abstract class Program
{
    private static async Task Main(string[] args)
    {
        // Create an authentication token. It can be Basic or Bearer authorization.
        var token = AuthToken.CreateBasicToken("user", "password");
        // Create an HTTP client with the base URL and the authentication token
        using var client = CreateHttpClient("https://tools.dxfeed.com/", token);

        var start = DateTimeOffset.Now.AddDays(-2).ToString("yyyyMMdd", InvariantCulture);
        var stop = DateTimeOffset.Now.AddDays(-1).ToString("yyyyMMdd", InvariantCulture);
        // URL for fetching candle events.
        var candleUrl = $"candledata?records=Candle&symbols=IBM{{=h}}&start={start}&stop={stop}&format=csv&compression=gzip";
        var response = await client.GetAsync(candleUrl);
        // Ensure the HTTP response status is successful.
        response.EnsureSuccessStatusCode();
        // Parse the response content into a list of Candle events.
        var candles = ParseEvents<Candle>(response);
        Console.WriteLine($"Received candles count: {candles.Count}");

        start = DateTimeOffset.Now.AddDays(-1).AddHours(-1).ToString("yyyyMMdd-hhmmss", InvariantCulture);
        stop = DateTimeOffset.Now.AddDays(-1).ToString("yyyyMMdd-hhmmss", InvariantCulture);
        // URL for fetching tns events.
        var tnsUrl = $"candledata?records=TimeAndSale&symbols=IBM&start={start}&stop={stop}&format=csv&compression=gzip";
        response = await client.GetAsync(tnsUrl);
        // Ensure the HTTP response status is successful.
        response.EnsureSuccessStatusCode();
        // Parse the response content into a list of TimeAndSale events.
        var timeAndSales = ParseEvents<TimeAndSale>(response);
        Console.WriteLine($"Received tns count: {timeAndSales.Count}");
    }

    /// <summary>
    /// Creates an <see cref="HttpClient"/> configured with the base URL and authorization token.
    /// </summary>
    /// <param name="baseUrl">The base URL for the API requests.</param>
    /// <param name="token">The authorization token.</param>
    /// <returns>A configured <see cref="HttpClient"/>  instance.</returns>
    private static HttpClient CreateHttpClient(string baseUrl, AuthToken? token)
    {
        var client = new HttpClient { BaseAddress = new Uri(baseUrl) };
        if (token != null)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token.Scheme, token.Value);
        }

        return client;
    }

    /// <summary>
    /// Parses events from the <see cref="HttpResponseMessage"/> based on the content type.
    /// </summary>
    /// <typeparam name="T">The type of event to parse.</typeparam>
    /// <param name="message">The <see cref="HttpResponseMessage"/> containing the event data.</param>
    /// <returns>A list of parsed events.</returns>
    private static List<T> ParseEvents<T>(HttpResponseMessage message)
        where T : ITimeSeriesEvent
    {
        var contentType = message.Content.Headers.ContentType?.MediaType;
        switch (contentType)
        {
            case "application/gzip":
            {
                using var stream = message.Content.ReadAsStream();
                using var gZipStream = new GZipStream(stream, CompressionMode.Decompress);
                using var reader = new StreamReader(gZipStream);
                return ParseEvents<T>(reader);
            }
            case "application/zip":
            {
                var events = new List<T>();
                using var stream = message.Content.ReadAsStream();
                using var archive = new ZipArchive(stream);
                foreach (var entry in archive.Entries)
                {
                    using var reader = new StreamReader(entry.Open());
                    events.AddRange(ParseEvents<T>(reader));
                }

                return events;
            }
            case "text/csv":
            {
                using var stream = message.Content.ReadAsStream();
                using var reader = new StreamReader(stream);
                return ParseEvents<T>(reader);
            }
            default:
                throw new InvalidOperationException($"Unknown content type: {contentType}");
        }
    }

    /// <summary>
    /// Parses events from the provided <see cref="StreamReader"/>.
    /// </summary>
    /// <typeparam name="T">The type of event to parse.</typeparam>
    /// <param name="reader">The <see cref="StreamReader"/> containing the event data.</param>
    /// <returns>A list of parsed events.</returns>
    private static List<T> ParseEvents<T>(StreamReader reader)
        where T : ITimeSeriesEvent
    {
        if (reader.EndOfStream)
        {
            return new List<T>();
        }

        var config = new CsvConfiguration(InvariantCulture)
        {
            AllowComments = false, HasHeaderRecord = true, MissingFieldFound = null, DetectDelimiter = true,
        };
        using var csv = new CsvReader(reader, config);
        csv.Context.RegisterClassMap<TimeAndSaleMap>();
        csv.Context.RegisterClassMap<CandleMap>();
        csv.Read();
        csv.ReadHeader();

        // Check event type.
        var expectedType = typeof(T).Name;
        var actualType = csv.HeaderRecord?.First();
        if (string.IsNullOrEmpty(actualType) || !actualType.Contains(expectedType, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Incorrect event type. Expected: {expectedType}, but found: {actualType ?? "null"}");
        }

        return csv.GetRecords<T>().ToList();
    }
}
