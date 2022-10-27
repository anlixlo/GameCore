using System;
using UnityEngine;

namespace GameCore
{
    [CreateAssetMenu(fileName = "NewConfigCommand", menuName = "GameCore/ConsoleModule/Commands/ConfigCommand")]
    public class ConfigCommand : ConsoleCommand
    {

        public override bool Process(string[] args)
        {
            if (args.Length == 0)
            {
                LogInitializedConfig();
            }

            if (args.Length == 1)
            {
                LogConfig(args);
            }

            if (args.Length == 2)
            {
                LogConfigVariable(args);
            }

            return true;
        }

        public void LogInitializedConfig()
        {
            var _configList = CoreManager.Instance.GetModule<ConfigModule>().config;

           Debug.Log(string.Format("[ConfigModule:LogInitializedConfig] Already initialized {0} Config", _configList.Count));
        }

        private void LogConfig(string[] args)
        {
            var _configVariables = CoreManager.Instance.GetModule<ConfigModule>().LogConfigAllVariable(args[0]);

            foreach(string variableStr in _configVariables)
            {
                Debug.Log(variableStr);
            }
        }

        private void LogConfigVariable(string[] args)
        {
            Debug.Log(CoreManager.Instance.GetModule<ConfigModule>().LogConfigVariable(args));
        }
    }
}