// <copyright file="InstrumentProfileHandle.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Ipf;
using DxFeed.Graal.Net.Native.Interop;
using static DxFeed.Graal.Net.Native.ErrorHandling.ErrorCheck;

namespace DxFeed.Graal.Net.Native.Ipf;

internal class InstrumentProfileHandle : JavaHandle
{
    public InstrumentProfileHandle()
    {
    }

    public InstrumentProfileHandle(IntPtr handle)
        : base(handle)
    {
    }

    public string Type
    {
        get => SafeCall(Import.GetType(CurrentThread, this));
        set => SafeCall(Import.SetType(CurrentThread, this, value));
    }

    public string Symbol
    {
        get => SafeCall(Import.GetSymbol(CurrentThread, this));
        set => SafeCall(Import.SetSymbol(CurrentThread, this, value));
    }

    public string Description
    {
        get => SafeCall(Import.GetDescription(CurrentThread, this));
        set => SafeCall(Import.SetDescription(CurrentThread, this, value));
    }

    public string LocalSymbol
    {
        get => SafeCall(Import.GetLocalSymbol(CurrentThread, this));
        set => SafeCall(Import.SetLocalSymbol(CurrentThread, this, value));
    }

    public string LocalDescription
    {
        get => SafeCall(Import.GetLocalDescription(CurrentThread, this));
        set => SafeCall(Import.SetLocalDescription(CurrentThread, this, value));
    }

    public string Country
    {
        get => SafeCall(Import.GetCountry(CurrentThread, this));
        set => SafeCall(Import.SetCountry(CurrentThread, this, value));
    }

    public string OPOL
    {
        get => SafeCall(Import.GetOPOL(CurrentThread, this));
        set => SafeCall(Import.SetOPOL(CurrentThread, this, value));
    }

    public string ExchangeData
    {
        get => SafeCall(Import.GetExchangeData(CurrentThread, this));
        set => SafeCall(Import.SetExchangeData(CurrentThread, this, value));
    }

    public string Exchanges
    {
        get => SafeCall(Import.GetExchanges(CurrentThread, this));
        set => SafeCall(Import.SetExchanges(CurrentThread, this, value));
    }

    public string Currency
    {
        get => SafeCall(Import.GetCurrency(CurrentThread, this));
        set => SafeCall(Import.SetCurrency(CurrentThread, this, value));
    }

    public string BaseCurrency
    {
        get => SafeCall(Import.GetBaseCurrency(CurrentThread, this));
        set => SafeCall(Import.SetBaseCurrency(CurrentThread, this, value));
    }

    public string CFI
    {
        get => SafeCall(Import.GetCFI(CurrentThread, this));
        set => SafeCall(Import.SetCFI(CurrentThread, this, value));
    }

    public string ISIN
    {
        get => SafeCall(Import.GetISIN(CurrentThread, this));
        set => SafeCall(Import.SetISIN(CurrentThread, this, value));
    }

    public string SEDOL
    {
        get => SafeCall(Import.GetSEDOL(CurrentThread, this));
        set => SafeCall(Import.SetSEDOL(CurrentThread, this, value));
    }

    public string CUSIP
    {
        get => SafeCall(Import.GetCUSIP(CurrentThread, this));
        set => SafeCall(Import.SetCUSIP(CurrentThread, this, value));
    }

    public int ICB
    {
        get => SafeCall(Import.GetICB(CurrentThread, this));
        set => SafeCall(Import.SetICB(CurrentThread, this, value));
    }

    public int SIC
    {
        get => SafeCall(Import.GetSIC(CurrentThread, this));
        set => SafeCall(Import.SetSIC(CurrentThread, this, value));
    }

    public double Multiplier
    {
        get => SafeCall(Import.GetMultiplier(CurrentThread, this));
        set => SafeCall(Import.SetMultiplier(CurrentThread, this, value));
    }

    public string Product
    {
        get => SafeCall(Import.GetProduct(CurrentThread, this));
        set => SafeCall(Import.SetProduct(CurrentThread, this, value));
    }

