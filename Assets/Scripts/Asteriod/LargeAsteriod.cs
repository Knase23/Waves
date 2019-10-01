using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LargeAsteriod : Asteriod
{
    public float minimumDistanceFromOtherLargeAsteriod = 1;

    public override bool ValidationOfPlacement(Vector3 position, Asteriod other)
    {
        if (SpehereIntersecting(position,other, minimumDistanceFromOtherLargeAsteriod))
        {
            return false;
        }

        return true;
    }
}
