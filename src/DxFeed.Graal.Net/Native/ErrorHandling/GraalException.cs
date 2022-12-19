// <copyright file="GraalException.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text;

namespace DxFeed.Graal.Net.Native.ErrorHandling;

/// <summary>
/// Represents errors that occur when calling graal function.
/// </summary>
[Serializable]
public sealed class GraalException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GraalException"/> class.
    /// </summary>
    /// <param name="errorCode">The graal error code.</param>
    public GraalException(GraalErrorCode errorCode)
    {
        ErrorCode = errorCode;
        GraalMessage = GetGraalErrorDescription(errorCode);
    }

    private GraalException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

    /// <summary>
    /// Gets graal error message.
    /// </summary>
    public string? GraalMessage { get; }

    /// <summary>
    /// Gets graal error code.
    /// </summary>
    public GraalErrorCode ErrorCode { get; }

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
            message.AppendLine("ErrorCode: " + (int)ErrorCode);
            message.AppendLine("Message: " + GraalMessage);
            return message.ToString();
        }
    }

    /// <summary>
    /// Gets the error description associated with the specified error code.
    /// </summary>
    /// <param name="value">The specified error code.</param>
    /// <returns>Returns error description.</returns>
    private static string GetGraalErrorDescription(GraalErrorCode value)
    {
        var str = value.ToString();
        var fi = value.GetType().GetField(str);
        if (fi == null)
        {
            return "Unknown error";
        }

        var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
        return attributes.Length > 0 ? attributes[0].Description : str;
    }
}