    public string Underlying
    {
        get => SafeCall(Import.GetUnderlying(CurrentThread, this));
        set => SafeCall(Import.SetUnderlying(CurrentThread, this, value));
    }

    public double SPC
    {
        get => SafeCall(Import.GetSPC(CurrentThread, this));
        set => SafeCall(Import.SetSPC(CurrentThread, this, value));
    }

    public string AdditionalUnderlyings
    {
        get => SafeCall(Import.GetAdditionalUnderlyings(CurrentThread, this));
        set => SafeCall(Import.SetAdditionalUnderlyings(CurrentThread, this, value));
    }

    public string MMY
    {
        get => SafeCall(Import.GetMMY(CurrentThread, this));
        set => SafeCall(Import.SetMMY(CurrentThread, this, value));
    }

    public int Expiration
    {
        get => SafeCall(Import.GetExpiration(CurrentThread, this));
        set => SafeCall(Import.SetExpiration(CurrentThread, this, value));
    }

    public int LastTrade
    {
        get => SafeCall(Import.GetLastTrade(CurrentThread, this));
        set => SafeCall(Import.SetLastTrade(CurrentThread, this, value));
    }

    public double Strike
    {
        get => SafeCall(Import.GetStrike(CurrentThread, this));
        set => SafeCall(Import.SetStrike(CurrentThread, this, value));
    }

    public string OptionType
    {
        get => SafeCall(Import.GetOptionType(CurrentThread, this));
        set => SafeCall(Import.SetOptionType(CurrentThread, this, value));
    }

    public string ExpirationStyle
    {
        get => SafeCall(Import.GetExpirationStyle(CurrentThread, this));
        set => SafeCall(Import.SetExpirationStyle(CurrentThread, this, value));
    }

    public string SettlementStyle
    {
        get => SafeCall(Import.GetSettlementStyle(CurrentThread, this));
        set => SafeCall(Import.SetSettlementStyle(CurrentThread, this, value));
    }

    public string PriceIncrements
    {
        get => SafeCall(Import.GetPriceIncrements(CurrentThread, this));
        set => SafeCall(Import.SetPriceIncrements(CurrentThread, this, value));
    }

    public string TradingHours
    {
        get => SafeCall(Import.GetTradingHours(CurrentThread, this));
        set => SafeCall(Import.SetTradingHours(CurrentThread, this, value));
    }

    public static InstrumentProfileHandle Create() =>
        SafeCall(Import.Create(CurrentThread));

    public static InstrumentProfileHandle Create(InstrumentProfileHandle handle) =>
        SafeCall(Import.Create(CurrentThread, handle));

    public string GetField(string name) =>
        SafeCall(Import.GetField(CurrentThread, this, name));

    public void SetField(string name, string value) =>
        SafeCall(Import.SetField(CurrentThread, this, name, value));

    public double GetNumericField(string name) =>
        SafeCall(Import.GetNumericField(CurrentThread, this, name));

    public void SetNumericField(string name, double value) =>
        SafeCall(Import.SetNumericField(CurrentThread, this, name, value));

    public int GetDateField(string name) =>
        SafeCall(Import.GetDateField(CurrentThread, this, name));

    public void SetDateField(string name, int value) =>
        SafeCall(Import.SetDateField(CurrentThread, this, name, value));

    public bool AddNonEmptyCustomFieldNames(ICollection<string> targetFieldNames)
    {
        var fields = SafeCall(Import.GetNonEmptyCustomFieldNames(CurrentThread, this));
        foreach (var field in fields)
        {
            targetFieldNames.Add(field);
        }

        return fields.Count != 0;
    }

