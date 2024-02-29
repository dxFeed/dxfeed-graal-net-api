// <copyright file="PromiseNative.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Events;
using DxFeed.Graal.Net.Native.ErrorHandling;
using DxFeed.Graal.Net.Native.Events;
using DxFeed.Graal.Net.Native.Interop;
using static DxFeed.Graal.Net.Native.ErrorHandling.ErrorCheck;

namespace DxFeed.Graal.Net.Native.Promise;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Created by marshaller")]
internal sealed unsafe class PromiseNative : JavaHandle
{
    public bool IsDone() => SafeCall(Import.IsDone(CurrentThread, this)) != 0;

    public bool HasResult() => SafeCall(Import.HasResult(CurrentThread, this)) != 0;

    public void ThrowIfJavaExceptionExists()
    {
        if (SafeCall(Import.HasException(CurrentThread, this)) != 0)
        {
            using var exceptionHandle = Import.GetException(CurrentThread, this);
            if (exceptionHandle.IsInvalid)
            {
                return;
            }

            exceptionHandle.ThrowException();
        }
    }

    public void Cancel() => SafeCall(Import.Cancel(CurrentThread, this));

    public IEventType Result()
    {
        var nativeResult = SafeCall(Import.GetResult(CurrentThread, this));
        try
        {
            var result = EventMapper.FromNative(nativeResult);
            return result;
        }
        finally
        {
            SafeCall(Import.ReleaseResult(CurrentThread, nativeResult));
        }
    }

    public IEnumerable<IEventType> Results()
    {
        var nativeResult = SafeCall(Import.GetResults(CurrentThread, this));
        try
        {
            var result = EventMapper.FromNative(nativeResult);
            return result;
        }
        finally
        {
            SafeCall(Import.ReleaseResults(CurrentThread, nativeResult));
        }
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
            EntryPoint = "dxfg_Promise_hasException")]
        public static extern int HasException(
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

        [DllImport(
            ImportInfo.DllName,
            EntryPoint = "dxfg_CList_EventType_release")]
        public static extern int ReleaseResults(
            nint thread,
            ListNative<EventTypeNative>* eventList);

        [DllImport(
            ImportInfo.DllName,
            EntryPoint = "dxfg_EventType_release")]
        public static extern int ReleaseResult(
            nint thread,
            EventTypeNative* nativeEvent);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "dxfg_Promise_getException")]
        public static extern JavaExceptionHandle GetException(
            nint thread,
            PromiseNative promiseHandle);
    }
}
