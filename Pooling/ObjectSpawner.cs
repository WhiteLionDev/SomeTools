using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public ObjectPool objectPool;
    [Range(1f, 5f)]
    public float coolDownTime;

    void Start()
    {
        InvokeRepeating("SpawnObject", 1f, coolDownTime);
    }
    
    public void SpawnObject()
    {
        if (objectPool != null)
        {
            GameObject go = objectPool.GetGameObject();
            go.transform.position = this.transform.position;
            go.GetComponent<Spawnable>().Spawner = this;
        }
    }

    public void RemoveObject(GameObject go)
    {
        if (objectPool != null)
        {
            go.transform.position = objectPool.transform.position;
            objectPool.ReleaseGameObject(go);
        }
    }
}