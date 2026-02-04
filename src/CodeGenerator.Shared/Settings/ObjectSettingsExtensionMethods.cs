using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Shared.Settings
{
    public static class ObjectSettingsExtensionMethods
    {
        public static string GetObjectSettingsInstanceId(this object? obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            var provider = ServiceProviderHolder.GetKeyedService<IObjectSettingsProvider>(obj.GetType());
            if (provider == null) throw new InvalidOperationException($"No IObjectSettingsProvider registered for type {obj.GetType().FullName}");
            return provider.GetObjectInstanceId(obj);
        }

        public static IObjectSettingsProvider GetSettingsProvider(this object? obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            var provider = ServiceProviderHolder.GetKeyedService<IObjectSettingsProvider>(obj.GetType());
            if (provider == null) throw new InvalidOperationException($"No IObjectSettingsProvider registered for type {obj.GetType().FullName}");
            return provider;
        }

        public static T? GetSettings<T>(this object? obj,string key, T? defaultValue)
        {
            return GetSettings<T>(obj, key) ?? defaultValue;
        }

        public static T? GetSettings<T>(this object? obj, string key)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            var provider = obj.GetSettingsProvider();
            var instanceId = obj.GetObjectSettingsInstanceId();
            var value = provider.GetInstanceSetting<T>(instanceId, key);
            if (value == null) 
            {
                value = provider.GetTypeSetting<T>(obj.GetType(), key);
            }
            return value;
        }

        public static T? GetTypeSetting<T>(this object? obj, string key, T? defaultValue)
        {
            return obj.GetTypeSetting<T>(key) ?? defaultValue;
        }
        public static T? GetTypeSetting<T>(this object? obj, string key)
        {
            var provider = obj.GetSettingsProvider();
            var value = provider.GetTypeSetting<T>(obj!.GetType(), key);
            return value;
        }
        public static T? GetInstanceSetting<T>(this object? obj, string key, T? defaultValue)
        {
            return obj.GetInstanceSetting<T>(key) ?? defaultValue;
        }
        public static T? GetInstanceSetting<T>(this object? obj, string key)
        {
            var provider = obj.GetSettingsProvider();
            var instanceId = obj!.GetObjectSettingsInstanceId();
            var value = provider.GetInstanceSetting<T>(instanceId, key);
            return value;
        }
        public static void SetInstanceSetting<T>(this object? obj, string key, T value) 
        {
            var provider = obj.GetSettingsProvider();
            var instanceId = obj!.GetObjectSettingsInstanceId();
            provider.SetInstanceSetting<T>(instanceId, key, value);
        }
        public static void SetTypeSetting<T>(this object? obj, string key, T value) 
        {
            var provider = obj.GetSettingsProvider();
            provider.SetTypeSetting<T>(obj!.GetType(), key, value);
        }
    }
}
