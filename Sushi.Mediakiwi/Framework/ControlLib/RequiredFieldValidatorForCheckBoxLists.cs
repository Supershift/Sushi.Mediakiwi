using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

namespace Sushi.Mediakiwi.Framework.ControlLib
{
    /// <summary>
    /// 
    /// </summary>
    public class RequiredFieldValidatorForCheckBoxLists :
                        System.Web.UI.WebControls.BaseValidator
    {
        private ListControl _listctrl;
        /// <summary>
        /// 
        /// </summary>
        public RequiredFieldValidatorForCheckBoxLists()
        {
            base.EnableClientScript = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override bool ControlPropertiesValid()
        {
            Control ctrl = FindControl(ControlToValidate);

            if (ctrl != null)
            {
                _listctrl = (ListControl)ctrl;
                return (_listctrl != null);
            }
            else
                return false;  // raise exception
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override bool EvaluateIsValid()
        {
            return _listctrl.SelectedIndex != -1;
        }
    }
}
