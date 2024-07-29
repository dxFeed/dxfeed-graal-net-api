// <copyright file="OptionChainsBuilderTest.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Ipf;
using DxFeed.Graal.Net.Ipf.Options;

namespace DxFeed.Graal.Net.Tests.Ipf.Options;

[TestFixture]
public class OptionChainsBuilderTests
{
    [Test]
    public void TestBuildOptionChains()
    {
        var profiles = new List<InstrumentProfile>
        {
            CreateInstrumentProfile("AAPL", "", 100, "OCXXXX"),
            CreateInstrumentProfile("", "AAPL", 105, "OPXXXX")
        };

        var builder = OptionChainsBuilder<InstrumentProfile>.Build(profiles);
        var chains = builder.Chains;

        Assert.That(chains.ContainsKey("AAPL"), Is.True);
        var chain = chains["AAPL"];
        var seriesSet = chain.GetSeries();
        Assert.That(seriesSet, Has.Count.EqualTo(1));

        var series = seriesSet.Min;
        Assert.Multiple(() =>
        {
            Assert.That(series?.Calls.ContainsKey(100), Is.True);
            Assert.That(series?.Puts.ContainsKey(105), Is.True);
        });
    }

    [Test]
    public void TestAddOption()
    {
        var builder = new OptionChainsBuilder<InstrumentProfile>
        {
            Product = "AAPL",
            Underlying = "AAPL",
            Expiration = 20240101,
            LastTrade = 20231231,
            Multiplier = 100,
            SPC = 1,
            AdditionalUnderlyings = "US$ 50",
            MMY = "202401",
            OptionType = "STAN",
            ExpirationStyle = "Weeklys",
            SettlementStyle = "Close",
            CFI = "OCXXXX",
            Strike = 100
        };

        var option = CreateInstrumentProfile("AAPL", "AAPL", 100, "OCXXXX");

        builder.AddOption(option);
        var chains = builder.Chains;

        Assert.That(chains.ContainsKey("AAPL"), Is.True);
        var chain = chains["AAPL"];
        var seriesSet = chain.GetSeries();
        Assert.That(seriesSet, Has.Count.EqualTo(1));

        var series = seriesSet.Min;
        Assert.That(series?.Calls.ContainsKey(100), Is.True);
    }

    private static InstrumentProfile CreateInstrumentProfile(string product, string underlying, int strike, string cfi) =>
        new()
        {
            Type = "OPTION",
            Product = product,
            Underlying = underlying,
            Expiration = 20240101,
            LastTrade = 20231231,
            Multiplier = 100,
            SPC = 1,
            AdditionalUnderlyings = "US$ 50",
            MMY = "202401",
            OptionType = "STAN",
            ExpirationStyle = "Weeklys",
            SettlementStyle = "Close",
            CFI = cfi,
            Strike = strike
        };
}
