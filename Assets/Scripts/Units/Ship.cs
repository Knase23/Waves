using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour, IPlayerShipControl, IDamagable
{
    public int speed = 10;

    public Color shipColor = Color.green;

    /// <summary>
    /// Health of the Ship
    /// </summary>
    public Health hp = new Health();

    /// <summary>
    /// Storing a picked up action waiting for which action to replace
    /// </summary>
    public Upgrade storedUpgrade;

    /// <summary>
    /// First action that the ship can use!
    /// </summary>
    public Action actionOne;

    /// <summary>
    /// Second action that the ship can use!
    /// </summary>
    public Action actionTwo;

    /// <summary>
    /// Third action that the ship can use!
    /// </summary>
    public Action actionThree;


    private Rigidbody rb;

    private void Start()
    {
        hp.OnDeath += OnDeath;
        rb = GetComponent<Rigidbody>();
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

    public void ActionOne()
    {
        actionOne.Execute();
    }

    public void ActionTwo()
    {
        actionTwo.Execute();
    }

    public void ActionThree()
    {
        actionThree.Execute();
    }

    public void Move(float horizontal, float vertical)
    {
        Vector3 increment = vertical * transform.forward * Time.deltaTime * speed;
        rb.velocity += increment;
        transform.Rotate(horizontal * Vector3.up * 10);
    }

    public void Move(Vector3 position)
    {
        transform.position = position;
    }
}
