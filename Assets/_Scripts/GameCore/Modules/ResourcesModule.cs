//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using UnityEngine;
//using UnityEngine.AddressableAssets;
//using UnityEngine.ResourceManagement.AsyncOperations;
//using UnityEngine.ResourceManagement.ResourceLocations;
//using UnityEngine.ResourceManagement.ResourceProviders;
//using UnityEngine.SceneManagement;
//using Object = UnityEngine.Object;

//namespace GameCore
//{
//    /// <summary>
//    /// 遊戲內部資源管理模組
//    /// </summary>
//    [AutoCreateModule]
//    public class ResourcesModule : BaseModule
//    {
//        private Dictionary<string, GameObjectPool> _gameobjectPools = new Dictionary<string, GameObjectPool>();
//        private Dictionary<Object, string> _gameObjectKeysMap = new Dictionary<Object, string>();
//        private Dictionary<Type, Dictionary<string, IAsyncOperationHandleInfo>> _asyncAssetHandles = new Dictionary<Type, Dictionary<string, IAsyncOperationHandleInfo>>();
//        private Dictionary<string, AsyncOperationHandle<SceneInstance>> _asyncSceneHandles = new Dictionary<string, AsyncOperationHandle<SceneInstance>>();

//        private List<string> _customIgnoreReleaseAssets = new List<string>();

//        private Transform _poolsRoot;
//        private const string _objectPoolName = "[ObjectPool]";

//        public override async void Initialize(object[] param = null)
//        {
//            GameObject _poolsRootObj = new GameObject(_objectPoolName);
//            Object.DontDestroyOnLoad(_poolsRootObj);

//            _poolsRoot = _poolsRootObj.transform;
//            await PoolSettings.Instance.Init();
//        }

//        #region Asset method

//        public async Task<T> GetAssetAsync<T>() where T : Object
//        {
//            string path = typeof(T).GetResourcePath();
//            return await GetAssetAsync<T>(path) as T;
//        }

//        public async Task<T> GetAssetAsync<T>(string address) where T : Object
//        {
//            return await GetAsyncOperationHandleInfo<T>(address);
//        }

//        #endregion

//        #region Scene method 

//        public async Task LoadSceneAsync(string address, LoadSceneMode loadMode = LoadSceneMode.Additive, bool activateOnLoad = true, int priority = 100)
//        {
//            Debug.Log(string.Format("[ResourcesSystem::LoadSceneAsync] Start load scene, Address: {0}.", address));

//            AsyncOperationHandle<SceneInstance> asyncOperationHandle = Addressables.LoadSceneAsync(address, loadMode, activateOnLoad, priority);
//            await asyncOperationHandle.Task;

//            SceneManager.SetActiveScene(asyncOperationHandle.Result.Scene);
//            Debug.Log(string.Format("[ResourcesSystem::LoadSceneAsync] Load scene done, Address: {0}.", address));

//            _asyncSceneHandles.Add(address, asyncOperationHandle);
//        }

//        public async Task UnloadSceneAsync(string address)
//        {
//            if (_asyncSceneHandles.ContainsKey(address))
//            {
//                Debug.Log(string.Format("[ResourcesSystem::UnloadSceneAsync] Start unload scene, Address: {0}.", address));

//                await Addressables.UnloadSceneAsync(_asyncSceneHandles[address], true).Task;

//                Debug.Log(string.Format("[ResourcesSystem::UnloadSceneAsync] Unload scene done, Address: {0}.", address));

//                _asyncSceneHandles.Remove(address);
//            }
//        }

//        #endregion

//        #region Spawn and Despawn method

//        public async void Spawn<T>(Action<T> callback, Vector3 position = default, Quaternion rotation = default) where T : Object
//        {
//            T result = await Spawn<T>(position, rotation);
//            callback?.Invoke(result);
//        }

//        public async void Spawn<T>(string address, Action<T> callback, Vector3 position = default, Quaternion rotation = default) where T : Object
//        {
//            T result = await Spawn<T>(address, position, rotation);
//            callback?.Invoke(result);
//        }

//        public async void Spawn(string address, Action<GameObject> callback, Vector3 position = default, Quaternion rotation = default)
//        {
//            GameObject result = await Spawn(address, position, rotation);
//            callback?.Invoke(result);
//        }

//        public async Task<T> Spawn<T>(Vector3 position = default, Quaternion rotation = default) where T : Object
//        {
//            string address = typeof(T).GetResourcePath();

//            if (string.IsNullOrEmpty(address)) return null;

//            return await InternalSpawn<T>(address, position, rotation);
//        }

//        public async Task<T> Spawn<T>(string address, Vector3 position = default, Quaternion rotation = default) where T : Object
//        {
//            return await InternalSpawn<T>(address, position, rotation);
//        }

//        public async Task<GameObject> Spawn(string address, Vector3 position = default, Quaternion rotation = default)
//        {
//            return await InternalSpawn<GameObject>(address, position, rotation);
//        }

