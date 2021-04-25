using Framework.Scripts.Singleton;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System;
using System.Threading.Tasks;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Framework.Scripts.Manager
{
    public class AddressableManager : ManagerSingleton<AddressableManager>
    {
        private static bool _initialized = false;
        public bool Ready => _initialized;

        public override async Task Init()
        {
            await Addressables.InitializeAsync().Task;
            await Addressables.DownloadDependenciesAsync("preload").Task;
            _initialized = true;
        }

        public async Task<T> LoadAsset<T>(object key) where T : Object
        {
            if (!_initialized)
                throw new Exception("AddressableManager has not initialized!");
#if UNITY_EDITOR
            T op = AssetDatabase.LoadAssetAtPath<T>((string) key);
#else
            T op = await Addressables.LoadAssetAsync<T>(key).Task;
#endif
            
            if (op == null)
            {
                var message = "Sync LoadAsset has null result " + key;
                throw new Exception(message);
            }

            return op;
        }

        public async Task<GameObject> Instantiate(object key, Transform parent = null, bool instantiateInWorldSpace = false)
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

        public void ReleaseInstance(GameObject gameobject)
        {
            Addressables.ReleaseInstance(gameobject);
        }
    }
}