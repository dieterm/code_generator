namespace CodeGenerator.Shared.Settings
{
    public class InstanceSettingChangedEventArgs : EventArgs
    {
        public string InstanceId { get; }
        public string Key { get; }
        public object? NewValue { get; }
        public InstanceSettingChangedEventArgs(string instanceId, string key, object? newValue)
        {
            InstanceId = instanceId;
            Key = key;
            NewValue = newValue;
        }
    }
}