//        private async Task<T> InternalSpawn<T>(string address, Vector3 position = default, Quaternion rotation = default) where T : Object
//        {
//            if (await TryGetAsset(address) == null) return null;

//            GameObjectPool gameObjectPool = await GetGameObjectPool(address);
//            T result = await gameObjectPool.Spawn<T>(position, rotation);

//            if (result == null)
//            {
//                Debug.LogError("[ResourcesSystem::Spawn] Can't spawn object, address : " + address);
//                return null;
//            }

//            _gameObjectKeysMap.Add(result, address);
//            return result;
//        }

//        private async Task<GameObject> TryGetAsset(string address)
//        {
//            GameObject gameObject = await GetAssetAsync<GameObject>(address);

//            //NOTE : Check gameObject is in AAS, AAS only support Object not component.
//            if (gameObject == null)
//                Debug.LogError("[ResourcesSystem::Spawn] Can't get asset with address : " + address);

//            return gameObject;
//        }

//        public async void DeSpawn(Object obj, bool isReleasePool = false)
//        {
//            if (_gameObjectKeysMap.TryGetValue(obj, out string address))
//            {
//                if (HasPool(address))
//                {
//                    _gameObjectKeysMap.Remove(obj);
//                    GameObjectPool gameObjectPool = await GetGameObjectPool(address);

//                    if (isReleasePool)
//                        ReleasePool(address);
//                    else
//                        gameObjectPool.Despawn(obj.GetHashCode());
//                }
//            }
//            else
//                Debug.LogError($"[ResourcesSystem::DeSpawn] You can't despawn not spawned object. Object name: {obj.name}");
//        }

//        #endregion

//        #region GameObject pool

//        public bool HasPool(string address)
//        {
//            return _gameobjectPools.ContainsKey(address);
//        }

//        public async Task<GameObjectPool> GetGameObjectPool(string address)
//        {
//            if (!HasPool(address))
//            {
//                IResourceLocation location = await AddressablesUtility.LocatorRuntimeKey(address);

//                if (!HasPool(address))
//                {
//                    PoolSettings.Instance.TryGetSetting(location, out PoolSettings.PoolSetting setting);
//                    _gameobjectPools.Add(address, new GameObjectPool(location, _poolsRoot, setting));
//                }
//            }

//            return _gameobjectPools[address];
//        }

//        public async Task PreloadPool(string address)
//        {
//            GameObjectPool objectPool = await GetGameObjectPool(address);
//            await objectPool.Preload();
//        }

//        public async Task PreloadPool<T>() where T : Object
//        {
//            string address = typeof(T).GetResourcePath();
//            GameObjectPool objectPool = await GetGameObjectPool(address);
//            await objectPool.Preload();
//        }

//        #endregion

//        #region Release

//        public void ReleasePool(string address)
//        {
//            Debug.Log($"[ResourcesSystem:ReleasePool] Release GameObject pool {address}.");

//            if (HasPool(address))
//            {
//                _gameobjectPools[address].ReleasePool();
//                _gameobjectPools.Remove(address);

//                Debug.Log($"[ResourcesSystem:ReleasePool] Release GameObject pool {address} done.");
//            }
//            else
//                Debug.LogError($"[ResourcesSystem:ReleasePool] GameObject pool : {address} is not exist.");
//        }

//        /// <summary>
//        /// This will call gc after release
//        /// </summary>
//        public void ReleaseAllPool()
//        {
//            Debug.Log($"[ResourcesSystem:ReleaseAllPool] Start Release all GameObject pool.");
//            List<GameObjectPool> ignorePool = new List<GameObjectPool>();
//            foreach (var pool in _gameobjectPools)
//            {
//                if (pool.Value.IgnoreAllRelease || _customIgnoreReleaseAssets.Contains(pool.Key))
//                {
//                    ignorePool.Add(pool.Value);
//                    continue;
//                }
//                pool.Value.ReleasePool();
//            }
//            _gameobjectPools.Clear();

//            foreach (var pool in ignorePool)
//                _gameobjectPools.Add(pool.Location.PrimaryKey, pool);

//            CallGC();
//            Debug.Log($"[ResourcesSystem:ReleaseAllPool] Start Release all GameObject pool done.");
//        }

//        public void ReleaseAsset<T>(string address) where T : Object
//        {
//            Debug.Log($"[ResourcesSystem:ReleaseAsset] Release asset {address}");

//            Dictionary<string, IAsyncOperationHandleInfo> infos = GetAsyncOperationHandleInfos<T>();
//            IAsyncOperationHandleInfo info = null;
//            if (infos.TryGetValue(address, out info))
//            {
//                if (HasPool(address))
//                    ReleasePool(address);

//                infos.Remove(address);
//                Addressables.Release(info.Result);
//            }

//            Debug.Log($"[ResourcesSystem:ReleaseAsset] Release asset {address} done.");
//        }

