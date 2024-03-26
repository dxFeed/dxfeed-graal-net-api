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
/// An exception class representing errors that occur during calls to GraalVM functions in interop scenarios.
/// </summary>
/// <remarks>
/// This class encapsulates detailed information about errors encountered in GraalVM operations,
/// including specific error codes and descriptive messages.
/// </remarks>
[Serializable]
public sealed class GraalException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GraalException"/> class.
    /// </summary>
    /// <param name="errorCode">The error code associated with the GraalVM error.</param>
    public GraalException(GraalErrorCode errorCode)
        : base(CreateErrorMessage(errorCode)) =>
        ErrorCode = errorCode;

#if NET8_0_OR_GREATER
    [Obsolete("NET8_0_OR_GREATER", DiagnosticId = "SYSLIB0051")]
#endif
    private GraalException(SerializationInfo info, StreamingContext context)
        : base(info, context) =>
        ErrorCode = (GraalErrorCode)info.GetValue(nameof(ErrorCode), typeof(GraalErrorCode))!;

    /// <summary>
    /// Gets the GraalVM error code associated with this exception.
    /// </summary>
    public GraalErrorCode ErrorCode { get; }

    /// <inheritdoc/>
#if NET8_0_OR_GREATER
    [Obsolete("NET8_0_OR_GREATER", DiagnosticId = "SYSLIB0051")]
#endif
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(ErrorCode), ErrorCode);
    }

    private static string CreateErrorMessage(GraalErrorCode errorCode)
    {
        var name = errorCode.ToString();
        var fi = errorCode.GetType().GetField(name);
        if (fi == null)
        {
            return $"Unknown error with error code: {errorCode}.";
        }

        var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
        return attributes.Length > 0 ? attributes[0].Description : name;
    }
}
