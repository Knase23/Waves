using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public abstract class Asteriod : MonoBehaviour
{
    public Sample placementRules;
    public bool ShowSearhRadius = true;

    private void OnDrawGizmosSelected()
    {
        if (placementRules != null)
        {
            Gizmos.color = new Color(1, 1, 1, 0.5f);
            Gizmos.DrawSphere(transform.position, placementRules.radius);

            if (ShowSearhRadius)
            {
                Gizmos.color = new Color(1f, 0, 0, 0.5f);
                Gizmos.DrawSphere(transform.position, placementRules.searchRadius);
            }
        }
    }
}
