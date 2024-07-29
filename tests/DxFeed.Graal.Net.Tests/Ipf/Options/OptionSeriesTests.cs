// <copyright file="OptionSeriesTests.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Ipf.Options;

// Use EqualConstraint for better assertion messages in case of failure.
#pragma warning disable NUnit2010

//Use ComparisonConstraint for better assertion messages in case of failure
#pragma warning disable NUnit2043

namespace DxFeed.Graal.Net.Tests.Ipf.Options;

[TestFixture]
public class OptionSeriesTests
{
    [Test]
    public void TestStrikes()
    {
        var series = new OptionSeries<string>();
        series.AddOption(true, 100, "CallOption1");
        series.AddOption(false, 95, "PutOption1");
        series.AddOption(true, 105, "CallOption2");

        var strikes = series.Strikes;
        Assert.That(strikes, Is.EquivalentTo(new List<double> { 95, 100, 105 }));
    }

    [Test]
    public void TestGetNStrikesAround()
    {
        var series = new OptionSeries<string>();
        series.AddOption(true, 100, "CallOption1");
        series.AddOption(false, 95, "PutOption1");
        series.AddOption(true, 105, "CallOption2");

        var centeredStrikes = series.GetNStrikesAround(2, 100);
        Assert.That(centeredStrikes, Is.EquivalentTo(new List<double> { 95, 100 }));
    }

    [Test]
    public void TestGetNStrikesAround_ThrowsException_WhenNIsNegative()
    {
        var series = new OptionSeries<string>();

        Assert.Throws<ArgumentException>(() => series.GetNStrikesAround(-1, 100), "Must not be less than zero.");
    }

    [Test]
    public void TestGetNStrikesAround_ScenarioWhenISmallerThanZero()
    {
        var series = new OptionSeries<string>();
        series.AddOption(true, 100, "CallOption1");
        series.AddOption(false, 95, "PutOption1");
        series.AddOption(true, 105, "CallOption2");

        var centeredStrikes = series.GetNStrikesAround(2, 97); // Strike 97 doesn't exist.
        Assert.That(centeredStrikes, Is.EquivalentTo(new List<double> { 95, 100 }));
    }

    [Test]
    public void TestClone()
    {
        var series = new OptionSeries<string>();
        series.AddOption(true, 100, "CallOption1");
        series.AddOption(false, 95, "PutOption1");

        var clone = (OptionSeries<string>)series.Clone();
        Assert.Multiple(() =>
        {
            Assert.That(clone.Strikes, Is.EquivalentTo(series.Strikes));
            Assert.That(clone.Calls[100], Is.EqualTo("CallOption1"));
            Assert.That(clone.Puts[95], Is.EqualTo("PutOption1"));
        });
    }

    [Test]
    public void TestEquality()
    {
        var series1 =
            new OptionSeries<string> { Expiration = 20240101, LastTrade = 20231231, Multiplier = 100, SPC = 1 };
        var series2 =
            new OptionSeries<string> { Expiration = 20240101, LastTrade = 20231231, Multiplier = 100, SPC = 1 };

        Assert.That(series1, Is.EqualTo(series2));
    }

    [Test]
    public void TestInequality()
    {
        var series1 =
            new OptionSeries<string> { Expiration = 20240101, LastTrade = 20231231, Multiplier = 100, SPC = 1 };
        var series2 =
            new OptionSeries<string> { Expiration = 20240102, LastTrade = 20231231, Multiplier = 100, SPC = 1 };

        Assert.That(series1, Is.Not.EqualTo(series2));
    }

    [Test]
    public void TestAddOption()
    {
        var series = new OptionSeries<string>();
        series.AddOption(true, 100, "CallOption1");

        Assert.Multiple(() =>
        {
            Assert.That(series.Calls.ContainsKey(100), Is.True);
            Assert.That(series.Calls[100], Is.EqualTo("CallOption1"));
        });
    }

    [Test]
    public void TestEqualityOperator()
    {
        var series1 =
            new OptionSeries<string> { Expiration = 20240101, LastTrade = 20231231, Multiplier = 100, SPC = 1 };
        var series2 =
            new OptionSeries<string> { Expiration = 20240101, LastTrade = 20231231, Multiplier = 100, SPC = 1 };

        Assert.That(series1 == series2, Is.True);
    }

    [Test]
    public void TestInequalityOperator()
    {
        var series1 =
            new OptionSeries<string> { Expiration = 20240101, LastTrade = 20231231, Multiplier = 100, SPC = 1 };
        var series2 =
            new OptionSeries<string> { Expiration = 20240102, LastTrade = 20231231, Multiplier = 100, SPC = 1 };

        Assert.That(series1 != series2, Is.True);
    }

    [Test]
    public void TestLessThanOperator()
    {
        var series1 =
            new OptionSeries<string> { Expiration = 20240101, LastTrade = 20231231, Multiplier = 100, SPC = 1 };
        var series2 =
            new OptionSeries<string> { Expiration = 20240102, LastTrade = 20231231, Multiplier = 100, SPC = 1 };

        Assert.That(series1 < series2, Is.True);
    }

    [Test]
    public void TestGreaterThanOperator()
    {
        var series1 =
            new OptionSeries<string> { Expiration = 20240102, LastTrade = 20231231, Multiplier = 100, SPC = 1 };
        var series2 =
            new OptionSeries<string> { Expiration = 20240101, LastTrade = 20231231, Multiplier = 100, SPC = 1 };

        Assert.That(series1 > series2, Is.True);
    }

    [Test]
    public void TestLessThanOrEqualOperator()
    {
        var series1 =
            new OptionSeries<string> { Expiration = 20240101, LastTrade = 20231231, Multiplier = 100, SPC = 1 };
        var series2 =
            new OptionSeries<string> { Expiration = 20240101, LastTrade = 20231231, Multiplier = 100, SPC = 1 };
        var series3 =
            new OptionSeries<string> { Expiration = 20240102, LastTrade = 20231231, Multiplier = 100, SPC = 1 };

        Assert.Multiple(() =>
        {
            Assert.That(series1 <= series2, Is.True);
            Assert.That(series1 <= series3, Is.True);
        });
    }

    [Test]
    public void TestGreaterThanOrEqualOperator()
    {
        var series1 =
            new OptionSeries<string> { Expiration = 20240101, LastTrade = 20231231, Multiplier = 100, SPC = 1 };
        var series2 =
            new OptionSeries<string> { Expiration = 20240101, LastTrade = 20231231, Multiplier = 100, SPC = 1 };
        var series3 =
            new OptionSeries<string> { Expiration = 20231231, LastTrade = 20231230, Multiplier = 100, SPC = 1 };

        Assert.Multiple(() =>
        {
            Assert.That(series1 >= series2, Is.True);
            Assert.That(series1 >= series3, Is.True);
        });
    }
}
