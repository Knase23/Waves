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

    public void Execute()
    {
        if(timer <= 0)
        {
            ActionExecute();
            timer = cooldownTime;
        }
    }

    protected abstract void ActionExecute();
    public void Update()
    {
        timer -= Time.deltaTime;
    }
}
