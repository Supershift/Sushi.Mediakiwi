using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.UI;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation
{
    /// <summary>
    /// Represents a BaseImplementation entity.
    /// </summary>
    public class BaseImplementation : ComponentListTemplate
    {
        /// <summary>
        /// Gets a value indicating whether this instance is new element.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is new element; otherwise, <c>false</c>.
        /// </value>
        public bool IsNewElement
        {
            get {
                return Context.Request.Query["item"] == "0"; 
            }
        }

        private ListItemCollection m_BoolChoice;
        /// <summary>
        /// Gets the bool choice.
        /// </summary>
        /// <value>The bool choice.</value>
        public ListItemCollection BoolChoice
        {
            get
            {
                if (m_BoolChoice == null)
                {
                    m_BoolChoice = new ListItemCollection();
                    m_BoolChoice.Add(new ListItem("No", "0"));
                    m_BoolChoice.Add(new ListItem("Yes", "1"));
                }
                return m_BoolChoice;
            }
        }
    }
}
