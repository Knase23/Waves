using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Damage Upgrade", menuName = "ScriptableObjects/Upgrade/DamageUpgrade", order = 1)]
public class DamageUpgrade : Upgrade
{
    public float amount = 1;
}
