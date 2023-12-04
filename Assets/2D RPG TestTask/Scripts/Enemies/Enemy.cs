using System;
using UnityEngine;

public class Enemy : Character
{
    [Header("ItemToDrop")]
    [SerializeField] private Item item;

    public event Action<Enemy> OnEnemyDeath;

    private Player target;

    private Animator animator;

    private CapsuleCollider2D capsuleCollider;

    private void OnEnable()
    {
        target = FindObjectOfType<Player>();    
    }

    private void Update()
    {
        if (!target.IsDead)
        {
            MoveTowardsTarget();
        }
    }

    private void MoveTowardsTarget()
    {
        Vector3 direction = (target.transform.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    protected override void OnTakeDamage()
    {
        if (TryGetComponent<Animator>(out animator))
        {
            animator.SetTrigger(Constants.ENEMY_TAKE_DAMAGE_PARAMETER);
        }

        SoundFXManager.PlaySound(SoundFXManager.Sound.EnemyHurt);
    }

    protected override void OnDead()
    {
        OnEnemyDeath?.Invoke(this); // Notify listeners that the enemy has died

        float waitSeconds = 1.5f;
        enabled = false;


        if (TryGetComponent<CapsuleCollider2D>(out capsuleCollider))
        {
            capsuleCollider.enabled = false;
        }

        SoundFXManager.PlaySound(SoundFXManager.Sound.EnemyDie);

        if (TryGetComponent<Animator>(out animator))
        {
            this.UniversalSequence(waitSeconds,
            () => animator.SetTrigger(Constants.ENEMY_DEAD_PARAMETER),
            () => ItemWorld.SpawnItemWorld(transform.position, item),
            () => Destroy(gameObject));
        }
    }
}
