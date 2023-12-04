using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class Item
{
    public enum ItemType 
    { 
        Sword,
        Axe,
        Carrot,
        Coin
    }

    public ItemType itemType;
    public int amount;

    public Sprite GetSprite()
    {
        return itemType switch
        {
            ItemType.Sword => ItemAssets.Instance.swordSprite,
            ItemType.Axe => ItemAssets.Instance.axeSprite,
            ItemType.Carrot => ItemAssets.Instance.carrotSprite,
            ItemType.Coin => ItemAssets.Instance.coinSprite,
            _ => throw new NotImplementedException()
        };
    }

    public bool IsStackable()
    {
        ItemType[] stackableTypes = { ItemType.Carrot, ItemType.Coin };

        return stackableTypes.Contains(itemType);
    }
}
