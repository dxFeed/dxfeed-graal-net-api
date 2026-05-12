// <copyright file="InPlaceExecutor.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Native.ErrorHandling;
using DxFeed.Graal.Net.Native.Graal;

namespace DxFeed.Graal.Net.Native.Executors;

/// <summary>
/// In-process executor that drains pending native tasks on the current Graal isolate thread.
/// Used with <see cref="Api.DXEndpoint"/> (for example in the <see cref="Api.DXEndpoint.Role.LocalHub"/> role)
/// for deterministic unit tests and advanced hosting scenarios.
/// </summary>
public sealed class InPlaceExecutor : SafeHandle
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InPlaceExecutor"/> class for interop marshalling.
    /// </summary>
    /// <remarks>
    /// Prefer <see cref="Create"/> instead of constructing this type directly.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public InPlaceExecutor()
        : base(IntPtr.Zero, true)
    {
    }

    /// <inheritdoc />
    public override bool IsInvalid =>
        handle == IntPtr.Zero;

    /// <summary>
    /// Creates a new executor bound to the current isolate thread.
    /// </summary>
    /// <returns>A new executor instance.</returns>
    public static InPlaceExecutor Create() =>
        ErrorCheck.SafeCall(Import.Create(IsolateThread.CurrentThread));

    /// <summary>
    /// Runs all tasks queued on this executor until the queue is drained.
    /// </summary>
    public void ProcessAllPendingTasks() =>
        ErrorCheck.SafeCall(Import.ProcessAllPendingTasks(IsolateThread.CurrentThread, this));

    /// <inheritdoc />
    protected override bool ReleaseHandle()
    {
        try
        {
            ErrorCheck.SafeCall(Import.Release(IsolateThread.CurrentThread, handle));
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

    private static class Import
    {
        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_ExecutorBaseOnConcurrentLinkedQueue_new")]
        public static extern InPlaceExecutor Create(nint thread);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_ExecutorBaseOnConcurrentLinkedQueue_processAllPendingTasks")]
        public static extern InPlaceExecutor ProcessAllPendingTasks(nint thread, InPlaceExecutor executor);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_JavaObjectHandler_release")]
        public static extern int Release(
            nint thread,
            nint handle);
    }
}
