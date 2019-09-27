using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PulseAction : Action
{
    public float speed, power, maxDistance;

    private Color color;

    private void Start()
    {
        color = GetComponent<Ship>().shipColor;
    }

    protected override void ActionExecute()
    {
        Pulse.CreatePulse(speed, power, maxDistance, transform, color);
    }

    public override void ApplyUpgrade(Ship ship, Upgrade upgrade)
    {

        if (upgrade is DamageUpgrade)
        {
            DamageUpgrade damageUpgrade = upgrade as DamageUpgrade;
            power += damageUpgrade.amount;
        }

        if (upgrade is CoolDownUpgrade)
        {
            CoolDownUpgrade cooldownUpgrade = upgrade as CoolDownUpgrade;
            cooldownTime -= cooldownUpgrade.amount;
        }

    }
}
