using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PulseAction : Action
{
    public float speed, power, maxDistance;
    public GameObject pulsePrefab;
    private Color color;

    private void Start()
    {
        color = GetComponent<Ship>().shipColor;
    }

    protected override void ActionExecute()
    {
        Pulse.CreatePulse(pulsePrefab, speed, power, maxDistance, transform,color);
    }

    public override void ApplyUpgrade(Ship ship, Upgrade upgrade)
    {
        if (upgrade is DamageUpgrade)
        {
            power += upgrade.amount;
        }

        if (upgrade is CoolDownUpgrade)
        {
            cooldownTime -= upgrade.amount;
        }

        if (upgrade is ActionSpeedUpgrade)
        {
            speed -= upgrade.amount;
        }

        base.ApplyUpgrade(ship,upgrade);
    }
}
