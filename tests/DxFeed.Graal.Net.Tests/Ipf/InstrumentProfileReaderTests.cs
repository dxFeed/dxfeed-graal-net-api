// <copyright file="InstrumentProfileReaderTests.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Ipf;

namespace DxFeed.Graal.Net.Tests.Ipf;

[TestFixture]
public class InstrumentProfileReaderTests
{
    private static readonly string TestIpf = Path.Combine(TestContext.CurrentContext.TestDirectory, "ipf.txt");

    [Test]
    public void ResolveSourceUrl_ShouldReturnUrlFormat_WhenHostPortPatternProvided()
    {
        var address = "host:7777";
        var expectedUrl = "http://host:7777/ipf/all.ipf.gz";

        var result = InstrumentProfileReader.ResolveSourceUrl(address);

        Assert.That(result, Is.EqualTo(expectedUrl));
    }

    [Test]
    public void ResolveSourceUrl_ShouldReturnOriginalAddress_WhenNotHostPortPattern()
    {
        var address = "file.txt";
        var result = InstrumentProfileReader.ResolveSourceUrl(address);

        Assert.That(result, Is.EqualTo(address));
    }

    [Test]
    public void GetLastModified_ShouldReturnLastModificationTime()
    {
        var reader = new InstrumentProfileReader();
        Assert.That(reader.WasComplete(), Is.False);
        reader.ReadFromFile(TestIpf);
        Assert.That(reader.WasComplete(), Is.True);

        var lastFileModified = ((DateTimeOffset)File.GetLastWriteTimeUtc(TestIpf)).ToUnixTimeMilliseconds();
        Assert.That(reader.GetLastModified(), Is.EqualTo(lastFileModified));
    }

    [Test]
    public void ReadFromFile_ShouldReturnCorrectsProfiles()
    {
        var reader = new InstrumentProfileReader();
        var profiles = reader.ReadFromFile(TestIpf);
        Assert.That(profiles, Has.Count.EqualTo(31));

        var profile = profiles.Find(profile => profile.Symbol.Equals("AAPL", StringComparison.Ordinal));
        Assert.That(profile, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(profile.Description, Is.EqualTo("Apple Inc. - Common Stock"));
            Assert.That(profile.Country, Is.EqualTo("US"));
            Assert.That(profile.OPOL, Is.EqualTo("XNAS"));
            Assert.That(profile.Exchanges, Is.EqualTo("ARCX;BATS;BATY;EDGA;EDGX;IEXG;LTSE;MEMX;MPRL;XADF;XASE;XBOS;XCHI;XCIS;XNAS;XNYS;XPSX"));
            Assert.That(profile.Currency, Is.EqualTo("USD"));
            Assert.That(profile.GetField("SUBTYPES"), Is.EqualTo("Common Share;"));
        });
    }
}
