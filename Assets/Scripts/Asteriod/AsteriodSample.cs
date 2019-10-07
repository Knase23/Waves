using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AsteriodSample : Sample
{
    public override bool CompareWithOtherSample(Vector3 position, Collider otherSample)
    {
        Asteriod other = otherSample.GetComponent<Asteriod>();
        return true;
    }
}
