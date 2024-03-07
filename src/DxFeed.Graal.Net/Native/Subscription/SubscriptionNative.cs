// <copyright file="SubscriptionNative.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Native.ErrorHandling;
using DxFeed.Graal.Net.Native.Events;
using DxFeed.Graal.Net.Native.Feed;
using DxFeed.Graal.Net.Native.Graal;
using DxFeed.Graal.Net.Native.Interop;
using DxFeed.Graal.Net.Native.SymbolMappers;

namespace DxFeed.Graal.Net.Native.Subscription;

/// <summary>
/// Native wrapper over the Java <c>com.dxfeed.api.DxFeedSubscription</c> class.
/// The location of the imported functions is in the header files <c>"dxfg_subscription.h"</c>.
/// </summary>
internal sealed unsafe class SubscriptionNative : IDisposable
{
    private readonly object _eventListenerHandleLock = new();
    private EventListenerHandle* _eventListenerHandle;
    private SubscriptionHandle* _subHandle;
    private bool _disposed;

    public SubscriptionNative(SubscriptionHandle* subHandle) =>
        _subHandle = subHandle;

    ~SubscriptionNative() =>
        ReleaseUnmanagedResources();

    public static SubscriptionNative CreateSubscription(EventCodeNative eventCode) =>
        new(SubscriptionImport.New(GetCurrentThread(), eventCode));

    public static SubscriptionNative CreateSubscription(IEnumerable<EventCodeNative> eventCodes) =>
        new(SubscriptionImport.New(GetCurrentThread(), eventCodes));

    /// <summary>
    /// Sets state change listener.
    /// Previously added listener will be removed.
    /// Only one listener allowed in this level.
    /// </summary>
    /// <param name="listenerFunc">Function pointer to the endpoint state change listener.</param>
    public void SetEventListener(EventListenerFunc listenerFunc)
    {
        lock (_eventListenerHandleLock)
        {
            ClearEventListener();
            var thread = GetCurrentThread();
            _eventListenerHandle = SubscriptionImport.CreateEventListener(thread, listenerFunc);
            SubscriptionImport.AddEventListener(thread, _subHandle, _eventListenerHandle);
        }
    }

    /// <summary>
    /// Removes a previously added listener. If no listener was added, nothing happened.
    /// </summary>
    public void ClearEventListener()
    {
        lock (_eventListenerHandleLock)
        {
            if ((nint)_eventListenerHandle == 0)
            {
                return;
            }

            var thread = GetCurrentThread();
            SubscriptionImport.RemoveEventListener(thread, _subHandle, _eventListenerHandle);
            SubscriptionImport.ReleaseEventListener(thread, _eventListenerHandle);
            _eventListenerHandle = (EventListenerHandle*)0;
        }
    }

    public void AddSymbols(params object[] symbols)
    {
        if (symbols.Length == 1)
        {
            SubscriptionImport.AddSymbol(GetCurrentThread(), _subHandle, symbols[0]);
        }
        else
        {
            SubscriptionImport.AddSymbols(GetCurrentThread(), _subHandle, symbols);
        }
    }

    public void RemoveSymbol(params object[] symbols)
    {
        if (symbols.Length == 1)
        {
            SubscriptionImport.RemoveSymbol(GetCurrentThread(), _subHandle, symbols[0]);
        }
        else
        {
            SubscriptionImport.RemoveSymbols(GetCurrentThread(), _subHandle, symbols);
        }
    }

    public IEnumerable<object> GetSymbols()
    {
        return SubscriptionImport.GetSymbols(GetCurrentThread(), _subHandle);
    }

    public void Clear() =>
        SubscriptionImport.Clear(GetCurrentThread(), _subHandle);

    public void Attach(FeedNative feedNative) =>
        SubscriptionImport.Attach(GetCurrentThread(), _subHandle, feedNative.GetHandle());

    public void Detach(FeedNative feedNative) =>
        SubscriptionImport.Detach(GetCurrentThread(), _subHandle, feedNative.GetHandle());

