using System;
using System.Globalization;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Threading;

namespace Sushi.Mediakiwi.AppCentre.Data.Supporting
{
    /// <summary>
    /// 
    /// </summary>
    public class LocalDateTime
    {
        /// <summary>
        /// Gets the specified UTC.
        /// </summary>
        /// <param name="utc">The UTC.</param>
        /// <returns></returns>
        public static string Get(DateTime utc)
        {
            return Get(utc, null);
        }

        /// <summary>
        /// Gets the specified UTC.
        /// </summary>
        /// <param name="utc">The UTC.</param>
        /// <param name="currentSite">The current site.</param>
        /// <returns></returns>
        public static string Get(DateTime utc, Sushi.Mediakiwi.Data.Site currentSite)
        {
            DateTime dateTime = GetDate(utc, currentSite);
            if (utc.Hour == 0 && utc.Minute == 0 && utc.Second == 0 && utc.Millisecond == 0)
            {
                if (utc == DateTime.MinValue) return null;
                return utc.ToShortDateString();
            }
            if (utc == DateTime.MinValue) return null;
            return utc.ToString();
        }

        /// <summary>
        /// Gets the specified UTC.
        /// </summary>
        /// <param name="utc">The UTC.</param>
        /// <param name="currentSite">The current site.</param>
        /// <returns></returns>
        //public static DateTime GetDate(DateTime utc, Sushi.Mediakiwi.Data.Site currentSite)
        //{
        //    if (utc == DateTime.MinValue) return utc;

        //    System.Globalization.CultureInfo info = Thread.CurrentThread.CurrentCulture;
        //    if (currentSite != null && !string.IsNullOrEmpty(currentSite.Culture))
        //    {
        //        info = new CultureInfo(currentSite.Culture);
        //        Thread.CurrentThread.CurrentCulture = info;

        //        if (currentSite.TimeZoneIndex == "GMT Standard Time" || currentSite.TimeZoneIndex == "W. Europe Standard Time" || currentSite.TimeZoneIndex == "Central Europe Standard Time" || currentSite.TimeZoneIndex == "Central European Standard Time")
        //        {
        //            //  Europe
        //            DaylightTime dlt = GetEuropeanDst(utc.Year);
        //            if (dlt.Start.Ticks <= DateTime.UtcNow.Ticks && dlt.End.Ticks >= DateTime.UtcNow.Ticks)
        //                utc = utc.AddMinutes(currentSite.TimeZone.DaylightBias);
        //            else
        //                utc = utc.AddMinutes(currentSite.TimeZone.StandardBias);
        //        }
        //        else
        //            utc = utc.AddMinutes(currentSite.TimeZone.StandardBias);

        //    }
        //    return utc;
        //}

        public static DateTime GetDate(DateTime utc, Sushi.Mediakiwi.Data.Site currentSite)
        {
            return GetDate(utc, currentSite, false);
        }

        /// <summary>
        /// Gets the Date
        /// </summary>
        /// <param name="utc"></param>
        /// <param name="currentSite"></param>
        /// <param name="specifyKind"></param>
        /// <returns></returns>
        public static DateTime GetDate(DateTime utc, Sushi.Mediakiwi.Data.Site currentSite, bool specifyKind)
        {
            if (utc == DateTime.MinValue) return utc;

            if (currentSite != null)
                utc = TimeZoneInfo.ConvertTimeFromUtc(utc, currentSite.TimeZone);

            if (specifyKind)
                utc = DateTime.SpecifyKind(utc, DateTimeKind.Local);

            return utc;
        }


        static System.Globalization.DaylightTime m_EuropeanDlt;
        static System.Globalization.DaylightTime GetEuropeanDst(int year)
        {
            if (m_EuropeanDlt == null)
            {
                DateTime dstStart = new DateTime(year, 4, 1, 1, 0, 0).AddDays(-1);
                while (dstStart.DayOfWeek != DayOfWeek.Sunday) dstStart = dstStart.AddDays(-1);

                DateTime dstEnd = new DateTime(year, 11, 1, 1, 0, 0).AddDays(-1);
                while (dstEnd.DayOfWeek != DayOfWeek.Sunday) dstEnd = dstEnd.AddDays(-1);

                m_EuropeanDlt = new System.Globalization.DaylightTime(dstStart, dstEnd, new TimeSpan(dstEnd.Ticks - dstStart.Ticks));
            }
            return m_EuropeanDlt;
        }
    }
}
