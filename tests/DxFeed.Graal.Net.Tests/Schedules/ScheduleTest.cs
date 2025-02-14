// <copyright file="ScheduleTest.cs" company="Devexperts LLC">
// Copyright © 2025 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Ipf;
using DxFeed.Graal.Net.Schedules;

namespace DxFeed.Graal.Net.Tests.Schedules;

[TestFixture]
public class ScheduleTest
{
    private static readonly string TestIpf = Path.Combine(TestContext.CurrentContext.TestDirectory, "ipf.txt");

    [Test]
    public void Day_ShouldReturnsNullValueForInvalidSession()
    {
        var profiles = new InstrumentProfileReader().ReadFromFile(TestIpf);
        foreach (var profile in profiles) {
            var schedule = Schedule.GetInstance(profile);
            var day = schedule.GetDayByYearMonthDay(01012024);
            var session = day.GetFirstSession(SessionFilter.AFTER_MARKET);
            Assert.That(session, Is.Null);
            session = day.GetLastSession(SessionFilter.AFTER_MARKET);
            Assert.That(session, Is.Null);
            Assert.Multiple(() =>
            {
                Assert.That(day.TryGetFirstSession(SessionFilter.AFTER_MARKET, out session), Is.False);
                Assert.That(session, Is.Null);
                Assert.That(day.TryGetLastSession(SessionFilter.AFTER_MARKET, out session), Is.False);
                Assert.That(session, Is.Null);
            });
        }
    }

    [Test]
    public void Day_ShouldReturnsNullValueForInvalidDay()
    {
        var profiles = new InstrumentProfileReader().ReadFromFile(TestIpf);
        foreach (var profile in profiles) {
            var schedule = Schedule.GetInstance(profile.TradingHours);
            var day = schedule.GetDayByYearMonthDay(01011950);
            Assert.Multiple(() =>
            {
                Assert.That(day.GetNextDay(DayFilter.SHORT_DAY), Is.Null);
                Assert.That(day.GetPrevDay(DayFilter.SHORT_DAY), Is.Null);
                Assert.That(day.TryGetNextDay(DayFilter.SHORT_DAY, out var nextDay), Is.False);
                Assert.That(nextDay, Is.Null);
                Assert.That(day.TryGetPrevDay(DayFilter.SHORT_DAY, out var prevDay), Is.False);
                Assert.That(prevDay, Is.Null);
            });
        }
    }

    [Test]
    public void Schedule_ShouldReturnCorrectVenues()
    {
        var profiles = new InstrumentProfileReader().ReadFromFile(TestIpf);
        var aapl = profiles.Find(profile => profile.Symbol.Equals("AAPL", StringComparison.Ordinal));
        var venues = Schedule.GetTradingVenues(aapl ?? throw new InvalidOperationException());
        Assert.That(venues, Has.Count.EqualTo(1));
        Assert.That(venues[0], Is.EqualTo("NewYorkETH"));
    }
}
