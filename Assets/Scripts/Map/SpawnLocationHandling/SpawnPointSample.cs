using UnityEngine;
[CreateAssetMenu(fileName = "Spawn Point Placement Rules", menuName = "Placement Rules/Spawn Point", order = 4)]
public class SpawnPointSample : Sample
{
    public override bool Validation(Vector3 position, Collider collider)
    {
        //If there is any in the search Radius,
        // Then the validation failed
        return false;
    }
}