// <copyright file="InstrumentProfileConnection.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Native.Ipf;

namespace DxFeed.Graal.Net.Ipf.Live;

/// <summary>
/// Connects to an instrument profile URL and reads instrument profiles with support of
/// streaming live updates.
/// Please see <b>Instrument Profile Format</b> documentation for complete description.
/// <p/>The key different between this class and InstrumentProfileReader is that the later just reads
/// a snapshot of a set of instrument profiles, while this classes allows to track live updates, e.g.
/// addition and removal of instruments.
/// <p/>To use this class you need an address of the data source from you data provider. The name of the IPF file can
/// also serve as an address for debugging purposes.
/// <p/>The recommended usage of this class to receive a live stream of instrument profile updates is:
/// </summary>
public class InstrumentProfileConnection
{
    private readonly InstrumentProfileConnectionHandle handle;
    private readonly InstrumentProfileCollector collector;

    private InstrumentProfileConnection(string address, InstrumentProfileCollector collector)
    {
        handle = InstrumentProfileConnectionHandle.Create(address, collector.GetHandle());
        this.collector = collector;
    }

    /// <summary>
    /// Creates instrument profile connection with a specified address and collector.
    /// Address may be just "&lt;host&gt;:&lt;port&gt;" of server, URL, or a file path.
    /// The "[update=&lt;period&gt;]" clause can be optionally added at the end of the address to
    /// specify an {@link #getUpdatePeriod() update period} via an address string.
    /// Default update period is 1 minute.
    /// </summary>
    /// <param name="address">The connection address.</param>
    /// <param name="collector">The instrument profile collector to push updates into.</param>
    /// <returns>New instrument profile connection.</returns>
    public static InstrumentProfileConnection CreateConnection(string address, InstrumentProfileCollector collector) =>
        new(address, collector);

    /// <summary>
    /// Returns the address of this instrument profile connection.
    /// It does not include additional options specified as part of the address.
    /// </summary>
    /// <returns>The address of this instrument profile connection.</returns>
    public string GetAddress() =>
        handle.GetAddress()!;

    /// <summary>
    /// Returns update period in milliseconds.
    /// It is period of an update check when the instrument profiles source does not support live updates
    /// and/or when connection is dropped.
    /// </summary>
    /// <returns>The update period in milliseconds.</returns>
    public long GetUpdatePeriod() =>
        handle.GetUpdatePeriod();

    /// <summary>
    /// Changes update period in milliseconds.
    /// </summary>
    /// <param name="updatePeriod">The update period in milliseconds.</param>
    public void SetUpdatePeriod(long updatePeriod) =>
        handle.SetUpdatePeriod(updatePeriod);

    /// <summary>
    /// Returns last modification time (in milliseconds) of instrument profiles or zero if it is unknown.
    /// Note, that while the time is represented in milliseconds, the actual granularity of time here is a second.
    /// </summary>
    /// <returns>Last modification time (in milliseconds) of instrument profiles or zero if it is unknown.</returns>
    public long GetLastModified() =>
        handle.GetLastModified();

    /// <summary>
    /// Starts this instrument profile connection.
    /// </summary>
    public void Start() =>
        handle.Start();

    /// <summary>
    /// Closes this instrument profile connection.
    /// </summary>
    public void Close() =>
        handle.Close();
}
