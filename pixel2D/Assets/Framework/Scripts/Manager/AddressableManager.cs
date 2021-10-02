/*
** Created by fengling
** DateTime:    2021-04-25 11:36:57
** Description: TODO 编写打包脚本
*/

using Framework.Scripts.Singleton;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System;
using System.Threading.Tasks;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Framework.Scripts.Manager
{
    public class AddressableManager : ManagerSingleton<AddressableManager>
    {
        private static bool _initialized = false;
        public bool Ready => _initialized;

        public override async Task ManagerInit()
        {
#if !UNITY_EDITOR
            PlayerPrefs.DeleteKey(Addressables.kAddressablesRuntimeDataPath);
#endif
            await Addressables.InitializeAsync().Task;
            await Addressables.DownloadDependenciesAsync("preload").Task;
            _initialized = true;
            await base.ManagerInit();
        }

        // 异步加载
        public async Task<T> LoadAssetAsync<T>(object key) where T : Object
        {
            if (!_initialized)
                throw new Exception("AddressableManager has not initialized!");
            T op = await Addressables.LoadAssetAsync<T>(key).Task;

            if (op == null)
            {
                var message = "Sync LoadAsset has null result " + key;
                throw new Exception(message);
            }

            return op;
        }

        // 同步加载
        public T LoadAsset<T>(object key) where T : Object
        {
            if (!_initialized)
                throw new Exception("AddressableManager has not initialized!");
            T op = Addressables.LoadAssetAsync<T>(key).WaitForCompletion();

            if (op == null)
            {
                var message = "Sync LoadAsset has null result " + key;
                throw new Exception(message);
            }

            return op;
        }

        public async Task<GameObject> InstantiateAsync(object key, Transform parent = null,
            bool instantiateInWorldSpace = false)
        {
            if (!_initialized)
                throw new Exception("AddressableManager has not initialized!");

            GameObject op = await Addressables.InstantiateAsync(key, parent, instantiateInWorldSpace).Task;
            if (op == null)
            {
                var message = "Sync Instantiate has null result " + key;
                throw new Exception(message);
            }

            return op;
        }

        public GameObject Instantiate(object key, Transform parent = null,
            bool instantiateInWorldSpace = false)
        {
            if (!_initialized)
                throw new Exception("AddressableManager has not initialized!");

            GameObject op = Addressables.InstantiateAsync(key, parent, instantiateInWorldSpace).WaitForCompletion();
            if (op == null)
            {
                var message = "Sync Instantiate has null result " + key;
                throw new Exception(message);
            }

            return op;
        }

        public async Task LoadScene(object key, LoadSceneMode loadSceneMode, bool activateOnLoad = true)
        {
            if (!_initialized)
                throw new Exception("AddressableManager has not initialized!");
            await Addressables.LoadSceneAsync(key, loadSceneMode, activateOnLoad).Task;
        }

        // todo 检查资源是否存在
        public bool CheckFile<T>(object key, out T result)
        {
            AsyncOperationHandle<T> loadAssetAsync;
            try
            {
                loadAssetAsync = Addressables.LoadAssetAsync<T>(key);
            }
            catch
            {
                result = default;
                return false;
            }
            result = loadAssetAsync.WaitForCompletion();
            return true;
        }

        public void ReleaseInstance(GameObject gameobject)
        {
            Addressables.ReleaseInstance(gameobject);
        }
    }
}