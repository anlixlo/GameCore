using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore
{
    public class ConfigModule : BaseModule
    {
        private const string DEFAULT_CONFIG_PATH = "Config/";

        //// Initialized Config Container
        public Dictionary<Type, BaseConfig> config => _config;
        private Dictionary<Type, BaseConfig> _config = new Dictionary<Type, BaseConfig>();

        public override void Initialize(params object[] param)
        {
            InitAllConfig();
        }

        private void InitConfig(BaseConfig baseConfig)
        {
            _config.Add(baseConfig.GetType(), baseConfig);
        }

        private void InitAllConfig()
        {
            var _configList = Resources.LoadAll(DEFAULT_CONFIG_PATH, typeof(BaseConfig));

            foreach (BaseConfig config in _configList)
            {
                InitConfig(config);
            }
        }

        public Type GetConfigType(string typeStr)
        {
            string typeString = this.GetType().Namespace + "." + typeStr;

            return Type.GetType(typeString, false, true);
        }

        public T GetConfig<T>() where T : BaseConfig
        {
            BaseConfig baseConfig = null;

            if (_config.TryGetValue(typeof(T), out baseConfig))
            {
                return baseConfig as T;
            }

            Debug.LogError(string.Format("[ConfigModule:GetConfig] Can't find config with {0}", typeof(T)));
            return null;
        }

        public BaseConfig GetConfig(Type type)
        {
            if (_config.ContainsKey(type))
            {
                return _config[type];
            }

            Debug.LogError(string.Format("[ConfigModule:GetConfig] Can't find config with {0}", type));
            return null;
        }

        public BaseConfig GetConfig(string typeStr)
        {
            Type configType = GetConfigType(typeStr);

            if (_config.ContainsKey(configType))
            {
                return _config[configType];
            }

            Debug.LogError(string.Format("[ConfigModule:GetConfig] Can't find config with {0}", typeStr));
            return null;
        }

        #region Console Log Command

        public List<String> LogConfigAllVariable(string typeStr)
        {
            List<String> result = new List<String>();

            Type configType = GetConfigType(typeStr);

            if (configType == null)
            {
                result.Add(string.Format("[ConfigModule:LogConfigAllVariable] Can't find BaseConfig Type named {0}", typeStr));
                return result;
            }

            BaseConfig baseConfig = GetConfig(configType);

            if (baseConfig == null)
            {
                result.Add(string.Format("[ConfigModule:LogConfigAllVariable] Can't find config with {0}", typeStr));
                return result;
            }

            FieldInfo[] configVariableArray = configType.GetFields();

            foreach (FieldInfo fieldInfo in configVariableArray)
            {
                result.Add(fieldInfo.Name + " : " + fieldInfo.GetValue(baseConfig));
            }

            return result;
        }

        public string LogConfigVariable(string[] args)
        {
            Type configType = GetConfigType(args[0]);

            if (configType == null)
            {
                return string.Format("[ConfigModule:LogConfigVariable] Can't find BaseConfig Type named {0}", args[0]);
            }

            BaseConfig baseConfig = GetConfig(configType);

            if (baseConfig == null)
            {
                return string.Format("[ConfigModule:LogConfigVariable] Can't find config with {0}", args[0]);
            }

            var _configField = configType.GetField(args[1]);

            if (_configField == null)
            {
                return string.Format("[ConfigModule:LogConfigVariable] Can't find config variable with {0}", args[1]);
            }

            var _configVariable = _configField.GetValue(baseConfig);

            if (_configVariable == null)
            {
                return string.Format("[ConfigModule:LogConfigVariable] Can't find config variable with {0}", args[1]);
            }

            return _configVariable.ToString();
        }

        #endregion
    }
}
