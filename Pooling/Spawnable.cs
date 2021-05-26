using UnityEngine;

public abstract class Spawnable : MonoBehaviour{

    public ObjectSpawner Spawner { get; set; }
    public bool Active { get; set; }

    public abstract void SetActiveState();

    public abstract void SetInactiveState();

}
