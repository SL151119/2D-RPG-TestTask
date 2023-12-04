using System.Collections.Generic;

public interface IShopCustomer
{
    public void BuyItem(ItemShop.ItemShopType itemShopType);

    public bool TrySpendGoldAmount(int goldAmount);

    public void SellItem(ItemShop.ItemShopType itemType, int quantity);

    public bool TryReceiveGoldAmount(int goldAmount);

    public List<Item> GetItemList();
}
