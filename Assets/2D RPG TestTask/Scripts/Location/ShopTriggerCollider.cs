using UnityEngine;

public class ShopTriggerCollider : MonoBehaviour
{
    [Header("UI Shop")]
    [SerializeField] private UIShop uiShop;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HandleTrigger(collision, true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        HandleTrigger(collision, false);
    }

    private void HandleTrigger(Collider2D collision, bool shouldShow)
    {
        if (collision.TryGetComponent<IShopCustomer>(out var shopCustomer))
        {
            if (shouldShow)
            {
                uiShop.Show(shopCustomer);
            }
            else
            {
                uiShop.Hide();
            }
        }
    }
}
