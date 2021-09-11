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
        }

        // 管理类初始化，有些需要异步，这里取消某些地方没有使用异步的warning提示
#pragma warning disable 1998
        public virtual async Task ManagerInit()
#pragma warning restore 1998
        {
            Debug.Log("*********** Start " + GetType().Name);
        }
    }
}