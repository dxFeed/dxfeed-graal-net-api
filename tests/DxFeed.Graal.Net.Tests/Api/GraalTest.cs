// <copyright file="GraalTest.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Api;

namespace DxFeed.Graal.Net.Tests.Api;

[TestFixture]
public class GraalTest
{
    [Test]
    public void CheckCreateIsolateInDifferentThread()
    {
        var thread = new Thread(() =>
        {
            SystemProperty.SetProperty("test", "test");
        });
        thread.Start();
        thread.Join();

        var endpoint = DXEndpoint.Create();
        thread = new Thread(() =>
        {
            SystemProperty.SetProperty("test", "test");
        });
        thread.Start();
        thread.Join();
        endpoint.Close();
        Assert.Pass();
    }

    [Test]
    public void CheckCreateIsolateInMainThread()
    {
        var endpoint = DXEndpoint.Create();
        var thread = new Thread(() =>
        {
            SystemProperty.SetProperty("test", "test");
        });
        thread.Start();
        thread.Join();
        endpoint.CloseAndAwaitTermination();
        Assert.Pass();
    }
}
