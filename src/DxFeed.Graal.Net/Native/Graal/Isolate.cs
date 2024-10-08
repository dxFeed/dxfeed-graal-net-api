// <copyright file="Isolate.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Native.ErrorHandling;
using Microsoft.Extensions.Configuration;

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
internal sealed class Isolate
{
    /// <summary>
    /// Prefix used to search for properties in environment variables.
    /// </summary>
    private static readonly string ENV_PREFIX = "DXFEED_";

    /// <summary>
    /// Defines path to a file with system properties.
    /// When the path to this properties file not provided,
    /// the file "dxfeed.system.properties" loaded from current runtime directory.
    /// It means that the corresponding file can be placed into the current directory with any need
    /// to specify additional properties.
    /// </summary>
    private static readonly string DXFEED_SYSTEM_PROPERTIES = "dxfeed.system.properties";

    /// <summary>
    /// A singleton lazy-initialization instance of <see cref="Isolate"/>.
    /// </summary>
    private static readonly Lazy<Isolate> LazyInstance = new(CreateIsolate);

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
    /// <value>Returns singleton instance of <see cref="Isolate"/>.</value>
    public static Isolate Instance =>
        LazyInstance.Value;

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
    /// Creates a new <see cref="Isolate"/> and sets specified system properties.
    /// System properties will be retrieved either from the corresponding file or from environment variables.
    /// </summary>
    /// <returns>Returns <see cref="Isolate"/>.</returns>
    private static Isolate CreateIsolate()
    {
        ErrorCheck.SafeCall(GraalCreateIsolate(0, out var isolate, out var thread));
        var propsFile = Environment.GetEnvironmentVariable($"{ENV_PREFIX}{DXFEED_SYSTEM_PROPERTIES}")
                        ?? $"{DXFEED_SYSTEM_PROPERTIES}";
        var props = new ConfigurationBuilder()
            .AddIniFile(propsFile, true)
            .AddEnvironmentVariables(ENV_PREFIX) // overrides ini-file properties
            .Build();
        foreach (var kvp in props.AsEnumerable())
        {
            SystemProperty.SetProperty(thread, kvp.Key, kvp.Value ?? string.Empty);
        }

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
