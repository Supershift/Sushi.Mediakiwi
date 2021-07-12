using Sushi.Mediakiwi.Framework.Api;
using Sushi.Mediakiwi.UI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Sushi.Mediakiwi.Framework
{
    public enum CanvasType
    {
        List = 0,
        ListInLayer = 1,
        Explorer = 3,
        ListItem = 4,
        ListItemInLayer = 5
    }

    public class CanvasData
    {
        public CanvasData() 
        { 
            this.Dashboard = new CanvasControlBuilder();
            this.LeftNavigation = new CanvasControlBuilder();
        }
        // 
        public CanvasType Type { get; set; }
        public CanvasControlBuilder Dashboard { get; set; }
        public CanvasControlBuilder LeftNavigation { get; set; }
    }
    public class CanvasControl
    {
        public CanvasControl() {}
        public string Format { get; set; }
        public object[] Data { get; set; }
        public CanvasControl(string format, params object[] data)
        {
            this.Format = format;
            this.Data = data;
        }
    }
    public class CanvasControlBuilder
    {
        StringBuilder _Build;
        List<CanvasControl> _ControlList;

        public CanvasControlBuilder() { _ControlList = new List<CanvasControl>(); }

        public void Append(string format)
        {
            if (format != null)
                _ControlList.Add(new CanvasControl(format, null));
        }

        public void AppendFormat(string format, params object[] data)
        {
            if (format != null && data != null)
                _ControlList.Add(new CanvasControl(format, data));
        }
        public bool Hide { get; set; }
        public override string ToString()
        {
            _Build = new StringBuilder();
            _ControlList.ForEach(x => { if (x.Data == null) _Build.Append(x.Format); else _Build.AppendFormat(x.Format, x.Data); });
            return _Build.ToString();
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class WimControlBuilder
    {
        /// <summary>
        /// Inform that hte control builder process has been terminated (mostly be a redirect).
        /// </summary>
        public bool IsTerminated { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WimControlBuilder"/> class.
        /// </summary>
        public WimControlBuilder()
        {
            m_Build = new StringBuilder();
            m_Cloaked = new StringBuilder();
            m_ControlList = new List<IControl>();
            Canvas = new CanvasData();
            //Data = new List<MediakiwiField>();
            ApiResponse = new MediakiwiApiFormResponse();
            ApiResponse.Fields = new List<MediakiwiField>();
            ApiResponse.Buttons = new List<MediakiwiField>();
            ApiResponse.Notifications = new List<MediakiwiNotification>();
        }

        public MediakiwiApiFormResponse ApiResponse { get; set; }

        public CanvasData Canvas { get; set; }

        //public List<MediakiwiField> Data { get; set; }
        StringBuilder m_Build;
        StringBuilder m_Cloaked;

        public string TopNavigation { get; set; }

        string m_Formdata;
        public string Formdata
        {
            get
            {
                if (m_Formdata == null)
                    m_Formdata = this.ToString();
                return m_Formdata;
            }
            set
            {
                if (m_Formdata == null)
                    m_Formdata = this.ToString();
                m_Formdata = value;
            }
        }
        public string Bottom { get; set; }
        public string SearchGrid { get; set; }

        StringBuilder m_Notifications;
        public StringBuilder Notifications
        {
            get { if (m_Notifications == null) m_Notifications = new StringBuilder(); return m_Notifications; }
            set { m_Notifications = value; } 
        }
        //public string Formdata  { get; set; }
        
        public string Leftnav { get; set; }
        public string Rightnav { get; set; }
        public string Tabularnav { get; set; }

        List<IControl> m_ControlList;

        public bool IsNull
        {
            get
            {
                return m_Build.Length == 0;
            }
        }


        /// <summary>
        /// Appends the specified literal control.
        /// </summary>
        /// <param name="data">The data.</param>
        public void Append(string data)
        {
            LiteralControl literal = new LiteralControl(data);
            m_ControlList.Add(literal);

            m_Build.Append(data);
        }

        public void AppendCloaked(string data)
        {
            LiteralControl literal = new LiteralControl(data);
            m_ControlList.Add(literal);

            m_Cloaked.Append(data);
        }

        public void AppendCloakedFormat(string format, params object[] data)
        {
            if (format != null && data != null)
            {
                try
                {
                    Regex cleanBs = new Regex(@"\<p\sconstruct=\"".*?>");
                    format = cleanBs.Replace(format, "");
                    LiteralControl literal = new LiteralControl(string.Format(format, data));
                    m_ControlList.Add(literal);
                    m_Cloaked.AppendFormat(format, data);
                }
                catch (Exception exc)
                {

                }

            }
        }

        /// <summary>
        /// Appends the specified builder.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Append(WimControlBuilder builder)
        {
            m_ControlList.AddRange(builder.Controls);
            m_Build.Append(builder.ToString());
            m_Cloaked.Append(builder.ToCloakedString());
        }

        /// <summary>
        /// Adds the specified control.
        /// </summary>
        /// <param name="control">The control.</param>
        public void Add(IControl control)
        {
            m_ControlList.Add(control);
        }

        /// <summary>
        /// Appends the format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="data">The data.</param>
        public void AppendFormat(string format, params string[] data)
        {
            if (format != null && data != null)
            {
                try
                {
                    Regex cleanBs = new Regex(@"\<p\sconstruct=\"".*?>");
                    format = cleanBs.Replace(format, "");
                    LiteralControl literal = new LiteralControl(string.Format(format, data));
                    m_ControlList.Add(literal);
                    m_Build.AppendFormat(format, data);
                }
                catch (Exception exc)
                {

                }
                
            }
        }

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="data">The data.</param>
        public void Insert(int index, string data)
        {
            LiteralControl literal = new LiteralControl(data);
            m_ControlList.Insert(index, literal);

            m_Build.Insert(0, data);
        }

        public IControl[] Controls
        {
            get
            {
                return m_ControlList.ToArray();
            }
        }

        /// <summary>
        /// Finds the object.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public LiteralControl[] FindObject(string text)
        {
            List<LiteralControl> list = new List<LiteralControl>();
            foreach(object obj in Controls)
            {
                if (obj.GetType() == typeof(LiteralControl))
                {
                    LiteralControl tmp = ((LiteralControl)obj);
                    if (tmp.Text.Contains(text))
                        list.Add(tmp);
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <value>The length.</value>
        public int Length
        {
            get
            {
                return m_Build.Length;
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return m_Build.ToString();
        }

        public string ToCloakedString(bool clear = false)
        {
            string cloaked = m_Cloaked.ToString();
            if (clear)
                m_Cloaked = new StringBuilder();
            return cloaked;
        }
    }
}
