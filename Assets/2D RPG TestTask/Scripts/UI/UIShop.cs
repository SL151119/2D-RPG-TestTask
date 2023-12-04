using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIShop : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button_UI buyButton;
    [SerializeField] private Button_UI sellButton;

    [Header("Trader Message")]
    [SerializeField] private GameObject traderMessage;

    [HideInInspector]
    public bool isTraderMessageActive;

    private bool isBuyingTab;

    private Transform itemShopContainer;
    private Transform itemShopTemplate;

    private IShopCustomer shopCustomer;

    private readonly List<Transform> createdButtons = new List<Transform>();

    private void Awake()
    {
        itemShopContainer = transform.Find(Constants.SHOP_SLOT_CONTAINER);
        itemShopTemplate = itemShopContainer.Find(Constants.SHOP_SLOT_TEMPLATE);
        itemShopTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        buyButton.GetComponent<Button_UI>().ClickFunc = () => SetupButtons(true);
        sellButton.GetComponent<Button_UI>().ClickFunc = () => SetupButtons(false);

        Hide();
    }

    private void CreateButtonForAction(ItemShop.ItemShopType itemType, int positionIndex, Func<ItemShop.ItemShopType, int> getPriceFunc)
    {
        CreateItemButton(itemType, ItemShop.GetSprite(itemType), itemType.ToString(), getPriceFunc(itemType), positionIndex);
    }

    private void CreateItemButton(ItemShop.ItemShopType itemType, Sprite itemSprite, string itemName, int itemCost, int positionIndex)
    {
        Transform shopItemTransform = Instantiate(itemShopTemplate, itemShopContainer);
        createdButtons.Add(shopItemTransform);

        shopItemTransform.gameObject.SetActive(true);
        RectTransform shopItemRectTransform = shopItemTransform.GetComponent<RectTransform>();

        float shopItemHeight = 90f;
        shopItemRectTransform.anchoredPosition = new Vector2(0, -shopItemHeight * positionIndex);

        shopItemTransform.Find(Constants.SHOP_ITEM_NAME).GetComponent<TextMeshProUGUI>().SetText(itemName);
        shopItemTransform.Find(Constants.SHOP_ITEM_COST).GetComponent<TextMeshProUGUI>().SetText(itemCost.ToString());

        shopItemTransform.Find(Constants.SHOP_ITEM_IMAGE).GetComponent<Image>().sprite = itemSprite;

        shopItemTransform.GetComponent<Button_UI>().ClickFunc = () =>
        {
            PerformShopAction(itemType);
        };
    }

    private void ClearCreatedButtons()
    {
        foreach (Transform itemButton in createdButtons)
        {
            if (itemButton != null)
            {
                Destroy(itemButton.gameObject);
            }
        }

        createdButtons.Clear();
    }

    private void SetupButtons(bool buttonForBuy)
    {
        SoundFXManager.PlaySound(SoundFXManager.Sound.ButtonClick);
        ClearCreatedButtons();

        isBuyingTab = buttonForBuy;

        ItemShop.ItemShopType[] shopItemTypes = { ItemShop.ItemShopType.Sword, ItemShop.ItemShopType.Axe, ItemShop.ItemShopType.Carrot };

        foreach (var itemType in shopItemTypes)
        {
            Func<ItemShop.ItemShopType, int> getPriceFunc = buttonForBuy ? ItemShop.GetCost : ItemShop.GetSellPrice;
            CreateButtonForAction(itemType, Array.IndexOf(shopItemTypes, itemType), getPriceFunc);
        }
    }

    private void PerformShopAction(ItemShop.ItemShopType itemType)
    {
        if (isBuyingTab)
        {
            TrySellItem(itemType);
        }
        else
        {
            TryBuyItem(itemType);
        }
    }

    private void TrySellItem(ItemShop.ItemShopType itemType)
    {
        if (shopCustomer.TrySpendGoldAmount(ItemShop.GetCost(itemType)))
        {
            shopCustomer.BuyItem(itemType);
        }
        else
        {
            ShowTraderMessage();
        }
    }

    private void TryBuyItem(ItemShop.ItemShopType itemType)
    {
        int quantity = 1;

        foreach (Item playerItem in shopCustomer.GetItemList())
        {
            if (playerItem.itemType == (Item.ItemType)itemType)
            {
                int buyPrice = ItemShop.GetSellPrice(itemType);

                if (shopCustomer.TryReceiveGoldAmount(buyPrice))
                {
                    shopCustomer.SellItem(itemType, quantity);
                }

                break;
            }
        }
    }

    private void ShowTraderMessage()
    {
        float waitForSeconds = 1.5f;

        if (!isTraderMessageActive)
        {
            this.UniversalSequenceTwo(waitForSeconds,
            () =>
            {
                SoundFXManager.PlaySound(SoundFXManager.Sound.NotEnoughGold);
                isTraderMessageActive = true;
                traderMessage.SetActive(true);
            },
            () =>
            {
                traderMessage.SetActive(false);
                isTraderMessageActive = false;
            });
        }
    }

    public void Show(IShopCustomer shopCustomer)
    {
        this.shopCustomer = shopCustomer;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        traderMessage.SetActive(false);
        isTraderMessageActive = false;
    }
}
