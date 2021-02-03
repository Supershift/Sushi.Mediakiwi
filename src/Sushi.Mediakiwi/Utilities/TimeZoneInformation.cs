using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Wim.Utilities
{
    /// <summary>
    /// 
    /// </summary>
    public class TimeZoneInformation
    {
        /// <summary>
        /// 
        /// </summary>
        public TimeZoneInformation()
        {
        }

        private static TimeZoneInformation[] s_zones = null;
        private static readonly object s_lockZones = new object();

        /// <summary>
        /// Get the currently selected time zone
        /// </summary>
        public static TimeZoneInformation CurrentTimeZone
        {
            get
            {
                if (!Wim.CommonConfiguration.IS_FULL_TRUST)
                {
                    return new TimeZoneInformation()
                    {
                        m_displayName = "Local server time",
                        m_name = "Local server time",
                        m_Index = "",
                    };
                }

                // The currently selected time zone information can
                // be retrieved using the Win32 GetTimeZoneInformation call,
                // but it only gives us names, offsets and dates - crucially,
                // not the Index.

                TIME_ZONE_INFORMATION tziNative;
                TimeZoneInformation[] zones = EnumZones();

                NativeMethods.GetTimeZoneInformation(out tziNative);

                // Getting the identity is tricky; the best we can do
                // is a match on the properties.

                // If the OS 'Automatically adjust clock for daylight saving changes' checkbox
                // is unchecked, the structure returned by GetTimeZoneInformation has
                // the DaylightBias and DaylightName members set the same as the corresponding
                // Standard members. Therefore we check against both values in case this has
                // been done.

                for (int idx = 0; idx < zones.Length; ++idx)
                {
                    if (zones[idx].m_tzi.bias == tziNative.Bias &&
                         zones[idx].m_tzi.standardBias == tziNative.StandardBias &&
                         zones[idx].m_Index == tziNative.StandardName &&
                         (zones[idx].m_tzi.daylightBias == tziNative.DaylightBias ||
                           zones[idx].m_tzi.standardBias == tziNative.DaylightBias) &&
                         (zones[idx].m_daylightName == tziNative.DaylightName ||
                           zones[idx].m_Index == tziNative.DaylightName))
                    {
                        return zones[idx];
                    }
                }

                return null;
            }
        }


        /// <summary>
        /// Get a TimeZoneInformation for a supplied index.
        /// </summary>
        /// <param name="index">The time zone standard name to find.</param>
        /// <returns>The corresponding TimeZoneInformation.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if the index is not found.</exception>
        public static TimeZoneInformation FromIndex(string index)
        {
            if (!Wim.CommonConfiguration.IS_FULL_TRUST)
            {
                return new TimeZoneInformation()
                    {
                        m_displayName = "Local server time",
                        m_name = "Local server time",
                        m_Index = "",
                    };
            }

            TimeZoneInformation[] zones = EnumZones();

            for (int i = 0; i < zones.Length; ++i)
            {
                if (zones[i].m_Index.Equals(index, StringComparison.OrdinalIgnoreCase))
                    return zones[i];
            }

            throw new ArgumentOutOfRangeException("index", index, "Unknown time zone index");
        }

        private class TimeZoneComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                TimeZoneInformation tzx, tzy;

                tzx = x as TimeZoneInformation;
                tzy = y as TimeZoneInformation;

                if (tzx == null || tzy == null)
                {
                    throw new ArgumentException("Parameter null or wrong type");
                }

                int biasDifference = tzx.Bias - tzy.Bias;

                if (biasDifference == 0)
                {
                    return tzx.DisplayName.CompareTo(tzy.DisplayName);
                }
                else
                {
                    return biasDifference;
                }
            }
        }

        /// <summary>
        /// Enumerate the available time zones
        /// </summary>
        /// <returns>The list of known time zones</returns>
        public static TimeZoneInformation[] EnumZones()
        {
            if (s_zones == null)
            {
                lock (s_lockZones)
                {
                    if (s_zones == null)
                    {
                        ArrayList zones = new ArrayList();

                        if (Wim.CommonConfiguration.IS_FULL_TRUST)
                        {
                            using (RegistryKey key =
                                        Registry.LocalMachine.OpenSubKey(
                                        @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Time Zones"))
                            {
                                string[] zoneNames = key.GetSubKeyNames();

                                foreach (string zoneName in zoneNames)
                                {
                                    using (RegistryKey subKey = key.OpenSubKey(zoneName))
                                    {
                                        TimeZoneInformation tzi = new TimeZoneInformation();
                                        tzi.m_name = zoneName;
                                        tzi.m_displayName = (string)subKey.GetValue("Display");
                                        tzi.m_Index = (string)subKey.GetValue("Std");
                                        tzi.m_daylightName = (string)subKey.GetValue("Dlt");

                                        tzi.InitTzi((byte[])subKey.GetValue("Tzi"));

                                        zones.Add(tzi);
                                    }
                                }
                            }
                        }
                        else
                        {
                            zones.Add(new TimeZoneInformation()
                            {
                                m_displayName = "Local server time",
                                m_name = "Local server time",
                                m_Index = "",
                            });
                        }

                        s_zones = new TimeZoneInformation[zones.Count];

                        zones.CopyTo(s_zones);
                        Array.Sort(s_zones, new TimeZoneComparer());
                    }
                }
            }

            return s_zones;
        }


        /// <summary>
        /// The zone's name.
        /// </summary>
        public string Name
        {
            get { return m_name; }
        }

        /// <summary>
        /// The zone's display name, e.g. '(GMT) Greenwich Mean Time : Dublin, Edinburgh, Lisbon, London'.
        /// </summary>
        public string DisplayName
        {
            get { return m_displayName; }
        }

        private string m_Index;
        /// <summary>
        /// The zone's name during 'standard' time (not daylight savings).
        /// </summary>
        public string Index
        {
            get { return m_Index; }
        }

        /// <summary>
        /// The zone's name during daylight savings time.
        /// </summary>
        public string DaylightName
        {
            get { return m_daylightName; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return m_displayName;
        }

        /// <summary>
        /// The standard Windows SYSTEMTIME structure.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct SYSTEMTIME
        {
            public UInt16 wYear;
            public UInt16 wMonth;
            public UInt16 wDayOfWeek;
            public UInt16 wDay;
            public UInt16 wHour;
            public UInt16 wMinute;
            public UInt16 wSecond;
            public UInt16 wMilliseconds;
        }

        // System.Runtime.InteropServices.ComTypes.FILETIME is already declared in System.Runtime.InteropServices.

        /// <summary>
        /// The layout of the Tzi value in the registry.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct TZI
        {
            public int bias;
            public int standardBias;
            public int daylightBias;
            public SYSTEMTIME standardDate;
            public SYSTEMTIME daylightDate;
        }


        /// <summary>
        /// The standard Win32 TIME_ZONE_INFORMATION structure.
        /// Thanks to www.pinvoke.net.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct TIME_ZONE_INFORMATION
        {
            [MarshalAs(UnmanagedType.I4)]
            public Int32 Bias;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string StandardName;
            public SYSTEMTIME StandardDate;
            [MarshalAs(UnmanagedType.I4)]
            public Int32 StandardBias;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string DaylightName;
            public SYSTEMTIME DaylightDate;
            [MarshalAs(UnmanagedType.I4)]
            public Int32 DaylightBias;
        }


        /// <summary>
        /// A container for P/Invoke declarations.
        /// </summary>
        private struct NativeMethods
        {
            private const string KERNEL32 = "kernel32.dll";

            [DllImport(KERNEL32)]
            public static extern uint GetTimeZoneInformation(out TIME_ZONE_INFORMATION
                lpTimeZoneInformation);

            [DllImport(KERNEL32)]
            public static extern bool SystemTimeToTzSpecificLocalTime(
                [In] ref TIME_ZONE_INFORMATION lpTimeZone,
                [In] ref SYSTEMTIME lpUniversalTime,
                out SYSTEMTIME lpLocalTime);

            [DllImport(KERNEL32)]
            public static extern bool SystemTimeToFileTime(
                [In] ref SYSTEMTIME lpSystemTime,
                out System.Runtime.InteropServices.ComTypes.FILETIME lpFileTime);

            [DllImport(KERNEL32)]
            public static extern bool FileTimeToSystemTime(
                [In] ref System.Runtime.InteropServices.ComTypes.FILETIME lpFileTime,
                out SYSTEMTIME lpSystemTime);

            /// <summary>
            /// Convert a local time to UTC, using the supplied time zone information.
            /// Windows XP and Server 2003 and later only.
            /// </summary>
            /// <param name="lpTimeZone">The time zone to use.</param>
            /// <param name="lpLocalTime">The local time to convert.</param>
            /// <param name="lpUniversalTime">The resultant time in UTC.</param>
            /// <returns>true if successful, false otherwise.</returns>
            [DllImport(KERNEL32)]
            public static extern bool TzSpecificLocalTimeToSystemTime(
                [In] ref TIME_ZONE_INFORMATION lpTimeZone,
                [In] ref SYSTEMTIME lpLocalTime,
                out SYSTEMTIME lpUniversalTime);
        }


        /// <summary>
        /// Initialise the m_tzi member.
        /// </summary>
        /// <param name="info">The Tzi data from the registry.</param>
        private void InitTzi(byte[] info)
        {
            if (info.Length != Marshal.SizeOf(m_tzi))
            {
                throw new ArgumentException("Information size is incorrect", "info");
            }

            // Could have sworn there's a Marshal operation to pack bytes into
            // a structure, but I can't see it. Do it manually.

            GCHandle h = GCHandle.Alloc(info, GCHandleType.Pinned);

            try
            {
                m_tzi = (TZI)Marshal.PtrToStructure(h.AddrOfPinnedObject(), typeof(TZI));
            }
            finally
            {
                h.Free();
            }
        }

        /// <summary>
        /// The offset from UTC. Local = UTC + Bias.
        /// </summary>
        public int Bias
        {
            // Biases in the registry are defined as UTC = local + bias
            // We return as Local = UTC + bias
            get { return -m_tzi.bias; }
        }

        /// <summary>
        /// The offset from UTC during standard time.
        /// </summary>
        public int StandardBias
        {
            get { return -(m_tzi.bias + m_tzi.standardBias); }
        }

        /// <summary>
        /// The offset from UTC during daylight time.
        /// </summary>
        public int DaylightBias
        {
            get { return -(m_tzi.bias + m_tzi.daylightBias); }
        }


        private TIME_ZONE_INFORMATION TziNative()
        {
            TIME_ZONE_INFORMATION tziNative = new TIME_ZONE_INFORMATION();

            tziNative.Bias = m_tzi.bias;
            tziNative.StandardDate = m_tzi.standardDate;
            tziNative.StandardBias = m_tzi.standardBias;
            tziNative.DaylightDate = m_tzi.daylightDate;
            tziNative.DaylightBias = m_tzi.daylightBias;

            return tziNative;
        }


        /// <summary>
        /// Convert a time interpreted as UTC to a time in this time zone.
        /// </summary>
        /// <param name="utc">The UTC time to convert.</param>
        /// <returns>The corresponding local time in this zone.</returns>
        public DateTime FromUniversalTime(DateTime utc)
        {
            if (!Wim.CommonConfiguration.IS_FULL_TRUST)
                return DateTime.Now;

            // Convert to SYSTEMTIME
            SYSTEMTIME stUTC = DateTimeToSystemTime(utc);

            // Set up the TIME_ZONE_INFORMATION

            TIME_ZONE_INFORMATION tziNative = TziNative();

            SYSTEMTIME stLocal;

            NativeMethods.SystemTimeToTzSpecificLocalTime(ref tziNative, ref stUTC, out stLocal);

            // Convert back to DateTime
            return SystemTimeToDateTime(ref stLocal);
        }


        /// <summary>
        /// Convert a time from UTC to the time zone with the supplied index.
        /// </summary>
        /// <param name="index">The time zone index.</param>
        /// <param name="utc">The time to convert.</param>
        /// <returns>The converted time.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the index is not found.</exception>
        public static DateTime FromUniversalTime(string index, DateTime utc)
        {
            if (!Wim.CommonConfiguration.IS_FULL_TRUST)
                return DateTime.Now;

            TimeZoneInformation tzi = FromIndex(index);

            return tzi.FromUniversalTime(utc);
        }


        /// <summary>
        /// Convert a time interpreted as a local time in this zone to the equivalent UTC.
        /// Note that there may be different possible interpretations at the daylight
        /// time boundaries.
        /// </summary>
        /// <param name="local">The local time to convert.</param>
        /// <returns>The corresponding UTC.</returns>
        /// <exception cref="NotSupportedException">Thrown if the method failed due to missing platform support.</exception>
        public DateTime ToUniversalTime(DateTime local)
        {
            if (!Wim.CommonConfiguration.IS_FULL_TRUST)
                return DateTime.UtcNow;

            SYSTEMTIME stLocal = DateTimeToSystemTime(local);

            TIME_ZONE_INFORMATION tziNative = TziNative();

            SYSTEMTIME stUTC;

            try
            {
                NativeMethods.TzSpecificLocalTimeToSystemTime(ref tziNative, ref stLocal, out stUTC);

                return SystemTimeToDateTime(ref stUTC);
            }
            catch (EntryPointNotFoundException e)
            {
                throw new NotSupportedException("This method is not supported on this operating system", e);
            }
        }


        /// <summary>
        /// Convert a time from the time zone with the supplied index to UTC.
        /// </summary>
        /// <param name="standardName">The time zone index.</param>
        /// <param name="local">The time to convert.</param>
        /// <returns>The converted time.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the index is not found.</exception>
        /// <exception cref="NotSupportedException">Thrown if the method failed due to missing platform support.</exception>
        public static DateTime ToUniversalTime(string standardName, DateTime local)
        {
            TimeZoneInformation tzi = FromIndex(standardName);

            return tzi.ToUniversalTime(local);
        }


        private static SYSTEMTIME DateTimeToSystemTime(DateTime dt)
        {
            SYSTEMTIME st;
            System.Runtime.InteropServices.ComTypes.FILETIME ft = new System.Runtime.InteropServices.ComTypes.FILETIME();

            ft.dwHighDateTime = (int)(dt.Ticks >> 32);
            ft.dwLowDateTime = (int)(dt.Ticks & 0xFFFFFFFFL);

            NativeMethods.FileTimeToSystemTime(ref ft, out st);

            return st;
        }


        private static DateTime SystemTimeToDateTime(ref SYSTEMTIME st)
        {
            System.Runtime.InteropServices.ComTypes.FILETIME ft = new System.Runtime.InteropServices.ComTypes.FILETIME();

            NativeMethods.SystemTimeToFileTime(ref st, out ft);

            DateTime dt = new DateTime((((long)ft.dwHighDateTime) << 32) | (uint)ft.dwLowDateTime);

            return dt;
        }

        private TZI m_tzi;
        private string m_name;
        private string m_displayName;
        //private int m_index;
        
        private string m_daylightName;
    }
}
