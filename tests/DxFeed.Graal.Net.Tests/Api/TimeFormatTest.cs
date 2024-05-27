// <copyright file="TimeFormatTest.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Globalization;
using DxFeed.Graal.Net.Native.ErrorHandling;
using DxFeed.Graal.Net.Utils;

namespace DxFeed.Graal.Net.Tests.Api;

[TestFixture]
internal sealed class TimeFormatTest
{
    [Test]
    public void TestDefaultParse()
    {
        var defaultTimeFormat = TimeFormat.Default;

        Assert.Multiple(() =>
        {
            Assert.That(defaultTimeFormat.Parse(" 0    "), Is.EqualTo(UnixEpoch));
            Assert.That(defaultTimeFormat.Parse("20070101-123456"),
                Is.EqualTo((DateTimeOffset)new DateTime(2007, 01, 01, 12, 34, 56, DateTimeKind.Local)));
            Assert.That(defaultTimeFormat.Parse("20070101-123456.123"),
                Is.EqualTo((DateTimeOffset)new DateTime(2007, 01, 01, 12, 34, 56, 123, DateTimeKind.Local)));
            Assert.That(defaultTimeFormat.Parse("2005-12-31 21:00:00"),
                Is.EqualTo((DateTimeOffset)new DateTime(2005, 12, 31, 21, 00, 00, DateTimeKind.Local)));
            Assert.That(defaultTimeFormat.Parse("2005-12-31 21:00:00+03"),
                Is.EqualTo(new DateTimeOffset(2005, 12, 31, 21, 00, 0, new TimeSpan(+3, 0, 0))));
            Assert.That(defaultTimeFormat.Parse("2005-12-31 21:00:00-03"),
                Is.EqualTo(new DateTimeOffset(2005, 12, 31, 21, 00, 0, new TimeSpan(-3, 0, 0))));
            Assert.That(defaultTimeFormat.Parse("2005-12-31Z"),
                Is.EqualTo((DateTimeOffset)new DateTime(2005, 12, 31, 00, 00, 00, DateTimeKind.Utc)));
            Assert.That(defaultTimeFormat.Parse("2005-11-30 21:00:00Z"),
                Is.EqualTo((DateTimeOffset)new DateTime(2005, 11, 30, 21, 00, 00, DateTimeKind.Utc)));
            Assert.That(defaultTimeFormat.Parse("2005-11-30T21:00:00Z"),
                Is.EqualTo((DateTimeOffset)new DateTime(2005, 11, 30, 21, 00, 00, DateTimeKind.Utc)));
            Assert.That(defaultTimeFormat.Parse("2005-11-30T21:00:00"),
                Is.EqualTo((DateTimeOffset)new DateTime(2005, 11, 30, 21, 00, 00, DateTimeKind.Local)));
        });

        var now = (DateTimeOffset)DateTime.Now.ToUniversalTime();
        now = now.AddTicks(-now.Ticks % TimeSpan.TicksPerSecond);
        var parsedNow = defaultTimeFormat.Parse(now.ToString($"yyyy-MM-dd HH:mm:sszz", CultureInfo.InvariantCulture));
        Assert.That(parsedNow, Is.EqualTo(now));
        var badValues = new List<string>
        {
            "2007-1102-12:34:56",
            "20070101-1234:56",
            "200711-02-12:34",
            "t12:34:5",
            "12:3456",
            "1234:56",
            "2008-1-10",
            "2004-12-12t",
            "2005-12-31 210",
            "-P10DT2H30MS",
            "1234567",
            "20010101t",
            "t1234567",
            "-",
            "",
            "1",
            "t12::34:56",
            "t12:",
            "123",
            "T",
            "P1234DT12H30M0S"
        };
        foreach (var badValue in badValues)
        {
            Assert.Throws<JavaException>(() => defaultTimeFormat.Parse(badValue));
        }
    }

    [Test]
    public void TestGMTParse() => checkGMTTimeFormat(TimeFormat.GMT);

    private static void checkGMTTimeFormat(TimeFormat gmtTimeFormat)
    {
        Assert.Multiple(() =>
        {
            Assert.That(gmtTimeFormat.Parse(" 0    "), Is.EqualTo(UnixEpoch));
            Assert.That(gmtTimeFormat.Parse("20070101-123456"),
                Is.EqualTo((DateTimeOffset)new DateTime(2007, 01, 01, 12, 34, 56, DateTimeKind.Utc)));
            Assert.That(gmtTimeFormat.Parse("20070101-123456.123"),
                Is.EqualTo((DateTimeOffset)new DateTime(2007, 01, 01, 12, 34, 56, 123, DateTimeKind.Utc)));
            Assert.That(gmtTimeFormat.Parse("2005-12-31 21:00:00"),
                Is.EqualTo((DateTimeOffset)new DateTime(2005, 12, 31, 21, 00, 00, 00, DateTimeKind.Utc)));
            Assert.That(gmtTimeFormat.Parse("2005-12-31 21:00:00+03"),
                Is.EqualTo(new DateTimeOffset(2005, 12, 31, 21, 00, 0, new TimeSpan(+3, 0, 0))));
            Assert.That(gmtTimeFormat.Parse("2005-12-31 21:00:00-03"),
                Is.EqualTo(new DateTimeOffset(2005, 12, 31, 21, 00, 0, new TimeSpan(-3, 0, 0))));
            Assert.That(gmtTimeFormat.Parse("2005-12-31Z"),
                Is.EqualTo((DateTimeOffset)new DateTime(2005, 12, 31, 00, 00, 00, 00, DateTimeKind.Utc)));
            Assert.That(gmtTimeFormat.Parse("2005-11-30 21:00:00Z"),
                Is.EqualTo((DateTimeOffset)new DateTime(2005, 11, 30, 21, 00, 00, 00, DateTimeKind.Utc)));
            Assert.That(gmtTimeFormat.Parse("2005-11-30T21:00:00Z"),
                Is.EqualTo((DateTimeOffset)new DateTime(2005, 11, 30, 21, 00, 00, 00, DateTimeKind.Utc)));
            Assert.That(gmtTimeFormat.Parse("2005-11-30T21:00:00"),
                Is.EqualTo((DateTimeOffset)new DateTime(2005, 11, 30, 21, 00, 00, 00, DateTimeKind.Utc)));
        });
        Assert.Throws<JavaException>(() => gmtTimeFormat.Parse("1"));
    }

