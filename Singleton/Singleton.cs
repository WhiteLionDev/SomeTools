using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {

    private static T _instance;

    /// <summary>
    /// Singleton design pattern
    /// </summary>
    /// <value>The instance.</value>
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).ToString();
                    _instance = obj.AddComponent<T>();
                }
            }
            return _instance;
        }
    }
}
