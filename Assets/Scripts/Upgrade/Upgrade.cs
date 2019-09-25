using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class Upgrade : MonoBehaviour
{

    public virtual void ApplyUpgrade(Ship ship)
    {
        

    }

    private void OnTriggerEnter(Collider other)
    {
        Ship ship = other.GetComponent<Ship>();
        
        if(ship)
        {
            ApplyUpgrade(ship);
            Destroy(gameObject);
        }

    }

    protected static T GetAppropriateAction<T>(Ship ship) where T:Action
    {
        T result = null;

        if(ship.actionOne.GetType() == typeof(T))
        {
            result = (T)ship.actionOne;
            return result;
        }
        if (ship.actionTwo.GetType() == typeof(T))
        {
            result = (T)ship.actionTwo;
            return result;
        }
        if (ship.actionThree.GetType() == typeof(T))
        {
            result = (T)ship.actionOne;
            return result;
        }
        return result;
    }
    

}
