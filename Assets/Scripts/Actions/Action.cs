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

    public virtual void ApplyUpgrade(Ship ship, Upgrade upgrade)
    {
        ship.StoreUpgrade(null);
    }
}
public struct TriggerActionPackage
{
    public long userId;
    public int actionNumber;
    public TriggerActionPackage(long userId,int actionNumber)
    {
        this.userId = userId;
        this.actionNumber = actionNumber;
    }
    public TriggerActionPackage(byte[] data)
    {
        userId = BitConverter.ToInt64(data,0);
        actionNumber = BitConverter.ToInt32(data, 8);
    }
    public byte[] ToBytes()
    {
        List<byte> vs = new List<byte>();

        vs.AddRange(BitConverter.GetBytes(userId));
        vs.AddRange(BitConverter.GetBytes(actionNumber));
        return vs.ToArray();
    }

}