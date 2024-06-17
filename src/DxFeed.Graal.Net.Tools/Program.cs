// <copyright file="Program.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Api;

namespace DxFeed.Graal.Net.Tools;

internal abstract class Program
{
    public static void Main(string[] args)
    {
        SystemProperty.SetProperty(DXEndpoint.DXFeedWildcardEnableProperty, "true");
        SystemProperty.SetProperty("dxfeed.experimental.dxlink.enable", "true");
        SystemProperty.SetProperty("scheme", "ext:opt:sysprops,resource:dxlink.xml");
        Tools.Run(args);
    }
}
