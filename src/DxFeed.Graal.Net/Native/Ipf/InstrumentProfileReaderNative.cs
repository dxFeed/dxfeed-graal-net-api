// <copyright file="InstrumentProfileReaderNative.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using DxFeed.Graal.Net.Ipf;
using DxFeed.Graal.Net.Native.Interop;
using DxFeed.Graal.Net.Native.Ipf.Handles;

namespace DxFeed.Graal.Net.Native.Ipf;

public class InstrumentProfileReaderNative : IDisposable
{
    private readonly InstrumentProfileReaderSafeHandle handle;

    private InstrumentProfileReaderNative(InstrumentProfileReaderSafeHandle builderHandle) =>
        handle = builderHandle;

    public static InstrumentProfileReaderNative Create() =>
        new(InstrumentProfileReaderSafeHandle.Create());

    public long GetLastModified() =>
        handle.GetLastModify();

    public bool WasComplete() =>
        handle.WasComplete();

    public List<InstrumentProfile> ReadFromFile(string address, string user, string password)
    {
        var result = new List<InstrumentProfile>();
        unsafe
        {
            ListNative<IpfNative>* profiles = null;
            try
            {
                profiles = handle.ReadFromFile(address, user, password);
                for (var i = 0; i < profiles->Size; i++)
                {
                    result.Add(IpfMapper.Convert(profiles->Elements[i]));
                }
            }
            finally
            {
                handle.IpfRelease(profiles);
            }
        }

        return result;
    }

    public void Dispose() =>
        handle.Dispose();
}
