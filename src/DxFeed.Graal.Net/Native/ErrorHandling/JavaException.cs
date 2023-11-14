// <copyright file="JavaException.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Runtime.Serialization;

namespace DxFeed.Graal.Net.Native.ErrorHandling;

/// <summary>
/// Exception class that represents errors occurring within Java code in interop scenarios.
/// </summary>
/// <remarks>
/// This class encapsulates detailed information about Java exceptions,
/// including the class name, message, and stack trace of the original Java exception.
/// It is utilized in .NET environments to represent Java exceptions in a structured and informative manner.
/// </remarks>
[Serializable]
public sealed class JavaException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JavaException"/> class.
    /// </summary>
    /// <param name="javaClassName">The class name of the Java exception.</param>
    /// <param name="javaMessage">The message of the Java exception.</param>
    /// <param name="javaStackTrace">The stack trace of the Java exception.</param>
    public JavaException(string? javaClassName, string? javaMessage, string? javaStackTrace)
    {
        JavaClassName = javaClassName;
        JavaMessage = javaMessage;
        JavaStackTrace = javaStackTrace;
    }

    private JavaException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        JavaClassName = info.GetString(nameof(JavaClassName));
        JavaMessage = info.GetString(nameof(JavaMessage));
        JavaStackTrace = info.GetString(nameof(JavaStackTrace));
    }

    /// <summary>
    /// Gets the class name of the Java exception.
    /// </summary>
    public string? JavaClassName { get; }

    /// <summary>
    /// Gets the message of the Java exception.
    /// </summary>
    public string? JavaMessage { get; }

    /// <summary>
    /// Gets the stack trace of the Java exception.
    /// </summary>
    public string? JavaStackTrace { get; }

    /// <inheritdoc/>
    public override string StackTrace =>
     JavaStackTrace + base.StackTrace;

    /// <inheritdoc/>
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(JavaClassName), JavaClassName);
        info.AddValue(nameof(JavaMessage), JavaMessage);
        info.AddValue(nameof(JavaStackTrace), JavaStackTrace);
    }
}
