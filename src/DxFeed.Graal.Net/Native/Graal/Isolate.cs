// <copyright file="Isolate.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Native.ErrorHandling;

namespace DxFeed.Graal.Net.Native.Graal;

/// <summary>
/// Represents an isolate (a JVM instance).
/// Contains a handle that is a pointer to the runtime data structure for the isolate.
/// <br/>
/// This project intentionally uses only one isolate instance for the following reasons:
/// <ul>
/// <li>
/// Old open issue on GitHub associated with a memory leak.
/// <a href="https://github.com/oracle/graal/issues/3474">Memory Leak on graal_isolatethread_t</a>.
/// </li>
/// <li>
/// There are no business cases associated with the creation of multiple isolates.
/// </li>
/// <li>
/// User error-prone logic when mixing multiple isolates.
/// </li>
/// </ul>
/// Each native method is associated with a specific <see cref="Isolate"/>,
/// and takes an <see cref="IsolateThread"/> as its first argument.
/// </summary>
internal class Isolate
{
    /// <summary>
    /// A singleton lazy-initialization instance of <see cref="Isolate"/>.
    /// </summary>
    private static IntPtr _vmOptionsPtr = IntPtr.Zero;

    /// <summary>
    /// A singleton lazy-initialization instance of <see cref="Isolate"/>.
    /// </summary>
    private static Lazy<Isolate>? _lazyInstance;

    /// <summary>
    /// Opaque pointer to the runtime data structure for an isolate.
    /// </summary>
    private readonly nint _isolateHandle;

    /// <summary>
    /// Initializes a new instance of the <see cref="Isolate"/> class with the specified isolate handle.
    /// </summary>
    /// <param name="isolateHandle">
    /// The specified isolate handle obtained using <see cref="GraalCreateIsolate"/>.
    /// </param>
    private Isolate(nint isolateHandle) =>
        _isolateHandle = isolateHandle;

    /// <summary>
    /// Gets a default application-wide singleton instance of <see cref="Isolate"/>.
    /// </summary>
    /// <returns>Returns singleton instance of <see cref="Isolate"/>.</returns>
    public static Isolate Instance()
    {
        _lazyInstance ??= new Lazy<Isolate>(CreateIsolate());
        return _lazyInstance.Value;
    }

    /// <summary>
    /// Gets the <see cref="IsolateThread"/> associated with the <see cref="Isolate"/> singleton
    /// and attached to the current managed thread.
    /// The <see cref="IsolateThread"/> is a thread-local data structure.
    /// Therefore, the <see cref="IsolateThread"/> must not be shared between threads.
    /// The resulting isolate can be passed into native function (with implicit conversion).
    /// It's the same as <see cref="IsolateThread"/>.<see cref="IsolateThread.CurrentThread"/>.
    /// </summary>
    /// <value>The <see cref="IsolateThread"/>.</value>
    public static IsolateThread CurrentThread =>
        IsolateThread.CurrentThread;

    /// <summary>
    /// Implicit conversions operator to pass <see cref="Isolate"/> to native calls.
    /// Simplifies interaction with native code.
    /// </summary>
    /// <param name="value">The <see cref="Isolate"/> value.</param>
    /// <returns>Returns isolate handle.</returns>
    public static implicit operator nint(Isolate value) =>
        value._isolateHandle;

    /// <summary>
    /// Sets <see cref="VmOptions"/> to create <see cref="Isolate"/>.
    /// </summary>
    /// <param name="vmOptions">
    /// The specified isolate handle obtained using <see cref="VmOptions"/>
    /// </param>
    public static void SetVmOptions(IntPtr vmOptions) => _vmOptionsPtr = vmOptions;

    /// <summary>
    /// Creates a new <see cref="Isolate"/> without parameters.
    /// The attached isolated thread is ignored.
    /// </summary>
    /// <returns>Returns <see cref="Isolate"/>.</returns>
    private static Isolate CreateIsolate()
    {
        var vmOptionsPtr = _vmOptionsPtr;
        ErrorCheck.GraalCall(GraalCreateIsolate(vmOptionsPtr, out var isolate, out _));
        return new Isolate(isolate);
    }

    /// <summary>
    /// Create a new isolate, considering the passed parameters (which may be NULL).
    /// On success, the current thread is attached to the created isolate, and the
    /// address of the isolate and the isolate thread are written to the passed pointers
    /// if they are not NULL.
    /// </summary>
    /// <param name="param">The specified parameters.</param>
    /// <param name="isolate">The created isolate.</param>
    /// <param name="isolateThread">The created and attached isolate thread.</param>
    /// <returns>Returns 0 on success, or a non-zero value on failure.</returns>
    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        EntryPoint = "graal_create_isolate")]
    private static extern GraalErrorCode GraalCreateIsolate(
        nint param,
        out nint isolate,
        out nint isolateThread);
}
