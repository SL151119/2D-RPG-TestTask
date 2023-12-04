using Cinemachine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Character, IShopCustomer
{
    [Header("Inputs")]
    [SerializeField] private InputActionReference movement, attack, pointerPosition;
    
    private Camera cachedCamera;
    private UIInventory uiInventory;
    private HealthBar healthBar;

    private Rigidbody2D rigidBody2D;
    private CapsuleCollider2D capsuleCollider2D;
    private WeaponParent weaponParent;
    private PlayerAnimations playerAnimations;
    private Inventory inventory;

    private Vector2 pointerInput, movementInput;
   
    private int currentHealth;

    private void OnEnable() => attack.action.performed += PerformAttack;

    private void Start()
    {
        InitializeComponents();
        InitializePlayer();
    }

    public void InitializeComponents()
    {
        cachedCamera = Camera.main;

        rigidBody2D = GetComponent<Rigidbody2D>();
        rigidBody2D.constraints &= ~RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;    //Unfreeze player

        playerAnimations = GetComponent<PlayerAnimations>();

        weaponParent = GetComponentInChildren<WeaponParent>();

        SetInventory();

        UpdateSwordVisibility();

        SetCurrentHealth();
    }

    private void FixedUpdate()
    {
        if (!IsDead)
        {
            Move();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {   
        if (collision.TryGetComponent<ItemWorld>(out var itemWorld))
        {   
            HandleItemPickup(itemWorld);
        }
    }

    private void HandleItemPickup(ItemWorld itemWorld)
    {
        SoundFXManager.PlaySound(SoundFXManager.GetPickupSound(itemWorld.GetItem()));

        inventory.AddItem(itemWorld.GetItem());
        uiInventory.SetPlayer(this);
        itemWorld.DestroySelf();

        UpdateSwordVisibility();
    }

    private void Update()
    {
        foreach (CinemachineVirtualCamera camera in FindObjectsOfType<CinemachineVirtualCamera>())
        {
            if (camera.Follow != transform)
            {
                camera.Follow = transform;
            }
        }

        pointerInput = GetPointerInput();
        weaponParent.PointerPosition = pointerInput;
    }

    public void InitializePlayer()
    {
        uiInventory = FindObjectOfType<UIInventory>();
        healthBar = FindObjectOfType<HealthBar>();
    }

    private void PerformAttack(InputAction.CallbackContext obj)
    {
        SoundFXManager.PlaySound(SoundFXManager.Sound.PlayerAttack);
        weaponParent.Attack();
    }

    private void AnimateCharacter()
    {
        Vector2 lookDirection = pointerInput - (Vector2)transform.position;
        playerAnimations.RotateToPointer(lookDirection);
        playerAnimations.RunningAnimation(movementInput);
    }

    private void Move()
    {
        movementInput = movement.action.ReadValue<Vector2>().normalized;

        UpdateCurrentSpeed();

        rigidBody2D.velocity = movementInput * currentSpeed;

        PlayMoveSound();

        AnimateCharacter();
    }

    private void UpdateCurrentSpeed()
    {
        if (movementInput.magnitude > 0 && currentSpeed >= 0)
        {
            currentSpeed += acceleration * moveSpeed * Time.deltaTime;
        }
        else
        {
            currentSpeed -= deacceleration * moveSpeed * Time.deltaTime;
        }
        currentSpeed = Mathf.Clamp(currentSpeed, 0, moveSpeed);
    }

    private void PlayMoveSound()
    {
        if (currentSpeed != 0)
        {
            SoundFXManager.PlaySound(SoundFXManager.Sound.PlayerMove);
        }
    }

    public Vector3 GetPosition() => transform.localPosition;

    private Vector2 GetPointerInput()
    {
        Vector3 mousePos = pointerPosition.action.ReadValue<Vector2>();
        mousePos.z = cachedCamera.nearClipPlane;
        return cachedCamera.ScreenToWorldPoint(mousePos);
    }

    private void SetInventory()
    {
        inventory = new Inventory(UseItem);
        uiInventory.SetInventory(inventory);
    }

    private void SetCurrentHealth()
    {
        currentHealth = Health;
        healthBar.SetHealth(currentHealth);
    }

    protected override void OnTakeDamage()
    {
        SetCurrentHealth();

        playerAnimations.HurtAnimation();

        SoundFXManager.PlaySound(SoundFXManager.Sound.PlayerHurt);
    }

    public List<Item> GetItemList() => inventory.GetItemList();

    public void UseItem(Item item)
    {
        switch (item.itemType)
        {
            case Item.ItemType.Carrot:
                TryHeal();
                break;
        }
    }

    private void TryHeal()
    {
        if (Health > 0 && Health < 100)
        {
            SoundFXManager.PlaySound(SoundFXManager.Sound.Healing);
            inventory.RemoveItem(new Item { itemType = Item.ItemType.Carrot, amount = 1 });
            Health += 5;
            SetCurrentHealth();
        }
    }

    public void BuyItem(ItemShop.ItemShopType itemType)
    {
        SoundFXManager.PlaySound(SoundFXManager.Sound.BuyItem);
        
        Item newItem = new Item { itemType = (Item.ItemType)itemType, amount = 1 };

        inventory.AddItem(newItem);

        UpdateInventoryAfterShop(itemType);
    }

    public void SellItem(ItemShop.ItemShopType itemType, int quantity)
    {
        SoundFXManager.PlaySound(SoundFXManager.Sound.SellItem);

        Item itemToSell = GetItemToSell(itemType);

        if (itemToSell != null)
        {
            itemToSell.amount -= quantity;

            if (itemToSell.amount <= 0)
            {       
                inventory.RemoveItem(itemToSell);
            }

            UpdateInventoryAfterShop(itemType);
        }
    }

    private Item GetItemToSell(ItemShop.ItemShopType itemType)
    {
        return GetItemList().FirstOrDefault(playerItem => playerItem.itemType == (Item.ItemType)itemType);
    }

    private void UpdateInventoryAfterShop(ItemShop.ItemShopType itemType)
    {
        uiInventory.SetInventory(inventory);

        if (itemType == ItemShop.ItemShopType.Sword)
        {
            UpdateSwordVisibility();
        }
    }

    public bool TrySpendGoldAmount(int spendGoldAmount)
    {
        int getCurrentGoldAmount = inventory.GetCurrentGoldAmount();

        List<Item> itemList = inventory.GetItemList();

        if (getCurrentGoldAmount >= spendGoldAmount)
        {
            foreach (Item inventoryItem in itemList)
            {
                if (inventoryItem.itemType == Item.ItemType.Coin)
                {
                    if (inventoryItem.amount >= spendGoldAmount)
                    {
                        inventoryItem.amount -= spendGoldAmount;
                        getCurrentGoldAmount -= spendGoldAmount;

                        if (inventoryItem.amount <= 0)
                        {
                            itemList.Remove(inventoryItem);     //Delete item from inventory
                        }

                        uiInventory.SetInventory(inventory);    //Update inventory UI

                        return true;
                    }
                }
            }
        }
        return false;
    }

    public bool TryReceiveGoldAmount(int receiveGoldAmount)
    {
        int getCurrentGoldAmount = inventory.GetCurrentGoldAmount();

        List<Item> itemList = inventory.GetItemList();

        bool foundCoinItem = false;

        foreach (Item inventoryItem in itemList)
        {
            if (inventoryItem.itemType == Item.ItemType.Coin)
            {
                inventoryItem.amount += receiveGoldAmount;
                getCurrentGoldAmount += receiveGoldAmount;
                foundCoinItem = true;

                uiInventory.SetInventory(inventory);
                return true;
            }
        }

        if (!foundCoinItem)
        {
            Item newCoinItem = new Item { itemType = Item.ItemType.Coin, amount = receiveGoldAmount };
            inventory.AddItem(newCoinItem);
            return true;
        }

        return false;
    }

    public void UpdateSwordVisibility()
    {
        Item swordItem = inventory.GetItemOfType(Item.ItemType.Sword);
        bool swordInInventory = swordItem != null;

        weaponParent.gameObject.SetActive(swordInInventory);

        if (swordInInventory)
        {
            attack.action.performed += PerformAttack;
        }
        else
        {
            attack.action.performed -= PerformAttack;
        }
    }

    protected override void OnDead()
    {
        enabled = false;

        if (TryGetComponent<CapsuleCollider2D>(out capsuleCollider2D))
        {
            capsuleCollider2D.enabled = false;
        }

        rigidBody2D.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;

        inventory.ClearInventory();
        UpdateSwordVisibility();

        SoundFXManager.PlaySound(SoundFXManager.Sound.PlayerDie);

        playerAnimations.DeadAnimation();
    }

    private void OnDestroy() => attack.action.performed -= PerformAttack;
}
