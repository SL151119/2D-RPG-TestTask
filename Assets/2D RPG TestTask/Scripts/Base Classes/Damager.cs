using UnityEngine;

public class Damager : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private int damage;

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        TryDoDamage(collision);
    }

    protected bool TryDoDamage(Collider2D collision)
    {
        bool hasHealth = collision.TryGetComponent<Character>(out var character);
        bool otherHealth = !collision.CompareTag(tag);

        if (hasHealth && otherHealth)
        {
            character.TakeDamage(damage);
            return true;
        }

        return false;
    }
}
