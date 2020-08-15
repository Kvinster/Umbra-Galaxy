using UnityEngine;

using System.Xml;

namespace STP.Config {
    public static class ConfigLoader {
        const string ConfigPathPrefix = "Configs/";
        
        public static T LoadConfig<T>(string variantName = null) where T : BaseConfig, new() {
            var configName = typeof(T).Name;
            var filePath   = string.IsNullOrEmpty(variantName)
                ? ConfigPathPrefix + configName
                : $"{ConfigPathPrefix}{configName}_{variantName}";
            var configText = Resources.Load<TextAsset>(filePath);
            if ( configText == null ) {
                Debug.LogErrorFormat("Can't load config for file path '{0}'", filePath);
                return null;
            }
            var document = new XmlDocument();
            document.LoadXml(configText.text);
            var config = new T();
            config.Load(document.DocumentElement);
            return config;
        }
    }
}
