// <copyright file="IAddressArg.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using CommandLine;

namespace DxFeed.Graal.Net.Tools.Arguments;

public interface IAddressArg
{
    public const int Index = 0;
    public const string MetaName = "address";
    public const bool IsRequired = true;

    public const string HelpText = @"
The address to connect to retrieve data (remote host or local tape file).
If you do not specify a prefix (tcp: or file:) it will try to automatically determine the connector.
Examples:
    <tcp:hostname:port> - for remote host.
    <file:path_to_file> - for local file.";

    [Value(Index, MetaName = MetaName, Required = IsRequired, HelpText = HelpText)]
    public string Address { get; set; }
}
