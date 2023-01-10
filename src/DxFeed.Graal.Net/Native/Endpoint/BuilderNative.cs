// <copyright file="BuilderNative.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Api;
using DxFeed.Graal.Net.Native.ErrorHandling;
using DxFeed.Graal.Net.Native.Graal;

namespace DxFeed.Graal.Net.Native.Endpoint;

/// <summary>
/// Native wrapper over the Java <c>com.dxfeed.api.DXEndpoint.Builder</c> class.
/// The location of the imported functions is in the header files <c>"dxfg_endpoint.h"</c>.
/// </summary>
internal sealed unsafe class BuilderNative : IDisposable
{
    private BuilderHandle* _builderHandle;
    private bool _disposed;

    private BuilderNative(BuilderHandle* builderHandle) =>
        _builderHandle = builderHandle;

    ~BuilderNative() =>
        ReleaseUnmanagedResources();

    public static BuilderNative Create() =>
        new(BuilderImport.New(GetCurrentThread()));

    public void WithRole(DXEndpoint.Role role) =>
        BuilderImport.WithRole(GetCurrentThread(), _builderHandle, role);

    public void WithProperty(string key, string value) =>
        BuilderImport.WithProperty(GetCurrentThread(), _builderHandle, key, value);

    public bool SupportsProperty(string key) =>
        BuilderImport.SupportsProperty(GetCurrentThread(), _builderHandle, key);

    public EndpointNative Build() =>
        new(BuilderImport.Build(GetCurrentThread(), _builderHandle));

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
        Isolate.Instance.IsolateThread;

    private void ReleaseUnmanagedResources()
    {
        try
        {
            BuilderImport.Release(GetCurrentThread(), _builderHandle);
            _builderHandle = (BuilderHandle*)0;
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
    private static class BuilderImport
    {
        public static BuilderHandle* New(nint thread) =>
            ErrorCheck.NativeCall(thread, NativeBuilderNew(thread));

        public static void Release(nint thread, BuilderHandle* builderHandle) =>
            ErrorCheck.NativeCall(thread, NativeBuilderRelease(thread, builderHandle));

        public static void WithRole(nint thread, BuilderHandle* builderHandle, DXEndpoint.Role role) =>
            ErrorCheck.NativeCall(thread, NativeWithRole(thread, builderHandle, (int)role));

        public static void WithProperty(nint thread, BuilderHandle* builderHandle, string key, string value) =>
            ErrorCheck.NativeCall(thread, NativeWithProperty(thread, builderHandle, key, value));

        public static bool SupportsProperty(nint thread, BuilderHandle* builderHandle, string key) =>
            ErrorCheck.NativeCall(thread, NativeSupportsProperty(thread, builderHandle, key)) != 0;

        public static EndpointHandle* Build(nint thread, BuilderHandle* builderHandle) =>
            ErrorCheck.NativeCall(thread, NativeBuild(thread, builderHandle));

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_DXEndpoint_newBuilder")]
        private static extern BuilderHandle* NativeBuilderNew(nint thread);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_JavaObjectHandler_release")]
        private static extern int NativeBuilderRelease(
            nint thread,
            BuilderHandle* builderHandle);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_DXEndpoint_Builder_withRole")]
        private static extern int NativeWithRole(
            nint thread,
            BuilderHandle* builderHandle,
            int role);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_DXEndpoint_Builder_withProperty")]
        private static extern int NativeWithProperty(
            nint thread,
            BuilderHandle* builderHandle,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string key,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string value);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_DXEndpoint_Builder_supportsProperty")]
        private static extern int NativeSupportsProperty(
            nint thread,
            BuilderHandle* builderHandle,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string key);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_DXEndpoint_Builder_build")]
        private static extern EndpointHandle* NativeBuild(
            nint thread,
            BuilderHandle* builderHandle);
    }
}
