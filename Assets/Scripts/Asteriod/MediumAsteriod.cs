using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MediumAsteriod : Asteriod
{
    public float largeAsteriodThreshold = 2;
    public float mediumAsteriodThreshold = 4;
    public override bool ValidationOfPlacement(Vector3 position, Asteriod other)
    {
        if (SpehereIntersecting(position, other, 1))
        {
            return false;
        }
        if(other is LargeAsteriod)
        {
            if (SpehereIntersecting(position, other, largeAsteriodThreshold))
            {
                return false;
            }
        }
        if(other is MediumAsteriod)
        {
            if (SpehereIntersecting(position, other, mediumAsteriodThreshold))
            {
                return false;
            }
        }
        return true;
    }
    
}

