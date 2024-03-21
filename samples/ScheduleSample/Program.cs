// <copyright file="Program.cs" company="Devexperts LLC">
// Copyright © 2024 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DxFeed.Graal.Net.Ipf;
using DxFeed.Graal.Net.Schedules;
using DxFeed.Graal.Net.Utils;

namespace DxFeed.Graal.Net.Samples;

internal abstract class Program
{
    public static void Main(string[] args)
    {
        if (args.Length is < 3 or > 4)
        {
            Console.WriteLine("wrong number of arguments");
            Console.WriteLine("usage:  ScheduleSample  <defaults>  <profiles>  <symbol>  [time]");
            Console.WriteLine("where:  <defaults>  is a URL to Schedule API defaults file");
            Console.WriteLine("        <profiles>  is a URL to IPF file");
            Console.WriteLine("        <symbol>    is a ticker symbol used for sample");
            Console.WriteLine("        [time]      is a time used for sample in a format yyyy-MM-dd-HH:mm:ss");
            Console.WriteLine("sample: schedule.properties.zip  sample.ipf.zip  IBM  2011-05-26-14:15:00");
            return;
        }

        UpdateScheduleDefaults(args[0]);
        var profiles = LoadInstrumentProfiles(args[1]);
        CheckAllSchedules(profiles.Values);
        var symbol = args[2];
        if (!profiles.TryGetValue(symbol, out var profile))
        {
            Console.WriteLine("Could not find profile for " + symbol);
            return;
        }

        Console.WriteLine("Found profile for " + symbol + ": " + profile.Description);
        var time = args.Length < 4
            ? DateTimeOffset.Now.ToUnixTimeMilliseconds()
            : TimeFormat.Default.Parse(args[3]).ToUnixTimeMilliseconds();
        Console.WriteLine("Using timestamp " + TimeFormat.Default.Format(time));
        PrintNext5Holidays(profile, time);
        PrintCurrentSession(profile, time);
        PrintNextTradingSession(profile, time);
        PrintNearestTradingSession(profile, time);
    }

    private static void UpdateScheduleDefaults(string url)
    {
        try
        {
            Schedule.SetDefaults(File.ReadAllBytes(url));
            Console.WriteLine("Schedule defaults updated successfully");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private static Dictionary<string, InstrumentProfile> LoadInstrumentProfiles(string url)
    {
        var profiles = new Dictionary<string, InstrumentProfile>();
        try
        {
            foreach (var profile in new InstrumentProfileReader().ReadFromFile(url))
            {
                profiles.Add(profile.Symbol, profile);
            }

            Console.WriteLine("Loaded " + profiles.Count + " instrument profiles");
        }
        catch (IOException e)
        {
            Console.WriteLine(e);
        }

        return profiles;
    }

    private static void CheckAllSchedules(ICollection<InstrumentProfile> profiles)
    {
        var successes = 0;
        foreach (var profile in profiles)
        {
            try
            {
                Schedule.GetInstance(profile);
                foreach (var venue in Schedule.GetTradingVenues(profile))
                {
                    Schedule.GetInstance(profile, venue);
                    successes++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error getting schedule for {profile.Symbol} ({profile.TradingHours}): {e}");
            }
        }

        Console.WriteLine($"Checked {profiles.Count} instrument profiles: {successes} successes, {(profiles.Count - successes)} failures");
    }

    private static void PrintNext5Holidays(InstrumentProfile profile, long time)
    {
        var schedule = Schedule.GetInstance(profile);
        var day = schedule.GetDayByTime(time);
        var output = new StringBuilder("5 next holidays for " + profile.Symbol + ":");
        for (var i = 0; i < 5; i++)
        {
            day = day.GetNextDay(DayFilter.HOLIDAY);
            if (day != null)
            {
                output.Append(' ');
                output.Append(day.YearMonthDay);
            }
            else
            {
                break;
            }
        }

        Console.WriteLine(output);
    }

    private static void PrintCurrentSession(InstrumentProfile profile, long time)
    {
        var schedule = Schedule.GetInstance(profile);
        var session = schedule.GetSessionByTime(time);
        Console.WriteLine("Current session for " + profile.Symbol + ": " + session + " in " + session.Day);
    }

    private static void PrintNextTradingSession(InstrumentProfile profile, long time)
    {
        var schedule = Schedule.GetInstance(profile);
        var session = schedule.GetSessionByTime(time);
        if (!session.IsTrading)
        {
            session = session.GetNextSession(SessionFilter.TRADING);
        }

        if (session != null)
        {
            Console.WriteLine("Next trading session for " + profile.Symbol + ": " + session + " in " + session.Day);
        }
    }

    private static void PrintNearestTradingSession(InstrumentProfile profile, long time)
    {
        var schedule = Schedule.GetInstance(profile);
        if (schedule.TryGetNearestSessionByTime(time, SessionFilter.TRADING, out var session))
        {
            Console.WriteLine("Nearest trading session for " + profile.Symbol + ": " + session + " in " + session.Day);
        }
    }
}
