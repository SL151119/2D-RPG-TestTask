using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIInventory : MonoBehaviour
{
    public static UIInventory Instance { get; private set; }

    private Inventory inventory;

    private Transform itemSlotContainer;
    private Transform itemSlotTemplate;

    private Player player;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        itemSlotContainer = transform.Find(Constants.ITEM_SLOT_CONTAINER);
        itemSlotTemplate = itemSlotContainer.Find(Constants.ITEM_SLOT_TEMPLATE);
    }

    public void SetPlayer(Player player) => this.player = player;

    public void SetInventory(Inventory inventory)
    {
        this.inventory = inventory;

        inventory.OnItemListChanged += Inventory_OnItemListChanged;

        RefreshInventoryItems();
    }

    private void Inventory_OnItemListChanged(object sender, System.EventArgs e)
    {
        RefreshInventoryItems();
    }

    private void RefreshInventoryItems()
    {
        ClearInventorySlots();

        int x = 0;
        int y = 0;
        float itemSlotCellSize = 147f;

        foreach (Item item in inventory.GetItemList())
        {
            RectTransform itemSlotRectTransform = Instantiate(itemSlotTemplate, itemSlotContainer).GetComponent<RectTransform>();
            itemSlotRectTransform.gameObject.SetActive(true);

            SetButtonFunctionality(itemSlotRectTransform, item);

            itemSlotRectTransform.anchoredPosition = new Vector2(x * itemSlotCellSize, y * itemSlotCellSize);
            SetItemSlotImage(itemSlotRectTransform, item);
            SetItemAmountText(itemSlotRectTransform, item);

            x++;
            if (x > 2)
            {
                x = 0;
                y--;
            }
        }
    }

    private void ClearInventorySlots()
    {
        foreach (Transform child in itemSlotContainer)
        {
            if (child == itemSlotTemplate) continue;
            Destroy(child.gameObject);
        }
    }

    private void SetButtonFunctionality(RectTransform itemSlotRectTransform, Item item)
    {
        Button_UI buttonUI = itemSlotRectTransform.GetComponent<Button_UI>();
        buttonUI.ClickFunc = () => inventory.UseItem(item);
        buttonUI.MouseRightClickFunc = () =>
        {
            Item duplicateItem = new Item { itemType = item.itemType, amount = item.amount };
            inventory.RemoveItem(item);
            ItemWorld.DropItem(player.GetPosition(), duplicateItem);
            player.UpdateSwordVisibility();
        };
    }

    private void SetItemSlotImage(RectTransform itemSlotRectTransform, Item item)
    {
        Image image = itemSlotRectTransform.Find(Constants.ITEM_SLOT_IMAGE).GetComponent<Image>();
        image.sprite = item.GetSprite();
    }

    private void SetItemAmountText(RectTransform itemSlotRectTransform, Item item)
    {
        TextMeshProUGUI uiText = itemSlotRectTransform.Find(Constants.ITEM_AMOUNT_TEXT).GetComponent<TextMeshProUGUI>();
        uiText.SetText(item.amount > 1 ? item.amount.ToString() : "");
    }
}
