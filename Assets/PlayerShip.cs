using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShip : MonoBehaviour, IDamagable
{
    public GameObject prefabOfPulse;
    public int speed = 10;
    public int pulsePower = 10;
    public int pulseSpeed = 20;
    public int pulseMaxDistance = 10;
    public Color shipColor = Color.green;
    public string VerticalControllAxis = "Vertical";
    public string HorizontalControllAxis = "Horizontal";
    public string FirePulseControllAxis = "FirePulse";
    public Health hp = new Health();
    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown(FirePulseControllAxis))
        {
            Pulse pp = new Pulse(pulseSpeed, pulsePower, pulseMaxDistance, transform, shipColor);
        }

        if(Input.GetButton(VerticalControllAxis))
        {
            transform.position += transform.forward * Time.deltaTime * speed;
        }
        if (Input.GetButton(HorizontalControllAxis))
        {
            transform.Rotate(Input.GetAxis(HorizontalControllAxis) * Vector3.up * 10);
        }

    }
    public void TakeDamage(float damage)
    {
        hp.TakeDamage(damage);
    }

}
public interface IDamagable
{
    void TakeDamage(float damage);

}
