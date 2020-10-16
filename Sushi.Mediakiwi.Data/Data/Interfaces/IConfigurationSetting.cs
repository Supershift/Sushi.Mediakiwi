using System;
using System.Collections.Generic;
using System.Text;

namespace Wim.Data.Interfaces
{
    public interface IConfigurationSetting
    {
        string GetValue(string initialValue, string settingProperty);
    }
}
