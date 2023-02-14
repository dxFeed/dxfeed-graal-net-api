// <copyright file="ThreadExitManager.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Utils;

// Element Must Begin With Upper Case Letter. Ignored, for simplify import native library classes.
#pragma warning disable SA1300

namespace DxFeed.Graal.Net.Native.Interop;

/// <summary>
/// Provides a method for registering a callback for a thread exit event.
/// This class is platform dependent and uses native library imports.
/// </summary>
internal static class ThreadExitManager
{
    /// <inheritdoc cref="RegisterCallback"/>
    private static readonly Func<nint, int> RegisterCallbackDelegate;

    /// <inheritdoc cref="UnregisterCallback"/>
    private static readonly Action<int> UnregisterCallbackDelegate;

    /// <inheritdoc cref="EnableCallbackForCurrentThread"/>
    private static readonly Action<int, nint> EnableCallbackForCurrentThreadDelegate;

    /// <summary>
    /// Initializes static members of the <see cref="ThreadExitManager"/> class.
    /// If the current operating system is not supported, all methods will throw <see cref="NotSupportedException"/>.
    /// </summary>
    static ThreadExitManager()
    {
        if (PlatformUtils.IsWindows)
        {
            RegisterCallbackDelegate = RegisterCallbackWindows;
            UnregisterCallbackDelegate = UnregisterCallbackWindows;
            EnableCallbackForCurrentThreadDelegate = EnableCallbackForCurrentThreadWindows;
        }
        else if (PlatformUtils.IsMacOs)
        {
            RegisterCallbackDelegate = RegisterCallbackMacOs;
            UnregisterCallbackDelegate = UnregisterCallbackMacOs;
            EnableCallbackForCurrentThreadDelegate = EnableCallbackForCurrentThreadMacOs;
        }
        else if (PlatformUtils.IsLinux)
        {
            if (PlatformUtils.IsMono)
            {
                RegisterCallbackDelegate = RegisterCallbackMono;
                UnregisterCallbackDelegate = UnregisterCallbackMono;
                EnableCallbackForCurrentThreadDelegate = EnableCallbackForCurrentThreadMono;
            }
            else
            {
                unsafe
                {
                    // Depending on the Linux distro, use either libcoreclr.so or libpthread.so.
                    try
                    {
                        int callbackId;

                        CheckCall(LinuxLibcoreclrImport.pthread_key_create(new IntPtr(&callbackId), 0));
                        CheckCall(LinuxLibcoreclrImport.pthread_key_delete(callbackId));

                        RegisterCallbackDelegate = RegisterCallbackLinuxLibcoreclr;
                        UnregisterCallbackDelegate = UnregisterCallbackLinuxLibcoreclr;
                        EnableCallbackForCurrentThreadDelegate = EnableCallbackForCurrentThreadLinuxLibcoreclr;
                    }
                    catch (EntryPointNotFoundException)
                    {
                        RegisterCallbackDelegate = RegisterCallbackLinuxLibpthread;
                        UnregisterCallbackDelegate = UnregisterCallbackLinuxLibpthread;
                        EnableCallbackForCurrentThreadDelegate = EnableCallbackForCurrentThreadLinuxLibpthread;
                    }
                }
            }
        }
        else
        {
            RegisterCallbackDelegate = RegisterCallbackNotSupported;
            UnregisterCallbackDelegate = UnregisterCallbackNotSupported;
            EnableCallbackForCurrentThreadDelegate = EnableCallbackForCurrentThreadNotSupported;
        }
    }

    /// <summary>
    /// Delegate for <see cref="RegisterCallback"/>.
    /// This delegate is called before the thread exits.
    /// </summary>
    /// <param name="threadLocalValue">
    /// Value from <see cref="EnableCallbackForCurrentThread"/>.
    /// </param>
    public delegate void ThreadExitCallback(nint threadLocalValue);

    /// <summary>
    /// Registers thread exit callback.
    /// </summary>
    /// <param name="callback">The <see cref="ThreadExitCallback"/>.</param>
    /// <returns>
    /// Returns id which can be passed to
    /// <see cref="UnregisterCallback"/> for unregister
    /// and  <see cref="EnableCallbackForCurrentThread"/> for enable callback.
    /// </returns>
    /// <exception cref="InvalidOperationException">If error occured.</exception>
    /// <exception cref="NotSupportedException">If current OS not supported.</exception>
    public static int RegisterCallback(ThreadExitCallback callback) =>
        RegisterCallbackDelegate(Marshal.GetFunctionPointerForDelegate(callback));

