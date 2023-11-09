// <copyright file="GraalException.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.ComponentModel;
using System.Runtime.Serialization;

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
        ErrorCode = (GraalErrorCode)info.GetValue(nameof(ErrorCode), typeof(GraalErrorCode))!;
        GraalMessage = info.GetString(nameof(GraalMessage))!;
    }

    /// <summary>
    /// Gets graal error code.
    /// </summary>
    public GraalErrorCode ErrorCode { get; }

    /// <summary>
    /// Gets graal error message.
    /// </summary>
    public string GraalMessage { get; }

    /// <summary>
    /// Gets a message that describes the current exception.
    /// </summary>
    /// <returns>The error message that explains the reason for the exception.</returns>
    public override string Message =>
        GraalMessage;

    /// <inheritdoc/>
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(ErrorCode), ErrorCode);
        info.AddValue(nameof(GraalMessage), GraalMessage);
    }

    /// <summary>
    /// Gets the error description associated with the specified error code.
    /// </summary>
    /// <param name="value">The specified error code.</param>
    /// <returns>Returns error description.</returns>
    private static string GetGraalErrorDescription(GraalErrorCode value)
    {
        var name = value.ToString();
        var fi = value.GetType().GetField(name);
        if (fi == null)
        {
            return $"Unknown error with error code {value}.";
        }

        var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
        return attributes.Length > 0 ? attributes[0].Description : name;
    }
}
