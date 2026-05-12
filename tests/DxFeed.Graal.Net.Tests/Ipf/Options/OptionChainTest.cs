// <copyright file="OptionChainTest.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Ipf;
using DxFeed.Graal.Net.Ipf.Options;

namespace DxFeed.Graal.Net.Tests.Ipf.Options;

[TestFixture]
public class OptionChainTests
{
    private const string Underlying = "AAPL";

    private static InstrumentProfile Call(string symbol, int expiration, int lastTrade, double strike) =>
        new()
        {
            Type = "OPTION",
            Symbol = symbol,
            Underlying = Underlying,
            Expiration = expiration,
            LastTrade = lastTrade,
            Multiplier = 1,
            SPC = 1,
            CFI = "OCXXXX",
            Strike = strike,
        };

    private static InstrumentProfile Put(string symbol, int expiration, int lastTrade, double strike) =>
        new()
        {
            Type = "OPTION",
            Symbol = symbol,
            Underlying = Underlying,
            Expiration = expiration,
            LastTrade = lastTrade,
            Multiplier = 1,
            SPC = 1,
            CFI = "OPXXXX",
            Strike = strike,
        };

    private static OptionChain<InstrumentProfile> ChainFor(params InstrumentProfile[] profiles)
    {
        var builder = OptionChainsBuilder<InstrumentProfile>.Build(profiles);
        return builder.Chains[Underlying];
    }

    [Test]
    public void TestAddOption()
    {
        var profile = Call("O1", 20240101, 20231231, 100);
        var chain = ChainFor(profile);

        var seriesSet = chain.GetSeries();
        Assert.That(seriesSet, Has.Count.EqualTo(1));

        var retrievedSeries = seriesSet.Min;
        Assert.That(retrievedSeries?.Calls[100], Is.SameAs(profile));
    }

    [Test]
    public void TestClone()
    {
        var profile = Call("O1", 20240101, 20231231, 100);
        var chain = ChainFor(profile);

        var clone = (OptionChain<InstrumentProfile>)chain.Clone();
        var seriesSet = clone.GetSeries();
        Assert.That(seriesSet, Has.Count.EqualTo(1));

        var retrievedSeries = seriesSet.Min;
        Assert.That(retrievedSeries?.Calls[100], Is.SameAs(profile));
    }

    [Test]
    public void TestGetSeries()
    {
        var chain = ChainFor(
            Call("O1", 20240101, 20231231, 100),
            Put("O2", 20240201, 20231231, 95));

        var seriesSet = chain.GetSeries();
        Assert.That(seriesSet, Has.Count.EqualTo(2));
    }
}
