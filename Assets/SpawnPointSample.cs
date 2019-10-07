using UnityEngine;

public class SpawnPointSample : Sample
{
    public override bool Validation(Vector3 position, Collider collider)
    {
        return true;
    }
}