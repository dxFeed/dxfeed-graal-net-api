// <copyright file="DXEndpointHandle.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Native.Feed;
using DxFeed.Graal.Net.Native.Interop;
using DxFeed.Graal.Net.Native.Publisher;
using static DxFeed.Graal.Net.Native.ErrorHandling.ErrorCheck;

namespace DxFeed.Graal.Net.Native.Endpoint;

/// <summary>
/// This class wraps an unsafe handler <see cref="BuilderHandle"/>.
/// The location of the imported functions is in the header files <c>"dxfg_endpoint.h"</c>.
/// </summary>
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Created by marshaller")]
internal sealed unsafe class DXEndpointHandle : JavaHandle
{
    public new void Close() =>
        NativeCall(CurrentThread, Import.Close(CurrentThread, this));

    public void CloseAndAwaitTermination() =>
        NativeCall(CurrentThread, Import.CloseAndAwaitTermination(CurrentThread, this));

    public void SetUser(string user) =>
        NativeCall(CurrentThread, Import.SetUser(CurrentThread, this, user));

    public void SetPassword(string password) =>
        NativeCall(CurrentThread, Import.SetPassword(CurrentThread, this, password));

    public void Connect(string address) =>
        NativeCall(CurrentThread, Import.Connect(CurrentThread, this, address));

    public void Reconnect() =>
        NativeCall(CurrentThread, Import.Reconnect(CurrentThread, this));

    public void Disconnect() =>
        NativeCall(CurrentThread, Import.Disconnect(CurrentThread, this));

    public void DisconnectAndClear() =>
        NativeCall(CurrentThread, Import.DisconnectAndClear(CurrentThread, this));

    public void AwaitProcessed() =>
        NativeCall(CurrentThread, Import.AwaitProcessed(CurrentThread, this));

    public void AwaitNotConnected() =>
        NativeCall(CurrentThread, Import.AwaitNotConnected(CurrentThread, this));

    public int GetState() =>
        NativeCall(CurrentThread, Import.GetState(CurrentThread, this));

    public void AddStateChangeListener(StateChangeListenerHandle listener) =>
        NativeCall(CurrentThread, Import.AddStateChangeListener(CurrentThread, this, listener));

    public void RemoveStateChangeListener(StateChangeListenerHandle listener) =>
        NativeCall(CurrentThread, Import.RemoveStateChangeListener(CurrentThread, this, listener));

    public FeedHandle* GetFeed() =>
        NativeCall(CurrentThread, Import.GetFeed(CurrentThread, this));

    public PublisherHandle* GetPublisher() =>
        NativeCall(CurrentThread, Import.GetPublisher(CurrentThread, this));

    private static class Import
    {
        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_DXEndpoint_close")]
        public static extern int Close(
            nint thread,
            DXEndpointHandle endpoint);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_DXEndpoint_closeAndAwaitTermination")]
        public static extern int CloseAndAwaitTermination(
            nint thread,
            DXEndpointHandle endpoint);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_DXEndpoint_user")]
        public static extern int SetUser(
            nint thread,
            DXEndpointHandle endpoint,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string user);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_DXEndpoint_password")]
        public static extern int SetPassword(
            nint thread,
            DXEndpointHandle endpoint,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string password);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_DXEndpoint_connect")]
        public static extern int Connect(
            nint thread,
            DXEndpointHandle endpoint,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string address);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_DXEndpoint_reconnect")]
        public static extern int Reconnect(
            nint thread,
            DXEndpointHandle endpoint);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_DXEndpoint_disconnect")]
        public static extern int Disconnect(
            nint thread,
            DXEndpointHandle endpoint);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_DXEndpoint_disconnectAndClear")]
        public static extern int DisconnectAndClear(
            nint thread,
            DXEndpointHandle endpoint);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_DXEndpoint_awaitProcessed")]
        public static extern int AwaitProcessed(
            nint thread,
            DXEndpointHandle endpoint);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_DXEndpoint_awaitNotConnected")]
        public static extern int AwaitNotConnected(
            nint thread,
            DXEndpointHandle endpoint);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_DXEndpoint_getState")]
        public static extern int GetState(
            nint thread,
            DXEndpointHandle endpoint);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_DXEndpoint_addStateChangeListener")]
        public static extern int AddStateChangeListener(
            nint thread,
            DXEndpointHandle endpoint,
            StateChangeListenerHandle listener);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_DXEndpoint_removeStateChangeListener")]
        public static extern int RemoveStateChangeListener(
            nint thread,
            DXEndpointHandle endpoint,
            StateChangeListenerHandle listener);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_DXEndpoint_getFeed")]
        public static extern FeedHandle* GetFeed(
            nint thread,
            DXEndpointHandle endpoint);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_DXEndpoint_getPublisher")]
        public static extern PublisherHandle* GetPublisher(
            nint thread,
            DXEndpointHandle endpoint);
    }
}
