using Sushi.Mediakiwi.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Logic
{
    public class PropertyLogic
    {
        /// <summary>
        /// Converts the property value.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        /// <param name="listID">The list ID.</param>
        /// <param name="listTypeID">The list type ID.</param>
        /// <returns></returns>
        public static object ConvertPropertyValue(string propertyName, object value, int listID, int? listTypeID)
        {
            if (string.IsNullOrEmpty(propertyName))
                return null;

            if (value != null && value.GetType() == typeof(string) && value.ToString() != string.Empty)
            {
                //  [20090410:MM Trying to detect the type]
                foreach (var p in Property.SelectAll(listID))
                {
                    if (p.FieldName == propertyName.Replace("Data.", ""))
                    {
                        if (p.FilterType == typeof(DateTime).ToString())
                            return new DateTime(long.Parse(value.ToString()));
                        if (p.FilterType == typeof(decimal).ToString())
                            return decimal.Parse(value.ToString());
                        if (p.FilterType == typeof(int).ToString())
                            return Utility.ConvertToInt(value);

                        if (p.ContentTypeID == ContentType.Choice_Checkbox)
                        {
                            if (value.ToString() == "0" || value.ToString() == string.Empty) return false;
                            return true;
                        }
                    }
                }
            }
            else
            {
                if (value == null)
                {
                    foreach (var p in Property.SelectAll(listID))
                    {
                        if (p.FieldName == propertyName.Replace("Data.", ""))
                        {
                            if (p.ContentTypeID == ContentType.Choice_Checkbox)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return value;
        }
    }
}
