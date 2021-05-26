using UnityEngine;

public class Obstacle : Spawnable
{
    public float speed;

    void Update()
    {
        if(Active)
            transform.Translate(Vector3.left * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name.Contains("Destroyer"))
        {
            Spawner.RemoveObject(gameObject);
        }
    }

    public override void SetActiveState()
    {
        Active = true;
    }

    public override void SetInactiveState()
    {
        Active = false;
    }

}
