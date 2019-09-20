using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PulseAction : Action
{
    public float speed, power, maxDistance;
    public Color color;

    protected override void ActionExecute()
    {
        Pulse.CreatePulse(speed, power, maxDistance, transform, color);
    }
}
