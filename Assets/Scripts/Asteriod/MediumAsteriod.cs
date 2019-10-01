using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MediumAsteriod : Asteriod
{

    public override bool ValidationOfPlacement(Vector3 position, Asteriod other)
    {
        if (SpehereIntersecting(position, other, radius))
        {
            return false;
        }
        if(other is LargeAsteriod)
        {

        }
        return true;
    }
}

