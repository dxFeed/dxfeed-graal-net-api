// <copyright file="InstrumentProfileTests.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Ipf;

namespace DxFeed.Graal.Net.Tests.Ipf;

[TestFixture]
public class InstrumentProfileTests
{
    [Test]
    public void Constructor_ShouldInitializeDefaultValues()
    {
        var profile = new InstrumentProfile();

        Assert.Multiple(() =>
        {
            Assert.That(profile.Type, Is.EqualTo(string.Empty));
            Assert.That(profile.Symbol, Is.EqualTo(string.Empty));
            Assert.That(profile.Description, Is.EqualTo(string.Empty));
            Assert.That(profile.LocalSymbol, Is.EqualTo(string.Empty));
            Assert.That(profile.LocalDescription, Is.EqualTo(string.Empty));
            Assert.That(profile.Country, Is.EqualTo(string.Empty));
            Assert.That(profile.OPOL, Is.EqualTo(string.Empty));
            Assert.That(profile.ExchangeData, Is.EqualTo(string.Empty));
            Assert.That(profile.Exchanges, Is.EqualTo(string.Empty));
            Assert.That(profile.Currency, Is.EqualTo(string.Empty));
            Assert.That(profile.BaseCurrency, Is.EqualTo(string.Empty));
            Assert.That(profile.CFI, Is.EqualTo(string.Empty));
            Assert.That(profile.ISIN, Is.EqualTo(string.Empty));
            Assert.That(profile.SEDOL, Is.EqualTo(string.Empty));
            Assert.That(profile.CUSIP, Is.EqualTo(string.Empty));
            Assert.That(profile.ICB, Is.EqualTo(0));
            Assert.That(profile.SIC, Is.EqualTo(0));
            Assert.That(profile.Multiplier, Is.EqualTo(0));
            Assert.That(profile.Product, Is.EqualTo(string.Empty));
            Assert.That(profile.Underlying, Is.EqualTo(string.Empty));
            Assert.That(profile.SPC, Is.EqualTo(0));
            Assert.That(profile.AdditionalUnderlyings, Is.EqualTo(string.Empty));
            Assert.That(profile.MMY, Is.EqualTo(string.Empty));
            Assert.That(profile.Expiration, Is.EqualTo(0));
            Assert.That(profile.LastTrade, Is.EqualTo(0));
            Assert.That(profile.Strike, Is.EqualTo(0));
            Assert.That(profile.OptionType, Is.EqualTo(string.Empty));
            Assert.That(profile.ExpirationStyle, Is.EqualTo(string.Empty));
            Assert.That(profile.SettlementStyle, Is.EqualTo(string.Empty));
            Assert.That(profile.PriceIncrements, Is.EqualTo(string.Empty));
            Assert.That(profile.TradingHours, Is.EqualTo(string.Empty));
            var customField = new List<string>();
            Assert.That(profile.AddNonEmptyCustomFieldNames(customField), Is.False);
            Assert.That(customField, Is.Empty);
        });
    }

    [Test]
    public void Properties_ShouldGetAndSetValues() =>
        AssertTestProfile(CreateTestProfile());

    [Test]
    public void AddNonEmptyCustomFieldNames_ShouldReturnTrueForNonEmptyField()
    {
        var profile = new InstrumentProfile();
        var actualFieldNames = new HashSet<string> { "Field1", "Field2", "Field3" };
        foreach (var field in actualFieldNames)
        {
            profile.SetField(field, "Value");
        }

        var expectedFieldNames = new HashSet<string>();
        Assert.Multiple(() =>
        {
            Assert.That(profile.AddNonEmptyCustomFieldNames(expectedFieldNames), Is.True);
            Assert.That(actualFieldNames.SetEquals(expectedFieldNames), Is.True);
        });
    }

    [Test]
    public void CopyConstructor_ShouldShouldCopyAllFields()
    {
        var profile = new InstrumentProfile(CreateTestProfile());
        AssertTestProfile(profile);
        Assert.Multiple(() =>
        {
            Assert.That(profile.GetHashCode(), Is.EqualTo(CreateTestProfile().GetHashCode()));
            Assert.That(profile, Is.EqualTo(CreateTestProfile()));
        });
    }

