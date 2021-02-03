using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data;

public static class ComponentListVersionExtension
{
    
    /// <summary>
    /// Gets the property value.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns></returns>
    public static string GetPropertyValue(this ComponentListVersion inComponentListVersion,  string propertyName)
    {
        Content m_content = Content.GetDeserialized(inComponentListVersion.Serialized_XML);

        if (m_content == null)
            return null;

        if (m_content.Fields == null)
            return null;

        foreach (Content.Field field in m_content.Fields)
        {
            if (field.Property == propertyName)
            {
                if (field.Type == (int)Sushi.Mediakiwi.Framework.ContentType.RichText)
                {
                    string candidate = Wim.Utility.ApplyRichtextLinks(null, field.Value);
                    return candidate;
                }
                return field.Value;
            }
        }
        return null;
    }

}

