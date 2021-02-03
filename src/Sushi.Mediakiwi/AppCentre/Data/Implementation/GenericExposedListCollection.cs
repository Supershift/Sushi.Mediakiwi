using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation
{
    /// <summary>
    /// 
    /// </summary>
    public class GenericExposedListCollection : BaseImplementation
    {
        ListItemCollection m_Countries_EN;
        /// <summary>
        /// Gets the countries_ EN.
        /// </summary>
        /// <value>The countries_ EN.</value>
        [Sushi.Mediakiwi.Framework.ExposedListCollection("Countrylist (EN)")]
        public ListItemCollection Countries_EN
        {
            get
            {
                if (m_Countries_EN == null)
                {
                    m_Countries_EN = new ListItemCollection();

                    foreach (Sushi.Mediakiwi.Data.Country c in Sushi.Mediakiwi.Data.Country.SelectAll("en"))
                        m_Countries_EN.Add(new ListItem(c.Country_EN, c.ID.ToString()));
                }
                return m_Countries_EN;
            }
        }

        ListItemCollection m_Countries_NL;
        /// <summary>
        /// Gets the countries_ NL.
        /// </summary>
        /// <value>The countries_ NL.</value>
        [Sushi.Mediakiwi.Framework.ExposedListCollection("Countrylist (NL)")]
        public ListItemCollection Countries_NL
        {
            get
            {
                if (m_Countries_NL == null)
                {
                    m_Countries_NL = new ListItemCollection();

                    foreach (Sushi.Mediakiwi.Data.Country c in Sushi.Mediakiwi.Data.Country.SelectAll("nl"))
                        m_Countries_NL.Add(new ListItem(c.Country_NL, c.ID.ToString()));
                }
                return m_Countries_NL;
            }
        }

        ListItemCollection m_Gender_EN;
        /// <summary>
        /// Gets the gender_ EN.
        /// </summary>
        /// <value>The gender_ EN.</value>
        [Sushi.Mediakiwi.Framework.ExposedListCollection("Gender (EN)")]
        public ListItemCollection Gender_EN
        {
            get
            {
                if (m_Gender_EN == null)
                {
                    m_Gender_EN = new ListItemCollection();
                    m_Gender_EN.Add(new ListItem("Male", "M"));
                    m_Gender_EN.Add(new ListItem("Female", "F"));
                }
                return m_Gender_EN;
            }
        }

        ListItemCollection m_Gender_NL;
        /// <summary>
        /// Gets the gender_ EN.
        /// </summary>
        /// <value>The gender_ EN.</value>
        [Sushi.Mediakiwi.Framework.ExposedListCollection("Gender (NL)")]
        public ListItemCollection Gender_NL
        {
            get
            {
                if (m_Gender_NL == null)
                {
                    m_Gender_NL = new ListItemCollection();
                    m_Gender_NL.Add(new ListItem("Man", "M"));
                    m_Gender_NL.Add(new ListItem("Vrouw", "F"));
                }
                return m_Gender_NL;
            }
        }

        ListItemCollection m_YesNo_EN;
        /// <summary>
        /// Gets the gender_ EN.
        /// </summary>
        /// <value>The gender_ EN.</value>
        [Sushi.Mediakiwi.Framework.ExposedListCollection("Yes / No (EN)")]
        public ListItemCollection YesNo_EN
        {
            get
            {
                if (m_YesNo_EN == null)
                {
                    m_YesNo_EN = new ListItemCollection();
                    m_YesNo_EN.Add(new ListItem("Yes", "1"));
                    m_YesNo_EN.Add(new ListItem("No", "0"));
                }
                return m_YesNo_EN;
            }
        }

        ListItemCollection m_YesNo_NL;
        /// <summary>
        /// Gets the gender_ EN.
        /// </summary>
        /// <value>The gender_ EN.</value>
        [Sushi.Mediakiwi.Framework.ExposedListCollection("Yes / No (NL)")]
        public ListItemCollection YesNo_NL
        {
            get
            {
                if (m_YesNo_NL == null)
                {
                    m_YesNo_NL = new ListItemCollection();
                    m_YesNo_NL.Add(new ListItem("Ja", "1"));
                    m_YesNo_NL.Add(new ListItem("Nee", "0"));
                }
                return m_YesNo_NL;
            }
        }
    }
}