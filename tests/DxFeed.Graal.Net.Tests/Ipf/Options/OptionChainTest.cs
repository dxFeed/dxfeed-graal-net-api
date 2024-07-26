// <copyright file="OptionChainTest.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Ipf.Options;

namespace DxFeed.Graal.Net.Tests.Ipf.Options;

[TestFixture]
public class OptionChainTests
{
    [Test]
    public void TestAddOption()
    {
        var chain = new OptionChain<string>("AAPL");
        var series = new OptionSeries<string> { Expiration = 20240101, LastTrade = 20231231 };
        chain.AddOption(series, true, 100, "CallOption1");

        var seriesSet = chain.GetSeries();
        Assert.That(seriesSet, Has.Count.EqualTo(1));

        var retrievedSeries = seriesSet.Min;
        Assert.That(retrievedSeries?.Calls[100], Is.EqualTo("CallOption1"));
    }

    [Test]
    public void TestClone()
    {
        var chain = new OptionChain<string>("AAPL");
        var series = new OptionSeries<string> { Expiration = 20240101, LastTrade = 20231231 };
        chain.AddOption(series, true, 100, "CallOption1");

        var clone = (OptionChain<string>)chain.Clone();
        var seriesSet = clone.GetSeries();
        Assert.That(seriesSet, Has.Count.EqualTo(1));

        var retrievedSeries = seriesSet.Min;
        Assert.That(retrievedSeries?.Calls[100], Is.EqualTo("CallOption1"));
    }

    [Test]
    public void TestGetSeries()
    {
        var chain = new OptionChain<string>("AAPL");
        var series1 = new OptionSeries<string> { Expiration = 20240101, LastTrade = 20231231 };
        var series2 = new OptionSeries<string> { Expiration = 20240201, LastTrade = 20231231 };
        chain.AddOption(series1, true, 100, "CallOption1");
        chain.AddOption(series2, false, 95, "PutOption1");

        var seriesSet = chain.GetSeries();
        Assert.That(seriesSet, Has.Count.EqualTo(2));
    }
}
