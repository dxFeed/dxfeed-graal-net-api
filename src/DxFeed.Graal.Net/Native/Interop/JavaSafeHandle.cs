// <copyright file="JavaSafeHandle.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Native.ErrorHandling;
using DxFeed.Graal.Net.Native.Graal;

namespace DxFeed.Graal.Net.Native.Interop;

/// <summary>
/// Provides an abstract base class for Java-related handles within a .NET interop environment.
/// It is designed to manage Java object handles and ensure their proper cleanup and disposal,
/// especially within the context of the GraalVM Native Image.
/// <br/>
/// Inheritors must override the <see cref="Release"/> method if special cleanup logic is required.
/// The common cleanup logic is described in the <see cref="ReleaseHandle"/> method.
/// </summary>
/// <remarks>
/// This class is designed to help manage Java objects when working with native interop scenarios.
/// The derived classes should implement specific logic related to their respective Java object types,
/// and can rely on this base class for basic handle lifecycle management.
/// </remarks>
internal abstract class JavaSafeHandle : SafeHandleZeroIsInvalid
{
    /// <inheritdoc cref="IsolateThread.CurrentThread"/>
    protected static IsolateThread CurrentThread =>
        IsolateThread.CurrentThread;

    /// <summary>
    /// Releases the handle ensuring associated Java resources are properly disposed of.
    /// </summary>
    /// <returns><c>true</c> if the handle was successfully released; otherwise, <c>false</c>.</returns>
    protected override bool ReleaseHandle()
    {
        try
        {
            Release();
            SetHandle(IntPtr.Zero);
            return true;
        }
        catch (Exception e)
        {
            // ToDo Add a log entry.
            Console.Error.WriteLine($"Exception in {GetType().Name} when releasing resource: {e}");
        }

        return false;
    }

    /// <summary>
    /// Releases the associated resources, performing error checks against the native call.
    /// This method can be overridden by derived classes to provide specialized release logic.
    /// </summary>
    /// <exception cref="JavaException"> If error occured.</exception>
    protected virtual void Release() =>
        ErrorCheck.NativeCall(CurrentThread, NativeRelease(CurrentThread, handle));

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        EntryPoint = "dxfg_JavaObjectHandler_release")]
    private static extern int NativeRelease(
        nint thread,
        nint handle);
}
