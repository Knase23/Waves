using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Collider))]
public class PickUp : MonoBehaviour
{
    public Upgrade upgrade;   

    private void OnTriggerEnter(Collider other)
    {
        Ship ship = other.GetComponent<Ship>();

        if (ship)
        {
            ship.StoreUpgrade(upgrade);
            Destroy(gameObject);
        }

    }
}
