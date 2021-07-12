namespace Wim.Data.Interfaces
{
    public interface IConfigurationSetting
    {
        string GetValue(string initialValue, string settingProperty);
    }
}
