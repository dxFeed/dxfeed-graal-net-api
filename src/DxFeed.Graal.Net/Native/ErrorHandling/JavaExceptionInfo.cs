// <copyright file="JavaExceptionInfo.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;

namespace DxFeed.Graal.Net.Native.ErrorHandling;

/// <summary>
/// Information about thrown Java exception.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal readonly record struct JavaExceptionInfo(
    [MarshalAs(UnmanagedType.LPUTF8Str)] string? ClassName,
    [MarshalAs(UnmanagedType.LPUTF8Str)] string? Message,
    [MarshalAs(UnmanagedType.LPUTF8Str)] string? StackTrace);
