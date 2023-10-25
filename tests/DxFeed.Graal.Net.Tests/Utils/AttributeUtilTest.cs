// <copyright file="AttributeUtilTest.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Utils;

namespace DxFeed.Graal.Net.Tests.Utils;

// Classes should not be empty. It's a test class.
#pragma warning disable S2094

// Unused private types or members should be removed. False positive.
#pragma warning disable S1144

[TestFixture]
public class AttributeUtilTests
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    private class DummyAttribute : Attribute
    {
        public DummyAttribute(string value) =>
            Value = value;

        public string Value { get; }
    }

    [Dummy("BaseDummyClass")]
    private class BaseDummyClass
    {
    }

    private class DummyInheritedClass : BaseDummyClass
    {
    }

    private class DummyNonAttributedClass
    {
    }

    [Test]
    public void ReturnsAttributeForAttributedClass()
    {
        var attr = AttributeUtil.GetCustomAttribute<DummyAttribute>(typeof(BaseDummyClass));
        Assert.That(attr, Is.Not.Null);
        Assert.That(attr?.Value, Is.EqualTo("BaseDummyClass"));
    }

    [Test]
    public void AttributeWithInheritFlagReturnsAttributeFromBase()
    {
        var attr = AttributeUtil.GetCustomAttribute<DummyAttribute>(typeof(DummyInheritedClass), true);
        Assert.That(attr, Is.Not.Null);
        Assert.That(attr?.Value, Is.EqualTo("BaseDummyClass"));
    }

    [Test]
    public void AttributeWithoutInheritFlagReturnsNullForInherited()
    {
        var attr = AttributeUtil.GetCustomAttribute<DummyAttribute>(typeof(DummyInheritedClass), false);
        Assert.That(attr, Is.Null);

        // Inherit flag is false by default.
        attr = AttributeUtil.GetCustomAttribute<DummyAttribute>(typeof(DummyInheritedClass));
        Assert.That(attr, Is.Null);
    }

    [Test]
    public void NonAttributedClassReturnsNull()
    {
        var attr = AttributeUtil.GetCustomAttribute<DummyAttribute>(typeof(DummyNonAttributedClass));
        Assert.That(attr, Is.Null);
    }

    [Test]
    public void WhenTypeIsNullThrowsArgumentNullException() =>
        Assert.Throws<ArgumentNullException>(() => AttributeUtil.GetCustomAttribute<DummyAttribute>(null!));
}
