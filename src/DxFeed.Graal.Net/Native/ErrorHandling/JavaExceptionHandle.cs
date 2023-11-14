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
/// Represents a handle for Java exceptions in a .NET interop environment.
/// </summary>
/// <remarks>
/// This class is responsible for managing Java exception handles and converting them into
/// .NET exceptions for consistent error handling across the interop boundary. It leverages
/// native methods to interact with Java exceptions and provides functionality to translate
/// and throw these exceptions in .NET.
/// </remarks>
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Created by marshaller")]
internal sealed class JavaExceptionHandle : JavaHandle
{
    /// <summary>
    /// Checks for the existence of a Java exception on the current thread
    /// and throws a corresponding .NET exception if one is found.
    /// </summary>
    /// <exception cref="JavaException">Thrown when a Java exception is present on the current thread.</exception>
    public static void ThrowIfJavaExceptionExists()
    {
        using var exceptionHandle = Import.GetAndClearThreadException(CurrentThread);
        if (exceptionHandle.IsInvalid)
        {
            return;
        }

        var exceptionInfo = Marshal.PtrToStructure<JavaExceptionInfo>(exceptionHandle.handle);
        throw new JavaException(exceptionInfo.ClassName, exceptionInfo.Message, exceptionInfo.StackTrace);
    }

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
