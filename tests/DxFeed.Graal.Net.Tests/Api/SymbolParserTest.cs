// <copyright file="SymbolParserTest.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Native.ErrorHandling;
using DxFeed.Graal.Net.Utils;

namespace DxFeed.Graal.Net.Tests.Api;

[TestFixture]
public class SymbolParserTest
{
    [Test]
    public void TestDefaultParse()
    {
        var expected = new HashSet<string> { "AAPL", "IBM" };
        Assert.Multiple(() =>
        {
            Assert.That(SymbolParser.Parse("AAPL,IBM").ToHashSet().SetEquals(expected));
            Assert.That(SymbolParser.Parse("AAPL").First(), Is.EqualTo("AAPL"));
            Assert.That(SymbolParser.Parse("*").First(), Is.EqualTo("*"));
            Assert.That(SymbolParser.Parse("all").First(), Is.EqualTo("*"));
        });
        Assert.Throws<JavaException>(() => SymbolParser.Parse(""));
    }
}
