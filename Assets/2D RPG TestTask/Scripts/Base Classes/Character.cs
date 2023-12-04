using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] protected float moveSpeed;

    [Header("Health")]
    [SerializeField] private int maxHealth;

    protected float currentSpeed, acceleration = 50, deacceleration = 150;

    public bool IsDead => Health <= 0;
    public int Health { get; set; }

    private void Awake()
    {
        Health = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        Health = Mathf.Clamp(Health, 0, maxHealth);

        OnTakeDamage();

        if (IsDead)
        {
            OnDead();
        }
    }

    protected virtual void OnTakeDamage()
    {

    }

    protected virtual void OnDead()
    {

    }
}
