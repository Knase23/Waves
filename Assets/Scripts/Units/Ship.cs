﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour, IPlayerShipControl ,IDamagable
{
    public int speed = 10;

    public Color shipColor = Color.green;

    #region Input Dependent
    
    #endregion

    public Health hp = new Health();

    public Action actionOne;
    public Action actionTwo;
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
