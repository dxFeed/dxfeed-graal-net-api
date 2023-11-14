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
/// Exception class representing errors that occur during calls to GraalVM functions in interop scenarios.
/// </summary>
/// <remarks>
/// This class encapsulates the detailed information about errors that arise from GraalVM operations,
/// including the specific error code and a descriptive message.
/// </remarks>
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
    /// Gets the error code associated with the GraalVM error.
    /// </summary>
    public GraalErrorCode ErrorCode { get; }

    /// <summary>
    /// Gets graal error message.
    /// </summary>
    public string GraalMessage { get; }

    /// <summary>
    /// Gets the descriptive message associated with the GraalVM error.
    /// </summary>
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
    /// Retrieves the descriptive error message corresponding to a specified GraalVM error code.
    /// </summary>
    /// <param name="value">The error code for which to retrieve the description.</param>
    /// <returns>The descriptive error message.</returns>
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
