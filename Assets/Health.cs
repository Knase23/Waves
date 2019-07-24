public class Health 
{
    float maxHealth;
    float health;
    public delegate void OnHealthZeroOrUnder();
    public OnHealthZeroOrUnder death;

    public Health(int maxHealth = 100)
    {
        this.maxHealth = maxHealth;
        health = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if(health <= 0)
        {
            death?.Invoke();
        }
    }
    public float GetHealth()
    {
        return health;
    }
    public float GetMaxHealth()
    {
        return maxHealth;
    }
}
