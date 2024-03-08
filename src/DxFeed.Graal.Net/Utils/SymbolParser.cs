// <copyright file="SymbolParser.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Collections;
using System.Collections.Generic;
using DxFeed.Graal.Net.Native.Utils;

namespace DxFeed.Graal.Net.Utils;

public class SymbolParser
{
    public static IEnumerable<string> Parse(string value) => SymbolParserNative.Parse(value);
}
