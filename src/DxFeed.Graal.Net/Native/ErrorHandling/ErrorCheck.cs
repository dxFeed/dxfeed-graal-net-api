// <copyright file="ErrorCheck.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;

namespace DxFeed.Graal.Net.Native.ErrorHandling;

/// <summary>
/// Provides utility methods for error checking and exception handling in native function calls.
/// This class contains methods that verify the results of native calls
/// and throw appropriate exceptions if errors are detected.
/// </summary>
internal static class ErrorCheck
{
    /// <summary>
    /// Verifies the result of a native function call that returns an <see cref="int"/>.
    /// Throws <see cref="JavaException"/> if error occured.
    /// If no error is detected, the original result is returned.
    /// </summary>
    /// <param name="result">The result of the native function call.</param>
    /// <returns>The original result if no error occurred.</returns>
    /// <exception cref="JavaException">If the native call resulted in an error.</exception>
    /// <remarks>
    /// The method checks for the presence of an exception in the same thread from which it was called.
    /// If invoked from a different thread than the one where the result was obtained,
    /// it will not throw an exception (even if it was thrown on Java side), potentially leading to undefined behavior.
    /// </remarks>
    public static int SafeCall(int result)
    {
        if (result < 0)
        {
            ThrowIfJavaThreadExceptionExists();
        }

        return result;
    }

    /// <summary>
    /// Verifies the result of a native function call that returns a <see cref="long"/>.
    /// Throws <see cref="JavaException"/> if error occured.
    /// If no error is detected, the original result is returned.
    /// </summary>
    /// <param name="result">The result of the native function call.</param>
    /// <returns>The original result if no error occurred.</returns>
    /// <exception cref="JavaException">If the native call resulted in an error.</exception>
    /// <remarks>
    /// The method checks for the presence of an exception in the same thread from which it was called.
    /// If invoked from a different thread than the one where the result was obtained,
    /// it will not throw an exception (even if it was thrown on Java side), potentially leading to undefined behavior.
    /// </remarks>
    public static long SafeCall(long result)
    {
        if (result < 0)
        {
            ThrowIfJavaThreadExceptionExists();
        }

        return result;
    }

    /// <summary>
    /// Verifies the result of a native function call that returns a <see cref="SafeHandle"/>.
    /// Throws <see cref="JavaException"/> if error occured.
    /// If no error is detected, the original result is returned.
    /// </summary>
    /// <param name="result">The result of the native function call.</param>
    /// <typeparam name="T">The any type.</typeparam>
    /// <returns>The original result if no error occurred.</returns>
    /// <exception cref="JavaException">If the native call resulted in an error.</exception>
    /// <remarks>
    /// The method checks for the presence of an exception in the same thread from which it was called.
    /// If invoked from a different thread than the one where the result was obtained,
    /// it will not throw an exception (even if it was thrown on Java side), potentially leading to undefined behavior.
    /// </remarks>
    public static T SafeCall<T>(T result)
    {
        if (result is null or SafeHandle { IsInvalid: true })
        {
            ThrowIfJavaThreadExceptionExists();
        }

        return result;
    }

    /// <summary>
    /// Verifies the result of a native function call that returns a T* (unmanaged pointer type).
    /// Throws <see cref="JavaException"/> if error occured.
    /// If no error is detected, the original result is returned.
    /// </summary>
    /// <param name="result">The result of the native function call.</param>
    /// <typeparam name="T">The unmanaged pointer type.</typeparam>
    /// <returns>The original result if no error occurred.</returns>
    /// <exception cref="JavaException">If the native call resulted in an error.</exception>
    /// <remarks>
    /// The method checks for the presence of an exception in the same thread from which it was called.
    /// If invoked from a different thread than the one where the result was obtained,
    /// it will not throw an exception (even if it was thrown on Java side), potentially leading to undefined behavior.
    /// </remarks>
    public static unsafe T* SafeCall<T>(T* result)
        where T : unmanaged
    {
        if ((nint)result == 0)
        {
            ThrowIfJavaThreadExceptionExists();
        }

        return result;
    }

    /// <summary>
    /// Verifies the result of a GraalVM functions call.
    /// If an error is detected (non-zero result), a <see cref="GraalException"/> is thrown.
    /// </summary>
    /// <param name="result">The result code from the GraalVM function call.</param>
    /// <exception cref="GraalException">If the GraalVM call resulted in an error.</exception>
    public static void SafeCall(GraalErrorCode result)
    {
        if (result != GraalErrorCode.NoError)
        {
            ThrowGraalException(result);
        }
    }

    /// <summary>
    /// Checks for the existence of a Java exception on the current thread
    /// and throws a <see cref="JavaException"/> if one is found.
    /// </summary>
    /// <remarks>
    /// If a Java exception is present, it converts the Java exception to a .NET <see cref="JavaException"/>
    /// and throws it. This ensures that Java exceptions are properly propagated and handled in .NET code.
    /// </remarks>
    private static void ThrowIfJavaThreadExceptionExists() =>
        JavaExceptionHandle.ThrowIfJavaThreadExceptionExists();

    /// <summary>
    /// Throws a <see cref="GraalException"/> based on a specified error code.
    /// </summary>
    /// <param name="errorCode">The error code representing a specific GraalVM error.</param>
    /// <remarks>
    /// This method creates and throws a <see cref="GraalException"/> that encapsulates the provided GraalVM error code
    /// along with a descriptive message. It allows for consistent handling of GraalVM-related errors.
    /// </remarks>
    private static void ThrowGraalException(GraalErrorCode errorCode) =>
        throw new GraalException(errorCode);
}
