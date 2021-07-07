using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework.ContentInfoItem;

namespace Sushi.Mediakiwi.Framework.ContentListItem
{
 

    /// <summary>
    /// Possible return types: System.String
    /// </summary>
    public class ComponentContainerAttribute : ContentSharedAttribute, IContentInfo, IListContentInfo 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlContainerAttribute"/> class.
        /// </summary>
        public ComponentContainerAttribute()
        {
        }

        /// <summary>
        /// Sets the candidate.
        /// </summary>
        /// <param name="isEditMode"></param>
        public void SetCandidate(bool isEditMode)
        {
        }

        DataTemplate _DataTemplate;

        /// <summary>
        /// Sets the candidate.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="isEditMode">if set to <c>true</c> [is edit mode].</param>
        public void SetCandidate(Field field, bool isEditMode)
        {
            _DataTemplate = Property.GetValue(SenderInstance, null) as DataTemplate;

            if (_DataTemplate == null || _DataTemplate.Content == null || _DataTemplate.Content.Fields == null)
                return;

            //_DataTemplate.Clean();

            var keys = this.Console.Request.Form.AllKeys;
            foreach (var item in _DataTemplate.Content.Fields)
            {
                string key = string.Concat(Property.Name, "_", item.Property);

                if (item.Type != (int)ContentType.MultiField)
                {
                    var found = (from x in keys where x == key select x).Count();
                    if (found == 1)
                        item.Value = this.Console.Request.Form[key];
                }
                else
                {
                    var fields = new List<MultiField>();
                    //  The multifield results, they all have a name like [ID]__[CONTENTTYPE_INT]__[COUNT-1-BASED]
                    var multiFound = (from x in keys where x.StartsWith(string.Concat(key, "__")) select x).ToList();
                    if (multiFound.Count > 0)
                    {
                        item.Value = MultiField.GetSerialized(this.Console.Request, multiFound.ToArray());
                        //foreach (var instance in multiFound)
                        //{
                        //    //  Split into 0 = [ID]  1 = [CONTENTTYPE_INT] 2 = [COUNT-1-BASED]
                        //    var split = instance.Split(new string[] { "__" }, StringSplitOptions.RemoveEmptyEntries);
                        //    var name = string.Concat(split[0], split[2]);
                        //    var type = (ContentType)Convert.ToInt32(split[1]);
                        //    string value;

                        //    if (!instance.Contains("$"))
                        //    {
                        //        //  Find other keys that are an extention of this one!
                        //        var others = (from x in multiFound where x.StartsWith(string.Concat(instance, "$")) select x).ToList();
                        //        if (others.Count > 0)
                        //        {
                        //            //  There are innner keys, skip this original one!
                        //            //  HACK currently only works with single item result sets!
                        //            continue;
                        //        }
                        //    }

                        //    if (instance.Contains("$"))
                        //        value = instance.Split('$')[1];
                        //    else
                        //        value = this.Console.Request.Form[instance];

                        //    fields.Add(new MultiField(name, type, value));
                        //}
                        ////  Serialize and add as a value (fields in field.value)
                        //item.Value = Data.Utility.GetSerialized(fields.ToArray());
                    }
                }
            }
        }

        /// <summary>
        /// Writes the candidate.
        /// </summary>
        /// <param name="build">The build.</param>
        /// <param name="isEditMode">if set to <c>true</c> [is edit mode].</param>
        /// <param name="isRequired">if set to <c>true</c> [is required].</param>
        /// <returns></returns>
        public Field WriteCandidate(WimControlBuilder build, bool isEditMode, bool isRequired, bool isCloaked)
        {
            if (_DataTemplate == null)
                return null;

            this.IsCloaked = isCloaked;
            this.Mandatory = isRequired;
            //6233
            //Console.Item = m_PageInstance.ID;
            ///Console.ItemType = RequestItemType.Page;

            Sushi.Mediakiwi.Data.ComponentVersion version = new Data.ComponentVersion();

            version.Serialized_XML = Data.Utility.GetSerialized(this._DataTemplate.Content);

            var c = new Data.ComponentVersion[] { version };

            int count = 0;
            int c_index = 0;
            bool isValidInput = true;
            bool isContainerClosed = false;
            bool isValidPage = true;
            bool showServiceColumn = true;
            List<Field> fieldList = null;
            Sushi.Mediakiwi.Data.Page page = null;

            //WimControlBuilder build = new WimControlBuilder();
            WimControlBuilder build2 = new WimControlBuilder();

            this.Console.Component.SetComponent(c.ToArray(),
                    version,
                    ref c_index,
                    ref isValidInput,
                    ref isContainerClosed,
                    ref isValidPage,
                    ref showServiceColumn,
                    ref fieldList,
                    ref count,
                    this.Console,
                    ref page,
                    ref build,
                    ref build2,
                    _DataTemplate.MetaData,
                    false, false, Property.Name
                    );

            this._DataTemplate.Content = version.GetContent();

            build.Append(build2.ToString());

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool IsValid(bool isRequired)
        {
            this.Mandatory = isRequired;
            return true;
        }
    }
}


    
