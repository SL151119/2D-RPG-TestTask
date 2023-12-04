using System;
using UnityEngine;

public class ItemShop : MonoBehaviour
{
    public enum ItemShopType
    {
        Sword,
        Axe,
        Carrot
    }

    public static int GetCost(ItemShopType itemType)
    {
        return itemType switch
        {
            ItemShopType.Sword => 150,
            ItemShopType.Axe => 40,
            ItemShopType.Carrot => 5,
            _ => throw new NotImplementedException()
        };
    }

    public static int GetSellPrice(ItemShopType itemType)
    {
        return itemType switch
        {
            ItemShopType.Sword => 70,
            ItemShopType.Axe => 20,
            ItemShopType.Carrot => 2,
            _ => throw new NotImplementedException()
        };
    }

    public static Sprite GetSprite(ItemShopType itemType)
    {
        return itemType switch
        {
            ItemShopType.Sword => ItemAssets.Instance.swordSprite,
            ItemShopType.Axe => ItemAssets.Instance.axeSprite,
            ItemShopType.Carrot => ItemAssets.Instance.carrotSprite,
            _ => throw new NotImplementedException()
        };
    }
}
