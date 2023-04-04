// <copyright file="StateChangeListenerHandle.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;

namespace DxFeed.Graal.Net.Native.Endpoint.Handles;

/// <summary>
/// A handle that represents a Java <c>java.beans.PropertyChangeListener</c> object.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal readonly struct StateChangeListenerHandle
{
    // ReSharper disable once MemberCanBePrivate.Global
    public readonly JavaObjectHandle JavaHandle;
}
