// <copyright file="ErrorCheck.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;

namespace DxFeed.Graal.Net.Native.ErrorHandling;

/// <summary>
/// Provides utility methods for error checking and exception handling of native calls.
/// This class ensures that any errors occurring in native code are properly propagated
/// and handled in the .NET environment.
/// </summary>
internal static class ErrorCheck
{
    /// <summary>
    /// Verifies the result of a native function call that returns an <see cref="int"/>.
    /// Throws <see cref="JavaException"/> if error occured.
    /// If there is no error, the result will be transmitted transparently.
    /// </summary>
    /// <param name="result">The result of the native function call.</param>
    /// <returns>The original result if no error occurred.</returns>
    /// <exception cref="JavaException">If the native call resulted in an error.</exception>
    public static int SafeCall(int result)
    {
        if (result < 0)
        {
            ThrowIfJavaExceptionExists();
        }

        return result;
    }

    /// <summary>
    /// Verifies the result of a native function call that returns a <see cref="long"/>.
    /// Throws <see cref="JavaException"/> if error occured.
    /// If there is no error, the result will be transmitted transparently.
    /// </summary>
    /// <param name="result">The result of the native function call.</param>
    /// <returns>The original result if no error occurred.</returns>
    /// <exception cref="JavaException">If the native call resulted in an error.</exception>
    public static long SafeCall(long result)
    {
        if (result < 0)
        {
            ThrowIfJavaExceptionExists();
        }

        return result;
    }

    /// <summary>
    /// Verifies the result of a native function call that returns a <see cref="SafeHandle"/>.
    /// Throws <see cref="JavaException"/> if error occured.
    /// If there is no error, the result will be transmitted transparently.
    /// </summary>
    /// <param name="result">The result of the native function call.</param>
    /// <typeparam name="T">The type of <see cref="SafeHandle"/>.</typeparam>
    /// <returns>The original result if no error occurred.</returns>
    /// <exception cref="JavaException">If the native call resulted in an error.</exception>
    public static T SafeCall<T>(T result)
    where T : SafeHandle
    {
        if (result.IsInvalid)
        {
            ThrowIfJavaExceptionExists();
        }

        return result;
    }

    /// <summary>
    /// Verifies the result of a native function call that returns a <see cref="SafeHandle"/>.
    /// Throws <see cref="JavaException"/> if error occured.
    /// If there is no error, the result will be transmitted transparently.
    /// </summary>
    /// <param name="result">The result of the native function call.</param>
    /// <typeparam name="T">The unmanaged pointer type.</typeparam>
    /// <returns>The original result if no error occurred.</returns>
    /// <exception cref="JavaException">If the native call resulted in an error.</exception>
    public static unsafe T* SafeCall<T>(T* result)
        where T : unmanaged
    {
        if ((nint)result == 0)
        {
            ThrowIfJavaExceptionExists();
        }

        return result;
    }

    /// <summary>
    /// Verifies the result of a GraalVM functions call.
    /// If an error is detected (non-zero result), a <see cref="GraalException"/> is thrown.
    /// </summary>
    /// <param name="result">The result code from the Graal function call.</param>
    /// <exception cref="GraalException">If the GraalVM call resulted in an error.</exception>
    public static void SafeCall(GraalErrorCode result)
    {
        if (result != GraalErrorCode.NoError)
        {
            ThrowGraalException(result);
        }
    }

    private static void ThrowIfJavaExceptionExists() =>
        JavaExceptionHandle.ThrowIfJavaExceptionExists();

    private static void ThrowGraalException(GraalErrorCode code) =>
        throw new GraalException(code);
}
