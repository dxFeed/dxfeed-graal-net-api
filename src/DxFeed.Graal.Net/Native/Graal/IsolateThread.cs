// <copyright file="IsolateThread.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Native.ErrorHandling;
using DxFeed.Graal.Net.Native.Interop;

namespace DxFeed.Graal.Net.Native.Graal;

/// <summary>
/// Represents a thread that is attached to an isolate.
/// Contains a handle that is a pointer to such a structure
/// that can be passed to an entry point as an execution context,
/// requiring the calling thread to be attached to this isolate.
/// </summary>
internal sealed class IsolateThread
{
    /// <summary>
    /// Thread exit callback.
    /// Use static field to make sure that delegate is alive.
    /// </summary>
    private static readonly ThreadExitManager.ThreadExitCallback OnThreadExit = DetachIsolateThreadOnThreadExit;

    /// <summary>
    /// Registered thread exit id callback.
    /// </summary>
    private static readonly int ThreadExitCallbackId = ThreadExitManager.RegisterCallback(OnThreadExit);

    /// <summary>
    /// Opaque pointer to such a structure can be passed to an entry point as the execution context,
    /// requiring that the calling thread has been attached to that isolate.
    /// </summary>
    private readonly nint _isolateThreadHandle;

    /// <summary>
    /// Initializes a new instance of the <see cref="IsolateThread"/> class
    /// with specified thread handle.
    /// </summary>
    /// <param name="isolateThreadHandle">
    /// The specified isolate thread handle obtained using <see cref="GraalAttachThread"/>.
    /// </param>
    private IsolateThread(nint isolateThreadHandle) =>
        _isolateThreadHandle = isolateThreadHandle;

    /// <summary>
    /// Gets the <see cref="IsolateThread"/> associated with the <see cref="Isolate"/> singleton
    /// and attached to the current managed thread.
    /// The <see cref="IsolateThread"/> is a thread-local data structure.
    /// Therefore, the <see cref="IsolateThread"/> must not be shared between threads.
    /// The resulting isolate can be passed into native function (with implicit conversion).
    /// It's the same as <see cref="Isolate"/>.<see cref="Isolate.CurrentThread"/>.
    /// </summary>
    /// <value>The <see cref="IsolateThread"/>.</value>
    public static IsolateThread CurrentThread =>
        GetOrAttachIsolateThread(Isolate.Instance);

    /// <summary>
    /// Implicit conversions operator to pass <see cref="IsolateThread"/> to native calls.
    /// Simplifies interaction with native code.
    /// </summary>
    /// <param name="value">The <see cref="IsolateThread"/> value.</param>
    /// <returns>Returns isolate thread handle.</returns>
    public static implicit operator nint(IsolateThread value) =>
        value._isolateThreadHandle;

    /// <summary>
    /// Gets or attaches current thread to specified isolate.
    /// If an isolate thread has been attached, it will be detached before the current thread exit.
    /// </summary>
    /// <param name="isolate">The <see cref="Isolate"/>, to which the thread will be attached.</param>
    /// <returns>Returns created <see cref="IsolateThread"/>.</returns>
    private static IsolateThread GetOrAttachIsolateThread(Isolate isolate)
    {
        var threadHandle = GraalGetCurrentThread(isolate);
        if (threadHandle == 0)
        {
            // Attach a thread only when the thread has not yet been attached.
            ErrorCheck.SafeCall(GraalAttachThread(isolate, out threadHandle));

            // Enable thread exit callback with current handle.
            ThreadExitManager.EnableCallbackForCurrentThread(ThreadExitCallbackId, threadHandle);
        }

        return new IsolateThread(threadHandle);
    }

    /// <summary>
    /// Detaches isolate thread. Called before exiting the current thread.
    /// </summary>
    /// <param name="isolateThreadHandle">
    /// The specified isolate thread handle obtained using <see cref="GraalAttachThread"/>.
    /// </param>
    private static void DetachIsolateThreadOnThreadExit(nint isolateThreadHandle)
    {
        try
        {
            if (isolateThreadHandle == 0)
            {
                return;
            }

            ErrorCheck.SafeCall(GraalDetachThread(isolateThreadHandle));
        }
        catch (Exception e)
        {
            // ToDo Add a log entry.
            Console.Error.WriteLine($"Exception in IsolateThread when detaching thread: {e}");
        }
    }

    /// <summary>
    /// Attaches the current thread to the passed isolate.
    /// On success, writes the address of the
    /// created isolate thread structure to the passed pointer and returns 0.
    /// If the thread has already been attached, the call succeeds and also provides
    /// the thread's isolate thread structure.
    /// </summary>
    /// <param name="isolate">The passed isolate.</param>
    /// <param name="isolateThread">The created isolate thread.</param>
    /// <returns>Returns 0 on success, or a non-zero value on failure.</returns>
    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        EntryPoint = "graal_attach_thread")]
    private static extern GraalErrorCode GraalAttachThread(
        nint isolate,
        out nint isolateThread);

    /// <summary>
    /// Gets an isolate to which the current thread is attached, returns the address of
    /// the thread's associated isolate thread structure. If the current thread is not
    /// attached to the passed isolate or if another error occurs, returns NULL.
    /// </summary>
    /// <param name="isolate">The passed isolate.</param>
    /// <returns>Returns isolateThread on success, or a null if current thread is not attached.</returns>
    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        EntryPoint = "graal_get_current_thread")]
    private static extern nint GraalGetCurrentThread(nint isolate);

    /// <summary>
    /// Detaches the passed isolate thread from its isolate and discards any state or
    /// context that is associated with it. At the time of the call, no code may still
    /// be executing in the isolate thread's context.
    /// </summary>
    /// <param name="isolateThread">The passed isolate thread.</param>
    /// <returns>Returns 0 on success, or a non-zero value on failure.</returns>
    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        EntryPoint = "graal_detach_thread")]
    private static extern GraalErrorCode GraalDetachThread(nint isolateThread);
}
