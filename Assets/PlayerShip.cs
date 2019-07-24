using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShip : MonoBehaviour, IDamagable
{
    public int speed = 10;
    public int pulsePower = 10;
    public int pulseSpeed = 20;
    public int pulseMaxDistance = 10;

    public float pulseCooldown = 0.3f;

    public float pulseCooldownTimer;

    public Color shipColor = Color.green;
    public string VerticalControllAxis = "Vertical";
    public string HorizontalControllAxis = "Horizontal";
    public string FirePulseControllAxis = "FirePulse";
    public Health hp = new Health();

    private Rigidbody rigidbody;

    private void Start()
    {
        hp.death += OnDeath;
        rigidbody = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown(FirePulseControllAxis) && pulseCooldownTimer <= 0)
        {
            Pulse.CreatePulse(pulseSpeed, pulsePower, pulseMaxDistance, transform, shipColor);
            pulseCooldownTimer = pulseCooldown;
        }
        if (pulseCooldownTimer > 0)
        {
            pulseCooldownTimer -= Time.deltaTime;
        }
        if (Input.GetButton(VerticalControllAxis))
        {
            Vector3 increment = Input.GetAxis(VerticalControllAxis) * transform.forward * Time.deltaTime * speed;
            rigidbody.velocity += increment;
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
    public void OnDeath()
    {
        Debug.Log(gameObject.name + " - You Dead!");
        gameObject.SetActive(false);
    }
}
public interface IDamagable
{
    void TakeDamage(float damage);

}
