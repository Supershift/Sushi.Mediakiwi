using System;
using System.Web;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Sushi.Mediakiwi.Framework
{
    //public partial class Relink : IHttpModule
    //{
    //    public void Init2()
    //    {
    //         //Wire up our event handlers
    //        //m_Application.ResolveRequestCache += new System.EventHandler(this.ResolveRequestCache);
    //        //m_Application.UpdateRequestCache += new System.EventHandler(this.UpdateRequestCache);
    //    }

    //    //private Wim.Utilities.CacheItemManager m_cman = new Wim.Utilities.CacheItemManager();
    //    //string m_currentCall;
    //    private void ResolveRequestCache(object sender, System.EventArgs e)
    //    {
    //        //m_cman = new Wim.Utilities.CacheItemManager();
            
    //        #region gmc
    //        if (HttpContext.Current.Request.QueryString["gmc"] == "now")
    //        {
    //            IDictionaryEnumerator dict = HttpContext.Current.Cache.GetEnumerator();
    //            long total = 0;
    //            using (Wim.Utilities.CacheItemManager cman = new Wim.Utilities.CacheItemManager())
    //            {
    //                while (dict.MoveNext())
    //                {
    //                    Wim.Utilities.CacheItem item = cman.GetItem(dict.Key.ToString());
    //                    total += item.Length;
    //                    if (item.Length > 0)
    //                        HttpContext.Current.Response.Write(string.Format("{1}: {0}: {2} [META: {3}]<br/>", dict.Key, item.Created.ToString("dd-MM-yyyy hh:mm:ss"), item.Length, item.MetaData));
    //                }
    //            }
    //            long kb = total / 1024;
    //            long mb = kb / 1024;

    //            HttpContext.Current.Response.Write(string.Format("<br/><b>cache memory: {0} bytes", HttpContext.Current.Cache.EffectivePrivateBytesLimit));
    //            HttpContext.Current.Response.Write(string.Format("<br/><b>Total used memory: <br/>-{0} bytes<br/>-{1} Kb<br/>-{2} Mb</b><br/>", total, kb, mb));
    //            HttpContext.Current.Response.End();
    //        }
    //        #endregion
                
    //        //m_Application.Response.Write();
    //        if (CurrentPage == null)
    //        {
    //            SetStreamFilter(HttpContext.Current.Request.Path);
    //            return;
    //        }

    //        bool tryToFindInCache = CurrentPage.IsAddedOutputCache;
    //        //if (HttpContext.Current.Request.Params["nocache"] == "1" || HttpContext.Current.Request.IsLocal)
    //        if (HttpContext.Current.Request.Params["nocache"] == "1")
    //        {
    //            tryToFindInCache = false;
    //        }
    //        else
    //        {
    //            //  With postback do not save
    //            if (HttpContext.Current.Request.UrlReferrer != null && HttpContext.Current.Request.UrlReferrer.ToString() == CurrentPage.HRefFull)
    //                tryToFindInCache = false;
    //        }

    //        m_currentCall = string.Format("wim.page.{0} [{1}]", m_CurrentPage.Id, 
    //            Wim.Utility.HashString(HttpContext.Current.Request.RawUrl.ToLower()));

    //        // is the url in the cache?
    //        if (tryToFindInCache && m_cman.IsCached(m_currentCall))
    //        {
    //            // Write it back from the cache
    //            Wim.Utilities.CacheItem item = (Wim.Utilities.CacheItem)m_cman.GetItem(m_currentCall);

    //            m_Application.Response.Write(PartialCacheApply(item.Value.ToString(), item.MetaData));
    //            // Finish the request
    //            m_Application.CompleteRequest();
    //        }
    //        else
    //        {
    //            SetStreamFilter(CurrentPage.HRef);
    //        }
    //    }

    //    void SetStreamFilter(string href)
    //    {
    //        return;
    //        //  See m_Application_PreRequestHandlerExecute

    //        // Create a new filter
    //        OutputStream mStreamFilter = new OutputStream(m_Application.Response.Filter, href);
    //        // Insert it onto the page
    //        m_Application.Response.Filter = mStreamFilter;
    //        // Save a reference to the filter in the request context so we can grab it in UpdateRequestCache
    //        m_Application.Context.Items.Add("mStreamFilter", mStreamFilter);
    //    }

    //    private void UpdateRequestCache(object sender, System.EventArgs e)
    //    {
    //        if (CurrentPage == null || !CurrentPage.IsAddedOutputCache)
    //            return;

    //        if (!m_cman.IsCached(m_currentCall))
    //        {
    //            // Grab the CacheStream out of the context
    //            OutputStream mStreamFilter = (OutputStream)m_Application.Context.Items["mStreamFilter"];
    //            // Remove the reference to the filter
    //            m_Application.Context.Items.Remove("mStreamFilter");
    //            // Create a buffer
    //            byte[] bBuffer = new byte[mStreamFilter.Length];
    //            // Rewind the stream
    //            mStreamFilter.Position = 0;
    //            // Get the bytes
    //            mStreamFilter.Read(bBuffer, 0, (int)mStreamFilter.Length);
    //            // Convert to a string
    //            System.Text.UTF8Encoding utf8 = new System.Text.UTF8Encoding();
    //            System.Text.StringBuilder strBuff = new System.Text.StringBuilder(utf8.GetString(bBuffer));
    //            // Save it away

    //            string output = PartialCacheReplacementTag(strBuff.ToString());
    //            m_cman.AddPage(m_currentCall, output, mStreamFilter.Length, m_Application.Context.Items["Wim.NoCache"], DateTime.Now.AddHours(24));
    //        }
    //    }

    //    string PartialCacheApply(string buffer, string meta)
    //    {
    //        if (meta == null) return buffer;

    //        string[] contextInfo = meta.Split(',');

    //        Guid guid2;
    //        string candidate = buffer;
    //        Regex strip = new Regex(@"(?<=(<component.*>))(?<TEXT>.*)(?=(</component>))", System.Text.RegularExpressions.RegexOptions.Singleline);

    //        Sushi.Mediakiwi.Data.Environment env = Sushi.Mediakiwi.Data.Environment.SelectOne();
    //        string url = string.Format("{0}/framework/no-cache.aspx", env.Url);

    //        foreach (string guid in contextInfo)
    //        {
    //            if (Wim.Utility.IsGuid(guid, out guid2))
    //            {
    //                Regex rex = new Regex(string.Format(@"<{0}/>", guid2.ToString()), RegexOptions.Singleline);
    //                string urlData = Wim.Utilities.WebScrape.GetUrlResponse(string.Format("{0}?p={1}&c={2}", url, this.m_CurrentPage.Id, guid.ToString()));
    //                candidate = rex.Replace(candidate, strip.Match(urlData).Value);
    //            }
    //        }
    //        return candidate;
    //    }

    //    string PartialCacheReplacementTag(string buffer)
    //    {
    //        if (!m_Application.Context.Items.Contains("Wim.NoCache")) return buffer;
    //        string[] contextInfo = m_Application.Context.Items["Wim.NoCache"].ToString().Split(',');

    //        Guid guid2;
    //        string candidate = buffer;
    //        foreach (string guid in contextInfo)
    //        {
    //            if (Wim.Utility.IsGuid(guid, out guid2))
    //            {
    //                Regex rex = new Regex(string.Format(@"<{0}>.*</{0}>", guid2.ToString()), RegexOptions.Singleline);
    //                candidate = rex.Replace(candidate, string.Format("<{0}/>", guid2.ToString()));
    //            }
    //        }
    //        return candidate;
    //    }
    //}
}