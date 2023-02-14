// <copyright file="EndpointNative.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Native.ErrorHandling;
using DxFeed.Graal.Net.Native.Feed;
using DxFeed.Graal.Net.Native.Graal;
using DxFeed.Graal.Net.Native.Publisher;

namespace DxFeed.Graal.Net.Native.Endpoint;

/// <summary>
/// Native wrapper over the Java <c>com.dxfeed.api.DXEndpoint</c> class.
/// The location of the imported functions is in the header files <c>"dxfg_endpoint.h"</c>.
/// </summary>
internal sealed unsafe class EndpointNative : IDisposable
{
    private static readonly EndpointFinalizeFunc OnEndpointFinalize = Finalize;
    private readonly Lazy<FeedNative> _feedNative;
    private readonly Lazy<PublisherNative> _publisherNative;
    private readonly object _stateChangeListenerHandleLock = new();
    private StateChangeListenerHandle* _stateChangeListenerHandle;
    private EndpointHandle* _endpointHandle;
    private bool _disposed;

    internal EndpointNative(EndpointHandle* endpointHandle)
    {
        _endpointHandle = endpointHandle;
        _feedNative = new Lazy<FeedNative>(() =>
            new FeedNative(EndpointImport.GetFeed(GetCurrentThread(), _endpointHandle)));
        _publisherNative = new Lazy<PublisherNative>(() =>
            new PublisherNative(EndpointImport.GetPublisher(GetCurrentThread(), _endpointHandle)));
    }

    ~EndpointNative() =>
        ReleaseUnmanagedResources();

    public void Close() =>
        EndpointImport.Close(GetCurrentThread(), _endpointHandle);

    public void CloseAndAwaitTermination() =>
        EndpointImport.CloseAndAwaitTermination(GetCurrentThread(), _endpointHandle);

    public void User(string user) =>
        EndpointImport.SetUser(GetCurrentThread(), _endpointHandle, user);

    public void Password(string password) =>
        EndpointImport.SetPassword(GetCurrentThread(), _endpointHandle, password);

    public void Connect(string address) =>
        EndpointImport.Connect(GetCurrentThread(), _endpointHandle, address);

    public void Reconnect() =>
        EndpointImport.Reconnect(GetCurrentThread(), _endpointHandle);

    public void Disconnect() =>
        EndpointImport.Disconnect(GetCurrentThread(), _endpointHandle);

    public void DisconnectAndClear() =>
        EndpointImport.DisconnectAndClear(GetCurrentThread(), _endpointHandle);

    public void AwaitProcessed() =>
        EndpointImport.AwaitProcessed(GetCurrentThread(), _endpointHandle);

    public void AwaitNotConnected() =>
        EndpointImport.AwaitNotConnected(GetCurrentThread(), _endpointHandle);

    public int GetState() =>
        EndpointImport.GetState(GetCurrentThread(), _endpointHandle);

    /// <summary>
    /// Sets state change listener.
    /// Previously added listener will be removed.
    /// Only one listener allowed in this level.
    /// </summary>
    /// <param name="listenerFunc">The function pointer to the endpoint state change listener.</param>
    public void SetStateChangeListener(StateChangeListenerFunc listenerFunc)
    {
        lock (_stateChangeListenerHandleLock)
        {
            ClearStateChangeListener();
            var thread = GetCurrentThread();
            _stateChangeListenerHandle = EndpointImport.CreateStateChangeListener(thread, listenerFunc);
            EndpointImport.AddStateChangeListener(thread, _endpointHandle, _stateChangeListenerHandle);
        }
    }

    /// <summary>
    /// Removes a previously added listener.
    /// If no listener was added, nothing happened.
    /// </summary>
    public void ClearStateChangeListener()
    {
        lock (_stateChangeListenerHandleLock)
        {
            if ((nint)_stateChangeListenerHandle == 0)
            {
                return;
            }

            var thread = GetCurrentThread();
            EndpointImport.RemoveStateChangeListener(thread, _endpointHandle, _stateChangeListenerHandle);
            EndpointImport.ReleaseStateChangeListener(thread, _stateChangeListenerHandle);
            _stateChangeListenerHandle = (StateChangeListenerHandle*)0;
        }
    }

