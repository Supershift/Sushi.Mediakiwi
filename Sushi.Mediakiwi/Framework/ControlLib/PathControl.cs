using System;
using System.ComponentModel;
using System.Data;
using System.Collections;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;

namespace Sushi.Mediakiwi.Framework.ControlLib
{
    /// <summary>
    /// 
    /// </summary>
    public enum TagType : short
    {
        /// <summary>
        /// 
        /// </summary>
        Script,
        /// <summary>
        /// 
        /// </summary>
        Link
    }

    /// <summary>
    /// 
    /// </summary>
    [ToolboxData("<{0}:PathControl runat='server' />")]
    public class PathControl : Control, IAttributeAccessor
    {

        /// <summary>
        /// 
        /// </summary>
        public PathControl() { }


        /// <summary>
        /// 
        /// </summary>
        [UrlProperty()]
        public string Path
        {
            get { return (ViewState["Path"] == null) ? "~/" : ViewState["Path"] as string; }
            set { ViewState["Path"] = value; }
        }

        public bool IncludeFileModification
        {
            get 
            {
                if (ViewState["IncludeFileModification"] == null)
                    return CommonConfiguration.USE_CSS_TICKS_FOR_PATHCONTROL;
                
                return (bool)ViewState["IncludeFileModification"];
            }
            set { ViewState["IncludeFileModification"] = value; }
        }

        /// <summary>
        /// Gets or sets the combined.
        /// </summary>
        /// <value>The combined.</value>
        public string Combined { get; set; }
        /// <summary>
        /// Gets or sets the combined folder.
        /// </summary>
        /// <value>The combined folder.</value>
        public string CombinedFolder { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public TagType TagType
        {
            get { return (ViewState["TagType"] == null) ? TagType.Link : (TagType)ViewState["TagType"]; }
            set { ViewState["TagType"] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Type
        {
            get { return (ViewState["Type"] == null) ? null : (string)ViewState["Type"]; }
            set { ViewState["Type"] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Attributes
        {
            get { return (ViewState["Attributes"] == null) ? null : (string)ViewState["Attributes"]; }
            set { ViewState["Attributes"] = value; }
        }

        Hashtable attribs = new Hashtable();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);

            RenderStartTag(writer);

            foreach (string key in attribs.Keys)
            {
                writer.WriteAttribute(key, (string)attribs[key]);
            }

            RenderEndTag(writer);
        }

        internal void DoCombine()
        {
            if (!Wim.CommonConfiguration.IS_LOCAL_DEVELOPMENT) return;
            string relative = Wim.Utility.AddApplicationPath(string.Concat(Path.Replace("~", "")));
            string path = HttpContext.Current.Server.MapPath(relative);

            long lastWrite = new System.IO.FileInfo(path).LastWriteTime.Ticks;
            
            bool hasChanged = false;

            string[] files = this.Combined.Split(',');
            foreach (string file in files)
            {
                string filepath = HttpContext.Current.Server.MapPath(Wim.Utility.AddApplicationPath(string.Concat(CombinedFolder.Replace("~", ""), file.Trim().Replace("~", ""))));
                long lastWrite2 = new System.IO.FileInfo(filepath).LastWriteTime.Ticks;
                if (lastWrite2 > lastWrite)
                    hasChanged = true;
            }
            if (!hasChanged)
            {
                if (System.IO.File.Exists(path))
                    return;
                else
                    hasChanged = true;
            }
            using (System.IO.TextWriter tw = new System.IO.StreamWriter(path))
            {
                foreach (string file in files)
                {
                    string filepath = HttpContext.Current.Server.MapPath(Wim.Utility.AddApplicationPath(string.Concat(CombinedFolder.Replace("~", ""), file.Trim().Replace("~", ""))));
                    string content = System.IO.File.ReadAllText(filepath);
                    tw.WriteLine(content);
                }
                tw.Close();
            }
            this.Path = relative;
        }

        void RenderStartTag(HtmlTextWriter writer)
        {
            if (!string.IsNullOrEmpty(this.Combined))
                DoCombine();

            if (TagType == TagType.Script)
            {
                string p = Wim.CommonConfiguration.APPLY_FIXED_PATH
                    ? Wim.Utility.AddApplicationPath(Path.Replace("~", ""))
                    : ResolveClientUrl(Path);

                if (IncludeFileModification && !p.Contains("?"))
                {
                    if (!Path.StartsWith("http://", StringComparison.InvariantCulture) && File.Exists(Path))
                    {
                        FileInfo fi = new FileInfo(Context.Server.MapPath(Path));
                        long ticks = 0;
                        if (fi.Exists)
                            ticks = fi.LastWriteTimeUtc.Ticks;
                        p += "?t=" + ticks.ToString();
                    }
                }

                writer.Write(string.Format("<script{1}{2} src=\"{0}\"",
                    p,
                    string.Format(" {0}", this.Attributes), 
                    this.Type == null 
                        ? "" 
                        : string.Format(" type=\"{0}\"", this.Type)
                        ));
            }
            else
            {
                string p = Wim.CommonConfiguration.APPLY_FIXED_PATH  
                    ? Wim.Utility.AddApplicationPath(Path.Replace("~", ""))
                    : ResolveClientUrl(Path)
                    ;

                if (IncludeFileModification && !p.Contains("?"))
                {
                    if (!Path.StartsWith("http://", StringComparison.InvariantCulture))
                    {
                        long ticks = 0;

                        // If Path is a physical Path, check for existence
                        if ((System.IO.Path.IsPathRooted(Path) && File.Exists(Path)) || !System.IO.Path.IsPathRooted(Path))
                        {
                            FileInfo fi = new FileInfo(Context.Server.MapPath(Path));
                            if (fi.Exists)
                                ticks = fi.LastWriteTimeUtc.Ticks;
                        }
                       
                        if (ticks > 0)
                            p += "?t=" + ticks.ToString();
                    }
                }

                writer.Write(string.Format("<link{1}{2} href=\"{0}\"",
                    //Path.Replace("~", "http://localhost//pretium_centraal/"),
                    p,
                    string.Format(" {0}", this.Attributes),
                    this.Type == null
                        ? ""
                        : string.Format(" type=\"{0}\"", this.Type)
                        ));
            }
        }

        void RenderEndTag(HtmlTextWriter writer)
        {

            if (TagType == TagType.Script)
            {
                writer.Write("></script>");
            }
            else
            {
                writer.Write(" />");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetAttribute(string key)
        {
            return attribs[key] as string;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetAttribute(string key, string value)
        {
            attribs[key] = value;
        }
    }
}