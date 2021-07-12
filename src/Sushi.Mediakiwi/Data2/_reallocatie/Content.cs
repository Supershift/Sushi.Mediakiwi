//using System;
//using System.Globalization;
//using System.Collections;
//using System.Data;
//using System.Data.SqlClient;
//using System.IO;
//using System.Xml;
//using System.Xml.Serialization;
//using System.Collections.Generic;
//using Sushi.Mediakiwi.Framework;
//using Sushi.Mediakiwi.Data;

//namespace Sushi.Mediakiwi
//{
//    /// <summary>
//    /// Represents a Content entity.
//    /// </summary>
//    public class Content : Sushi.Mediakiwi.Data.Content
//    {

//        internal void ApplyMetaData(MetaData[] arr)
//        {
//            List<Field> added = new List<Field>();
//            foreach (var meta in arr)
//            {
//                if (this.IsNull(meta.Name))
//                {
//                    added.Add(new Field(meta.Name, ((ContentType)Convert.ToInt32(meta.ContentTypeSelection)), meta.Default));
//                }
//            }
//            if (added.Count > 0)
//            {
//                if (this.Fields != null && this.Fields.Length > 0)
//                    added.AddRange(this.Fields);
//                this.Fields = added.ToArray();
//            }
//        }
//    }
//}