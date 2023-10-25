// <copyright file="InstrumentProfileCollectorNative.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Native.Ipf.Handles;

namespace DxFeed.Graal.Net.Native.Ipf;

internal class InstrumentProfileCollectorNative
{
    private readonly InstrumentProfileCollectorHandle handle;

    private InstrumentProfileCollectorNative(InstrumentProfileCollectorHandle builderHandle) =>
        handle = builderHandle;

    public static InstrumentProfileCollectorNative Create() =>
        new(InstrumentProfileCollectorHandle.Create());
}
