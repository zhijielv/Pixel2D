using Framework.Scripts.Singleton;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Framework.Scripts.Manager
{
    public class AddressableManager : ManagerSingleton<AddressableManager>
    {
        private static bool _initialized = false;
        public static bool Ready => _initialized;

        static void InitDone(AsyncOperationHandle<IResourceLocator> obj)
        {
            _initialized = true;
        }

        public void Start()
        {
            Addressables.InitializeAsync().Completed += InitDone;
            Addressables.DownloadDependenciesAsync("preload");
        }

        public T LoadAsset<T>(object key)
        {
            if (!_initialized)
                throw new Exception("Whoa there friend!  We haven't init'd yet!");

            var op = Addressables.LoadAssetAsync<T>(key);

            if (!op.IsDone)
                throw new Exception("Sync LoadAsset failed to load in a sync way! " + key);

            if (op.Result == null)
            {
                var message = "Sync LoadAsset has null result " + key;
                if (op.OperationException != null)
                    message += " Exception: " + op.OperationException;

                throw new Exception(message);
            }

            return op.Result;
        }

        public GameObject Instantiate(object key, Transform parent = null, bool instantiateInWorldSpace = false)
        {
            if (!_initialized)
                throw new Exception("Whoa there friend!  We haven't init'd yet!");

            var op = Addressables.InstantiateAsync(key, parent, instantiateInWorldSpace);

            if (!op.IsDone)
                throw new Exception("Sync Instantiate failed to finish! " + key);

            if (op.Result == null)
            {
                var message = "Sync Instantiate has null result " + key;
                if (op.OperationException != null)
                    message += " Exception: " + op.OperationException;

                throw new Exception(message);
            }

            return op.Result;
        }
    }
}