    [Test]
    public void TestFormat()
    {
        var defaultTimeFormat = TimeFormat.GMT;
        Assert.Multiple(() =>
        {
            Assert.That(defaultTimeFormat.Format(-1), Is.EqualTo("19691231-235959"));
            Assert.That(defaultTimeFormat.Format(0), Is.EqualTo("0"));
            Assert.That(defaultTimeFormat.Format(1), Is.EqualTo("19700101-000000"));
            Assert.That(defaultTimeFormat.Format(1709888958000), Is.EqualTo("20240308-090918"));
        });
    }

    [Test]
    public void TestWithMillisTimeFormat()
    {
        var defaultTimeFormat = TimeFormat.GMT.WithMillis();
        Assert.Multiple(() =>
        {
            Assert.That(defaultTimeFormat.Format(-1), Is.EqualTo("19691231-235959.999"));
            Assert.That(defaultTimeFormat.Format(0), Is.EqualTo("0"));
            Assert.That(defaultTimeFormat.Format(1), Is.EqualTo("19700101-000000.001"));
            Assert.That(defaultTimeFormat.Format(1709888958018), Is.EqualTo("20240308-090918.018"));
        });
    }

    [Test]
    public void TestIsoTimeFormat()
    {
        var defaultTimeFormat = TimeFormat.GMT.AsFullIso();
        Assert.Multiple(() =>
        {
            Assert.That(defaultTimeFormat.Format(-1), Is.EqualTo("1969-12-31T23:59:59.999Z"));
            Assert.That(defaultTimeFormat.Format(0), Is.EqualTo("0"));
            Assert.That(defaultTimeFormat.Format(1), Is.EqualTo("1970-01-01T00:00:00.001Z"));
            Assert.That(defaultTimeFormat.Format(1709888958018), Is.EqualTo("2024-03-08T09:09:18.018Z"));
        });
    }

    [Test]
    public void TestTimePeriod()
    {
        Assert.Multiple(() =>
        {
            Assert.That(TimePeriod.ValueOf("0").Seconds, Is.EqualTo(0L));
            Assert.That(TimePeriod.ValueOf("1s").TotalMilliseconds, Is.EqualTo(1000));
            Assert.That(TimePeriod.ValueOf(".123456789").TotalMilliseconds, Is.EqualTo(123L));
            Assert.That(TimePeriod.ValueOf("1.23456789").TotalSeconds,
                Is.EqualTo(TimePeriod.ValueOf("0d0h0m1.235s").TotalSeconds));
        });

        const long expectedValue = 873000000L;
        Assert.Multiple(() =>
        {
            Assert.That(TimePeriod.ValueOf("P10DT2H30M").TotalMilliseconds, Is.EqualTo(expectedValue));
            Assert.That(TimePeriod.ValueOf("10DT2H29M60.00").TotalMilliseconds, Is.EqualTo(expectedValue));
            Assert.That(TimePeriod.ValueOf("p10DT1H90M").TotalMilliseconds, Is.EqualTo(expectedValue));
            Assert.That(TimePeriod.ValueOf("9DT26H1800S").TotalMilliseconds, Is.EqualTo(expectedValue));
            Assert.That(TimePeriod.ValueOf("P10DT2H30M.0").TotalMilliseconds, Is.EqualTo(expectedValue));
            Assert.That(TimePeriod.ValueOf("p10d2H29m59.9995s").TotalMilliseconds, Is.EqualTo(expectedValue));
        });
        var badValues =
            new List<string>
            {
                "t1d",
                "p",
                string.Empty,
                "P2D3T",
                "P10DT2H30MS",
                ".",
                "p1mt",
                "239e-3",
                " PT1S",
                "pt1s2m",
                "PT1s ",
                "239ss",
                "t1,5s",
                "1,5"
            };
        foreach (var badValue in badValues)
        {
            Assert.Throws<JavaException>(() => TimePeriod.ValueOf(badValue));
        }
    }

    [Test]
    public void TestDateAsLong()
    {
        var gmt = TimeFormat.GMT;
        var a1 = TimeFormat.GMT.Parse("20010101"); // yyyymmdd
        var a2 = gmt.Parse("2001-01-01");
        Assert.That(a1, Is.EqualTo(a2));

        var b1 = gmt.Parse("121212"); // hhmmss
        var b2 = gmt.Parse("T12:12:12");
        Assert.That(b1, Is.EqualTo(b2));

        var c1 = gmt.Parse("1234567890"); // number
        var c2 = UnixEpoch.AddMilliseconds(1234567890);
        Assert.That(c1, Is.EqualTo(c2));
    }

    [Test]
    public void TestDateOutsideIsoRangeFormat() =>
        Assert.Multiple(() =>
        {
            Assert.That(TimeFormat.GMT.Format(long.MinValue),
                Is.EqualTo(long.MinValue.ToString(CultureInfo.InvariantCulture)));
            Assert.That(TimeFormat.GMT.Format(long.MaxValue),
                Is.EqualTo(long.MaxValue.ToString(CultureInfo.InvariantCulture)));
        });

    private static DateTimeOffset UnixEpoch =>
        new(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
}
