using UnityEngine;

namespace Framework.Singleton
{
    public class ManagerSingleton<T> : MonoBehaviour where T : ManagerSingleton<T>
    {
        public static T Instance { get; private set; }

        protected void Awake()
        {
            if (Instance == null)
            {
                Instance = (T) this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}