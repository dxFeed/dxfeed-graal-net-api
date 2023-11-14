// <copyright file="StateChangeListenerHandle.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Native.Interop;
using static DxFeed.Graal.Net.Api.DXEndpoint;
using static DxFeed.Graal.Net.Native.ErrorHandling.ErrorCheck;

namespace DxFeed.Graal.Net.Native.Endpoint;

/// <summary>
/// Represents a handle for a state change listener in a Java environment.
/// This class encapsulates the necessary logic to interact with native code for state change events.
/// </summary>
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Created by marshaller")]
internal sealed class StateChangeListenerHandle : JavaHandle
{
    /// <summary>
    /// Creates a new <see cref="StateChangeListenerHandle"/> instance.
    /// This method sets up a native callback for state change events.
    /// </summary>
    /// <param name="listener">The callback to be invoked on state changes.</param>
    /// <returns>A handle to the created state change listener.</returns>
    /// <exception cref="Exception">If an error occurs during the creation of the handle.</exception>
    public static unsafe StateChangeListenerHandle Create(StateChangeListener listener)
    {
        var handle = GCHandle.Alloc(listener, GCHandleType.Normal);
        StateChangeListenerHandle? javaHandle = null;
        try
        {
            javaHandle = SafeCall(CurrentThread, Import.New(CurrentThread, &OnStateChanges, handle));
            javaHandle.RegisterFinalize(handle);
            return javaHandle;
        }
        catch (Exception)
        {
            javaHandle?.Dispose();
            handle.Free();
            throw;
        }
    }

    /// <summary>
    /// Native callback method for handling state changes. This method is invoked by native code.
    /// </summary>
    /// <param name="thread">Identifier of the native thread.</param>
    /// <param name="oldState">The state prior to the change.</param>
    /// <param name="newState">The current state after the change.</param>
    /// <param name="handle">Handle to the managed callback delegate.</param>
    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static void OnStateChanges(nint thread, State oldState, State newState, GCHandle handle)
    {
        if (!handle.IsAllocated)
        {
            return;
        }

        var listener = handle.Target as StateChangeListener;
        try
        {
            listener?.Invoke(oldState, newState);
        }
        catch (Exception e)
        {
            // ToDo Add log entry.
            Console.Error.WriteLine($"Exception in endpoint state change listener({listener?.Method}): {e}");
        }
    }

    /// <summary>
    /// Internal class containing the import definitions for native methods.
    /// The location of imported functions is in the header files <c>"dxfg_endpoint.h"</c>.
    /// </summary>
    private static class Import
    {
        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_PropertyChangeListener_new")]
        public static extern unsafe StateChangeListenerHandle New(
            nint thread,
            delegate* unmanaged[Cdecl]<nint, State, State, GCHandle, void> listener,
            GCHandle handle);
    }
}
