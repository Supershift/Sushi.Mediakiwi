using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation.Forms
{
    public class EnvironmentForm : FormMap<Mediakiwi.Data.Environment>
    {
        public EnvironmentForm(Mediakiwi.Data.IEnvironment implement)
        {
            Load(implement as Mediakiwi.Data.Environment);
            Map(x => x.Title).TextField("Name", 50, true);
            Map(x => x.DisplayName).TextField("Displayname", 50, true);
            Map(x => x.Timezone).Dropdown("Dropdown", "AvailableTimeZones", true);
            Map(x => x.LogoLight).Image("Logo");
            Map(x => x.Version).TextLine("Version");

            Map<EnvironmentForm>(x => x.Section1, this).Section("SMTP Settings");
            Map(x => x.SmtpServer).TextField("Server", 250, true).Expression(OutputExpression.Alternating);
            Map(x => x.SmtpServerUser).TextField("User", 250, true).Expression(OutputExpression.Alternating);
            Map(x => x.SmtpEnableSSL).Checkbox("SSL").Expression(OutputExpression.Alternating);
            Map(x => x.SmtpServerPass).TextField("Password", 250, true).Expression(OutputExpression.Alternating).Password();

            Map(x => x.DefaultMailAddress).TextField("From email", 255, true).Expression(OutputExpression.Alternating);
            Map(x => x.ErrorMailAddress).TextField("Error email", 255, true).Expression(OutputExpression.Alternating);
        }

        public string Section1 { get; set; }

        private ListItemCollection m_AvailableTimeZones;
        /// <summary>
        /// Gets the available time zones.
        /// </summary>
        /// <value>The available time zones.</value>
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

        private ListItemCollection m_Sites;
        /// <summary>
        /// Gets the sites.
        /// </summary>
        /// <value>The sites.</value>
        public ListItemCollection Sites
        {
            get
            {
                if (m_Sites != null) return m_Sites;

                m_Sites = new ListItemCollection();
                foreach (Sushi.Mediakiwi.Data.Site site in Sushi.Mediakiwi.Data.Site.SelectAll())
                {
                    if (site.HasPages)
                        m_Sites.Add(new ListItem(site.Name, site.ID.ToString()));
                }
                return m_Sites;
            }
        }
    }
}
