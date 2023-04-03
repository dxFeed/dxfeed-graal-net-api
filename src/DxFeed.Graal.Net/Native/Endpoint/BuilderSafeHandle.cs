// <copyright file="BuilderSafeHandle.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Api;
using DxFeed.Graal.Net.Native.ErrorHandling;
using DxFeed.Graal.Net.Native.Graal;
using DxFeed.Graal.Net.Native.Interop;

namespace DxFeed.Graal.Net.Native.Endpoint;

/// <summary>
/// A handle that represents a Java <c>com.dxfeed.api.DXEndpoint.Builder</c> object.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal readonly struct BuilderHandle
{
    // ReSharper disable once MemberCanBePrivate.Global
    public readonly JavaObjectHandle Handle;
}

/// <summary>
/// This class wraps an unsafe handler <see cref="BuilderHandle"/>.
/// The location of the imported functions is in the header files <c>"dxfg_endpoint.h"</c>.
/// </summary>
internal sealed unsafe class BuilderSafeHandle : SafeHandleZeroIsInvalid
{
    private BuilderSafeHandle(BuilderHandle* handle) =>
        SetHandle((nint)handle);

    public static implicit operator BuilderHandle*(BuilderSafeHandle value) =>
        (BuilderHandle*)value.handle;

    public static BuilderSafeHandle Create()
    {
        var thread = Isolate.CurrentThread;
        return new(ErrorCheck.NativeCall(thread, NativeCreate(thread)));
    }

    public void WithRole(DXEndpoint.Role role)
    {
        var thread = Isolate.CurrentThread;
        ErrorCheck.NativeCall(thread, NativeWithRole(thread, this, (int)role));
    }

    public void WithProperty(string key, string value)
    {
        var thread = Isolate.CurrentThread;
        ErrorCheck.NativeCall(thread, NativeWithProperty(thread, this, key, value));
    }

    public bool SupportsProperty(string key)
    {
        var thread = Isolate.CurrentThread;
        return ErrorCheck.NativeCall(thread, NativeSupportsProperty(thread, this, key)) != 0;
    }

    public EndpointSafeHandle Build()
    {
        var thread = Isolate.CurrentThread;
        return new EndpointSafeHandle(ErrorCheck.NativeCall(thread, NativeBuild(thread, this)));
    }

    protected override bool ReleaseHandle()
    {
        try
        {
            var thread = Isolate.CurrentThread;
            ErrorCheck.NativeCall(thread, NativeRelease(thread, (BuilderHandle*)handle));
            handle = (nint)0;
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
        EntryPoint = "dxfg_DXEndpoint_newBuilder")]
    private static extern BuilderHandle* NativeCreate(nint thread);

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        EntryPoint = "dxfg_JavaObjectHandler_release")]
    private static extern int NativeRelease(
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
