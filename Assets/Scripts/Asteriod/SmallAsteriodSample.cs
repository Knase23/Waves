using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Small Asteriod Placement Rules", menuName = "Placement Rules/Small Asteriod", order = 1)]
public class SmallAsteriodSample : AsteriodSample
{
    // Small
    /*
     * 
     * The Small must have atleast 1 Medium or Large asteriod near it
     * 
     * There minimum distance from a Large Asteriod can be there Placement Radius + LargeMinTolerence 
     * There maximum distance from a Large Asteriod can be there Placement Radius + LargeMaxTolerence 
     * 
     * There minimum distance from a Medium Asteriod can be there Placement Radius + MediumMinTolerence
     * There maximum distance from a Medium Asteriod can be there Placement Radius + MediumMaxTolerence
     * 
     * There minimum distance from a Small Asteriod can be there Placement Radius + Tolerence 
     */
    public float largeAsteriodMinThreshold = 1;
    public float mediumAsteriodMinThreshold = 2;
    public float largeAsteriodMaxThreshold = 6;
    public float mediumAsteriodMaxThreshold = 7;
    private int largeOrMediumNearIt = 0;

    public override bool PreValidation(Vector3 position)
    {
        largeOrMediumNearIt = 0;
        return true;
    }
    public override bool Validation(Vector3 position, Collider collider)
    {
        Asteriod other = collider.GetComponent<Asteriod>();
        if (SpehereIntersecting(position, other, 0.8f))
        {
            return false;
        }
        if (other is LargeAsteriod)
        {
            if (SpehereIntersecting(position, other, largeAsteriodMinThreshold))
            {
                return false;
            }
            if (SpehereIntersecting(position, other, largeAsteriodMaxThreshold))
            {
                largeOrMediumNearIt++;
                return true;
            }
        }

        if (other is MediumAsteriod)
        {
            if (SpehereIntersecting(position, other, mediumAsteriodMinThreshold))
            {
                return false;
            }
            if (SpehereIntersecting(position, other, mediumAsteriodMaxThreshold))
            {
                largeOrMediumNearIt++;
                return true;
            }

        }
        return true;
    }
    public override bool PostValidation(Vector3 position)
    {
        if (largeOrMediumNearIt < 1)
        {
            return false;
        }
        return true;
    }
}
