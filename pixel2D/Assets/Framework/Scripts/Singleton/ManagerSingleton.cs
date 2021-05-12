using System.Threading.Tasks;
using UnityEngine;

namespace Framework.Scripts.Singleton
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

        public virtual async Task Init()
        {
            Debug.Log("*********** Start " + GetType().Name);
        }
    }
}