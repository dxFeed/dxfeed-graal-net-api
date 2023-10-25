// <copyright file="DayUtilTest.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Utils;
using NodaTime;

namespace DxFeed.Graal.Net.Tests.Utils;

[TestFixture]
public class DayUtilTest
{
    [Test]
    public void CheckEpoch() =>
        Assert.Multiple(() =>
        {
            Assert.That(DayUtil.GetDayIdByYearMonthDay(1970, 1, 1), Is.EqualTo(0));
            Assert.That(DayUtil.GetDayIdByYearMonthDay(19700101), Is.EqualTo(0));
            Assert.That(DayUtil.GetYearMonthDayByDayId(0), Is.EqualTo(19700101));
        });

    [Test]
    public void CheckRandomGregorianDates()
    {
        var minYear = CalendarSystem.Gregorian.MinYear;
        var maxYear = CalendarSystem.Gregorian.MaxYear;
        var r = new Random(1);
        for (var i = 0; i < 100000; ++i)
        {
            var year = minYear + r.Next(maxYear - minYear + 1);
            var month = 1 + r.Next(12);
            var day = 1 + r.Next(CalendarSystem.Gregorian.GetDaysInMonth(year, month));
            var yyyymmdd = (year < 0 ? -1 : 1) * ((MathUtil.Abs(year) * 10000) + (month * 100) + day);
            var dayId = Period.DaysBetween(
                new LocalDate(1970, 1, 1, CalendarSystem.Gregorian),
                new LocalDate(year, month, day, CalendarSystem.Gregorian));
            Assert.Multiple(() =>
            {
                Assert.That(DayUtil.GetDayIdByYearMonthDay(year, month, day), Is.EqualTo(dayId));
                Assert.That(DayUtil.GetDayIdByYearMonthDay(yyyymmdd), Is.EqualTo(dayId));
                Assert.That(DayUtil.GetYearMonthDayByDayId(dayId), Is.EqualTo(yyyymmdd));
            });
        }
    }

    [Test]
    public void CheckLeapYear()
    {
        const int month = 2;
        const int day = 29;
        for (var year = CalendarSystem.Gregorian.MinYear; year <= CalendarSystem.Gregorian.MaxYear; ++year)
        {
            if (!CalendarSystem.Gregorian.IsLeapYear(year))
            {
                continue;
            }

            var dayId = DayUtil.GetDayIdByYearMonthDay(year, month, day);
            var date = new ZonedDateTime(Instant.FromUnixTimeSeconds(0), DateTimeZone.Utc, CalendarSystem.Gregorian);
            date += Duration.FromDays(dayId);
            Assert.Multiple(() =>
            {
                Assert.That(date.Year, Is.EqualTo(year));
                Assert.That(date.Month, Is.EqualTo(month));
                Assert.That(date.Day, Is.EqualTo(day));
            });
        }
    }

    [Test]
    public void WhenInvalidMonthThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => DayUtil.GetDayIdByYearMonthDay(1970, -1, 1));
        Assert.Throws<ArgumentException>(() => DayUtil.GetDayIdByYearMonthDay(1970, 0, 1));
        Assert.Throws<ArgumentException>(() => DayUtil.GetDayIdByYearMonthDay(1970, 13, 1));
    }
}
