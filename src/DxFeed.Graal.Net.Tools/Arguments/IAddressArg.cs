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

    public const string HelpText =
        """
        The address(es) to connect to retrieve data (see "Help address").
        For Token-Based Authorization, use the following format: "<address>:<port>[login=entitle:<token>]".
        """;

    [Value(Index, MetaName = MetaName, HelpText = HelpText, Required = false)]
    public string Address { get; set; }
}

public interface IAddressArgRequired : IAddressArg
{
    [Value(Index, MetaName = MetaName, HelpText = HelpText, Required = true)]
    public new string Address { get; set; }
}
