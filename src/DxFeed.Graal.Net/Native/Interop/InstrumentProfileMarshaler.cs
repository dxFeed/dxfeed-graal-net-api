// <copyright file="InstrumentProfileMarshaler.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Ipf;
using DxFeed.Graal.Net.Native.Graal;
using static DxFeed.Graal.Net.Native.ErrorHandling.ErrorCheck;

namespace DxFeed.Graal.Net.Native.Interop;

internal class InstrumentProfileMarshaler : AbstractMarshaler
{
    private static readonly Lazy<InstrumentProfileMarshaler> Instance = new();

    public static ICustomMarshaler GetInstance(string cookie) =>
        Instance.Value;

    public override unsafe object? ConvertNativeToManaged(IntPtr native)
    {
        var profile = (InstrumentProfileNative*)native;
        return new InstrumentProfile
        {
            Type = profile->Type!,
            Symbol = profile->Symbol!,
            Description = profile->Description!,
            LocalSymbol = profile->LocalSymbol!,
            LocalDescription = profile->LocalDescription!,
            Country = profile->Country!,
            OPOL = profile->OPOL!,
            ExchangeData = profile->ExchangeData!,
            Exchanges = profile->Exchanges!,
            Currency = profile->Currency!,
            BaseCurrency = profile->BaseCurrency!,
            CFI = profile->CFI!,
            ISIN = profile->ISIN!,
            SEDOL = profile->SEDOL!,
            CUSIP = profile->CUSIP!,
            ICB = profile->ICB,
            SIC = profile->SIC,
            Multiplier = profile->Multiplier,
            Product = profile->Product!,
            Underlying = profile->Underlying!,
            SPC = profile->SPC,
            AdditionalUnderlyings = profile->AdditionalUnderlyings!,
            MMY = profile->MMY!,
            Expiration = profile->Expiration,
            LastTrade = profile->LastTrade,
            Strike = profile->Strike,
            OptionType = profile->OptionType!,
            ExpirationStyle = profile->ExpirationStyle!,
            SettlementStyle = profile->SettlementStyle!,
            PriceIncrements = profile->PriceIncrements!,
            TradingHours = profile->TradingHours!,
        };
    }

    public override unsafe IntPtr ConvertManagedToNative(object? managed)
    {
        if (managed is not InstrumentProfile profile)
        {
            throw new ArgumentException("Managed object must be a InstrumentProfile.", nameof(managed));
        }

        var ptr = (InstrumentProfileNative*)Marshal.AllocHGlobal(sizeof(InstrumentProfileNative));
        *ptr = new InstrumentProfileNative
        {
            Type = profile.Type,
            Symbol = profile.Symbol,
            Description = profile.Description,
            LocalSymbol = profile.LocalSymbol,
            LocalDescription = profile.LocalDescription,
            Country = profile.Country,
            OPOL = profile.OPOL,
            ExchangeData = profile.ExchangeData,
            Exchanges = profile.Exchanges,
            Currency = profile.Currency,
            BaseCurrency = profile.BaseCurrency,
            CFI = profile.CFI,
            ISIN = profile.ISIN,
            SEDOL = profile.SEDOL,
            CUSIP = profile.CUSIP,
            ICB = profile.ICB,
            SIC = profile.SIC,
            Multiplier = profile.Multiplier,
            Product = profile.Product,
            Underlying = profile.Underlying,
            SPC = profile.SPC,
            AdditionalUnderlyings = profile.AdditionalUnderlyings,
            MMY = profile.MMY,
            Expiration = profile.Expiration,
            LastTrade = profile.LastTrade,
            Strike = profile.Strike,
            OptionType = profile.OptionType,
            ExpirationStyle = profile.ExpirationStyle,
            SettlementStyle = profile.SettlementStyle,
            PriceIncrements = profile.PriceIncrements,
            TradingHours = profile.TradingHours,
        };
        return (IntPtr)ptr;
    }

    public override unsafe void CleanUpFromManaged(IntPtr ptr)
    {
        var profile = (InstrumentProfileNative*)ptr;
        profile->Type.Release();
        profile->Symbol.Release();
        profile->Description.Release();
        profile->LocalSymbol.Release();
        profile->LocalDescription.Release();
        profile->Country.Release();
        profile->OPOL.Release();
        profile->ExchangeData.Release();
        profile->Exchanges.Release();
        profile->Currency.Release();
        profile->BaseCurrency.Release();
        profile->CFI.Release();
        profile->ISIN.Release();
        profile->SEDOL.Release();
        profile->CUSIP.Release();
        profile->Product.Release();
        profile->Underlying.Release();
        profile->AdditionalUnderlyings.Release();
        profile->MMY.Release();
        profile->OptionType.Release();
        profile->ExpirationStyle.Release();
        profile->SettlementStyle.Release();
        profile->PriceIncrements.Release();
        profile->TradingHours.Release();
        Marshal.FreeHGlobal((IntPtr)profile);
    }

    public override void CleanUpFromNative(IntPtr ptr) =>
        SafeCall(Import.Release(Isolate.CurrentThread, ptr));

    public override void CleanUpListFromNative(IntPtr ptr) =>
        SafeCall(Import.ReleaseList(Isolate.CurrentThread, ptr));

    [StructLayout(LayoutKind.Sequential)]
    private readonly record struct InstrumentProfileNative(
        StringNative Type,
        StringNative Symbol,
        StringNative Description,
        StringNative LocalSymbol,
        StringNative LocalDescription,
        StringNative Country,
        StringNative OPOL,
        StringNative ExchangeData,
        StringNative Exchanges,
        StringNative Currency,
        StringNative BaseCurrency,
        StringNative CFI,
        StringNative ISIN,
        StringNative SEDOL,
        StringNative CUSIP,
        int ICB,
        int SIC,
        double Multiplier,
        StringNative Product,
        StringNative Underlying,
        double SPC,
        StringNative AdditionalUnderlyings,
        StringNative MMY,
        int Expiration,
        int LastTrade,
        double Strike,
        StringNative OptionType,
        StringNative ExpirationStyle,
        StringNative SettlementStyle,
        StringNative PriceIncrements,
        StringNative TradingHours);

    private static class Import
    {
        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_InstrumentProfile_release")]
        public static extern int Release(nint thread, nint handle);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_CList_InstrumentProfile_release")]
        public static extern int ReleaseList(nint thread, nint handle);
    }
}
