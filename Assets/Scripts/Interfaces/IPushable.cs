using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPushable 
{
    void PushAway(Vector3 directionToPush, float strength);
}
