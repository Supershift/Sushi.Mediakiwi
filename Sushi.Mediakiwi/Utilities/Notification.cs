using System.Collections;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Sushi.Mediakiwi.Data;
using System.Net;
using System.Net.Mail;
using System.Collections.Specialized;

namespace Sushi.Mediakiwi.Utilities
{
    /// <summary>
    /// 
    /// </summary>
    public class Mail
    {
        public class Payload
        {
            public Payload()
            {
                this.From = new MailAddress(Sushi.Mediakiwi.Data.Environment.Current.DefaultMailAddress, Sushi.Mediakiwi.Data.Environment.Current.DisplayName);
            }

            public MailAddress From { set; get; }
            public List<MailAddress> To { set; get; }
            public List<MailAddress> BCC { set; get; }
            public string Title { get; set; }
            public string Subject { get; set; }
            public bool HideHeader { get; set; }
            public string Body { get; set; }
            /// <summary>
            /// When true, the body will not be nested within an existing standard mail.
            /// </summary>
            public bool ReplaceFullBody { get; set; }
            public string URL { get; set; }
            public NameValueCollection ReplacementPlaceholders { get; set; }
        }

        #region Stylesheet
        static string m_Style = @"
            table {
				font-size : 100%;
				border-collapse : collapse;
				margin : 0px 0px 1em 0px;
				font-family : Arial, Sans-Serif; 
				font-size : 11px;
				width:590px; 
				color : #000000;
			}
				caption {
					font-size :	110%;
					font-weight : bold;
					text-align : left;	
				}
				tr {
					vertical-align : top;
				}
					th {
						text-align : left;
						font-weight : bold;
					}
					td {}
			table.form,
			table.data {
				width : 100%;
				margin : 1em 0px 1em 0px;
				font-family : Arial, Sans-Serif; 
				font-size : 11px;
				color : #000000;
			}
				table.form thead {}
					table.form thead tr {}
						table.form thead tr th {
							background : #000000 none;
							border-left : solid 1px #ffffff;
							border-right : none;
							color : #ffffff;
							padding : 3px 10px 3px 10px;
						}
				table.form tbody {}
					table.form tbody tr {}
						table.form tbody tr th {
							background-color : #949494;
							border : solid 1px #cacaca;
							color : #ffffff;
							width : 120px;								
							padding : 3px 10px 3px 10px
						}
							table.form tbody tr th label {
								color : #ffffff;
							}
						table.form tbody tr td {
							background-color : #f5f6f8;
							border : solid 1px #cacaca;
							padding : 5px;
						}
			table.data {}
				table.data tbody {}
					table.data tr {}
						table.data tr td {
							border : solid 1px #cfcfcf;
							color : #616161;
							padding : 3px 10px 3px 10px;
						}
							table.data tr td a {}
								table.data tr td a:link,
								table.data tr td a:visited {
									color : #616161;
									text-decoration : underline;
								}
								table.data tr td a:hover,
								table.data tr td a:active {
									color : #000000;
									text-decoration : underline;
								}
					table.data tr.odd {}
						table.data tr.odd td {}
					table.data tr.even {}
						table.data tr.even td {
							background-color : #eceef1;
						}
					table.data tr.link {}
						table.data tr.link td {
							border : solid 1px #cfcfcf;
							padding : 3px 10px 3px 10px;
						}
				table.data thead,
				table.data tfoot {}
					table.data thead tr,
					table.data tfoot tr {}
						table.data thead tr th,
						table.data tfoot tr td {
							background : #000000 none;
							border-left : solid 1px #ffffff;
							border-right : none;
							color : #ffffff;
							padding : 3px 10px 3px 10px;
						}";
        #endregion

        /// <summary>
        /// Sends the specified to.
        /// </summary>
        /// <param name="to">To.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        public static void Send(MailAddress to, string subject, string body)
        {
            Send(to, subject, body, null);
        }

        /// <summary>
        /// Sends the notification.
        /// </summary>
        /// <param name="to">To.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <param name="url">The URL.</param>
        public static void Send(MailAddress to, string subject, string body, string url)
        {
            var list = new List<MailAddress>();
            list.Add(to);
            Send(list, subject, body, url);
        }

