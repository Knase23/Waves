using System;
using UnityEngine;
[Serializable]
public class Health 
{
    [SerializeField]
    private float maxHealth;
    
    [SerializeField]
    private float health;

    public delegate void OnHealthZeroOrUnder();
    public OnHealthZeroOrUnder OnDeath;

    //public delegate void OnCriticalyDamaged();
    //public OnCriticalyDamaged onCriticalDamaged;



    public Health(int maxHealth = 100)
    {
        this.maxHealth = maxHealth;
        health = maxHealth;
    }

    /// <summary>
    /// Applies the damage taken. Can take in positive or negative. Recommend use positve values.
    /// </summary>
    /// <param name="damage">The amount of damage it takes, if its negative it will still reduce the health, but recommend only use positive numbers.</param>
    public void TakeDamage(float damage)
    {
        if (damage < 0)
            damage = -damage;

        health -= damage;
        CheckHealthCondition();
    }
    /// <summary>
    /// Checks if the condition of the item, sends invokes delegetes if the requerments are meet.
    /// </summary>
    private void CheckHealthCondition()
    {
        //if(health > 0 && health < maxHealth * 0.5f)
        //{
        //    onCriticalDamaged?.Invoke();
        //}

        if (health <= 0)
        {
            OnDeath?.Invoke();
        }
    }
    /// <summary>
    /// Gives the current health;
    /// </summary>
    /// <returns></returns>
    public float GetHealth()
    {
        return health;
    }

    /// <summary>
    /// Gives the current Max amount of Health
    /// </summary>
    /// <returns></returns>
    public float GetMaxHealth()
    {
        return maxHealth;
    }

    /// <summary>
    /// Changes the MaxHealth, and adjusts health according to how much MaxHealth incresed or decreases;
    /// </summary>
    /// <param name="amount">Amount of MaxHealth gained or Lost</param>
    public void ChangeMaxHealth(float amount)
    {
        maxHealth += amount;
        health += amount;

        CheckHealthCondition();
    }

}
