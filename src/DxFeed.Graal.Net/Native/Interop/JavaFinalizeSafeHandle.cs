// <copyright file="JavaFinalizeSafeHandle.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Native.ErrorHandling;

namespace DxFeed.Graal.Net.Native.Interop;

internal abstract class JavaFinalizeSafeHandle : JavaSafeHandle
{
    public void RegisterFinalize(object o) =>
        ErrorCheck.NativeCall(
            CurrentThread,
            NativeObjectFinalize(CurrentThread, handle, GCHandle.Alloc(o, GCHandleType.Weak)));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static void OnFinalize(nint thread, nint self) =>
        GCHandle.FromIntPtr(self).Free();

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        EntryPoint = "dxfg_Object_finalize")]
    private static extern int NativeObjectFinalize(
        nint thread,
        nint handle,
        GCHandle userData);
}
