// <copyright file="StringUtilTest.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Globalization;
using DxFeed.Graal.Net.Utils;
using static System.Globalization.CultureInfo;

namespace DxFeed.Graal.Net.Tests.Utils;

[TestFixture]
public class StringUtilTest
{
    [Test]
    public void TestEncodeNullableString() =>
        Assert.Multiple(() =>
        {
            Assert.That(StringUtil.EncodeNullableString(null), Is.EqualTo("null"));
            Assert.That(StringUtil.EncodeNullableString(""), Is.EqualTo(""));
            Assert.That(StringUtil.EncodeNullableString("test"), Is.EqualTo("test"));
        });

    [Test]
    public void TestEncodeChar()
    {
        Assert.Multiple(() =>
        {
            Assert.That(StringUtil.EncodeChar((char)0), Is.EqualTo("\\0"));
            Assert.That(StringUtil.EncodeChar(' '), Is.EqualTo(" "));
            Assert.That(StringUtil.EncodeChar('A'), Is.EqualTo("A"));
            Assert.That(StringUtil.EncodeChar('a'), Is.EqualTo("a"));
            Assert.That(StringUtil.EncodeChar('1'), Is.EqualTo("1"));
            Assert.That(StringUtil.EncodeChar('\u0001'), Is.EqualTo("\\u0001"));
            Assert.That(StringUtil.EncodeChar('\u00C1'), Is.EqualTo("\\u00c1"));
        });

        for (int c = char.MinValue; c <= char.MaxValue; ++c)
        {
            var str = StringUtil.EncodeChar((char)c);
            switch (c)
            {
                case 0:
                    Assert.That(str, Is.EqualTo("\\0"));
                    break;
                case >= 32 and <= 126:
                    Assert.That(Convert.ToChar(str, InvariantCulture), Is.EqualTo((char)c));
                    break;
                default:
                    Assert.Multiple(() =>
                    {
                        Assert.That(str, Does.StartWith("\\u"));
                        Assert.That(int.Parse(str[2..], NumberStyles.HexNumber, InvariantCulture), Is.EqualTo(c));
                    });
                    break;
            }
        }
    }

    [Test]
    public void TestCheckChar() =>
        Assert.Multiple(() =>
        {
            Assert.DoesNotThrow(() => StringUtil.CheckChar('\0', 0xFFFF, ""));
            Assert.DoesNotThrow(() => StringUtil.CheckChar('\u7FFF', 0x7FFF, ""));
            Assert.DoesNotThrow(() => StringUtil.CheckChar(' ', 0xFFFF, ""));
            Assert.DoesNotThrow(() => StringUtil.CheckChar('A', 0xFFFF, ""));
            Assert.DoesNotThrow(() => StringUtil.CheckChar('a', 0xFFFF, ""));
            Assert.DoesNotThrow(() => StringUtil.CheckChar('1', 0xFFFF, ""));
            Assert.Throws<ArgumentException>(() => StringUtil.CheckChar('\u0100', 0x01, ""));
            Assert.Throws<ArgumentException>(() => StringUtil.CheckChar('\uFFFF', 0x7FFF, ""));
        });
}
