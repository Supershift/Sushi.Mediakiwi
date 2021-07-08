namespace Sushi.Mediakiwi.Data.Interfaces
{
    public interface IConfigurationSetting
    {
        string GetValue(string initialValue, string settingProperty);
    }
}
