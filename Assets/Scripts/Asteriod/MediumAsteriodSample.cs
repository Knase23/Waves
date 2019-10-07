using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Medium Asteriod Placement Rules", menuName = "Placement Rules/Medium Asteriod", order = 2)]
public class MediumAsteriodSample : AsteriodSample
{
    // Medium
    /*
    * There minimum distance from a Large Asteriod can be there Placement Radius + LargeTolerence 
    * There minimum distance from a Medium Asteriod can be there Placement Radius + MediumTolerence
    */
    public float largeAsteriodThreshold = 2;
    public float mediumAsteriodThreshold = 4;

    public override bool Validation(Vector3 position, Collider collider)
    {
        //
        Asteriod other = collider.GetComponent<Asteriod>();
        if (SpehereIntersecting(position, other, 1))
        {
            return false;
        }
        if (other is LargeAsteriod)
        {
            if (SpehereIntersecting(position, other, largeAsteriodThreshold))
            {
                return false;
            }
        }
        if (other is MediumAsteriod)
        {
            if (SpehereIntersecting(position, other, mediumAsteriodThreshold))
            {
                return false;
            }
        }
        return true;
    }
}

