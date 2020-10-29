using System;
using System.Collections;
using System.Net.Mail;

namespace Sushi.Mediakiwi.Data
{
    public interface IEnvironment
    {
        bool Save();
        string this[string registry] { get; }
        string this[string registry, bool ifNotPresentAdd, string defaultValue, string description] { get; }

        DateTime Created { get; set; }
        DateTime CurrentTimezoneDateTime { get; }
        string DefaultMailAddress { get; set; }
        int? DefaultSiteID { get; set; }
        string DisplayName { get; set; }
        string Domain { get; set; }
        string ErrorMailAddress { get; set; }
        int ID { get; set; }
        int? LogoLight { get; set; }
        string Password { get; set; }
        //string Path { get; }
        string RelativePath { get; set; }
        string Repository { get; set; }
        string RepositoryFolder { get; set; }
        string Secret { get; }
        bool SmtpEnableSSL { get; set; }
        string SmtpServer { get; set; }
        string SmtpServerPass { get; set; }
        string SmtpServerUser { get; set; }
        string Timezone { get; set; }
        string Title { get; set; }
        DateTime UpdateInfo { get; set; }
        string Url { get; }
        decimal Version { get; set; }

        string GetRegistryValue(string registry, string defaultValue);
        SmtpClient SmtpClient();
        string LogoHrefFull { get; }
    }
}