        /// <summary>
        /// Sends the specified to.
        /// </summary>
        /// <param name="to">To.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <param name="url">The URL.</param>
        public static void Send(List<MailAddress> to, string subject, string body, string url)
        {
            Payload p = new Payload();
            p.To = to;
            p.Subject = subject;
            p.Body = body;
            p.URL = url;
            Send(p);
        }

        ///// <summary>
        ///// Sends the notification.
        ///// </summary>
        ///// <param name="subject">The subject.</param>
        ///// <param name="body">The body.</param>
        ///// <param name="email">The email.</param>
        //public static void Send(string subject, string body, string[] email)
        //{
        //    Send(subject, body, null, null, null, null, email);
        //}

        ///// <summary>
        ///// Sends the notification.
        ///// </summary>
        ///// <param name="subject">The subject.</param>
        ///// <param name="body">The body.</param>
        ///// <param name="fieldListTitle">The field list title.</param>
        ///// <param name="fieldList">The field list.</param>
        ///// <param name="dataList">The data list.</param>
        ///// <param name="url">The URL.</param>
        ///// <param name="emailaddresses">The emailaddresses.</param>
        //public static void Send(string subject, string body, string fieldListTitle, System.Collections.Specialized.NameValueCollection fieldList, IList dataList, string url, string[] emailaddresses)
        //{
        //}

        public static void Send(Payload payload)
        {
            var environment = Environment.Current;

            MailMessage message = new MailMessage();
            message.From = new MailAddress(environment.DefaultMailAddress, environment.DisplayName);

            string fieldForm = null;
            //if (payload.ReplacementPlaceholders != null)
            //{
            //    StringBuilder build = new StringBuilder();
            //    build.AppendFormat(@"<table class=""form""><thead><tr><th colspan=""2"">{0}</th></tr></thead><tbody>", fieldListTitle);
            //    build.Append("<tbody>");

            //    foreach (string key in fieldList.Keys)
            //        build.AppendFormat("<tr><th><label>{0}:</label></th><td>{1}</td></tr>", key, fieldList[key]);

            //    build.Append("</tbody></table>");
            //    fieldForm = build.ToString();
            //}

            if (payload.To != null)
            {
                foreach (var item in payload.To)
                    message.To.Add(item);
            }

            if (payload.BCC != null)
            {
                foreach (var item in payload.BCC)
                    message.Bcc.Add(item);
            }

            payload.HideHeader = true;

            message.Subject = payload.Subject;
            message.IsBodyHtml = true;
            message.BodyEncoding = Encoding.UTF8;
            if (payload.ReplaceFullBody)
                message.Body = payload.Body;
            else
                message.Body = string.Format(@"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01 Transitional//EN"" ""http://www.w3.org/TR/html4/loose.dtd"">
<html>
	<head>
		<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />
	</head>
	<body marginheight=""4"" marginwidth=""4"" link=""#1a171b"" alink=""#1a171b"" vlink=""#1a171b"">
		<table cellspacing=""0"" cellpadding=""0"" width=""100%"" border=""0"" style=""font-family: Arial;font-size: 12px;color:#555555;text-align: left; width:100%"">{5}
			<tr>
				<td bgcolor=""#ffffff"" colspan=""2"" width=""100%"" style=""padding:20px; margin: 0"">
					<font size=""4"" color=""1a1a1a""><strong>{1}</strong></font>
					<br /><br />
					{2}{4}
				</td>
			</tr>
		</table>
	</body>
</html>"
                , m_Style
                , string.IsNullOrEmpty(payload.Title) ? payload.Subject : payload.Title
                , payload.Body
                , payload.URL
                , fieldForm
                , payload.HideHeader ? null : string.Format(@"
			<tr>
				<td bgcolor=""#1a1a1a"" width=""100%"" height=""100"" style=""padding:0px 20px 0px 20px""><a href=""{1}""><img border=""0"" src=""{0}"" /></a> <br></td>
			</tr>
", string.Empty//Environment.Current.LogoHrefFull
, payload.URL)
                );

            Sushi.Mediakiwi.Data.Environment.Current.SmtpClient().Send(message);
        }
    }
}
