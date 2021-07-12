using System;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data
{
    public interface IEnvironment
    {
        DateTime Created { get; set; }
        string DefaultMailAddress { get; set; }
        int? DefaultSiteID { get; set; }
        string DisplayName { get; set; }
        string ErrorMailAddress { get; set; }
        int ID { get; set; }
        string Password { get; set; }
        string Repository { get; set; }
        string RepositoryFolder { get; set; }
        string Secret { get; }
        bool SmtpEnableSSL { get; set; }
        string SmtpServer { get; set; }
        string SmtpServerPass { get; set; }
        string SmtpServerUser { get; set; }
        string Timezone { get; set; }
        string Title { get; set; }
        string LogoHrefFull { get; set; }

        DateTime UpdateInfo { get; set; }
        decimal Version { get; set; }
        bool Save();

        Task<bool> SaveAsync();

        SmtpClient SmtpClient();

        DateTime CurrentTimezoneDateTime { get; }
    }
}