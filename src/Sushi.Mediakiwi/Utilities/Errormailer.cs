using System;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Net.Mail;

namespace Wim.Utilities
{
    /// <summary>
    /// 
    /// </summary>
    public class Errormailer
    {
        SmtpClient m_Client;
        private System.Web.UI.Page m_page;
        private System.Exception m_exception;
        private string m_server;
        private string m_from;
        private string m_to;
        private string m_subject;
        private string m_body;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        public Errormailer(System.Web.UI.Page page, string from, string to, string subject) 
            : this( page, "127.0.0.1", from, to, subject)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="smtpserver"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        public Errormailer(System.Web.UI.Page page, string smtpserver, string from, string to, string subject)
            : this ( page, smtpserver, from, to, subject, page.Server.GetLastError() )
        { }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="smtpserver"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="exception"></param>
        public Errormailer(System.Web.UI.Page page, string smtpserver, string from, string to, string subject, System.Exception exception)
        {
            this.m_page = page;
            this.m_exception = exception;
            this.m_server = smtpserver;
            this.m_from = from;
            this.m_to = to;
            this.m_subject = subject;
            this.SetMailBody();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Errormailer"/> class.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="client">The client.</param>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="subject">The subject.</param>
        public Errormailer(System.Web.UI.Page page, SmtpClient client, string from, string to, string subject)
        {
            this.m_page = page;
            this.m_exception = page.Server.GetLastError();
            this.m_Client = client;
            this.m_from = from;
            this.m_to = to;
            this.m_subject = subject;
            this.SetMailBody();
        }

        private string GetCollectionItems( System.Collections.Specialized.NameValueCollection postItem )
        {
            string postValue = string.Empty;
            for ( int i = 0; i < postItem.Count; i++ )
            {
                postValue += string.Format( "<tr><th>{0}</th><td>{1}</td></tr>", postItem.Keys[i], postItem[i] );
            }
            postValue = string.Format( "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\">{0}</table>", postValue );
            return postValue;
        }

        private string GetCollectionItems( System.Web.HttpCookieCollection postItem )
        {
            string postValue = string.Empty;
            for ( int i = 0; i < postItem.Count; i++ )
            {
                postValue += string.Format( "<tr><th>{0}</th><td>{1}</td></tr>", postItem.Keys[i], postItem[i].Value );
            }
            postValue = string.Format( "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\">{0}</table>", postValue );
            return postValue;
        }


        private void SetMailBody()
        {
            System.Exception innerEx = m_exception.InnerException;

            Sushi.Mediakiwi.Data.Page currentWimEnabledPage = null;
            if (System.Web.HttpContext.Current != null)
            {
                if (System.Web.HttpContext.Current.Items["Wim.Page"] != null)
                    currentWimEnabledPage = System.Web.HttpContext.Current.Items["Wim.Page"] as Sushi.Mediakiwi.Data.Page;
            }

            string vistorCookieLog = null;

            try
            {
                vistorCookieLog = Sushi.Mediakiwi.Data.Identity.Visitor.Select().CookieParserLog;
            }
            catch (Exception ex){
                vistorCookieLog = ex.Message;
            }

            this.m_body = 
                string.Concat(
                "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\" ><HTML><HEAD>",
                "<style type=\"text/css\">",
                "body { PADDING: 10px; MARGIN: 0px; }",
                "th { color : #64605f; FONT-FAMILY:Arial,Helvetica,sans-serif; FONT-SIZE: 12px; background-color : #e9e4e3; border-bottom : solid 0.2em #f3eeed; white-space : nowrap; padding : 4px; text-align : left; vertical-align : top; }", 
                "td { color : #1d0a63;border-bottom : solid 0.2em #f3eeed; padding : 4px; FONT-FAMILY:Arial,Helvetica,sans-serif; FONT-SIZE: 12px;}",
                "</style>",
                "</HEAD><body><table border=\"0\" cellpadding=\"0\" cellspacing=\"0\">",
                "<tr><th>Machine:</th><td>", System.Environment.MachineName, "</td></tr>",
                "<tr><th>URL:</th><td>", m_page.Request.Url.ToString(), "</td></tr>",
                currentWimEnabledPage == null ? "" : string.Format("<tr><th>URL (Wim):</th><td>{0}</td></tr>", string.Concat(currentWimEnabledPage.LinkText, " (", currentWimEnabledPage.ID, ")<br/>", currentWimEnabledPage.HRefFull)),
                "<tr><th>URL Referrer:</th><td>", m_page.Request.UrlReferrer == null ? "none" : m_page.Request.UrlReferrer.ToString() ,"</td></tr>",
                "<tr><th>Visitors agent:</th><td>", m_page.Request.Headers["User-Agent"], "</td></tr>",
                "<tr><th>Pageclass:</th><td>", m_page.ToString(), "</td></tr>",
                "<tr><th>Exception:</th><td>", Wim.Utility.GetHtmlFormattedLastServerError(m_exception), "</td></tr>",
                "<tr><th>Visitor cookie log:</th><td>", vistorCookieLog, "</td></tr>",
                "<tr><th>Cookie collection:</th><td>", this.GetCollectionItems(m_page.Request.Cookies), "</td></tr>",
                "<tr><th>Querystring collection:</th><td>", this.GetCollectionItems( m_page.Request.QueryString ), "</td></tr>",
                "<tr><th>Form collection:</th><td>", this.GetCollectionItems( m_page.Request.Form ), "</td></tr>",
                "<tr><th>Server varables:</th><td>", this.GetCollectionItems( m_page.Request.ServerVariables ), "</td></tr>",
                "</table>",
                "</body></HTML>"
                );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Send()
        {
            this.SendHtmlEmail();
            return true;
        }

        #region SendHtmlEmail
        private void SendHtmlEmail()
        {
            MailMessage mail = new MailMessage();

            foreach (string toAddress in m_to.Split(';'))
                mail.To.Add(toAddress);

            mail.From = new MailAddress(m_from);
            mail.Subject = this.m_subject;
            mail.Body = this.m_body;
            mail.IsBodyHtml = true;
            mail.BodyEncoding = Encoding.UTF8;

            if (m_Client == null)
            {
                SmtpClient smtp = new SmtpClient(m_server);
                smtp.Send(mail);
            }
            else
                m_Client.Send(mail);
        }
        #endregion SendHtmlEmail
    }
}