using Sushi.Mediakiwi.Framework.ContentListItem;
using System;

namespace Sushi.Mediakiwi.Framework
{
    public class SublistSettings
    {
        public event ContentInfoEventHandler OnChange
        {
            add { _element.OnChange += value; }
            remove { _element.OnChange -= value; }
        }

        public SublistSettings()
        {
        }
        protected SubListSelectAttribute _element;
        public SublistSettings(SubListSelectAttribute element)
        {
            _element = element;
        }
        public SublistSettings Expression(OutputExpression setting)
        {
            _element.Expression = setting;
            return new SublistSettings(_element);
        }
        public SublistSettings Hide(bool isHidden = true)
        {
            _element.IsHidden = isHidden;
            return new SublistSettings(_element);
        }
        public SublistSettings FieldName(string fieldName)
        {
            _element.FieldName = fieldName;
            return new SublistSettings(_element);
        }
        public SublistSettings OnlyOneItem(bool canContainOneItem = true)
        {
            _element.CanContainOneItem = canContainOneItem;
            return new SublistSettings(_element);
        }
        public SublistSettings ScrollBar(bool hasScrollBar = true)
        {
            if (_element.LayerSpecification == null)
            {
                _element.LayerSpecification = new Grid.LayerSpecification();
            }

            _element.LayerSpecification.HasScrolling = hasScrollBar;
            return new SublistSettings(_element);
        }
        public SublistSettings Height(int height, bool isPercentage = false)
        {
            if (_element.LayerSpecification == null)
            {
                _element.LayerSpecification = new Grid.LayerSpecification();
            }

            _element.LayerSpecification.Height = height;
            _element.LayerSpecification.IsHeightPercentage = isPercentage;

            return new SublistSettings(_element);
        }
        public SublistSettings Width(int width, bool isPercentage = false)
        {
            if (_element.LayerSpecification == null)
            {
                _element.LayerSpecification = new Grid.LayerSpecification();
            }

            _element.LayerSpecification.Width = width;
            _element.LayerSpecification.IsWidthPercentage = isPercentage;

            return new SublistSettings(_element);
        }
        public SublistSettings Title(string title)
        {
            if (_element.LayerSpecification == null)
            {
                _element.LayerSpecification = new Grid.LayerSpecification();
            }

            _element.LayerSpecification.Title = title;
            return new SublistSettings(_element);
        }
    }
    public class ButtonSettings
    {
        public event ContentInfoEventHandler OnChange
        {
            add { _element.OnChange += value; }
            remove { _element.OnChange -= value; }
        }

        public ButtonSettings()
        {
        }
        protected ButtonAttribute _element;
        public ButtonSettings(ButtonAttribute element)
        {
            _element = element;
        }
        public ButtonSettings Expression(OutputExpression setting)
        {
            _element.Expression = setting;
            return new ButtonSettings(_element);
        }
        public ButtonSettings Hide(bool isHidden = true)
        {
            _element.IsHidden = isHidden;
            return new ButtonSettings(_element);
        }
        public ButtonSettings FieldName(string fieldName)
        {
            _element.FieldName = fieldName;
            return new ButtonSettings(_element);
        }

        public ButtonSettings Confirmation(string title, string question, string acceptancelabel, string rejectionlabel, bool shouldAskConfirmation = true)
        {
            _element.ConfirmationTitle = title;
            _element.ConfirmationQuestion = question;
            _element.ConfirmationAcceptLabel = acceptancelabel;
            _element.ConfirmationRejectLabel = rejectionlabel;
            _element.AskConfirmation = shouldAskConfirmation;
            return new ButtonSettings(_element);
        }
        public ButtonSettings Primary(bool isPrimary = true)
        {
            _element.IsPrimary = isPrimary;
            return new ButtonSettings(_element);
        }
        public ButtonSettings OpenList(Type componentList, bool openInLayer = true)
        {
            var target = Data.ComponentList.SelectOne(componentList);
            if (target != null)
            {
                _element.OpenInPopupLayer = openInLayer;
                _element.ListInPopupLayer = target.GUID.ToString();
            }
            return new ButtonSettings(_element);
        }
        public ButtonSettings Title(string title)
        {
            _element.PopupTitle = title;
            return new ButtonSettings(_element);
        }
        public ButtonSettings Size(LayerSize size)
        {
            _element.PopupLayerSize = size;
            return new ButtonSettings(_element);
        }
        public ButtonSettings ScrollBar(bool hasScrollBar = true)
        {
            _element.PopupLayerHasScrollBar = hasScrollBar;
            return new ButtonSettings(_element);
        }
        public ButtonSettings Height(int height, bool isPercentage = false)
        {
            if (isPercentage)
                _element.PopupLayerHeight = $"{height}%";
            else
                _element.PopupLayerHeight = $"{height}px";

            return new ButtonSettings(_element);
        }
        public ButtonSettings Width(int width, bool isPercentage = false)
        {
            if (isPercentage)
                _element.PopupLayerWidth = $"{width}%";
            else
                _element.PopupLayerWidth = $"{width}px";

            return new ButtonSettings(_element);
        }
        public ButtonSettings OpenUrl(Uri url, bool openInLayer = false, string target = "_blank")
        {
            _element.OpenInPopupLayer = openInLayer;
            _element.CustomUrl = url.ToString();

            if (!openInLayer)
                _element.Target = target;

            _element.TriggerSaveEvent = false;
            return new ButtonSettings(_element);
        }
    }

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
