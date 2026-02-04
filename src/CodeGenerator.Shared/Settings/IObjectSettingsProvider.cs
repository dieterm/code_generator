using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Shared.Settings
{
    public interface IObjectSettingsProvider
    {
        public Type TargetObjectType { get; }
        public string GetObjectInstanceId(object instance);
        public T? GetInstanceSetting<T>(string instanceId, string key);
        public T? GetTypeSetting<T>(Type type, string key);
        public void SetInstanceSetting<T>(string instanceId, string key, T value);
        public void SetTypeSetting<T>(Type type, string key, T value);

        public event EventHandler<InstanceSettingChangedEventArgs>? InstanceSettingChanged;
        public event EventHandler<TypeSettingChangedEventArgs>? TypeSettingChanged;
    }
}