    [Test]
    public void ToString_ShouldReturnExpectedFormat()
    {
        var profile = new InstrumentProfile { Type = "STOCK", Symbol = "GOOG" };
        var expectedString = "STOCK GOOG";

        Assert.That(profile.ToString(), Is.EqualTo(expectedString));
    }

    private static InstrumentProfile CreateTestProfile()
    {
        var profile = new InstrumentProfile
        {
            Type = "FUTURE",
            Symbol = "GOOG",
            Description = "Google Inc.",
            LocalSymbol = "Гугл",
            LocalDescription = "Гугл Инк.",
            Country = "US",
            OPOL = "XNAS",
            ExchangeData = "Data",
            Exchanges = "XNYS",
            Currency = "USD",
            BaseCurrency = "USD",
            CFI = "ESXXXX",
            ISIN = "US38259P5089",
            SEDOL = "2310967",
            CUSIP = "38259P508",
            ICB = 9535,
            SIC = 7371,
            Multiplier = 100,
            Product = "/YG",
            Underlying = "C",
            SPC = 1,
            AdditionalUnderlyings = "SE 50",
            MMY = "202312",
            Expiration = 20231231,
            LastTrade = 20231230,
            Strike = 1800,
            OptionType = "STAN",
            ExpirationStyle = "Quarterlys",
            SettlementStyle = "Close",
            PriceIncrements = "0.01 3; 0.05",
            TradingHours = "NewYorkETH()"
        };
        profile.SetField("Field1", "Test");
        profile.SetNumericField("Field2", 12.34);
        profile.SetDateField("Field3", 1234);
        return profile;
    }

    private static void AssertTestProfile(InstrumentProfile profile) =>
        Assert.Multiple(() =>
        {
            Assert.That(profile.Type, Is.EqualTo("FUTURE"));
            Assert.That(profile.Symbol, Is.EqualTo("GOOG"));
            Assert.That(profile.Description, Is.EqualTo("Google Inc."));
            Assert.That(profile.LocalSymbol, Is.EqualTo("Гугл"));
            Assert.That(profile.LocalDescription, Is.EqualTo("Гугл Инк."));
            Assert.That(profile.Country, Is.EqualTo("US"));
            Assert.That(profile.OPOL, Is.EqualTo("XNAS"));
            Assert.That(profile.ExchangeData, Is.EqualTo("Data"));
            Assert.That(profile.Exchanges, Is.EqualTo("XNYS"));
            Assert.That(profile.Currency, Is.EqualTo("USD"));
            Assert.That(profile.BaseCurrency, Is.EqualTo("USD"));
            Assert.That(profile.CFI, Is.EqualTo("ESXXXX"));
            Assert.That(profile.ISIN, Is.EqualTo("US38259P5089"));
            Assert.That(profile.SEDOL, Is.EqualTo("2310967"));
            Assert.That(profile.CUSIP, Is.EqualTo("38259P508"));
            Assert.That(profile.ICB, Is.EqualTo(9535));
            Assert.That(profile.SIC, Is.EqualTo(7371));
            Assert.That(profile.Multiplier, Is.EqualTo(100));
            Assert.That(profile.Product, Is.EqualTo("/YG"));
            Assert.That(profile.Underlying, Is.EqualTo("C"));
            Assert.That(profile.SPC, Is.EqualTo(1));
            Assert.That(profile.AdditionalUnderlyings, Is.EqualTo("SE 50"));
            Assert.That(profile.MMY, Is.EqualTo("202312"));
            Assert.That(profile.Expiration, Is.EqualTo(20231231));
            Assert.That(profile.LastTrade, Is.EqualTo(20231230));
            Assert.That(profile.Strike, Is.EqualTo(1800));
            Assert.That(profile.OptionType, Is.EqualTo("STAN"));
            Assert.That(profile.ExpirationStyle, Is.EqualTo("Quarterlys"));
            Assert.That(profile.SettlementStyle, Is.EqualTo("Close"));
            Assert.That(profile.PriceIncrements, Is.EqualTo("0.01 3; 0.05"));
            Assert.That(profile.TradingHours, Is.EqualTo("NewYorkETH()"));
            Assert.That(profile.GetField("Field1"), Is.EqualTo("Test"));
            Assert.That(profile.GetNumericField("Field2"), Is.EqualTo(12.34));
            Assert.That(profile.GetDateField("Field3"), Is.EqualTo(1234));
        });
}
