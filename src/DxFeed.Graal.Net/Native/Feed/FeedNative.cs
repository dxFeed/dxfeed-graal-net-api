// <copyright file="FeedNative.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Collections.Generic;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Native.ErrorHandling;
using DxFeed.Graal.Net.Native.Events;
using DxFeed.Graal.Net.Native.Graal;
using DxFeed.Graal.Net.Native.Interop;
using DxFeed.Graal.Net.Native.Subscription;

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

    internal FeedHandle* GetHandle() =>
        _feedHandle;

    private static nint GetCurrentThread() =>
        Isolate.CurrentThread;

    /// <summary>
    /// Contains imported functions from native code.
    /// </summary>
    private static class FeedImport
    {
        public static SubscriptionHandle* CreateSubscription(
            nint thread,
            FeedHandle* feedHandle,
            EventCodeNative eventCode) =>
            ErrorCheck.SafeCall(thread, NativeCreateSubscription(thread, feedHandle, eventCode));

        public static SubscriptionHandle* CreateSubscription(
            nint thread,
            FeedHandle* feedHandle,
            IEnumerable<EventCodeNative> eventCodes)
        {
            var codes = ListNative<EventCodeNative>.Create(eventCodes);
            try
            {
                return ErrorCheck.SafeCall(thread, NativeCreateSubscription(thread, feedHandle, codes));
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
            ErrorCheck.SafeCall(thread, NativeAttachSubscription(thread, feedHandle, subHandle));

        public static void DetachSubscription(
            nint thread,
            FeedHandle* feedHandle,
            SubscriptionHandle* subHandle) =>
            ErrorCheck.SafeCall(thread, NativeDetachSubscription(thread, feedHandle, subHandle));

        public static void DetachSubscriptionAndClear(
            nint thread,
            FeedHandle* feedHandle,
            SubscriptionHandle* subHandle) =>
            ErrorCheck.SafeCall(thread, NativeDetachSubscriptionAndClear(thread, feedHandle, subHandle));

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
