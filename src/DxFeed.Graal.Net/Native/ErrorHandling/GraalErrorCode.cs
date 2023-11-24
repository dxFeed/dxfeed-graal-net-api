// <copyright file="GraalErrorCode.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.ComponentModel;

namespace DxFeed.Graal.Net.Native.ErrorHandling;

/// <summary>
/// Errors returned when calling GraalVM functions.
/// The error description was obtained from <a href="https://github.com/oracle/graal/blob/jdk-21.0.1/substratevm/src/com.oracle.svm.core/src/com/oracle/svm/core/c/function/CEntryPointErrors.java#L43">GraalVM GitHub.</a>
/// </summary>
public enum GraalErrorCode
{
    /// <summary>
    /// No error occurred.
    /// </summary>
    [Description("No error occurred.")]
    NoError = 0,

    /// <summary>
    /// An unspecified error occurred.
    /// </summary>
    [Description("An unspecified error occurred.")]
    Unspecified = 1,

    /// <summary>
    /// An argument was NULL.
    /// </summary>
    [Description("An argument was NULL.")]
    NullArgument = 2,

    /// <summary>
    /// Memory allocation failed, the OS is probably out of memory.
    /// </summary>
    [Description("Memory allocation failed, the OS is probably out of memory.")]
    AllocationFailed = 3,

    /// <summary>
    /// The specified thread is not attached to the isolate.
    /// </summary>
    [Description("The specified thread is not attached to the isolate.")]
    UnattachedThread = 4,

    /// <summary>
    /// The specified isolate is unknown.
    /// </summary>
    [Description("The specified isolate is unknown.")]
    UninitializedIsolate = 5,

    /// <summary>
    /// Locating the image file failed.
    /// </summary>
    [Description("Locating the image file failed.")]
    LocateImageFailed = 6,

    /// <summary>
    /// Opening the located image file failed.
    /// </summary>
    [Description("Opening the located image file failed.")]
    OpenImageFailed = 7,

    /// <summary>
    /// Mapping the heap from the image file into memory failed.
    /// </summary>
    [Description("Mapping the heap from the image file into memory failed.")]
    MapHeapFailed = 8,

    /// <summary>
    /// Reserving address space for the new isolate failed.
    /// </summary>
    [Description("Reserving address space for the new isolate failed.")]
    ReserveAddressSpaceFailed = 801,

    /// <summary>
    /// The image heap does not fit in the available address space.
    /// </summary>
    [Description("The image heap does not fit in the available address space.")]
    InsufficientAddressSpace = 802,

    /// <summary>
    /// Setting the protection of the heap memory failed.
    /// </summary>
    [Description("Setting the protection of the heap memory failed.")]
    ProtectHeapFailed = 9,

    /// <summary>
    /// The version of the specified isolate parameters is unsupported.
    /// </summary>
    [Description("The version of the specified isolate parameters is unsupported.")]
    UnsupportedIsolateParametersVersion = 10,

    /// <summary>
    /// Initialization of threading in the isolate failed.
    /// </summary>
    [Description("Initialization of threading in the isolate failed.")]
    ThreadingInitializationFailed = 11,

    /// <summary>
    /// Some exception is not caught.
    /// </summary>
    [Description("Some exception is not caught.")]
    UncaughtException = 12,

    /// <summary>
    /// Initialization the isolate failed.
    /// </summary>
    [Description("Initialization the isolate failed.")]
    IsolateInitializationFailed = 13,

    /// <summary>
    /// Opening the located auxiliary image file failed.
    /// </summary>
    [Description("Opening the located auxiliary image file failed.")]
    OpenAuxImageFailed = 14,

    /// <summary>
    /// Reading the opened auxiliary image file failed.
    /// </summary>
    [Description("Reading the opened auxiliary image file failed.")]
    ReadAuxImageMetaFailed = 15,

    /// <summary>
    /// Mapping the auxiliary image file into memory failed.
    /// </summary>
    [Description("Mapping the auxiliary image file into memory failed.")]
    MapAuxImageFailed = 16,

    /// <summary>
    /// Insufficient memory for the auxiliary image.
    /// </summary>
    [Description("Insufficient memory for the auxiliary image.")]
    InsufficientAuxImageMemory = 17,

    /// <summary>
    /// Auxiliary images are not supported on this platform or edition.
    /// </summary>
    [Description("Auxiliary images are not supported on this platform or edition.")]
    AuxImageUnsupported = 18,

    /// <summary>
    /// Releasing the isolate's address space failed.
    /// </summary>
    [Description("Releasing the isolate's address space failed.")]
    FreeAddressSpaceFailed = 19,

    /// <summary>
    /// Releasing the isolate's image heap memory failed.
    /// </summary>
    [Description("Releasing the isolate's image heap memory failed.")]
    FreeImageHeapFailed = 20,

    /// <summary>
    /// The auxiliary image was built from a different primary image.
    /// </summary>
    [Description("The auxiliary image was built from a different primary image.")]
    AuxImagePrimaryImageMismatch = 21,

    /// <summary>
    /// The isolate arguments could not be parsed.
    /// </summary>
    [Description("The isolate arguments could not be parsed.")]
    ArgumentParsingFailed = 22,

    /// <summary>
    /// Current target does not support the CPU features that are required by the image.
    /// </summary>
    [Description("Current target does not support the CPU features that are required by the image.")]
    CpuFeatureCheckFailed = 23,

    /// <summary>
    /// Image page size is incompatible with run-time page size.
    /// Rebuild image with -H:PageSize=[pagesize] to set appropriately.
    /// </summary>
    [Description("Image page size is incompatible with run-time page size. " +
                 "Rebuild image with -H:PageSize=[pagesize] to set appropriately.")]
    PageSizeCheckFailed = 24,

    /// <summary>
    /// Creating an in-memory file for the GOT failed.
    /// </summary>
    [Description("Creating an in-memory file for the GOT failed.")]
    DynamicMethodAddressResolutionGotFdCreateFailed = 25,

    /// <summary>
    /// Resizing the in-memory file for the GOT failed.
    /// </summary>
    [Description("Resizing the in-memory file for the GOT failed.")]
    DynamicMethodAddressResolutionGotFdResizeFailed = 26,

    /// <summary>
    /// Mapping and populating the in-memory file for the GOT failed.
    /// </summary>
    [Description("Mapping and populating the in-memory file for the GOT failed.")]
    DynamicMethodAddressResolutionGotFdMapFailed = 27,

    /// <summary>
    /// Mapping the GOT before an isolate's heap failed (no mapping).
    /// </summary>
    [Description("Mapping the GOT before an isolate's heap failed (no mapping).")]
    DynamicMethodAddressResolutionGotMmapFailed = 28,

    /// <summary>
    /// Mapping the GOT before an isolate's heap failed (wrong mapping).
    /// </summary>
    [Description("Mapping the GOT before an isolate's heap failed (wrong mapping).")]
    DynamicMethodAddressResolutionGotWrongMmap = 29,

    /// <summary>
    /// Mapping the GOT before an isolate's heap failed (invalid file).
    /// </summary>
    [Description("Mapping the GOT before an isolate's heap failed (invalid file).")]
    DynamicMethodAddressResolutionGotFdInvalid = 30,

    /// <summary>
    /// Could not create unique GOT file even after retrying.
    /// </summary>
    [Description("Could not create unique GOT file even after retrying.")]
    DynamicMethodAddressResolutionGotUniqueFileCreateFailed = 31,

    /// <summary>
    /// Could not determine the stack boundaries.
    /// </summary>
    [Description("Could not determine the stack boundaries.")]
    UnknownStackBoundaries = 32,
}
