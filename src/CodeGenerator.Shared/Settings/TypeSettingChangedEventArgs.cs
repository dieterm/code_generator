namespace CodeGenerator.Shared.Settings
{
    public class TypeSettingChangedEventArgs : EventArgs
    {
        public Type SettingType { get; }
        public string Key { get; }
        public object? NewValue { get; }
        public TypeSettingChangedEventArgs(Type settingType, string key, object? newValue)
        {
            SettingType = settingType;
            Key = key;
            NewValue = newValue;
        }
    }
}