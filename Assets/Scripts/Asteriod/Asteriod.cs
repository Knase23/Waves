using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public abstract class Asteriod : MonoBehaviour
{
    public float radius = 1;
    public float searchRadius = 1;

    public bool ShowSearhRadius = true;

    // Everything that is true for All Asteriods.
    // Can be overided but it needs to have base.ValidatePlacement with it. 
    public virtual bool ValidatePlacement(Vector3 position)
    {
        if (Physics.CheckSphere(position, radius, 1 << 9))
        {
            return false;
        }

        Collider[] listOfHits = Physics.OverlapSphere(position, searchRadius, 1 << 9);
        for (int i = 0; i < listOfHits.Length; i++)
        {
            if (listOfHits[i].tag != "Asteriod")
            {
                continue;
            }
            if (!ValidationOfPlacement(position, listOfHits[i].GetComponent<Asteriod>()))
            {
                return false;
            }

        }
        // Placement is valid, you can place the Asteriod there
        return true;

    }

    /// <summary>
    /// Checks if it can be placed on that position for a certain Asteriod.
    /// </summary>
    /// <param name="postion"></param>
    /// <returns></returns>
    public abstract bool ValidationOfPlacement(Vector3 position, Asteriod other);

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 1, 1, 0.5f);
        Gizmos.DrawSphere(transform.position, radius);

        if (ShowSearhRadius)
        {
            Gizmos.color = new Color(1f, 0, 0, 0.5f);
            Gizmos.DrawSphere(transform.position, searchRadius);
        }
    }
    /// <summary>
    /// True if it is intersecting
    /// </summary>
    /// <param name="position"></param>
    /// <param name="other"></param>
    /// <param name="distanceThreshold"></param>
    /// <returns>Is it intersecting with that object</returns>
    public bool SpehereIntersecting(Vector3 position, Asteriod other, float distanceThreshold)
    {
        Vector3 vectorBetweenAsteriods = other.transform.position - position;
        float diffrence = Mathf.Abs(other.radius + radius - vectorBetweenAsteriods.magnitude);
        if (diffrence >= distanceThreshold)
        {
            return false;
        }

        return true;
    }

}
