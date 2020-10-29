using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data;

public static class PageTemplateExtension
{


    /// <summary>
    /// Gets the sections.
    /// </summary>
    /// <returns></returns>
    public static string[] GetPageSections(this PageTemplate inPageTemplate)
    {

        List<string> sections = new List<string>();
        if (inPageTemplate.Data != null && inPageTemplate.Data.HasProperty("TAB.INFO"))
            sections.AddRange(inPageTemplate.Data["TAB.INFO"].Value.Split(','));

        var availableTemplateList = Sushi.Mediakiwi.Data.AvailableTemplate.SelectAll(inPageTemplate.ID);
        var sectionViaTemplate = (from item in availableTemplateList select item.Target).Distinct().ToList();

        if (sectionViaTemplate.Count == 1)
        {
            if (sectionViaTemplate[0] == null)
            {
                var count1 = availableTemplateList.Count(x => !x.IsSecundary);
                var count2 = availableTemplateList.Count(x => x.IsSecundary);

                if (count1 > 0)
                {
                    sectionViaTemplate[0] = CommonConfiguration.DEFAULT_CONTENT_TAB;
                    if (count2 > 0)
                        sectionViaTemplate.Add(CommonConfiguration.DEFAULT_SERVICE_TAB);
                }
                else if (count2 > 0)
                {
                    sectionViaTemplate[0] = CommonConfiguration.DEFAULT_SERVICE_TAB;
                }
            }
        }

        var legacyContentTab = inPageTemplate.Data["TAB.LCT"].Value;

        bool isEmpty = sections.Count == 0;
        sectionViaTemplate.ForEach(section => { sections.Add(section); });

        if (!isEmpty)
            sections = (from item in sections select item).Distinct().ToList();

        if (sections.Count == 0)
            sections.Add(CommonConfiguration.DEFAULT_CONTENT_TAB);

        return sections.ToArray();
    }
}

