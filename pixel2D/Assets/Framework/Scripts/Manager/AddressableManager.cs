using Framework.Scripts.Singleton;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Framework.Scripts.Manager
{
    public class AddressableManager : ManagerSingleton<AddressableManager>
    {
        public AddressableManager()
        {
            Addressables.InitializeAsync();
            Addressables.DownloadDependenciesAsync("preload");
        }

        public void LoadView(string viewName, Transform parent, bool worldSpace = false, bool trackHandle = true)
        {
            Addressables.InstantiateAsync(viewName, parent, worldSpace, trackHandle);
        }
    }
}