// <copyright file="DXTimeFormatTest.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.Intrinsics.Arm;
using DxFeed.Graal.Net.Native.ErrorHandling;
using DxFeed.Graal.Net.Utils;
using Microsoft.VisualBasic;

namespace DxFeed.Graal.Net.Tests.Api;

[TestFixture]
public class DXTimeFormatTest
{
    [Test]
    public void TestDefaultParse()
    {
        var defaultTimeFormat = DXTimeFormat.Default();
        var dt = TimeFormat.Local.Parse("2005-12-31 21:00:00+03");
        Assert.Multiple(() =>
        {
            Assert.That(DXTimeFormat.Default().Parse("2005-12-31 21:00:00+03"), Is.EqualTo(dt));
            Assert.That(DXTimeFormat.GMT().Parse("2005-12-31 21:00:00+03"), Is.EqualTo(dt));
        });
        Assert.Multiple(() =>
        {
            Assert.That(defaultTimeFormat.Parse(" 0    "), Is.EqualTo(DateTimeOffset.UnixEpoch));

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
        var parsedNow = defaultTimeFormat.Parse(now.ToString($"yyyy-MM-dd HH:mm:sszz"));
        Assert.That(parsedNow, Is.EqualTo(now));
        Assert.Throws<JavaException>(() => defaultTimeFormat.Parse("1"));
    }

    [Test]
    public void TestGMTParse()
    {
        checkGMTTimeFormat(DXTimeFormat.GMT());
        checkGMTTimeFormat(DXTimeFormat.WithTimeZone("Etc/UTC"));
        //according java api. TZ returns the specified TimeZone, or the GMT zone if the given ID cannot be understood.
        checkGMTTimeFormat(DXTimeFormat.WithTimeZone("Wrong TimeZOne"));
    }

    private static void checkGMTTimeFormat(DXTimeFormat gmtTimeFormat)
    {
        Assert.Multiple(() =>
        {
            Assert.That(gmtTimeFormat.Parse(" 0    "), Is.EqualTo(DateTimeOffset.UnixEpoch));
            Assert.That(gmtTimeFormat.Parse("20070101-123456"),
                Is.EqualTo((DateTimeOffset)new DateTime(2007, 01, 01, 12, 34, 56, DateTimeKind.Utc)));
            Assert.That(gmtTimeFormat.Parse("20070101-123456.123"),
                Is.EqualTo((DateTimeOffset)new DateTime(2007, 01, 01, 12, 34, 56, 123, DateTimeKind.Utc)));
            Assert.That(gmtTimeFormat.Parse("2005-12-31 21:00:00"),
                Is.EqualTo((DateTimeOffset)new DateTime(2005, 12, 31, 21, 00, 00 , 00, DateTimeKind.Utc)));
            Assert.That(gmtTimeFormat.Parse("2005-12-31 21:00:00+03"),
                Is.EqualTo(new DateTimeOffset(2005, 12, 31, 21, 00, 0, new TimeSpan(+3, 0, 0))));
            Assert.That(gmtTimeFormat.Parse("2005-12-31 21:00:00-03"),
                Is.EqualTo(new DateTimeOffset(2005, 12, 31, 21, 00, 0, new TimeSpan(-3, 0, 0))));
            Assert.That(gmtTimeFormat.Parse("2005-12-31Z"),
                Is.EqualTo((DateTimeOffset)new DateTime(2005, 12, 31, 00, 00, 00 , 00, DateTimeKind.Utc)));
            Assert.That(gmtTimeFormat.Parse("2005-11-30 21:00:00Z"),
                Is.EqualTo((DateTimeOffset)new DateTime(2005, 11, 30, 21, 00, 00 , 00, DateTimeKind.Utc)));
            Assert.That(gmtTimeFormat.Parse("2005-11-30T21:00:00Z"),
                Is.EqualTo((DateTimeOffset)new DateTime(2005, 11, 30, 21, 00, 00 , 00, DateTimeKind.Utc)));
            Assert.That(gmtTimeFormat.Parse("2005-11-30T21:00:00"),
                Is.EqualTo((DateTimeOffset)new DateTime(2005, 11, 30, 21, 00, 00 , 00, DateTimeKind.Utc)));
        });

        Assert.Throws<JavaException>(() => gmtTimeFormat.Parse("1"));
    }
}
