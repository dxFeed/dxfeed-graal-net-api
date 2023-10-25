// <copyright file="ErrorCheck.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;

namespace DxFeed.Graal.Net.Native.ErrorHandling;

/// <summary>
/// Utility class for check native calls.
/// The location of the imported functions is in the header files <c>"dxfg_catch_exception.h"</c>.
/// </summary>
internal static class ErrorCheck
{
    /// <summary>
    /// Checks for a call to a native function that returns an <see cref="int"/>.
    /// Throws <see cref="JavaExceptionInfo"/> if error occured.
    /// If there is no error, the result will be transmitted transparently.
    /// </summary>
    /// <param name="thread">The current isolate thread.</param>
    /// <param name="result">The resul of native call.</param>
    /// <returns>The passed <see cref="int"/> result of the native call.</returns>
    /// <exception cref="JavaException">If error occured.</exception>
    public static int NativeCall(nint thread, int result)
    {
        if (result < 0)
        {
            ThrowThreadJavaExceptionIfExist(thread);
        }

        return result;
    }

    /// <summary>
    /// Checks for a call to a native function that returns an <see cref="long"/>.
    /// Throws <see cref="JavaExceptionInfo"/> if error occured.
    /// If there is no error, the result will be transmitted transparently.
    /// </summary>
    /// <param name="thread">The current isolate thread.</param>
    /// <param name="result">The resul of native call.</param>
    /// <returns>The passed <see cref="long"/> result of the native call.</returns>
    /// <exception cref="JavaException">If error occured.</exception>
    public static long NativeCall(nint thread, long result)
    {
        if (result < 0)
        {
            ThrowThreadJavaExceptionIfExist(thread);
        }

        return result;
    }

    public static T NativeCall<T>(nint thread, T result)
    where T : SafeHandle
    {
        if (result.IsInvalid)
        {
            ThrowThreadJavaExceptionIfExist(thread);
        }

        return result;
    }

    /// <summary>
    /// Checks for a call to a native function that returns an T* (unmanaged pointer type).
    /// Throws <see cref="JavaExceptionInfo"/> if error occured.
    /// If there is no error, the result will be transmitted transparently.
    /// </summary>
    /// <param name="thread">The current isolate thread.</param>
    /// <param name="result">The resul of native call.</param>
    /// <typeparam name="T">The unmanaged pointer type.</typeparam>
    /// <returns>The passed T* result of the native call.</returns>
    /// <exception cref="JavaException">If error occured.</exception>
    public static unsafe T* NativeCall<T>(nint thread, T* result)
        where T : unmanaged
    {
        if ((nint)result == 0)
        {
            ThrowThreadJavaExceptionIfExist(thread);
        }

        return result;
    }

    /// <summary>
    /// Checks for graal function call.
    /// Throws <see cref="GraalException"/> if error occured.
    /// </summary>
    /// <param name="result">The result of graal call.</param>
    /// <exception cref="GraalException">If error occured.</exception>
    public static void GraalCall(GraalErrorCode result)
    {
        if (result != GraalErrorCode.NoError)
        {
            ThrowGraalException(result);
        }
    }

    private static void ThrowThreadJavaExceptionIfExist(nint thread)
    {
        var exceptionInfo = ExceptionImport.GetAndClearThreadException(thread);
        if (exceptionInfo != null)
        {
            ThrowJavaException((JavaExceptionInfo)exceptionInfo);
        }
    }

    private static void ThrowJavaException(JavaExceptionInfo javaExceptionInfo) =>
        throw new JavaException(javaExceptionInfo.ClassName, javaExceptionInfo.Message, javaExceptionInfo.StackTrace);

    private static void ThrowGraalException(GraalErrorCode code) =>
        throw new GraalException(code);

    /// <summary>
    /// Contains imported functions from native code.
    /// </summary>
    private static class ExceptionImport
    {
        /// <summary>
        /// Gets current Java exception associated with current thread.
        /// </summary>
        /// <param name="thread">The pointer to a run-time data structure for the thread.</param>
        /// <returns>Returns <see cref="JavaExceptionInfo"/> or null, if not exception thrown.</returns>
        public static JavaExceptionInfo? GetAndClearThreadException(nint thread)
        {
            var exceptionInfoPtr = NativeGetAndClearThreadException(thread);
            if (exceptionInfoPtr == 0)
            {
                return null;
            }

            try
            {
                return Marshal.PtrToStructure<JavaExceptionInfo>(exceptionInfoPtr);
            }
            finally
            {
                NativeExceptionRelease(thread, exceptionInfoPtr);
            }
        }

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "dxfg_get_and_clear_thread_exception_t")]
        private static extern nint NativeGetAndClearThreadException(nint thread);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "dxfg_Exception_release")]
        private static extern void NativeExceptionRelease(nint thread, nint exception);
    }
}
