using System;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.Data;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation
{
    /// <summary>
    /// 
    /// </summary>
    public class NoteList : ComponentListTemplate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoteList"/> class.
        /// </summary>
        public NoteList()
        {
            wim.HideProperties = true;
            //wim.ShowInFullWidthMode = true;
            wim.CanContainSingleInstancePerDefinedList = true;
            ListLoad += NoteList_ListLoad;
        }

        async Task NoteList_ListLoad(ComponentListEventArgs e)
        {
            int lastError = wim.CurrentVisitor.Data["last.error"].ParseInt().GetValueOrDefault();
            var note = await Mediakiwi.Data.Notification.SelectOneAsync(lastError);
            
            string errorShort = note.Text.Split(new string[] { "<b>Error:</b><br/>", "<br/><br/><b>Source:</b>" }, StringSplitOptions.RemoveEmptyEntries)[0];
            int errorCode = Utility.ConvertToInt(errorShort.Split(':')[0]);
            errorShort = errorShort.Split(new string[] { string.Concat(errorCode, ":") }, StringSplitOptions.RemoveEmptyEntries)[0];

            if (errorCode == 0)
                errorShort = Sushi.Mediakiwi.Framework.ErrorCode.GetCultureText("err_0", wim.CurrentApplicationUser.LanguageCulture);

            wim.CurrentList.Option_HideBreadCrumbs = true;
            if (CommonConfiguration.IS_LOCAL_DEVELOPMENT || wim.CurrentApplicationUser.IsDeveloper)
            {
                wim.ListInfoApply(
                    Sushi.Mediakiwi.Framework.ErrorCode.GetCultureText("ErrorText", wim.CurrentApplicationUser.LanguageCulture), 
                    errorShort, 
                    null);

                wim.Notification.AddNotification(note.Text);
            }
            else
            {
                wim.ListTitle =Sushi.Mediakiwi.Framework.ErrorCode.GetCultureText("ErrorText", wim.CurrentApplicationUser.LanguageCulture);
                wim.Notification.AddError(errorShort);
            }
           
        }
    }
}
