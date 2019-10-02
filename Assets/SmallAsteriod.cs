using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallAsteriod : Asteriod
{
    public float largeAsteriodMinThreshold = 1;
    public float mediumAsteriodMinThreshold = 2;
    public float largeAsteriodMaxThreshold = 6;
    public float mediumAsteriodMaxThreshold = 7;
    public int largeOrMediumNearIt = 0;
    public override bool ValidationOfPlacement(Vector3 position, Asteriod other)
    {
        if (SpehereIntersecting(position, other, 1))
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


    public override bool ValidatePlacement(Vector3 position)
    {
        largeOrMediumNearIt = 0;

        if (!base.ValidatePlacement(position))
        {
            return false;
        }
        //Must have atleast a medium or large asteriod near it;
        if (largeOrMediumNearIt < 1)
        {
            return false;
        }
        return true;
    }
}
