using System;
using System.Web;
using System.Data;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Sushi.Mediakiwi.Data.DalReflection;

namespace Sushi.Mediakiwi.Framework
{
    /// <summary>
    /// 
    /// </summary>
    [XmlType("Rule")]
    public class ValidationRule
    {
        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static ValidationRule SelectOne(int ID)
        {
            ValidationRules config = ValidationRules.Load();
            foreach (ValidationRule r in config.Rules)
            {
                if (r.ID == ID)
                    return r;
            }
            return null;
        }

        private int m_ID;
        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        public int ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        private string m_Title;
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title
        {
            get { return m_Title; }
            set { m_Title = value; }
        }

        private string m_RegEx;
        /// <summary>
        /// Gets or sets the reg ex.
        /// </summary>
        /// <value>The reg ex.</value>
        public string RegEx
        {
            get { return m_RegEx; }
            set { m_RegEx = value; }
        }
    }
}
