// <copyright file="PromiseNative.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Events;
using DxFeed.Graal.Net.Native.Events;
using DxFeed.Graal.Net.Native.Interop;
using static DxFeed.Graal.Net.Native.ErrorHandling.ErrorCheck;

namespace DxFeed.Graal.Net.Native.Promise;

internal sealed unsafe class PromiseNative : JavaHandle
{
    public bool IsDone() => SafeCall(Import.IsDone(CurrentThread, this)) != 0;

    public bool HasResult() => SafeCall(Import.HasResult(CurrentThread, this)) != 0;

    public void Cancel() => SafeCall(Import.Cancel(CurrentThread, this));

    public IEventType Result()
    {
        var nativeResult = SafeCall(Import.GetResult(CurrentThread, this));
        var result = EventMapper.FromNative(nativeResult);
        return result;
    }

    public IEnumerable<IEventType> Results()
    {
        var nativeResult = SafeCall(Import.GetResults(CurrentThread, this));
        var result = EventMapper.FromNative(nativeResult);
        return result;
    }

    private static class Import
    {
        [DllImport(
            ImportInfo.DllName,
            EntryPoint = "dxfg_Promise_isDone")]
        public static extern int IsDone(
            nint thread,
            PromiseNative promiseHandle);

        [DllImport(
            ImportInfo.DllName,
            EntryPoint = "dxfg_Promise_hasResult")]
        public static extern int HasResult(
            nint thread,
            PromiseNative promiseHandle);

        [DllImport(
            ImportInfo.DllName,
            EntryPoint = "dxfg_Promise_cancel")]
        public static extern int Cancel(
            nint thread,
            PromiseNative promiseHandle);

        [DllImport(
            ImportInfo.DllName,
            EntryPoint = "dxfg_Promise_EventType_getResult")]
        public static extern EventTypeNative* GetResult(
            nint thread,
            PromiseNative promiseHandle);

        [DllImport(
            ImportInfo.DllName,
            EntryPoint = "dxfg_Promise_List_EventType_getResult")]
        public static extern ListNative<EventTypeNative>* GetResults(
            nint thread,
            PromiseNative promiseHandle);
    }
}
