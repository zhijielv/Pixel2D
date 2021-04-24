using Framework.Scripts.Singleton;
using UnityEngine.AddressableAssets;

namespace Framework.Scripts.Manager
{
    public class AddressableManager : ManagerSingleton<AddressableManager>
    {
        public AddressableManager()
        {
            Addressables.DownloadDependenciesAsync("preload");
        }
    }
}