// <copyright file="ProfileMapper.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using DxFeed.Graal.Net.Events;
using DxFeed.Graal.Net.Events.Market;

namespace DxFeed.Graal.Net.Native.Events.Market;

internal class ProfileMapper : EventTypeMapper<Profile, ProfileNative>
{
    public override unsafe IEventType FromNative(EventTypeNative* eventType) =>
        Convert((ProfileNative*)eventType);

    public override unsafe EventTypeNative* ToNative(IEventType eventType) =>
        (EventTypeNative*)Convert((Profile)eventType);

    public override unsafe void Release(EventTypeNative* eventType)
    {
        if (eventType == (EventTypeNative*)0)
        {
            return;
        }

        var profileNative = (ProfileNative*)eventType;
        profileNative->StatusReason.Release();
        profileNative->Description.Release();
        ReleaseEventType(eventType);
    }

    protected override unsafe Profile Convert(ProfileNative* eventType)
    {
        var profile = CreateEventType(eventType);
        profile.Description = eventType->Description;
        profile.StatusReason = eventType->StatusReason;
        profile.HaltStartTime = eventType->HaltStartTime;
        profile.HaltEndTime = eventType->HaltEndTime;
        profile.HighLimitPrice = eventType->HighLimitPrice;
        profile.LowLimitPrice = eventType->LowLimitPrice;
        profile.High52WeekPrice = eventType->High52WeekPrice;
        profile.Low52WeekPrice = eventType->Low52WeekPrice;
        profile.Flags = eventType->Flags;
        return profile;
    }

    protected override unsafe ProfileNative* Convert(Profile eventType)
    {
        var ptr = AllocEventType();
        *ptr = new()
        {
            EventType = CreateEventType(eventType),
            Description = eventType.Description,
            StatusReason = eventType.StatusReason,
            HaltStartTime = eventType.HaltStartTime,
            HaltEndTime = eventType.HaltEndTime,
            HighLimitPrice = eventType.HighLimitPrice,
            LowLimitPrice = eventType.LowLimitPrice,
            High52WeekPrice = eventType.High52WeekPrice,
            Low52WeekPrice = eventType.Low52WeekPrice,
            Flags = eventType.Flags,
        };
        return ptr;
    }
}