    /// <summary>
    /// Unregisters thread exit callback that has been registers with <see cref="RegisterCallback"/>.
    /// NOTE: Callback may be called as a result of this method call on some platforms.
    /// </summary>
    /// <param name="callbackId">The callback id returned from <see cref="RegisterCallback"/>.</param>
    /// <exception cref="InvalidOperationException">If error occured.</exception>
    /// <exception cref="NotSupportedException">If current OS not supported.</exception>
    public static void UnregisterCallback(int callbackId) =>
        UnregisterCallbackDelegate(callbackId);

    /// <summary>
    /// Enables a thread exit event(with specified thread-local value) for the current thread.
    /// The callback will be called before the thread exit in the same thread.
    /// </summary>
    /// <param name="callbackId">The callback id returned from <see cref="RegisterCallback"/>.</param>
    /// <param name="threadLocalValue">
    /// The thread-local variable which be passed to <see cref="ThreadExitCallback"/>.
    /// </param>
    /// <exception cref="InvalidOperationException">If error occured.</exception>
    /// <exception cref="NotSupportedException">If current OS not supported.</exception>
    public static void EnableCallbackForCurrentThread(int callbackId, nint threadLocalValue) =>
        EnableCallbackForCurrentThreadDelegate(callbackId, threadLocalValue);

    /// <inheritdoc cref="RegisterCallback"/>
    private static int RegisterCallbackWindows(nint callbackPtr)
    {
        var callbackId = WindowsThreadImport.FlsAlloc(callbackPtr);

        if (callbackId == WindowsThreadImport.FlsOutOfIndexes)
        {
            throw new InvalidOperationException("FlsAlloc failed: " + Marshal.GetLastWin32Error());
        }

        return callbackId;
    }

    /// <inheritdoc cref="RegisterCallback"/>
    private static unsafe int RegisterCallbackMacOs(nint callbackPtr)
    {
        int callbackId;
        CheckCall(MacOsThreadImport.pthread_key_create(new IntPtr(&callbackId), callbackPtr));
        return callbackId;
    }

    /// <inheritdoc cref="RegisterCallback"/>
    private static unsafe int RegisterCallbackLinuxLibcoreclr(nint callbackPtr)
    {
        int callbackId;
        CheckCall(LinuxLibcoreclrImport.pthread_key_create(new IntPtr(&callbackId), callbackPtr));
        return callbackId;
    }

    /// <inheritdoc cref="RegisterCallback"/>
    private static unsafe int RegisterCallbackLinuxLibpthread(nint callbackPtr)
    {
        int callbackId;
        CheckCall(LinuxLibpthreadImport.pthread_key_create(new IntPtr(&callbackId), callbackPtr));
        return callbackId;
    }

    /// <inheritdoc cref="RegisterCallback"/>
    private static unsafe int RegisterCallbackMono(nint callbackPtr)
    {
        int callbackId;
        CheckCall(MonoThreadImport.pthread_key_create(new IntPtr(&callbackId), callbackPtr));
        return callbackId;
    }

    /// <inheritdoc cref="RegisterCallback"/>
    private static int RegisterCallbackNotSupported(nint callbackPtr)
    {
        ThrowNotSupportedException();
        return -1;
    }

    /// <inheritdoc cref="UnregisterCallbackDelegate"/>
    private static void UnregisterCallbackWindows(int callbackId)
    {
        var result = WindowsThreadImport.FlsFree(callbackId);

        if (!result)
        {
            throw new InvalidOperationException("FlsFree failed: " + Marshal.GetLastWin32Error());
        }
    }

    /// <inheritdoc cref="UnregisterCallback"/>
    private static void UnregisterCallbackMacOs(int callbackId) =>
        CheckCall(MacOsThreadImport.pthread_key_delete(callbackId));

    /// <inheritdoc cref="UnregisterCallback"/>
    private static void UnregisterCallbackLinuxLibcoreclr(int callbackId) =>
        CheckCall(LinuxLibcoreclrImport.pthread_key_delete(callbackId));

    /// <inheritdoc cref="UnregisterCallback"/>
    private static void UnregisterCallbackLinuxLibpthread(int callbackId) =>
        CheckCall(LinuxLibpthreadImport.pthread_key_delete(callbackId));

    /// <inheritdoc cref="UnregisterCallback"/>
    private static void UnregisterCallbackMono(int callbackId) =>
        CheckCall(MonoThreadImport.pthread_key_delete(callbackId));

    /// <inheritdoc cref="UnregisterCallback"/>
    private static void UnregisterCallbackNotSupported(int callbackId) =>
        ThrowNotSupportedException();