    private static class Import
    {
        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_InstrumentProfile_new")]
        public static extern InstrumentProfileHandle Create(nint thread);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            EntryPoint = "dxfg_InstrumentProfile_new2")]
        public static extern InstrumentProfileHandle Create(nint thread, InstrumentProfileHandle ip);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_getType")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
        public static extern string GetType(nint thread, InstrumentProfileHandle ip);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_setType")]
        public static extern int SetType(
            nint thread,
            InstrumentProfileHandle ip,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string value);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_getSymbol")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
        public static extern string GetSymbol(nint thread, InstrumentProfileHandle ip);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_setSymbol")]
        public static extern int SetSymbol(
            nint thread,
            InstrumentProfileHandle ip,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string value);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_getDescription")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
        public static extern string GetDescription(nint thread, InstrumentProfileHandle ip);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_setDescription")]
        public static extern int SetDescription(
            nint thread,
            InstrumentProfileHandle ip,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string value);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_getLocalSymbol")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
        public static extern string GetLocalSymbol(nint thread, InstrumentProfileHandle ip);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_setLocalSymbol")]
        public static extern int SetLocalSymbol(
            nint thread,
            InstrumentProfileHandle ip,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string value);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_getLocalDescription")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
        public static extern string GetLocalDescription(nint thread, InstrumentProfileHandle ip);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_setLocalDescription")]
        public static extern int SetLocalDescription(
            nint thread,
            InstrumentProfileHandle ip,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string value);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_getCountry")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
        public static extern string GetCountry(nint thread, InstrumentProfileHandle ip);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_setCountry")]
        public static extern int SetCountry(
            nint thread,
            InstrumentProfileHandle ip,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string value);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_getOPOL")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
        public static extern string GetOPOL(nint thread, InstrumentProfileHandle ip);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_setOPOL")]
        public static extern int SetOPOL(
            nint thread,
            InstrumentProfileHandle ip,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string value);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_getExchangeData")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
        public static extern string GetExchangeData(nint thread, InstrumentProfileHandle ip);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_setExchangeData")]
        public static extern int SetExchangeData(
            nint thread,
            InstrumentProfileHandle ip,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string value);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_getExchanges")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
        public static extern string GetExchanges(nint thread, InstrumentProfileHandle ip);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_setExchanges")]
        public static extern int SetExchanges(
            nint thread,
            InstrumentProfileHandle ip,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string value);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_getCurrency")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
        public static extern string GetCurrency(nint thread, InstrumentProfileHandle ip);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_setCurrency")]
        public static extern int SetCurrency(
            nint thread,
            InstrumentProfileHandle ip,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string value);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_getBaseCurrency")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
        public static extern string GetBaseCurrency(nint thread, InstrumentProfileHandle ip);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_setBaseCurrency")]
        public static extern int SetBaseCurrency(
            nint thread,
            InstrumentProfileHandle ip,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string value);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_getCFI")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
        public static extern string GetCFI(nint thread, InstrumentProfileHandle ip);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_setCFI")]
        public static extern int SetCFI(
            nint thread,
            InstrumentProfileHandle ip,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string value);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_getISIN")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
        public static extern string GetISIN(nint thread, InstrumentProfileHandle ip);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_setISIN")]
        public static extern int SetISIN(
            nint thread,
            InstrumentProfileHandle ip,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string value);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_getSEDOL")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
        public static extern string GetSEDOL(nint thread, InstrumentProfileHandle ip);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_setSEDOL")]
        public static extern int SetSEDOL(
            nint thread,
            InstrumentProfileHandle ip,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string value);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_getCUSIP")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
        public static extern string GetCUSIP(nint thread, InstrumentProfileHandle ip);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_setCUSIP")]
        public static extern int SetCUSIP(
            nint thread,
            InstrumentProfileHandle ip,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string value);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_getICB")]
        public static extern int GetICB(nint thread, InstrumentProfileHandle ip);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_setICB")]
        public static extern int SetICB(nint thread, InstrumentProfileHandle ip, int value);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_getSIC")]
        public static extern int GetSIC(nint thread, InstrumentProfileHandle ip);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_setSIC")]
        public static extern int SetSIC(nint thread, InstrumentProfileHandle ip, int value);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_getMultiplier")]
        public static extern double GetMultiplier(nint thread, InstrumentProfileHandle ip);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_setMultiplier")]
        public static extern int SetMultiplier(nint thread, InstrumentProfileHandle ip, double value);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_getProduct")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
        public static extern string GetProduct(nint thread, InstrumentProfileHandle ip);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_setProduct")]
        public static extern int SetProduct(
            nint thread,
            InstrumentProfileHandle ip,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string value);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_getUnderlying")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
        public static extern string GetUnderlying(nint thread, InstrumentProfileHandle ip);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_setUnderlying")]
        public static extern int SetUnderlying(
            nint thread,
            InstrumentProfileHandle ip,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string value);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_getSPC")]
        public static extern double GetSPC(nint thread, InstrumentProfileHandle ip);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_setSPC")]
        public static extern int SetSPC(nint thread, InstrumentProfileHandle ip, double value);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_getAdditionalUnderlyings")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
        public static extern string GetAdditionalUnderlyings(nint thread, InstrumentProfileHandle ip);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_setAdditionalUnderlyings")]
        public static extern int SetAdditionalUnderlyings(
            nint thread,
            InstrumentProfileHandle ip,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string value);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_getMMY")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
        public static extern string GetMMY(nint thread, InstrumentProfileHandle ip);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_setMMY")]
        public static extern int SetMMY(
            nint thread,
            InstrumentProfileHandle ip,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string value);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_getExpiration")]
        public static extern int GetExpiration(nint thread, InstrumentProfileHandle ip);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_setExpiration")]
        public static extern int SetExpiration(nint thread, InstrumentProfileHandle ip, int value);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_getLastTrade")]
        public static extern int GetLastTrade(nint thread, InstrumentProfileHandle ip);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_setLastTrade")]
        public static extern int SetLastTrade(nint thread, InstrumentProfileHandle ip, int value);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_getStrike")]
        public static extern double GetStrike(nint thread, InstrumentProfileHandle ip);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_setStrike")]
        public static extern int SetStrike(nint thread, InstrumentProfileHandle ip, double value);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_getOptionType")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
        public static extern string GetOptionType(nint thread, InstrumentProfileHandle ip);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_setOptionType")]
        public static extern int SetOptionType(
            nint thread,
            InstrumentProfileHandle ip,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string value);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_getExpirationStyle")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
        public static extern string GetExpirationStyle(nint thread, InstrumentProfileHandle ip);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_setExpirationStyle")]
        public static extern int SetExpirationStyle(
            nint thread,
            InstrumentProfileHandle ip,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string value);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_getSettlementStyle")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
        public static extern string GetSettlementStyle(nint thread, InstrumentProfileHandle ip);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_setSettlementStyle")]
        public static extern int SetSettlementStyle(
            nint thread,
            InstrumentProfileHandle ip,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string value);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_getPriceIncrements")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
        public static extern string GetPriceIncrements(nint thread, InstrumentProfileHandle ip);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_setPriceIncrements")]
        public static extern int SetPriceIncrements(
            nint thread,
            InstrumentProfileHandle ip,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string value);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_getTradingHours")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
        public static extern string GetTradingHours(nint thread, InstrumentProfileHandle ip);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_setTradingHours")]
        public static extern int SetTradingHours(
            nint thread,
            InstrumentProfileHandle ip,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string value);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_getField")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
        public static extern string GetField(
            nint thread,
            InstrumentProfileHandle ip,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string name);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_setField")]
        public static extern int SetField(
            nint thread,
            InstrumentProfileHandle ip,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string name,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string value);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_getNumericField")]
        public static extern double GetNumericField(
            nint thread,
            InstrumentProfileHandle ip,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string name);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_setNumericField")]
        public static extern int SetNumericField(
            nint thread,
            InstrumentProfileHandle ip,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string name,
            double value);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_getDateField")]
        public static extern int GetDateField(
            nint thread,
            InstrumentProfileHandle ip,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string name);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_setDateField")]
        public static extern int SetDateField(
            nint thread,
            InstrumentProfileHandle ip,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))]
            string name,
            int value);

        [DllImport(
            ImportInfo.DllName,
            CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            EntryPoint = "dxfg_InstrumentProfile_getNonEmptyCustomFieldNames")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ListMarshaler<StringMarshaler>))]
        public static extern List<string> GetNonEmptyCustomFieldNames(
            nint thread,
            InstrumentProfileHandle ip);
    }
}
