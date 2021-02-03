using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation.Forms
{
    public class SiteForm : FormMap<Mediakiwi.Data.Site>
    {
        public SiteForm(Mediakiwi.Data.Site implement)
        {
            Load(implement as Mediakiwi.Data.Site);

            Map(x => x.Name).TextField("Title", 50, true).Expression(OutputExpression.Alternating);
            Map(x => x.IsActive).Checkbox("Active").Expression(OutputExpression.Alternating);
            Map(x => x.CountryID).Dropdown("Country", nameof(SiteForm.AvailableCountries), false).Expression(OutputExpression.Alternating); ;
            Map(x => x.TimeZoneIndex).Dropdown("Timezone", nameof(SiteForm.AvailableTimeZones), false).Expression(OutputExpression.Alternating); ;
            Map(x => x.Language).Dropdown("Language", nameof(SiteForm.AvailableCulturesCollection), false).Expression(OutputExpression.Alternating); ;
            Map(x => x.Culture).Dropdown("Culture", nameof(SiteForm.AvailableCulturesCollection), false).Expression(OutputExpression.Alternating); ;
            Map(x => x.MasterID).Dropdown("Inherit from", nameof(SiteForm.AvailableSitesCollection), false);
            
            Map<SiteForm>(x => x.Section1, this).Section("Settings");

            Map(x => x.HasLists).Checkbox("Lists").Expression(OutputExpression.Alternating);
            Map(x => x.HasPages).Checkbox("Pages").Expression(OutputExpression.Alternating);
        }

        public string Section1 { get; set; }


        private ListItemCollection m_AvailableCountries;
        /// <summary>
        /// Gets the available countries.
        /// </summary>
        /// <value>The available countries.</value>
        public ListItemCollection AvailableCountries
        {
            get
            {
                if (m_AvailableCountries == null)
                {
                    m_AvailableCountries = new ListItemCollection();
                    m_AvailableCountries.Add(new ListItem("", ""));
                    foreach (Country country in Country.SelectAll(false, "en"))
                    {
                        m_AvailableCountries.Add(new ListItem(country.Country_EN, country.ID.ToString()));
                    }
                }
                return m_AvailableCountries;
            }
        }

        private ListItemCollection m_AvailableTimeZones;
        /// <summary>
        /// Gets the available time zones.
        /// </summary>
        /// <value>The available time zones.</value>
        //public ListItemCollection AvailableTimeZones
        //{
        //    get
        //    {
        //        if (m_AvailableTimeZones == null)
        //        {
        //            m_AvailableTimeZones = new ListItemCollection();
        //            m_AvailableTimeZones.Add(new ListItem("", ""));
        //            try
        //            {
        //                //Wim.Utilities.TimeZoneInformation tz = Utilities.TimeZoneInformation.CurrentTimeZone;
        //                //Wim.Utilities.TimeZoneInformation.FromIndex("").FromUniversalTime()
        //                foreach (Wim.Utilities.TimeZoneInformation tz in Wim.Utilities.TimeZoneInformation.EnumZones())
        //                {
        //                    m_AvailableTimeZones.Add(new ListItem(tz.DisplayName, tz.Index));
        //                }
        //            }
        //            catch (Exception) { }
        //        }
        //        return m_AvailableTimeZones;
        //    }
        //}
        public ListItemCollection AvailableTimeZones
        {
            get
            {
                if (m_AvailableTimeZones == null)
                {
                    m_AvailableTimeZones = new ListItemCollection();
                    m_AvailableTimeZones.Add(new ListItem("", ""));
                    try
                    {
                        foreach (var tz in System.TimeZoneInfo.GetSystemTimeZones())
                        {
                            m_AvailableTimeZones.Add(new ListItem(tz.DisplayName, tz.Id));
                        }
                    }
                    catch (Exception) { }
                }
                return m_AvailableTimeZones;
            }
        }


        /// <summary>
        /// Gets the cultures collection.
        /// </summary>
        /// <param name="mostCultureTypes">The most culture types.</param>
        /// <param name="onlyReturnNeutral">if set to <c>true</c> [only return neutral].</param>
        /// <returns></returns>
        ListItemCollection GetCulturesCollection(CultureTypes[] mostCultureTypes, bool onlyReturnNeutral)
        {
            ListItemCollection collection = new ListItemCollection();
            collection.Add(new ListItem("", ""));

            SortedList<string, string> list = new SortedList<string, string>();

            CultureInfo[] allCultures;
            foreach (CultureTypes ct in mostCultureTypes)
            {
                allCultures = CultureInfo.GetCultures(ct);
                foreach (CultureInfo ci in allCultures)
                {
                    if (onlyReturnNeutral && !ci.IsNeutralCulture) continue;
                    list.Add(ci.EnglishName, ci.Name);
                }
            }

            IDictionaryEnumerator enumerator = (IDictionaryEnumerator)list.GetEnumerator();
            while (enumerator.MoveNext())
            {
                collection.Add(new ListItem(enumerator.Key.ToString(), enumerator.Value.ToString()));
            }
            return collection;
        }

        private ListItemCollection m_AvailableCultures;
        /// <summary>
        /// Gets the available cultures collection.
        /// </summary>
        /// <value>The available cultures collection.</value>
        public ListItemCollection AvailableCulturesCollection
        {
            get
            {
                if (m_AvailableCultures == null)
                    m_AvailableCultures = GetCulturesCollection(new CultureTypes[] { CultureTypes.SpecificCultures }, false);
                return m_AvailableCultures;
            }
        }

        private ListItemCollection m_AvailableLanguageCollection;
        /// <summary>
        /// Gets the available language collection.
        /// </summary>
        /// <value>The available language collection.</value>
        public ListItemCollection AvailableLanguageCollection
        {
            get
            {
                if (m_AvailableLanguageCollection == null)
                    m_AvailableLanguageCollection = GetCulturesCollection(new CultureTypes[] { CultureTypes.AllCultures }, true);
                return m_AvailableLanguageCollection;
            }
        }

        bool m_InheritenceIsSet;
        /// <summary>
        /// Gets a value indicating whether [inheritence is set].
        /// </summary>
        /// <value><c>true</c> if [inheritence is set]; otherwise, <c>false</c>.</value>
        public bool InheritenceIsSet
        {
            get { return m_InheritenceIsSet; }
        }

        private ListItemCollection m_AvailableSites;
        /// <summary>
        /// Gets the available sites collection.
        /// </summary>
        /// <value>The available sites collection.</value>
        public ListItemCollection AvailableSitesCollection
        {
            get
            {
                if (m_AvailableSites == null)
                {
                    m_AvailableSites = new ListItemCollection();
                    m_AvailableSites.Add(new ListItem(string.Empty));

                    foreach (var site in Mediakiwi.Data.Site.SelectAll())
                    {
                        if (site.ID == this.Instance.ID)
                            continue;

                        // do not include administration
                        if (site.Type == 1)
                            continue;


                        m_AvailableSites.Add(new ListItem(site.Name, site.ID.ToString()));
                    }
                }
                return m_AvailableSites;
            }
        }
    }
}
