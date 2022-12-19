// <copyright file="SystemPropertyTest.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Native.ErrorHandling;

namespace DxFeed.Graal.Net.Tests;

[TestFixture]
public class SystemPropertyTest
{
    [Test]
    public void CheckReadsUnsetProperty()
    {
        // On getting an unset property, returns null.
        var result = SystemProperty.GetProperty("non-exist-prop");
        Assert.That(result, Is.EqualTo(null));
    }

    [Test]
    public void ThrowJavaExceptionWhenKeyNull() =>
        Assert.Throws<JavaException>(() => SystemProperty.SetProperty(null!, "value"));

    [Test]
    public void ThrowJavaExceptionWhenValueNull() =>
        Assert.Throws<JavaException>(() => SystemProperty.SetProperty("key", null!));

    [Test]
    public void CheckReadWriteProperty()
    {
        const string key = "key_1";
        const string value = "value_1";

        Assert.That(SystemProperty.GetProperty(key), Is.EqualTo(null));
        SystemProperty.SetProperty(key, value);
        Assert.That(SystemProperty.GetProperty(key), Is.EqualTo(value));
    }

    [Test]
    public void CheckMultipleReadWriteProperty()
    {
        const int count = 100;
        const string key = "key";
        const string val = "val";

        // Sets properties.
        for (var i = 0; i < count; ++i)
        {
            var currentKey = $"{key}_{i})";
            var currentVal = $"{val}_{i})";
            SystemProperty.SetProperty(currentKey, currentVal);
        }

        // Gets properties.
        for (var i = 0; i < count; ++i)
        {
            var currentKey = $"{key}_{i})";
            var expectedVal = $"{val}_{i})";
            var currentVal = SystemProperty.GetProperty(currentKey);
            Assert.That(currentVal, Is.EqualTo(expectedVal));
        }
    }

    [Test]
    public void CheckReadWriteUtf8Property()
    {
        const string key = "key_1";
        const string value = "AAPL/, !, Ā, ༀ, 😋, 𐐨";

        SystemProperty.SetProperty(key, value);
        Assert.That(SystemProperty.GetProperty(key), Is.EqualTo(value));
    }
}
