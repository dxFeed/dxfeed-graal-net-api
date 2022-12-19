// <copyright file="GraalErrorCode.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.ComponentModel;

namespace DxFeed.Graal.Net.Native.ErrorHandling;

// Missing XML comment for publicly visible type or member.
// Already documented in Description attribute.
#pragma warning disable CS1591

// Enumeration items should be documented.
// Already documented in Description attribute.
#pragma warning disable SA1602

/// <summary>
/// List of graal error codes.
/// The error description was obtained from github.
/// <a href="https://github.com/oracle/graal/blob/f195395329fba573afc6f81c5e70a18ac334dd10/substratevm/src/com.oracle.svm.core/src/com/oracle/svm/core/c/function/CEntryPointErrors.java#L43">
/// Graal GitHub.</a>
/// </summary>
public enum GraalErrorCode
{
    [Description("No error occurred.")]
    NoError = 0,

    [Description("An unspecified error occurred.")]
    Unspecified = 1,

    [Description("An argument was NULL.")]
    NullArgument = 2,

    [Description("The specified thread is not attached to the isolate.")]
    UnattachedThread = 4,

    [Description("The specified isolate is unknown.")]
    UninitializedIsolate = 5,

    [Description("Locating the image file failed.")]
    LocateImageFailed = 6,

    [Description("Opening the located image file failed.")]
    OpenImageFailed = 7,

    [Description("Mapping the heap from the image file into memory failed.")]
    MapHeapFailed = 8,

    [Description("Reserving address space for the new isolate failed.")]
    ReserveAddressSpaceFailed = 801,

    [Description("The image heap does not fit in the available address space.")]
    InsufficientAddressSpace = 802,

    [Description("Setting the protection of the heap memory failed.")]
    ProtectHeapFailed = 9,

    [Description("The version of the specified isolate parameters is unsupported.")]
    UnsupportedIsolateParametersVersion = 10,

    [Description("Initialization of threading in the isolate failed.")]
    ThreadingInitializationFailed = 11,

    [Description("Some exception is not caught.")]
    UncaughtException = 12,

    [Description("Initialization the isolate failed.")]
    IsolateInitializationFailed = 13,

    [Description("Opening the located auxiliary image file failed.")]
    OpenAuxImageFailed = 14,

    [Description("Reading the opened auxiliary image file failed.")]
    ReadAuxImageMetaFailed = 15,

    [Description("Mapping the auxiliary image file into memory failed.")]
    MapAuxImageFailed = 16,

    [Description("Insufficient memory for the auxiliary image.")]
    InsufficientAuxImageMemory = 17,

    [Description("Auxiliary images are not supported on this platform or edition.")]
    AuxImageUnsupported = 18,

    [Description("Releasing the isolate's address space failed.")]
    FreeAddressSpaceFailed = 19,

    [Description("Releasing the isolate's image heap memory failed.")]
    FreeImageHeapFailed = 20,

    [Description("The auxiliary image was built from a different primary image.")]
    AuxImagePrimaryImageMismatch = 21,

    [Description("The isolate arguments could not be parsed.")]
    ArgumentParsingFailed = 22,

    [Description("Current target does not support the following CPU features that are required by the image.")]
    CpuFeatureCheckFailed = 23,
}
