// <copyright file="StringUtilTest.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Globalization;
using DxFeed.Graal.Net.Utils;

namespace DxFeed.Graal.Net.Tests.Utils;

[TestFixture]
public class StringUtilTest
{
    [Test]
    public void TestEncodeNullableString()
    {
        Assert.AreEqual("null", StringUtil.EncodeNullableString(null));
        Assert.AreEqual("", StringUtil.EncodeNullableString(""));
        Assert.AreEqual("test", StringUtil.EncodeNullableString("test"));
    }

    [Test]
    public void TestEncodeChar()
    {
        Assert.AreEqual("\\0", StringUtil.EncodeChar((char)0));
        Assert.AreEqual(" ", StringUtil.EncodeChar(' '));
        Assert.AreEqual("A", StringUtil.EncodeChar('A'));
        Assert.AreEqual("a", StringUtil.EncodeChar('a'));
        Assert.AreEqual("1", StringUtil.EncodeChar('1'));
        Assert.AreEqual("\\u0001", StringUtil.EncodeChar('\u0001'));
        Assert.AreEqual("\\u00c1", StringUtil.EncodeChar('\u00C1'));

        for (int c = char.MinValue; c <= char.MaxValue; ++c)
        {
            var str = StringUtil.EncodeChar((char)c);
            switch (c)
            {
                case 0:
                    Assert.AreEqual("\\0", str);
                    break;
                case >= 32 and <= 126:
                    Assert.That(Convert.ToChar(str), Is.EqualTo((char)c));
                    break;
                default:
                    Assert.Multiple(() =>
                    {
                        Assert.That(str.StartsWith("\\u"), Is.True);
                        Assert.That(int.Parse(str[2..], NumberStyles.HexNumber), Is.EqualTo(c));
                    });
                    break;
            }
        }
    }

    [Test]
    public void TestCheckChar()
    {
        Assert.DoesNotThrow(() => StringUtil.CheckChar('\0', 0xFFFF, ""));
        Assert.DoesNotThrow(() => StringUtil.CheckChar('\u7FFF', 0x7FFF, ""));
        Assert.DoesNotThrow(() => StringUtil.CheckChar(' ', 0xFFFF, ""));
        Assert.DoesNotThrow(() => StringUtil.CheckChar('A', 0xFFFF, ""));
        Assert.DoesNotThrow(() => StringUtil.CheckChar('a', 0xFFFF, ""));
        Assert.DoesNotThrow(() => StringUtil.CheckChar('1', 0xFFFF, ""));
        Assert.Throws<ArgumentException>(() => StringUtil.CheckChar('\u0100', 0x01, ""));
        Assert.Throws<ArgumentException>(() => StringUtil.CheckChar('\uFFFF', 0x7FFF, ""));
    }
}
