using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageUpgrade : Upgrade
{
    public float amount = 1;
    public override void ApplyUpgrade(Ship ship)
    {
        //Example Increase Damage for Pulse Action
        PulseAction action = GetAppropriateAction<PulseAction>(ship);
        action.power += amount;
    }



}
