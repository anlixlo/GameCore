using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore
{
    public class CoreManager : ScriptableSingleton<CoreManager>
    {
        private static UpdateHandler _updateHandler;

        // Initialized Module Container
        private Dictionary<Type, BaseModule> _modules = new Dictionary<Type, BaseModule>();

        #region RuntimeInitializeOnLoadMethod

        [RuntimeInitializeOnLoadMethod]
        private static void OnRuntimeMethodLoad()
        {
            InitializeUpdateHandler();
        }

        #endregion

        #region Module Control

        /// <summary>
        /// Initialize module to module contrainer
        /// </summary>
        public T InitializeModule<T>(params object[] param) where T : BaseModule
        {
            Type moduleType = typeof(T);

            if (!_modules.ContainsKey(moduleType))
            {
                T module = (T)Activator.CreateInstance(moduleType);
                InitModule(module);
                module.Initialize(param);
                Debug.Log($"[CoreManager:InitializeModule] Initialize New module Type : {moduleType}");
                return module;
            }
            else
            {
                Debug.Log($"[CoreManager:InitializeModule] {moduleType} is already exist!");
                return null;
            }
        }

        private void InitModule(BaseModule module)
        {
            Type moduleType = module.GetType();
            if (!_modules.ContainsKey(moduleType))
            {

                if (_updateHandler != null)
                {
                    _updateHandler.InitializeUpdate(module);
                }

                _modules.Add(moduleType, module);
            }
            else
            {
                Debug.Log($"[CoreManager:InitModule] {moduleType} is already initialized!");
            }
        }

        /// <summary>
        /// Get specific module from module container
        /// </summary>
        public T GetModule<T>() where T : BaseModule
        {
            BaseModule module = null;
            if (_modules.TryGetValue(typeof(T), out module))
            {
                return module as T;
            }
            Debug.LogError(string.Format("[CoreManager:GetModule] Can't find module with {0}", typeof(T)));
            return null;
        }

        /// <summary>
        /// Release module from module container
        /// </summary>
        public void ReleaseModule<T>(params object[] param) where T : BaseModule
        {
            Type moduleType = typeof(T);
            if (_modules.TryGetValue(moduleType, out BaseModule module))
            {
                if (_updateHandler != null)
                {
                    _updateHandler.ReleaseUpdate(module);
                }
                module.Release(param);

                _modules.Remove(moduleType);
                Debug.Log($"[CoreManager:ReleaseModule] Module {moduleType} is Released");
            }
            else
            {
                Debug.LogError($"[CoreManager:ReleaseModule] Module is not in system, Type : {moduleType}");
            }
        }

        public bool HasModule<T>() where T : BaseModule
        {
            return _modules.ContainsKey(typeof(T));
        }

        #endregion

        #region UpdateHandler Control

        private static void InitializeUpdateHandler()
        {
            ReleaseUpdateHandler();

            if (_updateHandler == null)
            {
                GameObject _obj = new GameObject("UpdateHandler");
                _updateHandler = _obj.AddComponent<UpdateHandler>();
                DontDestroyOnLoad(_obj);
            }
        }

        private static void ReleaseUpdateHandler()
        {
            UpdateHandler[] _handlerArray = FindObjectsOfType<UpdateHandler>();

            foreach (UpdateHandler _handler in _handlerArray)
            {
                DestroyImmediate(_handler.gameObject);
            }
        }

        #endregion
    }
}
