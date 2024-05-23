// <copyright file="EnumUtilTest.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Utils;

namespace DxFeed.Graal.Net.Tests.Utils;

[TestFixture]
public class EnumUtilTest
{
    private enum TestEnum
    {
        Value1,
        Value2,
        Value3
    }

    [Test]
    public void GetCountValues_ReturnsCorrectCount()
    {
        var count = EnumUtil.GetCountValues<TestEnum>();
        Assert.That(count, Is.EqualTo(3));
    }

    [Test]
    public void CreateEnumBitMaskArrayByValue_ReturnsCorrectArray()
    {
        var defaultValue = TestEnum.Value3;
        var expectedLength = 4; // Nearest power of two greater than or equal to 3
        var result = EnumUtil.CreateEnumBitMaskArrayByValue(defaultValue);

        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Length.EqualTo(expectedLength));
            Assert.That(result[0], Is.EqualTo(TestEnum.Value1));
            Assert.That(result[1], Is.EqualTo(TestEnum.Value2));
            Assert.That(result[2], Is.EqualTo(TestEnum.Value3));
            Assert.That(result[3], Is.EqualTo(defaultValue)); // Default value
        });
    }

    [Test]
    public void CreateEnumArrayByValue_ReturnsCorrectArray()
    {
        var defaultValue = TestEnum.Value3;
        var length = 5;
        var result = EnumUtil.CreateEnumArrayByValue(defaultValue, length);

        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Length.EqualTo(length));
            Assert.That(result[0], Is.EqualTo(TestEnum.Value1));
            Assert.That(result[1], Is.EqualTo(TestEnum.Value2));
            Assert.That(result[2], Is.EqualTo(TestEnum.Value3));
            Assert.That(result[3], Is.EqualTo(defaultValue)); // Default value
            Assert.That(result[4], Is.EqualTo(defaultValue)); // Default value
        });
    }

    [Test]
    public void CreateEnumArrayByValue_TruncatesArrayIfLengthIsLess()
    {
        var defaultValue = TestEnum.Value3;
        var length = 2;
        var result = EnumUtil.CreateEnumArrayByValue(defaultValue, length);

        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Length.EqualTo(length));
            Assert.That(result[0], Is.EqualTo(TestEnum.Value1));
            Assert.That(result[1], Is.EqualTo(TestEnum.Value2));
        });

        length = 0;
        result = EnumUtil.CreateEnumArrayByValue(defaultValue, length);

        Assert.That(result, Has.Length.EqualTo(length));
    }

    [Test]
    public void CreateEnumArrayByValue_ThrowsArgumentException_ForNegativeLength()
    {
        var defaultValue = TestEnum.Value3;
        var length = -1;

        Assert.Throws<ArgumentException>(() => EnumUtil.CreateEnumArrayByValue(defaultValue, length));
    }
}
