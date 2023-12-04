using UnityEngine;
using TMPro;

public class ItemWorld : MonoBehaviour
{
    private Item item;

    private SpriteRenderer spriteRenderer;
    private TextMeshPro textMeshPro;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        textMeshPro = transform.Find(Constants.ITEM_AMOUNT_TEXT).GetComponent<TextMeshPro>();
    }

    public static ItemWorld SpawnItemWorld(Vector3 position, Item item)
    {
        Transform transform = Instantiate(ItemAssets.Instance.itemWorldPrefab, position, Quaternion.identity);

        ItemWorld itemWorld = transform.GetComponent<ItemWorld>();
        itemWorld.SetItem(item);

        return itemWorld;
    }

    public static ItemWorld DropItem(Vector3 dropPosition, Item item)
    {
        SoundFXManager.PlaySound(SoundFXManager.Sound.DropItem);
        Vector3 randomDir = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
        ItemWorld itemWorld = SpawnItemWorld(dropPosition + randomDir * 1f, item);
        return itemWorld;
    }

    public void SetItem(Item item)
    {
        this.item = item;
        spriteRenderer.sprite = item.GetSprite();

        textMeshPro.SetText(item.amount > 1 ? item.amount.ToString() : "");
    }

    public Item GetItem()
    {
        return item; 
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
