// <copyright file="StateChangeListenerSafeHandle.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Native.ErrorHandling;
using DxFeed.Graal.Net.Native.Graal;
using DxFeed.Graal.Net.Native.Interop;

namespace DxFeed.Graal.Net.Native.Endpoint.Handles;

/// <summary>
/// This class wraps an unsafe handler <see cref="StateChangeListenerHandle"/>.
/// The location of the imported functions is in the header files <c>"dxfg_endpoint.h"</c>.
/// </summary>
internal sealed unsafe class StateChangeListenerSafeHandle : SafeHandleZeroIsInvalid
{
    private StateChangeListenerSafeHandle(StateChangeListenerHandle* handle) =>
        SetHandle((nint)handle);

    public static implicit operator StateChangeListenerHandle*(StateChangeListenerSafeHandle value) =>
        (StateChangeListenerHandle*)value.handle;

    public static StateChangeListenerSafeHandle Create(
        delegate* unmanaged[Cdecl]<nint, int, int, nint, void> listener,
        GCHandle userData)
    {
        var thread = Isolate.CurrentThread;
        return new(ErrorCheck.NativeCall(
            thread,
            NativeCreate(thread, listener, GCHandle.ToIntPtr(userData))));
    }

    protected override bool ReleaseHandle()
    {
        try
        {
            var thread = Isolate.CurrentThread;
            ErrorCheck.NativeCall(thread, NativeRelease(thread, (StateChangeListenerHandle*)handle));
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
        EntryPoint = "dxfg_PropertyChangeListener_new")]
    private static extern StateChangeListenerHandle* NativeCreate(
        nint thread,
        delegate* unmanaged[Cdecl]<nint, int, int, nint, void> listenerFunc,
        nint userData);

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        EntryPoint = "dxfg_JavaObjectHandler_release")]
    private static extern int NativeRelease(
        nint thread,
        StateChangeListenerHandle* listenerHandle);
}
