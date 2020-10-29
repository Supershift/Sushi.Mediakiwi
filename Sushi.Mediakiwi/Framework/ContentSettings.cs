using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Framework
{

    public class ContentSettings
    {
        public event ContentInfoEventHandler OnChange
        {
            add { _element.OnChange += value; }
            remove { _element.OnChange -= value; }
        }

        public ContentSettings()
        {
        }
        protected IContentInfo _element;
        public ContentSettings(IContentInfo element)
        {
            _element = element;
        }
        public ContentSettings Expression(OutputExpression setting)
        {
            _element.Expression = setting;
            return new ContentSettings(_element);
        }
        public ContentSettings Hide(bool isHidden = true)
        {
            _element.IsHidden = isHidden;
            return new ContentSettings(_element);
        }
        public ContentSettings Show(bool isVisible = true)
        {
            _element.IsHidden = !isVisible;
            return new ContentSettings(_element);
        }
        public ContentSettings Cloak(bool isCloaked = true)
        {
            _element.IsCloaked = isCloaked;
            return new ContentSettings(_element);
        }
        public ContentSettings FieldName(string fieldName)
        {
            _element.FieldName = fieldName;
            return new ContentSettings(_element);
        }
        public ContentSettings ReadOnly(bool isReadOnly = true)
        {
            _element.IsReadOnly = isReadOnly;
            return new ContentSettings(_element);
        }
    }
}
