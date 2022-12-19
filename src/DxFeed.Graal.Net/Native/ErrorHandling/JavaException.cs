// <copyright file="JavaException.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Runtime.Serialization;
using System.Text;

namespace DxFeed.Graal.Net.Native.ErrorHandling;

/// <summary>
/// Represents errors that occur inside Java code.
/// </summary>
[Serializable]
public sealed class JavaException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JavaException"/> class.
    /// </summary>
    /// <param name="javaClassName">The Java exception class name.</param>
    /// <param name="javaMessage">The Java exception message.</param>
    /// <param name="javaStackTrace">The Java stack trace.</param>
    public JavaException(string? javaClassName, string? javaMessage, string? javaStackTrace)
    {
        JavaClassName = javaClassName;
        JavaMessage = javaMessage;
        JavaStackTrace = javaStackTrace;
    }

    private JavaException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

    /// <summary>
    /// Gets Java exception class name.
    /// </summary>
    public string? JavaClassName { get; }

    /// <summary>
    /// Gets Java exception message.
    /// </summary>
    public string? JavaMessage { get; }

    /// <summary>
    /// Gets Java stack trace.
    /// </summary>
    public string? JavaStackTrace { get; }

    /// <summary>
    /// Gets a message that describes the current exception.
    /// </summary>
    /// <returns>The error message that explains the reason for the exception.</returns>
    public override string Message
    {
        get
        {
            var message = new StringBuilder();
            message.AppendLine();
            message.AppendLine("Java Class: " + JavaClassName);
            message.AppendLine("Java Message: " + JavaMessage);
            message.AppendLine("Java StackTrace: " + JavaStackTrace);
            return message.ToString();
        }
    }
}
