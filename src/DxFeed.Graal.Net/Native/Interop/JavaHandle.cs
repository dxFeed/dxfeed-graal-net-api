// <copyright file="JavaHandle.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
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
/// within the context of the GraalVM Native Image.
/// <br/>
/// Inheritors must override the <see cref="Release"/> method if special cleanup logic is required.
/// The common cleanup logic is described in the <see cref="ReleaseHandle"/> method.
/// </summary>
/// <remarks>
/// This class is designed to help manage Java objects when working with native interop scenarios.
/// The derived classes should implement specific logic related to their respective Java object types,
/// and can rely on this base class for basic handle lifecycle management.
/// </remarks>
internal abstract class JavaHandle : SafeHandle
{
    private static readonly Delegate OnFinalizeDelegate = new OnFinalizeDelegateType(OnFinalize);

    /// <summary>
    /// Initializes a new instance of the <see cref="JavaHandle"/> class.
    /// By default, this class assumes ownership of the handle.
    /// </summary>
    /// <param name="isOwnHandle">
    /// <c>true</c> to reliably let release the handle during the finalization phase; otherwise, <c>false</c>.
    /// </param>
    protected JavaHandle(bool isOwnHandle = true)
        : base(IntPtr.Zero, isOwnHandle)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JavaHandle"/> class with a specified handle value.
    /// By its default setting, this class assumes ownership of the handle.
    /// </summary>
    /// <param name="handle">The handle value to set.</param>
    /// <param name="isOwnHandle">
    /// <c>true</c> to reliably let release the handle during the finalization phase; otherwise, <c>false</c>.
    /// </param>
    protected JavaHandle(IntPtr handle, bool isOwnHandle = true)
        : base(IntPtr.Zero, isOwnHandle) =>
        SetHandle(handle);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void OnFinalizeDelegateType(IntPtr thread, GCHandle netHandle);

    /// <summary>
    /// Gets a value indicating whether the handle is invalid.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the handle is considered invalid (i.e., has a value of zero); otherwise, <c>false</c>.
    /// </returns>
    public override bool IsInvalid =>
        handle == IntPtr.Zero;

    /// <inheritdoc cref="IsolateThread.CurrentThread"/>
    protected static IsolateThread CurrentThread =>
        IsolateThread.CurrentThread;

    /// <summary>
    /// Creates a Java handle and registers it for finalization, along with a .NET object's GCHandle.
    /// </summary>
    /// <typeparam name="T">The type of Java handle to create, inheriting from <see cref="JavaHandle"/>.</typeparam>
    /// <param name="obj">The .NET object to associate with the Java handle for finalization purposes.</param>
    /// <param name="create">A function that takes a <see cref="GCHandle"/> and returns an instance of the Java handle.</param>
    /// <returns>An instance of the Java handle of type <typeparamref name="T"/>.</returns>
    /// <remarks>
    /// This method allocates a <see cref="GCHandle"/> for the provided .NET object and uses the <paramref name="create"/>
    /// function to create an instance of the Java handle. It then registers the handle for finalization in the native layer.
    /// If an exception occurs during the process, any allocated resources are properly disposed of.
    /// </remarks>
    /// <exception cref="Exception">Throws an exception if the handle creation or registration process fails.</exception>
    protected static T CreateAndRegisterFinalize<T>(object obj, Func<GCHandle, T> create)
        where T : JavaHandle
    {
        var netHandle = GCHandle.Alloc(obj, GCHandleType.Normal);
        T? javaHandle = null;
        try
        {
            javaHandle = create(netHandle);
            ErrorCheck.SafeCall(Import.RegisterFinalize(CurrentThread, javaHandle, OnFinalizeDelegate, netHandle));
            return javaHandle;
        }
        catch (Exception)
        {
            javaHandle?.Dispose();
            netHandle.Free();
            throw;
        }
    }

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
            Console.Error.WriteLine($"Exception in {GetType().Name} when the resource is released: {e}");
        }

        return false;
    }

    /// <summary>
    /// Releases the associated resources, performing error checks against the native call.
    /// This method can be overridden by derived classes to provide specialized release logic.
    /// </summary>
    /// <exception cref="JavaException"> If error occured.</exception>
    protected virtual void Release() =>
        ErrorCheck.SafeCall(Import.Release(CurrentThread, handle));

    /// <summary>
    /// Registers the provided .NET <see cref="GCHandle"/> object for finalization
    /// in tandem with the underlying Java object.
    /// </summary>
    /// <param name="netHandle">The .NET <see cref="GCHandle"/> object to register for finalization.</param>
    /// <remarks>
    /// This method uses the provided <see cref="GCHandle"/> (which wraps a .NET object)
    /// and links it with the Java object for coordinated cleanup. As the Java object undergoes finalization,
    /// the associated .NET object will be deregistered, and its handle will be released.
    /// </remarks>
    /// <seealso cref="OnFinalize"/>
    protected void RegisterFinalize(GCHandle netHandle) =>
        ErrorCheck.SafeCall(Import.RegisterFinalize(CurrentThread, this, OnFinalizeDelegate, netHandle));

    /// <summary>
    /// Handles the finalization callback from the native side.
    /// </summary>
    /// <param name="thread">The native thread context.</param>
    /// <param name="netHandle">
    /// The .NET <see cref="GCHandle"/> object associated with the Java object being finalized.
    /// </param>
    private static void OnFinalize(nint thread, GCHandle netHandle)
    {
        if (netHandle.IsAllocated)
        {
            netHandle.Free();
        }
    }

    private static class Import
    {
        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_JavaObjectHandler_release")]
        public static extern int Release(
            nint thread,
            nint handle);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_Object_finalize")]
        public static extern int RegisterFinalize(
            nint thread,
            JavaHandle javaHandle,
            Delegate listener,
            GCHandle netHandle);
    }
}
