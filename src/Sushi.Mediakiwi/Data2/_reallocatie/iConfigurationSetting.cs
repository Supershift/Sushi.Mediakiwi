using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sushi.Mediakiwi.Data
{
    public interface iConfigurationSetting
    {
        string GetValue(string initialValue, string settingProperty);
    }
}