    public FeedNative GetFeed() =>
        _feedNative.Value;

    public PublisherNative GetPublisher() =>
        _publisherNative.Value;

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    private static nint GetCurrentThread() =>
        Isolate.CurrentThread;

    private static void Finalize(nint isolate, nint userData)
    {
        // ToDo Implement finalize callback.
    }

    private void ReleaseUnmanagedResources()
    {
        try
        {
            // When the object is disposed, attached state change listeners are not actually detached.
            // This allows the user to see the state of the Closed.
            // But the state change listeners handle will be release
            // and the Java garbage collector will clean it up later.
            var thread = GetCurrentThread();
            EndpointImport.Close(thread, _endpointHandle);
            EndpointImport.Release(thread, _endpointHandle);
            _endpointHandle = (EndpointHandle*)IntPtr.Zero;

            // Dispose are not generally thread-safe.
            // ReSharper disable once InconsistentlySynchronizedField
            EndpointImport.ReleaseStateChangeListener(thread, _stateChangeListenerHandle);

            // Dispose are not generally thread-safe.
            // ReSharper disable once InconsistentlySynchronizedField
            _stateChangeListenerHandle = (StateChangeListenerHandle*)IntPtr.Zero;
        }
        catch (Exception e)
        {
            // ToDo Add a log entry.
            Console.Error.WriteLine($"Exception in {GetType().Name} when releasing resource: {e}");
        }
    }

    /// <summary>
    /// Contains imported functions from native code.
    /// This is a thin wrapper with error checking.
    /// </summary>
    private static class EndpointImport
    {
        public static void Release(nint thread, EndpointHandle* endpointHandle) =>
            ErrorCheck.NativeCall(thread, NativeEndpointRelease(thread, endpointHandle));

        public static void Close(nint thread, EndpointHandle* endpointHandle) =>
            ErrorCheck.NativeCall(thread, NativeClose(thread, endpointHandle));

        public static void CloseAndAwaitTermination(nint thread, EndpointHandle* endpointHandle) =>
            ErrorCheck.NativeCall(thread, NativeCloseAndAwaitTermination(thread, endpointHandle));

        public static void SetUser(nint thread, EndpointHandle* endpointHandle, string user) =>
            ErrorCheck.NativeCall(thread, NativeSetUser(thread, endpointHandle, user));

        public static void SetPassword(nint thread, EndpointHandle* endpointHandle, string password) =>
            ErrorCheck.NativeCall(thread, NativeSetPassword(thread, endpointHandle, password));

        public static void Connect(nint thread, EndpointHandle* endpointHandle, string address) =>
            ErrorCheck.NativeCall(thread, NativeConnect(thread, endpointHandle, address));

        public static void Reconnect(nint thread, EndpointHandle* endpointHandle) =>
            ErrorCheck.NativeCall(thread, NativeReconnect(thread, endpointHandle));

        public static void Disconnect(nint thread, EndpointHandle* endpointHandle) =>
            ErrorCheck.NativeCall(thread, NativeDisconnect(thread, endpointHandle));

        public static void DisconnectAndClear(nint thread, EndpointHandle* endpointHandle) =>
            ErrorCheck.NativeCall(thread, NativeDisconnectAndClear(thread, endpointHandle));

        public static void AwaitProcessed(nint thread, EndpointHandle* endpointHandle) =>
            ErrorCheck.NativeCall(thread, NativeAwaitProcessed(thread, endpointHandle));

        public static void AwaitNotConnected(nint thread, EndpointHandle* endpointHandle) =>
            ErrorCheck.NativeCall(thread, NativeAwaitNotConnected(thread, endpointHandle));

        public static int GetState(nint thread, EndpointHandle* endpointHandle) =>
            ErrorCheck.NativeCall(thread, NativeGetState(thread, endpointHandle));

        public static StateChangeListenerHandle* CreateStateChangeListener(
            nint thread,
            StateChangeListenerFunc listenerFunc) =>
            ErrorCheck.NativeCall(thread, NativeCreateStateChangeListener(thread, listenerFunc, 0));

