using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour, IPlayerShipControl, IDamagable
{
    public float movmentSpeed = 10;
    public float rotationSpeed = 2;

    public Color shipColor = Color.green;

    /// <summary>
    /// Health of the Ship
    /// </summary>
    public Health hp = new Health();

    /// <summary>
    /// Storing a picked up action waiting for which action to replace
    /// </summary>
    [SerializeField]
    private Upgrade storedUpgrade;

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

    public void StoreUpgrade(Upgrade upgrade)
    {
        if (upgrade == null)
        {
            storedUpgrade = null;
            return;
        }

        if (upgrade is MovmentSpeedUpgrade)
        {
            movmentSpeed = upgrade.amount;
            return;
        }

        storedUpgrade = upgrade;

    }

    #region IPlayerShipControl Functions

    #region Action 1
    public void ActionOne()
    {
        if (actionOne == null)
            return;

        actionOne.Execute();
    }
    public void ActionOneUpgrade()
    {
        if (actionOne == null)
            return;
        actionOne.ApplyUpgrade(this, storedUpgrade);
    }
    #endregion

    #region Action 2
    public void ActionTwo()
    {
        if (actionTwo == null)
            return;

        actionTwo.Execute();
    }
    public void ActionTwoUpgrade()
    {
        if (actionTwo == null)
            return;

        actionTwo.ApplyUpgrade(this, storedUpgrade);
    }
    #endregion

    #region Action 3
    public void ActionThree()
    {
        if (actionThree == null)
            return;

        actionThree.Execute();
    }
    public void ActionThreeUpgrade()
    {
        if (actionThree == null)
            return;

        actionThree.ApplyUpgrade(this, storedUpgrade);
    }
    #endregion

    #region Movement
    public void Move(float horizontal, float vertical)
    {
        Vector3 increment = vertical * transform.forward * Time.deltaTime * movmentSpeed;
        rb.velocity += increment;
        transform.Rotate(horizontal * Vector3.up * rotationSpeed);
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }

    public void Move(Vector3 position)
    {
        transform.position = position;
    }
    #endregion

    #endregion

    #region IDamageble Functions
    public void TakeDamage(float damage)
    {
        hp.TakeDamage(damage);
    }
    #endregion;

    #region Health Delegate Functions
    public void OnDeath()
    {
        Debug.Log(gameObject.name + " - You Dead!");
        gameObject.SetActive(false);
    }
    #endregion
}