    /// <inheritdoc cref="EnableCallbackForCurrentThread"/>
    private static void EnableCallbackForCurrentThreadWindows(int callbackId, nint threadLocalValue)
    {
        var result = WindowsThreadImport.FlsSetValue(callbackId, threadLocalValue);

        if (!result)
        {
            throw new InvalidOperationException("FlsSetValue failed: " + Marshal.GetLastWin32Error());
        }
    }

    /// <inheritdoc cref="EnableCallbackForCurrentThread"/>
    private static void EnableCallbackForCurrentThreadMacOs(int callbackId, nint threadLocalValue) =>
        CheckCall(MacOsThreadImport.pthread_setspecific(callbackId, threadLocalValue));

    /// <inheritdoc cref="EnableCallbackForCurrentThread"/>
    private static void EnableCallbackForCurrentThreadLinuxLibcoreclr(int callbackId, nint threadLocalValue) =>
        CheckCall(LinuxLibcoreclrImport.pthread_setspecific(callbackId, threadLocalValue));

    /// <inheritdoc cref="EnableCallbackForCurrentThread"/>
    private static void EnableCallbackForCurrentThreadLinuxLibpthread(int callbackId, nint threadLocalValue) =>
        CheckCall(LinuxLibpthreadImport.pthread_setspecific(callbackId, threadLocalValue));

    /// <inheritdoc cref="EnableCallbackForCurrentThread"/>
    private static void EnableCallbackForCurrentThreadMono(int callbackId, nint threadLocalValue) =>
        CheckCall(MonoThreadImport.pthread_setspecific(callbackId, threadLocalValue));

    /// <inheritdoc cref="EnableCallbackForCurrentThread"/>
    private static void EnableCallbackForCurrentThreadNotSupported(int callbackId, nint threadLocalValue) =>
        ThrowNotSupportedException();

    /// <summary>
    /// Checks native call result.
    /// </summary>
    /// <param name="result">The result of a native call.</param>
    /// <exception cref="InvalidOperationException">If error occured.</exception>
    private static void CheckCall(int result)
    {
        if (result != 0)
        {
            throw new InvalidOperationException($"Native call failed: {result}");
        }
    }

    private static void ThrowNotSupportedException() =>
        throw new NotSupportedException($"Unsupported OS: {PlatformUtils.OsNameAndVersion}");

    /// <summary>
    /// Windows kernel32.dll imports.
    /// </summary>
    private static class WindowsThreadImport
    {
        public const string DllName = "kernel32.dll";
        public const int FlsOutOfIndexes = -1;

        [DllImport(DllName, SetLastError = true)]
        public static extern int FlsAlloc(nint destructorCallback);

        [DllImport(DllName, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FlsFree(int key);

        [DllImport(DllName, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FlsSetValue(int key, nint threadLocalValue);
    }

    /// <summary>
    /// macOS libSystem.dylib imports.
    /// </summary>
    private static class MacOsThreadImport
    {
        public const string DllName = "libSystem.dylib";

        [DllImport(DllName)]
        public static extern int pthread_key_create(nint key, nint destructorCallback);

        [DllImport(DllName)]
        public static extern int pthread_key_delete(int key);

        [DllImport(DllName)]
        public static extern int pthread_setspecific(int key, nint threadLocalValue);
    }

    /// <summary>
    /// Linux libcoreclr.so imports.
    /// </summary>
    private static class LinuxLibcoreclrImport
    {
        public const string DllName = "libcoreclr.so";

        [DllImport(DllName)]
        public static extern int pthread_key_create(nint key, nint destructorCallback);

        [DllImport(DllName)]
        public static extern int pthread_key_delete(int key);

        [DllImport(DllName)]
        public static extern int pthread_setspecific(int key, nint threadLocalValue);
    }

    /// <summary>
    /// Linux libpthread.so imports.
    /// </summary>
    private static class LinuxLibpthreadImport
    {
        public const string DllName = "libpthread.so";

        [DllImport(DllName)]
        public static extern int pthread_key_create(nint key, nint destructorCallback);

        [DllImport(DllName)]
        public static extern int pthread_key_delete(int key);

        [DllImport(DllName)]
        public static extern int pthread_setspecific(int key, nint threadLocalValue);
    }

    /// <summary>
    /// Mono on Linux requires __Internal instead of libcoreclr.so or libpthread.so.
    /// </summary>
    private static class MonoThreadImport
    {
        public const string DllName = "__Internal";

        [DllImport(DllName)]
        public static extern int pthread_key_create(nint key, nint destructorCallback);

        [DllImport(DllName)]
        public static extern int pthread_key_delete(int key);

        [DllImport(DllName)]
        public static extern int pthread_setspecific(int key, nint threadLocalValue);
    }
}