        public static void ReleaseStateChangeListener(
            nint thread,
            StateChangeListenerHandle* stateChangeListenerHandle) =>
            ErrorCheck.NativeCall(thread, NativeReleaseStateChangeListener(thread, stateChangeListenerHandle));

        public static void AddStateChangeListener(
            nint thread,
            EndpointHandle* endpointHandle,
            StateChangeListenerHandle* stateChangeListenerHandle) =>
            ErrorCheck.NativeCall(
                thread,
                NativeAddStateChangeListener(thread, endpointHandle, stateChangeListenerHandle, OnEndpointFinalize, 0));

        public static void RemoveStateChangeListener(
            nint thread,
            EndpointHandle* endpointHandle,
            StateChangeListenerHandle* stateChangeListenerHandle) =>
            ErrorCheck.NativeCall(
                thread,
                NativeRemoveStateChangeListener(thread, endpointHandle, stateChangeListenerHandle));

        public static FeedHandle* GetFeed(nint thread, EndpointHandle* endpointHandle) =>
            ErrorCheck.NativeCall(thread, NativeGetFeed(thread, endpointHandle));

        public static PublisherHandle* GetPublisher(nint thread, EndpointHandle* endpointHandle) =>
            ErrorCheck.NativeCall(thread, NativeGetPublisher(thread, endpointHandle));

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_JavaObjectHandler_release")]
        private static extern int NativeEndpointRelease(
            nint thread,
            EndpointHandle* endpointHandle);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_DXEndpoint_close")]
        private static extern int NativeClose(
            nint thread,
            EndpointHandle* endpointHandle);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_DXEndpoint_closeAndAwaitTermination")]
        private static extern int NativeCloseAndAwaitTermination(
            nint thread,
            EndpointHandle* endpointHandle);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_DXEndpoint_user")]
        private static extern int NativeSetUser(
            nint thread,
            EndpointHandle* endpointHandle,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string user);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_DXEndpoint_password")]
        private static extern int NativeSetPassword(
            nint thread,
            EndpointHandle* endpointHandle,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string password);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_DXEndpoint_connect")]
        private static extern int NativeConnect(
            nint thread,
            EndpointHandle* endpointHandle,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string address);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_DXEndpoint_reconnect")]
        private static extern int NativeReconnect(
            nint thread,
            EndpointHandle* endpointHandle);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_DXEndpoint_disconnect")]
        private static extern int NativeDisconnect(
            nint thread,
            EndpointHandle* endpointHandle);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_DXEndpoint_disconnectAndClear")]
        private static extern int NativeDisconnectAndClear(
            nint thread,
            EndpointHandle* endpointHandle);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_DXEndpoint_awaitProcessed")]
        private static extern int NativeAwaitProcessed(
            nint thread,
            EndpointHandle* endpointHandle);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_DXEndpoint_awaitNotConnected")]
        private static extern int NativeAwaitNotConnected(
            nint thread,
            EndpointHandle* endpointHandle);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_DXEndpoint_getState")]
        private static extern int NativeGetState(
            nint thread,
            EndpointHandle* endpointHandle);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_PropertyChangeListener_new")]
        private static extern StateChangeListenerHandle* NativeCreateStateChangeListener(
            nint thread,
            StateChangeListenerFunc listenerFunc,
            nint userData);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_JavaObjectHandler_release")]
        private static extern int NativeReleaseStateChangeListener(
            nint thread,
            StateChangeListenerHandle* listenerHandle);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_DXEndpoint_addStateChangeListener")]
        private static extern int NativeAddStateChangeListener(
            nint thread,
            EndpointHandle* endpointHandle,
            StateChangeListenerHandle* listenerHandle,
            EndpointFinalizeFunc endpointFinalize,
            nint userData);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_DXEndpoint_removeStateChangeListener")]
        private static extern int NativeRemoveStateChangeListener(
            nint thread,
            EndpointHandle* endpointHandle,
            StateChangeListenerHandle* listenerHandle);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_DXEndpoint_getFeed")]
        private static extern FeedHandle* NativeGetFeed(
            nint thread,
            EndpointHandle* endpointHandle);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_DXEndpoint_getPublisher")]
        private static extern PublisherHandle* NativeGetPublisher(
            nint thread,
            EndpointHandle* endpointHandle);
    }
}
