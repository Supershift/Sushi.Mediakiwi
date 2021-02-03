using System;
using System.Linq;
using System.Data;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Text;
using Sushi.Mediakiwi.Data.DalReflection;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation.Support
{
    /// <summary>
    /// 
    /// </summary>
    public class NameValues : DatabaseEntity
    {
        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <returns></returns>
        public static Sushi.Mediakiwi.Data.NameValue[] SelectAll_FormElementOptionList()
        {
            try
            {
                NameValues implement = new NameValues();
                implement.SqlTable = "wim_FormElementOptionLists";
                implement.SqlOrder = "FormElementOption_Name ASC";
                return implement.SelectNameValueCollection("FormElementOption_Name", "FormElementOptionList_Key");
            }
            catch (Exception) { return new Sushi.Mediakiwi.Data.NameValue[0]; }
        }

        public static Sushi.Mediakiwi.Data.NameValue[] SelectAll_FormElementOptionListItems()
        {
            try
            {
                NameValues implement = new NameValues();
                implement.SqlTable = "wim_FormElementOptionLists";
                implement.SqlOrder = "FormElementOption_Name ASC";
                return implement.SelectNameValueCollection("FormElementOption_Name", "FormElementOptionList_Key");
            }
            catch (Exception) { return new Sushi.Mediakiwi.Data.NameValue[0]; }
        }
    }
}
