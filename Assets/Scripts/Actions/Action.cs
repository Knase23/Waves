using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]
public abstract class Action : MonoBehaviour
{
    [SerializeField]
    public float cooldownTime;

    private float timer;

    public void Execute(long userId)
    {
        if(timer <= 0)
        {
            ActionExecute(userId);
            timer = cooldownTime;
        }
    }

    protected abstract void ActionExecute(long userId);
    public void Update()
    {
        timer -= Time.deltaTime;
    }

    public virtual void ApplyUpgrade(Ship ship, Upgrade upgrade)
    {
        ship.StoreUpgrade(null);
    }
}
