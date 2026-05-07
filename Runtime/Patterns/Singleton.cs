using UnityEngine;

namespace LSH.Core
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static bool _isApplicationQuitting = false;

        public static T Instance
        {
            get
            {
                if (_isApplicationQuitting) return null;
                if (_instance == null)
                {
                    _instance = FindAnyObjectByType<T>();
                    if (_instance == null)
                        Debug.LogError($"{typeof(T).Name} is not initialized yet.");
                }
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this as T;
            if (transform.parent != null)
            {
                transform.SetParent(null);
            }
            DontDestroyOnLoad(gameObject);
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
                _isApplicationQuitting = true;
        }
    }
}