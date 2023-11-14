// <copyright file="JavaStringHandle.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Native.ErrorHandling;

namespace DxFeed.Graal.Net.Native.Interop;

/// <summary>
/// Represents a specialized safe handle for managing Java string objects within a .NET interop environment.
/// This class is responsible for ensuring the proper cleanup and
/// disposal of Java strings when dealing with native interop scenarios.
/// </summary>
/// <remarks>
/// The <see cref="JavaStringHandle"/> class encapsulates a Java string handle
/// and provides mechanisms for retrieving its content as a .NET string and for releasing its native resources.
/// </remarks>
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Created by marshaller")]
internal sealed class JavaStringHandle : JavaHandle
{
    /// <summary>
    /// Returns the Java string represented by the handle, converted to a .NET string.
    /// </summary>
    /// <returns>A .NET string representation of the Java string.</returns>
    public override string? ToString() =>
        Marshal.PtrToStringUTF8(handle);

    /// <inheritdoc/>
    protected override void Release() =>
        ErrorCheck.SafeCall(Import.Release(CurrentThread, handle));

    private static class Import
    {
        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_String_release")]
        public static extern int Release(nint thread, nint handle);
    }
}
