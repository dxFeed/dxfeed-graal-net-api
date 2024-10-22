// <copyright file="FeedNative.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Collections.Generic;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Events;
using DxFeed.Graal.Net.Native.Events;
using DxFeed.Graal.Net.Native.Graal;
using DxFeed.Graal.Net.Native.Interop;
using DxFeed.Graal.Net.Native.Promise;
using DxFeed.Graal.Net.Native.Subscription;
using DxFeed.Graal.Net.Native.SymbolMappers;
using static DxFeed.Graal.Net.Native.ErrorHandling.ErrorCheck;

namespace DxFeed.Graal.Net.Native.Feed;

/// <summary>
/// Native wrapper over the Java <c>com.dxfeed.api.DXFeed</c> class.
/// The location of the imported functions is in the header files <c>"dxfg_feed.h"</c>.
/// </summary>
internal sealed unsafe class FeedNative
{
    private readonly FeedHandle* _feedHandle;

    public FeedNative(FeedHandle* feedHandle) =>
        _feedHandle = feedHandle;

    // ToDo Delete all CreateSubscription method from this class, subscription must be creates only in SubscriptionNative.
    public SubscriptionNative CreateSubscription(EventCodeNative eventCode) =>
        new(FeedImport.CreateSubscription(GetCurrentThread(), _feedHandle, eventCode));

    public SubscriptionNative CreateSubscription(IEnumerable<EventCodeNative> eventCodes) =>
        new(FeedImport.CreateSubscription(GetCurrentThread(), _feedHandle, eventCodes));

    public void AttachSubscription(SubscriptionNative subscriptionNative) =>
        FeedImport.AttachSubscription(GetCurrentThread(), _feedHandle, subscriptionNative.GetHandle());

    public void DetachSubscription(SubscriptionNative subscriptionNative) =>
        FeedImport.DetachSubscription(GetCurrentThread(), _feedHandle, subscriptionNative.GetHandle());

    public void DetachSubscriptionAndClear(SubscriptionNative subscriptionNative) =>
        FeedImport.DetachSubscriptionAndClear(GetCurrentThread(), _feedHandle, subscriptionNative.GetHandle());

    public T GetLastEvent<T>(T e)
        where T : ILastingEvent
    {
        var eventCode = EventCodeAttribute.GetEventCode(typeof(T));
        var handle = GetLastEventIfSubscribed(eventCode, e.EventSymbol!);
        if (handle == null)
        {
            return e;
        }

        try
        {
            EventMapper.FillFromNative(handle, e);
            return e;
        }
        finally
        {
            SafeCall(FeedImport.ReleaseNativeEvent(GetCurrentThread(), handle));
        }
    }

    public IList<T> GetLastEvents<T>(IList<T> events)
        where T : ILastingEvent
    {
        foreach (var e in events)
        {
            GetLastEvent(e);
        }

        return events;
    }

    public T? GetLastEventIfSubscribed<T>(object symbol)
        where T : ILastingEvent
    {
        var eventCode = EventCodeAttribute.GetEventCode(typeof(T));
        var handle = FeedImport.GetLastEventIfSubscribed(GetCurrentThread(), _feedHandle, eventCode, symbol);
        if (handle == null)
        {
            return default;
        }

        try
        {
            return (T?)EventMapper.FromNative(handle);
        }
        finally
        {
            SafeCall(FeedImport.ReleaseNativeEvent(GetCurrentThread(), handle));
        }
    }

    public PromiseNative GetLastEventPromise(EventCodeNative eventCode, object symbol) =>
        FeedImport.GetLastEventPromise(GetCurrentThread(), _feedHandle, eventCode, symbol);

    public PromiseNative GetTimeSeriesPromise(EventCodeNative eventCode, object symbol, long from, long to) =>
        FeedImport.GetTimeSeriesPromise(GetCurrentThread(), _feedHandle, eventCode, symbol, from, to);

