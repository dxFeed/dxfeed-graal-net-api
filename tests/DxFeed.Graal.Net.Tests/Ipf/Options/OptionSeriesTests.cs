// <copyright file="OptionSeriesTests.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Ipf;
using DxFeed.Graal.Net.Ipf.Options;

// Use EqualConstraint for better assertion messages in case of failure.
#pragma warning disable NUnit2010

// Use ComparisonConstraint for better assertion messages in case of failure
#pragma warning disable NUnit2043

namespace DxFeed.Graal.Net.Tests.Ipf.Options;

[TestFixture]
public class OptionSeriesTests
{
    private const string Underlying = "TESTU";

    private static InstrumentProfile Option(
        string symbol,
        int expiration,
        int lastTrade,
        double multiplier,
        double spc,
        bool isCall,
        double strike) =>
        new()
        {
            Type = "OPTION",
            Symbol = symbol,
            Underlying = Underlying,
            Expiration = expiration,
            LastTrade = lastTrade,
            Multiplier = multiplier,
            SPC = spc,
            CFI = isCall ? "OCXXXX" : "OPXXXX",
            Strike = strike,
        };

    private static OptionSeries<InstrumentProfile> BuildSeries(params InstrumentProfile[] profiles)
    {
        var builder = OptionChainsBuilder<InstrumentProfile>.Build(profiles);
        return builder.Chains[Underlying].GetSeries().Min!;
    }

    private static OptionSeries<InstrumentProfile> SeriesWithDefaults(
        int expiration,
        int lastTrade,
        double multiplier,
        double spc,
        string symbol,
        bool isCall,
        double strike) =>
        BuildSeries(Option(symbol, expiration, lastTrade, multiplier, spc, isCall, strike));

    [Test]
    public void TestStrikes()
    {
        var series = BuildSeries(
            Option("C100", 20240101, 20231231, 1, 1, true, 100),
            Option("P95", 20240101, 20231231, 1, 1, false, 95),
            Option("C105", 20240101, 20231231, 1, 1, true, 105));

        var strikes = series.Strikes;
        Assert.That(strikes, Is.EquivalentTo(new List<double> { 95, 100, 105 }));
    }

    [Test]
    public void TestGetNStrikesAround()
    {
        var series = BuildSeries(
            Option("C100", 20240101, 20231231, 1, 1, true, 100),
            Option("P95", 20240101, 20231231, 1, 1, false, 95),
            Option("C105", 20240101, 20231231, 1, 1, true, 105));

        var centeredStrikes = series.GetNStrikesAround(2, 100);
        Assert.That(centeredStrikes, Is.EquivalentTo(new List<double> { 95, 100 }));
    }

    [Test]
    public void TestGetNStrikesAround_ThrowsException_WhenNIsNegative()
    {
        var series = BuildSeries(Option("C100", 20240101, 20231231, 1, 1, true, 100));

        Assert.Throws<ArgumentException>(() => series.GetNStrikesAround(-1, 100), "Must not be less than zero.");
    }

    [Test]
    public void TestGetNStrikesAround_ScenarioWhenISmallerThanZero()
    {
        var series = BuildSeries(
            Option("C100", 20240101, 20231231, 1, 1, true, 100),
            Option("P95", 20240101, 20231231, 1, 1, false, 95),
            Option("C105", 20240101, 20231231, 1, 1, true, 105));

        var centeredStrikes = series.GetNStrikesAround(2, 97); // Strike 97 doesn't exist.
        Assert.That(centeredStrikes, Is.EquivalentTo(new List<double> { 95, 100 }));
    }

    [Test]
    public void TestClone()
    {
        var series = BuildSeries(
            Option("C100", 20240101, 20231231, 1, 1, true, 100),
            Option("P95", 20240101, 20231231, 1, 1, false, 95));

        var clone = (OptionSeries<InstrumentProfile>)series.Clone();
        Assert.Multiple(() =>
        {
            Assert.That(clone.Strikes, Is.EquivalentTo(series.Strikes));
            Assert.That(clone.Calls[100], Is.SameAs(series.Calls[100]));
            Assert.That(clone.Puts[95], Is.SameAs(series.Puts[95]));
        });
    }

    [Test]
    public void TestEquality()
    {
        var series1 = SeriesWithDefaults(20240101, 20231231, 100, 1, "S1", true, 100);
        var series2 = SeriesWithDefaults(20240101, 20231231, 100, 1, "S2", true, 200);

        Assert.That(series1, Is.EqualTo(series2));
    }

    [Test]
    public void TestInequality()
    {
        var series1 = SeriesWithDefaults(20240101, 20231231, 100, 1, "S1", true, 100);
        var series2 = SeriesWithDefaults(20240102, 20231231, 100, 1, "S2", true, 100);

        Assert.That(series1, Is.Not.EqualTo(series2));
    }

    [Test]
    public void TestAddOption()
    {
        var profile = Option("C100", 20240101, 20231231, 1, 1, true, 100);
        var series = BuildSeries(profile);

        Assert.Multiple(() =>
        {
            Assert.That(series.Calls.ContainsKey(100), Is.True);
            Assert.That(series.Calls[100], Is.SameAs(profile));
        });
    }

    [Test]
    public void TestEqualityOperator()
    {
        var series1 = SeriesWithDefaults(20240101, 20231231, 100, 1, "S1", true, 100);
        var series2 = SeriesWithDefaults(20240101, 20231231, 100, 1, "S2", true, 200);

        Assert.That(series1 == series2, Is.True);
    }

    [Test]
    public void TestInequalityOperator()
    {
        var series1 = SeriesWithDefaults(20240101, 20231231, 100, 1, "S1", true, 100);
        var series2 = SeriesWithDefaults(20240102, 20231231, 100, 1, "S2", true, 100);

        Assert.That(series1 != series2, Is.True);
    }

    [Test]
    public void TestLessThanOperator()
    {
        var series1 = SeriesWithDefaults(20240101, 20231231, 100, 1, "S1", true, 100);
        var series2 = SeriesWithDefaults(20240102, 20231231, 100, 1, "S2", true, 100);

        Assert.That(series1 < series2, Is.True);
    }

    [Test]
    public void TestGreaterThanOperator()
    {
        var series1 = SeriesWithDefaults(20240102, 20231231, 100, 1, "S1", true, 100);
        var series2 = SeriesWithDefaults(20240101, 20231231, 100, 1, "S2", true, 100);

        Assert.That(series1 > series2, Is.True);
    }

    [Test]
    public void TestLessThanOrEqualOperator()
    {
        var series1 = SeriesWithDefaults(20240101, 20231231, 100, 1, "A", true, 100);
        var series2 = SeriesWithDefaults(20240101, 20231231, 100, 1, "B", true, 100);
        var series3 = SeriesWithDefaults(20240102, 20231231, 100, 1, "C", true, 100);

        Assert.Multiple(() =>
        {
            Assert.That(series1 <= series2, Is.True);
            Assert.That(series1 <= series3, Is.True);
        });
    }

    [Test]
    public void TestGreaterThanOrEqualOperator()
    {
        var series1 = SeriesWithDefaults(20240101, 20231231, 100, 1, "A", true, 100);
        var series2 = SeriesWithDefaults(20240101, 20231231, 100, 1, "B", true, 100);
        var series3 = SeriesWithDefaults(20231231, 20231230, 100, 1, "C", true, 100);

        Assert.Multiple(() =>
        {
            Assert.That(series1 >= series2, Is.True);
            Assert.That(series1 >= series3, Is.True);
        });
    }
}
