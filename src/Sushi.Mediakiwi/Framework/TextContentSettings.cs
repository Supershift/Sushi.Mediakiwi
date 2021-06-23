using Sushi.Mediakiwi.Framework.ContentListItem;

namespace Sushi.Mediakiwi.Framework
{
    public class TextContentSettings
    {
        public TextContentSettings()
        {
        }
        protected TextFieldAttribute _element;
        public TextContentSettings(TextFieldAttribute element)
        {
            _element = element;
        }
        public TextContentSettings Expression(OutputExpression setting)
        {
            _element.Expression = setting;
            return new TextContentSettings(_element);
        }
        public TextContentSettings Hide(bool isHidden = true)
        {
            _element.IsHidden = isHidden;
            return new TextContentSettings(_element);
        }
        public ContentSettings Show(bool isVisible = true)
        {
            _element.IsHidden = !isVisible;
            return new ContentSettings(_element);
        }
        public TextContentSettings Cloak(bool isCloaked = true)
        {
            _element.IsCloaked = isCloaked;
            return new TextContentSettings(_element);
        }
        public TextContentSettings ReadOnly(bool isReadOnly = true)
        {
            _element.IsReadOnly = isReadOnly;
            return new TextContentSettings(_element);
        }
        public TextContentSettings Password(bool isPasswordField = true)
        {
            _element.IsPasswordField = isPasswordField;
            return new TextContentSettings(_element);
        }
        public TextContentSettings ClassName(string className)
        {
            _element.ClassName = className;
            return new TextContentSettings(_element);
        }
        public TextContentSettings FieldName(string fieldName)
        {
            _element.FieldName = fieldName;
            return new TextContentSettings(_element);
        }
        public TextContentSettings PreInputHtml(string html)
        {
            _element.PreInputHtml = html;
            return new TextContentSettings(_element);
        }
        public TextContentSettings PostInputHtml(string html)
        {
            _element.PostInputHtml = html;
            return new TextContentSettings(_element);
        }

        /// <summary>
        /// Triggers the postback change event, also assigns AutoPostback
        /// </summary>
        public event ContentInfoEventHandler OnChange
        {
            add
            {
                _element.AutoPostBack = true;
                _element.OnChange += value;
            }
            remove { _element.OnChange -= value; }
        }
    }
}
