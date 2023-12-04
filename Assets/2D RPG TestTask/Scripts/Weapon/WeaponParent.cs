using UnityEngine;

public class WeaponParent : Damager
{
    [Header("Sprite Rendering and Animation")]
    [SerializeField] private SpriteRenderer characterRenderer, weaponRenderer;
    [SerializeField] private Animator animator;

    [Header("Attack Configuration")]
    [SerializeField] private float attackDelay = 0.4f;

    [Header("Attack Range")]
    [SerializeField] private Transform circleOrigin;
    [SerializeField] private float radius;

    private bool attackBlocked;
    private bool IsAttacking;

    public Vector2 PointerPosition { get; set; }

    private void Update()
    {
        if (IsAttacking)
        {
            return;
        }

        UpdateRotationAndSortingOrder();
    }

    private void UpdateRotationAndSortingOrder()
    {
        Vector2 direction = (PointerPosition - (Vector2)transform.position).normalized;
        transform.right = direction;

        Vector2 scale = transform.localScale;
        scale.x = Mathf.Sign(direction.x) * Mathf.Abs(scale.x);
        scale.y = Mathf.Sign(direction.x) * Mathf.Abs(scale.y);
        transform.localScale = scale;

        weaponRenderer.sortingOrder = transform.eulerAngles.z > 0 && transform.eulerAngles.z < 180
            ? characterRenderer.sortingOrder - 1
            : characterRenderer.sortingOrder + 1;
    }

    public void ResetIsAttacking()
    {
        IsAttacking = false;
    }

    public void Attack()
    {
        if (attackBlocked)
        {
            return;
        }

        animator.SetTrigger(Constants.ATTACK_PARAMETER);
        IsAttacking = true;
        attackBlocked = true;
        this.UniversalWait(attackDelay, () => attackBlocked = false);
    }

    public void DetectColliders()
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(circleOrigin.position, radius);

        bool anyDamaged = false;

        foreach (Collider2D target in targets)
        {
            anyDamaged |= TryDoDamage(target);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(circleOrigin.position, radius);
    }
}