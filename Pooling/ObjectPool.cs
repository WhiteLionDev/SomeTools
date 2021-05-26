using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    public GameObject prefab;
    public int amount;
    private List<GameObject> _available = new List<GameObject>();

    void Awake()
    {
        InstantiateGameObjects();
    }

    private void InstantiateGameObjects()
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject go = Instantiate(prefab, transform.position, Quaternion.identity) as GameObject;
            go.SendMessage("SetInactiveState", SendMessageOptions.RequireReceiver);
            _available.Add(go);
        }
    }

    public GameObject GetGameObject()
    {
        if (_available.Count > 0)
        {
            GameObject go = _available[0];
            go.SendMessage("SetActiveState", SendMessageOptions.RequireReceiver);
            _available.RemoveAt(0);
            return go;
        }
        else
        {
            GameObject go = Instantiate(prefab, transform.position, Quaternion.identity) as GameObject;
            go.SendMessage("SetActiveState", SendMessageOptions.RequireReceiver);
            return go;
        }
    }

    public void ReleaseGameObject(GameObject go)
    {
        go.SendMessage("SetInactiveState", SendMessageOptions.RequireReceiver);
        _available.Add(go);
    }
}