    internal FeedHandle* GetHandle() =>
        _feedHandle;

    private static nint GetCurrentThread() =>
        Isolate.CurrentThread;

    private EventTypeNative* GetLastEventIfSubscribed(EventCodeNative eventCode, object symbol) =>
        FeedImport.GetLastEventIfSubscribed(GetCurrentThread(), _feedHandle, eventCode, symbol);

    /// <summary>
    /// Contains imported functions from native code.
    /// </summary>
    private static class FeedImport
    {
        public static SubscriptionHandle* CreateSubscription(
            nint thread,
            FeedHandle* feedHandle,
            EventCodeNative eventCode) =>
            SafeCall(NativeCreateSubscription(thread, feedHandle, eventCode));

        public static SubscriptionHandle* CreateSubscription(
            nint thread,
            FeedHandle* feedHandle,
            IEnumerable<EventCodeNative> eventCodes)
        {
            var codes = ListNative<EventCodeNative>.Create(eventCodes);
            try
            {
                return SafeCall(NativeCreateSubscription(thread, feedHandle, codes));
            }
            finally
            {
                ListNative<EventCodeNative>.Release(codes);
            }
        }

        public static void AttachSubscription(
            nint thread,
            FeedHandle* feedHandle,
            SubscriptionHandle* subHandle) =>
            SafeCall(NativeAttachSubscription(thread, feedHandle, subHandle));

        public static void DetachSubscription(
            nint thread,
            FeedHandle* feedHandle,
            SubscriptionHandle* subHandle) =>
            SafeCall(NativeDetachSubscription(thread, feedHandle, subHandle));

        public static void DetachSubscriptionAndClear(
            nint thread,
            FeedHandle* feedHandle,
            SubscriptionHandle* subHandle) =>
            SafeCall(NativeDetachSubscriptionAndClear(thread, feedHandle, subHandle));

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_DXFeed_getLastEventIfSubscribed")]
        public static extern EventTypeNative* GetLastEventIfSubscribed(
            nint thread,
            FeedHandle* feedHandle,
            EventCodeNative eventCodes,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SymbolMarshaler))]
            object symbol);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_DXFeed_getTimeSeriesPromise")]
        public static extern PromiseNative GetTimeSeriesPromise(
            nint thread,
            FeedHandle* feedHandle,
            EventCodeNative eventCodes,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SymbolMarshaler))]
            object value,
            long from,
            long to);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_DXFeed_getLastEventPromise")]
        public static extern PromiseNative GetLastEventPromise(
            nint thread,
            FeedHandle* feedHandle,
            EventCodeNative eventCodes,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SymbolMarshaler))]
            object value);

        [DllImport(
            ImportInfo.DllName,
            EntryPoint = "dxfg_EventType_release")]
        public static extern int ReleaseNativeEvent(
            nint thread,
            EventTypeNative* nativeEvent);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_DXFeed_createSubscription")]
        private static extern SubscriptionHandle* NativeCreateSubscription(
            nint thread,
            FeedHandle* feedHandle,
            EventCodeNative eventCode);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_DXFeed_createSubscription2")]
        private static extern SubscriptionHandle* NativeCreateSubscription(
            nint thread,
            FeedHandle* feedHandle,
            ListNative<EventCodeNative>* eventCodes);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_DXFeed_attachSubscription")]
        private static extern int NativeAttachSubscription(
            nint thread,
            FeedHandle* feedHandle,
            SubscriptionHandle* subHandle);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_DXFeed_detachSubscription")]
        private static extern int NativeDetachSubscription(
            nint thread,
            FeedHandle* feedHandle,
            SubscriptionHandle* subHandle);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_DXFeed_detachSubscriptionAndClear")]
        private static extern int NativeDetachSubscriptionAndClear(
            nint thread,
            FeedHandle* feedHandle,
            SubscriptionHandle* subHandle);
    }
}
