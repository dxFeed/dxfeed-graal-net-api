// <copyright file="StateChangeListenerHandle.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Native.Interop;
using static DxFeed.Graal.Net.Api.DXEndpoint;
using static DxFeed.Graal.Net.Native.ErrorHandling.ErrorCheck;

namespace DxFeed.Graal.Net.Native.Endpoint;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Created by marshaler")]
internal sealed class StateChangeListenerHandle : JavaHandle
{
    private static readonly Delegate OnStateChangesDelegate = new OnStateChangesDelegateType(OnStateChanges);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void OnStateChangesDelegateType(IntPtr thread, State oldState, State newState, GCHandle handle);

    public static StateChangeListenerHandle Create(StateChangeListener listener) =>
        CreateAndRegisterFinalize(listener, handle => SafeCall(Import.New(CurrentThread, OnStateChangesDelegate, handle)));

    private static void OnStateChanges(IntPtr thread, State oldState, State newState, GCHandle handle)
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
            Console.Error.WriteLine($"Exception in user {nameof(StateChangeListener)}. {e}");
        }
    }

    private static class Import
    {
        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_PropertyChangeListener_new")]
        public static extern StateChangeListenerHandle New(
            nint thread,
            Delegate listener,
            GCHandle handle);
    }
}