    public bool IsClosed() =>
        SubscriptionImport.IsClosed(GetCurrentThread(), _subHandle);

    public void Close() =>
        SubscriptionImport.Close(GetCurrentThread(), _subHandle);

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
        _disposed = true;
    }

    internal SubscriptionHandle* GetHandle() =>
        _subHandle;

    private static nint GetCurrentThread() =>
        IsolateThread.CurrentThread;

    private void ReleaseUnmanagedResources()
    {
        try
        {
            var thread = GetCurrentThread();
            SubscriptionImport.Close(thread, _subHandle);
            SubscriptionImport.Release(thread, _subHandle);
            _subHandle = (SubscriptionHandle*)IntPtr.Zero;

            // Dispose are not generally thread-safe.
            // ReSharper disable once InconsistentlySynchronizedField
            SubscriptionImport.ReleaseEventListener(thread, _eventListenerHandle);

            // Dispose are not generally thread-safe.
            // ReSharper disable once InconsistentlySynchronizedField
            _eventListenerHandle = (EventListenerHandle*)IntPtr.Zero;
        }
        catch (Exception e)
        {
            // ToDo Add a log entry.
            Console.Error.WriteLine($"Exception in {nameof(GetType)} when releasing resource: {e}");
        }
    }

    private static class SubscriptionImport
    {
        public static SubscriptionHandle* New(nint thread, EventCodeNative eventCode) =>
            ErrorCheck.SafeCall(NativeSubscriptionNew(thread, eventCode));

        public static SubscriptionHandle* New(nint thread, IEnumerable<EventCodeNative> eventCodes)
        {
            var codes = ListNative<EventCodeNative>.Create(eventCodes);
            try
            {
                return ErrorCheck.SafeCall(NativeSubscriptionNew(thread, codes));
            }
            finally
            {
                ListNative<EventCodeNative>.Release(codes);
            }
        }

        public static void Release(nint thread, SubscriptionHandle* subHandle) =>
            ErrorCheck.SafeCall(NativeSubscriptionRelease(thread, subHandle));

        public static void Close(nint thread, SubscriptionHandle* subHandle) =>
            ErrorCheck.SafeCall(NativeClose(thread, subHandle));

        public static EventListenerHandle* CreateEventListener(nint thread, EventListenerFunc listenerFunc) =>
            ErrorCheck.SafeCall(NativeCreateEventListener(thread, listenerFunc, IntPtr.Zero));

        public static void ReleaseEventListener(nint thread, EventListenerHandle* eventListenerHandle) =>
            ErrorCheck.SafeCall(NativeReleaseEventListener(thread, eventListenerHandle));

        public static void AddEventListener(
            nint thread,
            SubscriptionHandle* subHandle,
            EventListenerHandle* eventListenerHandle) =>
            ErrorCheck.SafeCall(NativeAddEventListener(thread, subHandle, eventListenerHandle));

        public static void RemoveEventListener(
            nint thread,
            SubscriptionHandle* subHandle,
            EventListenerHandle* eventListenerHandle) =>
            ErrorCheck.SafeCall(NativeRemoveEventListener(thread, subHandle, eventListenerHandle));

        public static void AddSymbol(nint thread, SubscriptionHandle* subHandle, object symbol)
        {
            ErrorCheck.SafeCall(NativeAddSymbol(thread, subHandle, symbol));
        }

        public static void AddSymbols(nint thread, SubscriptionHandle* subHandle, object[] symbol)
        {
            ErrorCheck.SafeCall(NativeAddSymbols(thread, subHandle, symbol));
        }

        public static void RemoveSymbol(nint thread, SubscriptionHandle* subHandle, object symbol)
        {
            ErrorCheck.SafeCall(NativeRemoveSymbol(thread, subHandle, symbol));
        }

        public static void RemoveSymbols(nint thread, SubscriptionHandle* subHandle, object[] symbol)
        {
            ErrorCheck.SafeCall(NativeRemoveSymbols(thread, subHandle, symbol));
        }

        public static IEnumerable<object> GetSymbols(nint thread, SubscriptionHandle* subHandle)
        {
            return (IEnumerable<object>)ErrorCheck.SafeCall(NativeGetSymbols(thread, subHandle));
        }

        public static void Clear(nint thread, SubscriptionHandle* subHandle) =>
            ErrorCheck.SafeCall(NativeClear(thread, subHandle));

        public static void Attach(nint thread, SubscriptionHandle* subHandle, FeedHandle* feedHandle) =>
            ErrorCheck.SafeCall(NativeAttach(thread, subHandle, feedHandle));

        public static void Detach(nint thread, SubscriptionHandle* subHandle, FeedHandle* feedHandle) =>
            ErrorCheck.SafeCall(NativeDetach(thread, subHandle, feedHandle));

        public static bool IsClosed(nint thread, SubscriptionHandle* subHandle) =>
            ErrorCheck.SafeCall(NativeIsClosed(thread, subHandle)) != 0;

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_DXFeedSubscription_new")]
        private static extern SubscriptionHandle* NativeSubscriptionNew(
            nint thread,
            EventCodeNative eventCode);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_DXFeedSubscription_new2")]
        private static extern SubscriptionHandle* NativeSubscriptionNew(
            nint thread,
            ListNative<EventCodeNative>* eventCodes);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_JavaObjectHandler_release")]
        private static extern int NativeSubscriptionRelease(
            nint thread,
            SubscriptionHandle* subHandle);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_DXFeedSubscription_close")]
        private static extern int NativeClose(
            nint thread,
            SubscriptionHandle* subHandle);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_DXFeedEventListener_new")]
        private static extern EventListenerHandle* NativeCreateEventListener(
            nint thread,
            EventListenerFunc listenerFunc,
            nint userData);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_JavaObjectHandler_release")]
        private static extern int NativeReleaseEventListener(
            nint thread,
            EventListenerHandle* listenerHandle);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_DXFeedSubscription_addEventListener")]
        private static extern int NativeAddEventListener(
            nint thread,
            SubscriptionHandle* subHandle,
            EventListenerHandle* listenerHandle);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_DXFeedSubscription_removeEventListener")]
        private static extern int NativeRemoveEventListener(
            nint thread,
            SubscriptionHandle* subHandle,
            EventListenerHandle* listenerHandle);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_DXFeedSubscription_addSymbol")]
        private static extern int NativeAddSymbol(
            nint thread,
            SubscriptionHandle* subHandle,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SymbolMarshaller))]
            object value);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_DXFeedSubscription_addSymbols")]
        private static extern int NativeAddSymbols(
            nint thread,
            SubscriptionHandle* subHandle,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ListMarshaler<SymbolMarshaller>))]
            object[] value);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_DXFeedSubscription_removeSymbol")]
        private static extern int NativeRemoveSymbol(
            nint thread,
            SubscriptionHandle* subHandle,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(SymbolMarshaller))]
            object value);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_DXFeedSubscription_removeSymbols")]
        private static extern int NativeRemoveSymbols(
            nint thread,
            SubscriptionHandle* subHandle,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ListMarshaler<SymbolMarshaller>))]
            object[] value);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_DXFeedSubscription_getSymbols")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ListMarshaler<SymbolMarshaller>))]
        private static extern IEnumerable<object> NativeGetSymbols(
            nint thread,
            SubscriptionHandle* subHandle);


        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_DXFeedSubscription_clear")]
        private static extern int NativeClear(
            nint thread,
            SubscriptionHandle* subHandle);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_DXFeedSubscription_attach")]
        private static extern int NativeAttach(
            nint thread,
            SubscriptionHandle* subHandle,
            FeedHandle* feedHandle);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_DXFeedSubscription_detach")]
        private static extern int NativeDetach(
            nint thread,
            SubscriptionHandle* subHandle,
            FeedHandle* feedHandle);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_DXFeedSubscription_isClosed")]
        private static extern int NativeIsClosed(
            nint thread,
            SubscriptionHandle* subHandle);
    }
}
