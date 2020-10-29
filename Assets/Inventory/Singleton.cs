using UnityEngine;

namespace Assets
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static bool IsApplicationQuit;
        public static System.Object _lock = new System.Object();
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (IsApplicationQuit)
                    return null;

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = FindObjectOfType<T>();
                        if (_instance == null)
                        {
                            GameObject singleton = new GameObject("[SINGLETON]" + typeof(T));
                            _instance = singleton.AddComponent<T>();
                        }
                    }

                    return _instance;
                }
            }
        }

        public virtual void OnDestroy()
        {
           // IsApplicationQuit = true;
        }
    }
}