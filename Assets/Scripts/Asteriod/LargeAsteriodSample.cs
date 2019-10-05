using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Large Asteriod Placement Rules", menuName = "Placement Rules/Large Asteriod", order = 1)]
public class LargeAsteriodSample : Sample
{
    // Large 
    /*
     * 
     * They have a set Radius, determined by the prefabs Asteriod scripts placement radius
     * There minimum distance from a Another Large Asteriod can be there Placement Radius + Tolerence 
     */
    public float minimumDistanceFromOtherLargeAsteriod = 1;

    public override bool Validation(Vector3 position, Collider collider)
    {
        Asteriod other = collider.GetComponent<Asteriod>();
        //Is it the minimum Distance from each other
        if (SpehereIntersecting(position, other, minimumDistanceFromOtherLargeAsteriod))
        {
            return false;
        }

        return true;
    }
}
