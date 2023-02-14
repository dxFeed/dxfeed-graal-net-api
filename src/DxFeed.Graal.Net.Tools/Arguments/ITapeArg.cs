// <copyright file="ITapeArg.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using CommandLine;

namespace DxFeed.Graal.Net.Tools.Arguments;

public interface ITapeArg
{
    public const char ShortName = 't';
    public const string LongName = "tape";
    public const bool IsRequired = false;

    public const string HelpText = @"
Tape all incoming data into the spcified file.
This option has several parameters:

""format"" parameter defines format of stored data. Its value can be one of ""text"", ""binary"" or
""blob:<record>:<symbol>"" (binary format is used by default).
Blob is a special format that is used for compact store of a single-record, single-symbol data stream.

""saveas"" parameter overrides the type of stored messages.
Data messages can be stored as ""ticker_data"", ""stream_data"",""history_data"", or ""raw_data"".";

    [Option(ShortName, LongName, Required = IsRequired, HelpText = HelpText)]
    public string? Tape { get; set; }
}
