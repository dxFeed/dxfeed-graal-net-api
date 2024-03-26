// <copyright file="JavaException.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Runtime.Serialization;

namespace DxFeed.Graal.Net.Native.ErrorHandling;

/// <summary>
/// An exception class representing errors that occur within Java code during interop scenarios.
/// </summary>
/// <remarks>
/// This class encapsulates detailed information about Java exceptions,
/// including the class name, message, and stack trace from the originating Java exception.
/// </remarks>
[Serializable]
public sealed class JavaException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JavaException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="className">The class name of the Java exception.</param>
    /// <param name="stackTrace">The stack trace of the Java exception.</param>
    public JavaException(string? message, string? className, string? stackTrace)
        : base(CreateErrorMessage(message, className))
    {
        JavaClassName = className;
        JavaStackTrace = stackTrace;
    }

#if NET8_0_OR_GREATER
    [Obsolete("NET8_0_OR_GREATER", DiagnosticId = "SYSLIB0051")]
#endif
    private JavaException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        JavaClassName = info.GetString(nameof(JavaClassName));
        JavaStackTrace = info.GetString(nameof(JavaStackTrace));
    }

    /// <summary>
    /// Gets the class name of the Java exception.
    /// </summary>
    public string? JavaClassName { get; }

    /// <summary>
    /// Gets the stack trace of the Java exception.
    /// </summary>
    public string? JavaStackTrace { get; }

    /// <inheritdoc/>
    public override string StackTrace =>
        JavaStackTrace + base.StackTrace;

    /// <inheritdoc/>
#if NET8_0_OR_GREATER
    [Obsolete("NET8_0_OR_GREATER", DiagnosticId = "SYSLIB0051")]
#endif
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(JavaClassName), JavaClassName);
        info.AddValue(nameof(JavaStackTrace), JavaStackTrace);
    }

    private static string CreateErrorMessage(string? message, string? className) =>
        $"Java exception of type '{className}' was thrown. {message}";
}
