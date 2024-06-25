// <copyright file="InstrumentProfileReader.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using DxFeed.Graal.Net.Auth;
using DxFeed.Graal.Net.Native.Ipf;

namespace DxFeed.Graal.Net.Ipf;

/// <summary>
/// Reads instrument profiles from the stream using Instrument Profile Format (IPF).
/// Please see <b>Instrument Profile Format</b> documentation for complete description.
/// This reader automatically uses data formats as specified in the stream.
/// <br/>
/// This reader is intended for "one time only" usage: create new instances for new IPF reads.
/// <br/>
/// For backward compatibility reader can be configured with system property "com.dxfeed.ipf.complete" to control
/// the strategy for missing "##COMPLETE" tag when reading IPF, possible values are:
/// <ul>
///     <li><c>warn</c> - show warning in the log (default)</li>
///     <li><c>error</c> - throw exception (future default)</li>
///     <li><c>ignore</c> - do nothing (for backward compatibility)</li>
/// </ul>
/// </summary>
public class InstrumentProfileReader
{
    private readonly InstrumentProfileReaderNative ipfReaderNative = InstrumentProfileReaderNative.Create();

    /// <summary>
    /// Resolves the given address specification into its corresponding URL format, particularly transforming
    /// simple "host:port" address specifications into a full HTTP URL.
    /// </summary>
    /// <remarks>
    /// If the provided address appears to be a "host:port" pattern without any forward slashes,
    /// this method will attempt to convert it to a standardized HTTP URL format targeting an
    /// "ipf/all.ipf.gz" endpoint. If the address contains a query parameter (e.g., "host:port?param=value"),
    /// the parameter will be preserved in the resulting URL. Addresses that do not fit the "host:port"
    /// pattern, or that have invalid port numbers, are returned unchanged.
    /// </remarks>
    /// <param name="address">The address specification to be resolved into a URL.</param>
    /// <returns>The resolved URL corresponding to the specified address.</returns>
    public static string? ResolveSourceUrl(string address) =>
        InstrumentProfileReaderNative.ResolveSourceUrl(address);

    /// <summary>
    /// Returns last modification time (in milliseconds) from last <see cref="ReadFromFile(string)"/> operation
    /// or zero if it is unknown.
    /// </summary>
    /// <returns>The last modification time.</returns>
    public long GetLastModified() =>
        ipfReaderNative.GetLastModified();

    /// <summary>
    /// Returns <c>true</c> if IPF was fully read on last <see cref="ReadFromFile(string)"/> operation.
    /// </summary>
    /// <returns><c>true</c> if IPF was fully read; otherwise, <c>false</c>.</returns>
    public bool WasComplete() =>
        ipfReaderNative.WasComplete();

    /// <summary>
    /// Reads and returns instrument profiles from specified file.
    /// This method recognizes data compression formats "zip" and "gzip" automatically.
    /// In case of <em>zip</em> the first file entry will be read and parsed as a plain data stream.
    /// In case of <em>gzip</em> compressed content will be read and processed.
    /// In other cases data considered uncompressed and will be parsed as is.
    /// <br/>
    /// Authentication information can be supplied to this method as part of URL user info
    /// like <c>"http://user:password@host:port/path/file.ipf"</c>.
    /// <br/>
    /// This operation updates <see cref="GetLastModified"/> and <see cref="WasComplete"/>.
    /// </summary>
    /// <param name="address">The URL of file to read from.</param>
    /// <returns>The list of instrument profiles.</returns>
    public List<InstrumentProfile> ReadFromFile(string address) =>
        ReadFromFile(address, null, null);

    /// <summary>
    /// Reads and returns instrument profiles from specified file.
    /// This method recognizes data compression formats "zip" and "gzip" automatically.
    /// In case of <em>zip</em> the first file entry will be read and parsed as a plain data stream.
    /// In case of <em>gzip</em> compressed content will be read and processed.
    /// In other cases data considered uncompressed and will be parsed as is.
    /// <br/>
    /// Specified user and password take precedence over authentication information that is supplied to this method
    /// as part of URL user info like <c>"http://user:password@host:port/path/file.ipf"</c>.
    /// <br/>
    /// This operation updates <see cref="GetLastModified"/> and <see cref="WasComplete"/>.
    /// </summary>
    /// <param name="address">The URL of file to read from.</param>
    /// <param name="user">The username (maybe null).</param>
    /// <param name="password">The password (maybe null).</param>
    /// <returns>The list of instrument profiles.</returns>
    public List<InstrumentProfile> ReadFromFile(string address, string? user, string? password) =>
        ipfReaderNative.ReadFromFile(address, user, password);

    /// <summary>
    /// Reads and returns instrument profiles from specified address with a specified token credentials.
    /// This method recognizes data compression formats "zip" and "gzip" automatically.
    /// In case of <em>zip</em> the first file entry will be read and parsed as a plain data stream.
    /// In case of <em>gzip</em> compressed content will be read and processed.
    /// In other cases data considered uncompressed and will be parsed as is.
    ///
    /// <p/>Specified token take precedence over authentication information that is supplied to this method
    /// as part of URL user info like <c>"http://user:password@host:port/path/file.ipf"</c>.
    ///
    /// This operation updates <see cref="GetLastModified"/> and <see cref="WasComplete"/>.
    /// </summary>
    /// <param name="address">The URL of file to read from.</param>
    /// <param name="token">The token (maybe null).</param>
    /// <returns>The list of instrument profiles.</returns>
    public List<InstrumentProfile> ReadFromFile(string address, AuthToken? token) =>
        throw new NotImplementedException();
}
