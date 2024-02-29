// <copyright file="JavaExceptionHandle.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Native.Interop;

namespace DxFeed.Graal.Net.Native.ErrorHandling;

/// <summary>
/// Represents a handle to a Java exception object.
/// </summary>
/// <remarks>
/// This class provides functionality for managing Java exception objects
/// when interacting with Java code from .NET. It encapsulates native methods to interact with
/// Java exceptions, allowing .NET code to properly handle these exceptions.
/// The class also includes methods to check for existing Java exceptions in the current thread,
/// retrieve and clear them, and convert them to .NET exceptions for seamless error handling
/// across the interop boundary.
/// </remarks>
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Created by marshaller")]
internal sealed class JavaExceptionHandle : JavaHandle
{
    /// <summary>
    /// Checks for the existence of a Java exception on the current thread
    /// and throws a <see cref="JavaException"/> if one is found.
    /// </summary>
    /// <exception cref="JavaException">If a Java exception is present on the current thread.</exception>
    public static void ThrowIfJavaThreadExceptionExists()
    {
        using var exceptionHandle = Import.GetAndClearThreadException(CurrentThread);
        if (exceptionHandle.IsInvalid)
        {
            return;
        }

        exceptionHandle.ThrowException();
    }

    public void ThrowException()
    {
        var exceptionInfo = Marshal.PtrToStructure<JavaExceptionInfo>(handle);
        throw new JavaException(exceptionInfo.Message, exceptionInfo.ClassName, exceptionInfo.StackTrace);
    }

    /// <inheritdoc/>
    protected override void Release() =>
        Import.Release(CurrentThread, handle);

    [StructLayout(LayoutKind.Sequential)]
    private readonly record struct JavaExceptionInfo(
        [MarshalAs(UnmanagedType.LPUTF8Str)] string? ClassName,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string? Message,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string? StackTrace);

    /// <summary>
    /// Internal class containing the import definitions for native methods.
    /// The location of imported functions is in the header files <c>"dxfg_catch_exception.h"</c>.
    /// </summary>
    private static class Import
    {
        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "dxfg_Exception_release")]
        public static extern void Release(nint thread, nint exception);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "dxfg_get_and_clear_thread_exception_t")]
        public static extern JavaExceptionHandle GetAndClearThreadException(nint thread);
    }
}
