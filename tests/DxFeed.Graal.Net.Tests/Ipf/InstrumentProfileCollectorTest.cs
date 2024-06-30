// <copyright file="InstrumentProfileCollectorTest.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Globalization;
using DxFeed.Graal.Net.Ipf;
using DxFeed.Graal.Net.Ipf.Live;

namespace DxFeed.Graal.Net.Tests.Ipf;

[TestFixture]
public class InstrumentProfileCollectorTests
{
    private InstrumentProfileCollector collector;

    [SetUp]
    public void SetUp() =>
        collector = new InstrumentProfileCollector();

    [Test]
    public void TestUpdateRemoved()
    {
        var update0 = collector.GetLastUpdateTime();
        AssertViewIdentities(); // empty

        // first instrument
        var i1 = new InstrumentProfile();
        RandomInstrument(i1, 20140618);
        collector.UpdateInstrumentProfile(i1);
        var update1 = collector.GetLastUpdateTime();
        Assert.That(update1, Is.GreaterThan(update0));
        AssertViewIdentities(i1);

        // second instrument has same symbol but "REMOVED" type
        var i2 = new InstrumentProfile();
        RandomInstrument(i2, 20140618);
        i2.Type = InstrumentProfileType.REMOVED.Name;
        collector.UpdateInstrumentProfile(i2);
        var update2 = collector.GetLastUpdateTime();
        Assert.That(update2, Is.GreaterThan(update1));
        AssertViewIdentities(); // becomes empty

        // removed again (nothing shall change)
        var i3 = new InstrumentProfile();
        RandomInstrument(i3, 20140618);
        i3.Type = InstrumentProfileType.REMOVED.Name;
        collector.UpdateInstrumentProfile(i3);
        var update3 = collector.GetLastUpdateTime();
        Assert.That(update3, Is.EqualTo(update2));
        AssertViewIdentities(); // becomes empty
    }

    private static void RandomInstrument(InstrumentProfile ip, long seed)
    {
        var r = new Random((int)seed);
        ip.Symbol = r.Next(10000).ToString(CultureInfo.InvariantCulture);
    }

    private void AssertViewIdentities(params InstrumentProfile[] ips)
    {
        var expected = new HashSet<InstrumentProfile>(ips);
        var actual = new HashSet<InstrumentProfile>();
        foreach (var instrument in collector.View())
        {
            Assert.That(instrument.Type, Is.Not.EqualTo(InstrumentProfileType.REMOVED.Name));
            actual.Add(instrument);
        }

        Assert.That(expected, Is.EqualTo(actual));
    }
}
