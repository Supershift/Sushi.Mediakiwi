using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using Sushi.Mediakiwi.Data;

public static class SubscriptionExtension
{
    /// <summary>
    /// Gets the is active icon.
    /// </summary>
    /// <value>The is active icon.</value>
    public static string GetActiveIcon(this ISubscription inSubscription)
    {
        if (inSubscription.IsActive)
            return Wim.Utility.GetIconImageString(Wim.Utility.IconImage.Yes);

        return Wim.Utility.GetIconImageString(Wim.Utility.IconImage.No);
    }



    /// <summary>
    /// Gets the interval text.
    /// </summary>
    /// <value>The interval text.</value>
    public static string GetIntervalText(this ISubscription inSubscription)
    {

        foreach (ListItem li in GetIntervalCollection())
            if (li.Value == inSubscription.IntervalType.ToString()) return li.Text;

        return null;
    }


    /// <summary>
    /// Gets the interval collection.
    /// </summary>
    /// <value>The interval collection.</value>
    public static ListItemCollection GetIntervalCollection()
    {

        ListItemCollection m_IntervalCollection = new ListItemCollection();

        m_IntervalCollection.Add(new ListItem(""));
        m_IntervalCollection.Add(new ListItem("Every hour", "60"));
        m_IntervalCollection.Add(new ListItem("Every 2 hours", "120"));
        m_IntervalCollection.Add(new ListItem("Every 4 hours", "240"));
        m_IntervalCollection.Add(new ListItem("Every 6 hours", "360"));
        m_IntervalCollection.Add(new ListItem("Every 12 hours", "720"));
        m_IntervalCollection.Add(new ListItem("Every day", "1440"));
        m_IntervalCollection.Add(new ListItem("Every 2 days", "2880"));
        m_IntervalCollection.Add(new ListItem("Every week", "3360"));
        //m_IntervalCollection.Add(new ListItem("Every 1th of the month", "-1"));
        //m_IntervalCollection.Add(new ListItem("Every 15th of the month", "-2"));

        return m_IntervalCollection;
    }
}
