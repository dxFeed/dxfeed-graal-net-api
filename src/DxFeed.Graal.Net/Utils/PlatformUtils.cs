// <copyright file="PlatformUtils.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Runtime.InteropServices;

namespace DxFeed.Graal.Net.Utils;

/// <summary>
/// Provides information to identify the current platform.
/// </summary>
public static class PlatformUtils
{
    /// <summary>
    /// Indicates whether the current application is running on Windows.
    /// </summary>
    public static readonly bool IsWindows = OperatingSystem.IsWindows();

    /// <summary>
    /// Indicates whether the current application is running on macOS.
    /// </summary>
    public static readonly bool IsMacOs = OperatingSystem.IsMacOS();

    /// <summary>
    /// Indicates whether the current application is running on Linux.
    /// </summary>
    public static readonly bool IsLinux = OperatingSystem.IsLinux();

    /// <summary>
    /// Indicates whether the current application is running on Mono .NET Runtime.
    /// </summary>
    public static readonly bool IsMono = Type.GetType("Mono.Runtime") != null;

    /// <summary>
    /// Gets os name and version.
    /// </summary>
    public static readonly string OsNameAndVersion = GetOsNameAndVersion();

    /// <summary>
    /// Gets the platform architecture on which the current app is running.
    /// The returned value is intended to represent the actual architecture of the underlying operating system.
    /// It is a best effort to ignore the architecture emulation infrastructure that may be involved to run the process.
    /// The returned value takes into account emulation built into Windows and macOS operating systems.
    /// The returned value does not take into account emulation
    /// using QEMU that is typically used on Linux operating system.
    /// </summary>
    public static readonly Architecture OsArch = RuntimeInformation.OSArchitecture;

    /// <summary>
    /// Indicates whether the current application is running on Apple Silicon SoC.
    /// </summary>
    public static readonly bool IsAppleSilicon = IsMacOs && OsArch == Architecture.Arm64;

    /// <summary>
    /// Gets the number of logical processors on the machine.
    /// If the process is running with CPU affinity, the number of processors that the process is affinitized to.
    /// If the process is running with a CPU utilization limit,
    /// the CPU utilization limit rounded up to the next whole number.
    /// </summary>
    public static readonly int LogicalCoreCount = Environment.ProcessorCount;

    /// <summary>
    /// Gets string contains the name and version of the OS, architecture, number of cores.
    /// Used for debugging and logging purposes.
    /// </summary>
    public static readonly string PlatformDiagInfo = GetPlatformDiagInfo();

    private static string GetOsNameAndVersion()
    {
        var osVersion = Environment.OSVersion;
        if (IsWindows)
        {
            return $"Windows({osVersion.Version})";
        }

        if (IsMacOs)
        {
            return $"macOS({osVersion.Version})";
        }

        if (IsLinux)
        {
            return $"Linux({osVersion.Version})";
        }

        return osVersion.ToString();
    }

    private static string GetPlatformDiagInfo() =>
        $"{OsNameAndVersion} {OsArch}({LogicalCoreCount} core(s))";
}
