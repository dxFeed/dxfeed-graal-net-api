// <copyright file="PriceLevelCheckerNative.cs" company="Devexperts LLC">
// Copyright © 2026 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Events.Market;
using DxFeed.Graal.Net.Native.ErrorHandling;
using DxFeed.Graal.Net.Native.Events;
using DxFeed.Graal.Net.Native.Graal;
using DxFeed.Graal.Net.Native.Interop;

namespace DxFeed.Graal.Net.Native.Orcs;

internal sealed class PriceLevelCheckerNative
{
    public static bool Validate(List<Order> orders, TimeSpan timeGapBound, bool printQuotes)
    {
        unsafe
        {
            var eventList = EventMapper.ToNative(orders);

            try
            {
                ErrorCheck.SafeCall(NativeValidate(
                    Isolate.CurrentThread,
                    eventList,
                    (long)timeGapBound.TotalMilliseconds,
                    printQuotes ? 1 : 0,
                    out var isValid));

                return isValid != 0;
            }
            finally
            {
                EventMapper.Release(eventList);
            }
        }
    }

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "dxfg_PriceLevelChecker_validate")]
    private static extern unsafe int NativeValidate(
        nint thread,
        ListNative<EventTypeNative>* orders,
        long timeGapBound,
        int printQuotes,
        out int isValid);
}
