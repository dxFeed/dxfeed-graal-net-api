// <copyright file="EndpointSafeHandle.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Native.ErrorHandling;
using DxFeed.Graal.Net.Native.Feed;
using DxFeed.Graal.Net.Native.Graal;
using DxFeed.Graal.Net.Native.Interop;
using DxFeed.Graal.Net.Native.Publisher;

namespace DxFeed.Graal.Net.Native.Endpoint.Handles;

/// <summary>
/// This class wraps an unsafe handler <see cref="BuilderHandle"/>.
/// The location of the imported functions is in the header files <c>"dxfg_endpoint.h"</c>.
/// </summary>
internal sealed unsafe class EndpointSafeHandle : SafeHandleZeroIsInvalid
{
    public EndpointSafeHandle(EndpointHandle* handle) =>
        SetHandle((nint)handle);

    public static implicit operator EndpointHandle*(EndpointSafeHandle value) =>
        (EndpointHandle*)value.handle;

    public new void Close()
    {
        var thread = Isolate.CurrentThread;
        ErrorCheck.NativeCall(thread, NativeClose(thread, this));
    }

    public void CloseAndAwaitTermination()
    {
        var thread = Isolate.CurrentThread;
        ErrorCheck.NativeCall(thread, NativeCloseAndAwaitTermination(thread, this));
    }

    public void SetUser(string user)
    {
        var thread = Isolate.CurrentThread;
        ErrorCheck.NativeCall(thread, NativeSetUser(thread, this, user));
    }

    public void SetPassword(string password)
    {
        var thread = Isolate.CurrentThread;
        ErrorCheck.NativeCall(thread, NativeSetPassword(thread, this, password));
    }

    public void Connect(string address)
    {
        var thread = Isolate.CurrentThread;
        ErrorCheck.NativeCall(thread, NativeConnect(thread, this, address));
    }

    public void Reconnect()
    {
        var thread = Isolate.CurrentThread;
        ErrorCheck.NativeCall(thread, NativeReconnect(thread, this));
    }

    public void Disconnect()
    {
        var thread = Isolate.CurrentThread;
        ErrorCheck.NativeCall(thread, NativeDisconnect(thread, this));
    }

    public void DisconnectAndClear()
    {
        var thread = Isolate.CurrentThread;
        ErrorCheck.NativeCall(thread, NativeDisconnectAndClear(thread, this));
    }

    public void AwaitProcessed()
    {
        var thread = Isolate.CurrentThread;
        ErrorCheck.NativeCall(thread, NativeAwaitProcessed(thread, this));
    }

    public void AwaitNotConnected()
    {
        var thread = Isolate.CurrentThread;
        ErrorCheck.NativeCall(thread, NativeAwaitNotConnected(thread, this));
    }

    public int GetState()
    {
        var thread = Isolate.CurrentThread;
        return ErrorCheck.NativeCall(thread, NativeGetState(thread, this));
    }

    public void AddStateChangeListener(StateChangeListenerSafeHandle stateChangeListenerHandle)
    {
        var thread = Isolate.CurrentThread;
        ErrorCheck.NativeCall(
            thread,
            NativeAddStateChangeListener(thread, this, stateChangeListenerHandle));
    }

    public FeedHandle* GetFeed()
    {
        var thread = Isolate.CurrentThread;
        return ErrorCheck.NativeCall(thread, NativeGetFeed(thread, this));
    }

    public PublisherHandle* GetPublisher()
    {
        var thread = Isolate.CurrentThread;
        return ErrorCheck.NativeCall(thread, NativeGetPublisher(thread, this));
    }

    protected override bool ReleaseHandle()
    {
        try
        {
            var thread = Isolate.CurrentThread;
            ErrorCheck.NativeCall(thread, NativeRelease(thread, (EndpointHandle*)handle));
            handle = (IntPtr)0;
            return true;
        }
        catch (Exception e)
        {
            // ToDo Add a log entry.
            Console.Error.WriteLine($"Exception in {GetType().Name} when releasing resource: {e}");
        }

        return false;
    }

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        EntryPoint = "dxfg_JavaObjectHandler_release")]
    private static extern int NativeRelease(
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
        EntryPoint = "dxfg_DXEndpoint_addStateChangeListener")]
    private static extern int NativeAddStateChangeListener(
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