//        /// <summary>
//        /// This will call gc after release
//        /// </summary>
//        public void ReleaseAllAsset()
//        {
//            Debug.Log($"[ResourcesSystem:ReleaseAllAsset] Release all asset.");
//            ReleaseAllPool();

//            Dictionary<Type, Dictionary<string, IAsyncOperationHandleInfo>> ignoreHandle = new Dictionary<Type, Dictionary<string, IAsyncOperationHandleInfo>>();
//            foreach (var assetHandles in _asyncAssetHandles)
//            {
//                Type handleType = assetHandles.Key;
//                Dictionary<string, IAsyncOperationHandleInfo> handles = assetHandles.Value;

//                foreach (KeyValuePair<string, IAsyncOperationHandleInfo> assetHandle in handles)
//                {
//                    string address = assetHandle.Key;
//                    if ((PoolSettings.Instance.TryGetSetting(address, out PoolSettings.PoolSetting setting) && setting.IgnoreAllRelease) || _customIgnoreReleaseAssets.Contains(address))
//                    {
//                        if (!ignoreHandle.ContainsKey(handleType))
//                            ignoreHandle.Add(handleType, new Dictionary<string, IAsyncOperationHandleInfo>());

//                        IAsyncOperationHandleInfo handle = _asyncAssetHandles[handleType][address];
//                        ignoreHandle[handleType].Add(address, handle);
//                        continue;
//                    }

//                    Addressables.Release(assetHandle.Value.Result);
//                }
//            }
//            _asyncAssetHandles.Clear();

//            foreach (var handle in ignoreHandle)
//                _asyncAssetHandles.Add(handle.Key, handle.Value);

//            CallGC();
//            Debug.Log($"[ResourcesSystem:ReleaseAllAsset] Release all asset done.");
//        }

//        /// <summary>
//        /// This will release all asset include pool.
//        /// </summary>
//        public void Release()
//        {
//            ReleaseAllPool();
//            ReleaseAllAsset();
//        }

//        public void AddIgnoreReleaseAsset(string address)
//        {
//            if (!_customIgnoreReleaseAssets.Contains(address))
//                _customIgnoreReleaseAssets.Add(address);
//        }

//        public void RemoveIgnoreReleaseAsset(string address)
//        {
//            if (_customIgnoreReleaseAssets.Contains(address))
//                _customIgnoreReleaseAssets.Remove(address);
//        }

//        #endregion

//        #region AsyncHandle

//        private Dictionary<string, IAsyncOperationHandleInfo> GetAsyncOperationHandleInfos<T>() where T : Object
//        {
//            Type type = typeof(T);
//            Dictionary<string, IAsyncOperationHandleInfo> infos = null;
//            if (!_asyncAssetHandles.TryGetValue(type, out infos))
//            {
//                var newInfos = new Dictionary<string, IAsyncOperationHandleInfo>();
//                _asyncAssetHandles.Add(type, newInfos);
//                infos = newInfos;
//            }
//            return infos;
//        }

//        private async Task<T> GetAsyncOperationHandleInfo<T>(string address) where T : Object
//        {
//            IAsyncOperationHandleInfo info = await GetOrCreateAssetAsyncOperationHandleInfo<T>(address);
//            return info.Result as T;
//        }

//        private async Task<AsyncOperationHandleInfo<T>> GetOrCreateAssetAsyncOperationHandleInfo<T>(string address) where T : Object
//        {
//            Type type = typeof(T);
//            if (type.IsSubclassOf(typeof(Component)))
//            {
//                Debug.LogError($"[ResourcesSystem:GetOrCreateAssetAsyncOperationHandleInfo] Get asset form ass just suppot Object type, not support component.");
//                return null;
//            }

//            Dictionary<string, IAsyncOperationHandleInfo> infos = GetAsyncOperationHandleInfos<T>();
//            bool result = infos.TryGetValue(address, out IAsyncOperationHandleInfo info);
//            if (result)
//            {
//                await info.Task();
//            }
//            if (info != null && info.Result == null)
//            {
//                // Remove cache.
//                ReleaseAsset<T>(address);
//                result = false;
//            }
//            if (!result)
//            {
//                AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(address);
//                var handleInfo = new AsyncOperationHandleInfo<T>(address, handle);
//                infos.Add(address, handleInfo);
//                info = handleInfo;
//                await info.Task();
//            }
//            return info as AsyncOperationHandleInfo<T>;
//        }
//        #endregion

//        private void CallGC()
//        {
//            Debug.Log($"[ResourcesSystem:ReleaseAllAsset] Start garbage collection, current memory used : {GC.GetTotalMemory(false)}");
//            UnityEngine.Resources.UnloadUnusedAssets();
//            GC.Collect();
//            Debug.Log($"[ResourcesSystem:ReleaseAllAsset] Garbage collection is done, current memory used : {GC.GetTotalMemory(false)}");
//        }
//    }
//}