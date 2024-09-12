using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_Inventory : MonoBehaviour
{
    public static Player_Inventory Instance; // Access this class by its Instance
    Player_Action pA;

    public List<Item> itemList;

    [SerializeField] private Button switchWeaponImage; // Referensi ke Image yang digunakan untuk mengganti senjata
    [SerializeField] private Button switchUseItemImage; // Referensi ke Image yang digunakan untuk mengganti senjata

    [HideInInspector] public bool meleeOrRanged = true;
    [HideInInspector] public bool itemUse1 = true;

    [Header("UI ELEMENTS")]
    public Item emptyItem;
    [HideInInspector] public List<Item> equippedCombat = new List<Item>(2);
    [HideInInspector] public Item equippedWeapon;
    [HideInInspector] public Item equippedItem;
    [HideInInspector] public List<Item> quickSlots = new List<Item>(2);

    InventoryUI inventoryUI;
    [HideInInspector] public bool inventoryOpened;
    [HideInInspector] public bool inventoryClosed;

    [SerializeField] ParticleSystem healParticle;
    public Button inventoryButton;  // Drag and drop the button in the inspector
    public Button closeInventoryButton;  // Drag and drop the close button in the inspector

    private void Awake()
    {
        Instance = this;
        pA = GetComponent<Player_Action>();

        // Making sure everything in it is a clone
        List<Item> newList = new List<Item>();
        foreach (Item item in itemList)
        {
            newList.Add(Instantiate(item));
        }
        itemList.Clear();
        itemList = newList;

        emptyItem = Instantiate(emptyItem);
    }

    private void Start()
    {
        inventoryUI = PlayerUI.Instance.inventoryUI.GetComponent<InventoryUI>();
        inventoryButton.onClick.AddListener(ToggleInventory); // Add listener to button
        closeInventoryButton.onClick.AddListener(CloseInventory); // Add listener to close button

 // Pastikan switchWeaponImage tidak null
        if (switchWeaponImage != null)
        {
            // Tambahkan event listener untuk klik pada Image
            Button button = switchWeaponImage.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(ToggleWeapon);
            }
            else
            {
                Debug.LogError("Image component does not have a Button component attached.");
            }
        }
        else
        {
            Debug.LogError("SwitchWeaponImage is not assigned.");
        }


        if (switchUseItemImage != null)
        {
            // Tambahkan event listener untuk klik pada Image
            Button button = switchUseItemImage.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(ToggleUseItem);
            }
            else
            {
                Debug.LogError("Image component does not have a Button component attached.");
            }
        }
        else
        {
            Debug.LogError("switchUseItemImage is not assigned.");
        }
    }

   private void Update()
    {
        

            UpdateEquippedWeaponUI();
            UpdateItemUseUI();
    }


    private void ToggleInventory()
{
    if (SoundManager.Instance != null)
        SoundManager.Instance.PlaySound("Click");

    inventoryOpened = !inventoryOpened;
    inventoryClosed = !inventoryOpened; // Update inventoryClosed
    PlayerUI.Instance.inventoryUI.SetActive(inventoryOpened);

    GameController.Instance.ShowPersistentUI(!inventoryOpened);

    if (inventoryOpened)
    {
        GameController.Instance.PauseGame();
        Instance.AddItem(ItemPool.Instance.GetItem("BuahCabai"));
        Instance.AddItem(ItemPool.Instance.GetItem("Batu"));
        Instance.AddItem(ItemPool.Instance.GetItem("Batu"));
        Instance.AddItem(ItemPool.Instance.GetItem("Batu"));
        Instance.AddItem(ItemPool.Instance.GetItem("Daging Panggang"));
        Instance.AddItem(ItemPool.Instance.GetItem("Daging Panggang"));
        Instance.AddItem(ItemPool.Instance.GetItem("Pedang Ren"));
        Instance.AddItem(ItemPool.Instance.GetItem("Penyiram Tanaman"));
        inventoryUI.UpdateInventoryUI(); // Update UI when inventory is opened
    }
    else
    {
        GameController.Instance.ResumeGame();
    }
}


    private void CloseInventory()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySound("Click");

        inventoryOpened = false;
        inventoryClosed = true;
        PlayerUI.Instance.inventoryUI.SetActive(false);
        GameController.Instance.ShowPersistentUI(true);
        GameController.Instance.ResumeGame();
    }

    public void AddItem(Item item)
    {
        item = Instantiate(item);
        if (item.isStackable && itemList.Exists(x => x.itemName == item.itemName))
        {
            itemList.Find(x => x.itemName == item.itemName).stackCount++;
        }
        else
        {
            itemList.Add(item);
        }

        print(item.itemName + " added to inventory");
         // Update the inventory UI
    PlayerUI.Instance.inventoryUI.GetComponent<InventoryUI>().UpdateInventoryUI();
    }

    public void RemoveItem(Item item)
    {
        item = Instantiate(item);
        if (item.isStackable)
        {
            itemList.Find(x => x.itemName == item.itemName).stackCount--;
            if (itemList.Find(x => x.itemName == item.itemName).stackCount <= 0)
                itemList.Remove(itemList.Find(x => x.itemName == item.itemName));
        }
        else
            itemList.Remove(itemList.Find(x => x.itemName == item.itemName));
    }

    public Item FindItemInInventory(string name)
    {
        if (itemList.Exists(x => x.itemName == name))
        {
            return itemList.Find(x => x.itemName == name);
        }
        return ItemPool.Instance.GetItem("Empty");
    }

    public void EquipItem(Item item, int index)
    {
        if (!itemList.Exists(x => x.itemName == item.itemName) && item.itemName != "Empty")
            return;

        equippedCombat[index] = item;
        PlayerUI.Instance.inventoryUI.GetComponent<InventoryUI>().SetActiveItem(index, item);
        print(item.itemName + " equipped");
    }

    // Add item to quick slot according index (0,1)
    public void AddQuickSlot(Item item, int index)
    {
        if (!itemList.Exists(x => x.itemName == item.itemName) && item.itemName != "Empty")
            return;

        switch (item.type)
        {
            case ItemType.Heal:
            case ItemType.Buff:
                break;
            default: break;
        }

        quickSlots[index] = item;
        PlayerUI.Instance.inventoryUI.GetComponent<InventoryUI>().SetActiveItem(index + 2, item);
        print(item.itemName + "equipped");
    }

    // Use which quick slot (1,2)
    public void UseQuickSlot(int which)
    {
        // Making sure there is an item in the quick slot
        Item item = quickSlots[which - 1];
        if (item == null || item.itemName == "Empty")
        {
            print("No item bish");
            return;
        }

        // Using them from inventory
        print("using quick slot " + (which - 1));
        item = itemList.Find(x => x.itemName == item.itemName);
        item.stackCount--;
        if (item.stackCount <= 0)
        {
            // SoundManager.Instance.PlaySound("Eat");

            itemList.Remove(item);
            AddQuickSlot(emptyItem, which - 1);
        }

        // Have its effect
        switch (item.type)
        {
            case ItemType.Heal:
                // Heal Player
                print("HEALED");
                healParticle.Play();
                Player_Health.Instance.Heal(10);
                break;

            case ItemType.Buff:
                // Buff player
                print("BUFFED");
                break;

            default: break;
        }
    }

      // Fungsi untuk mengganti senjata
    public void ToggleWeapon()
    {
        meleeOrRanged = !meleeOrRanged;
        UpdateEquippedWeaponUI();
        Debug.Log("Weapon Toggle");
    }
    // Fungsi untuk mengganti item yang dapat digunakan
    public void ToggleUseItem()
    {
        itemUse1 = !itemUse1;
        UpdateItemUseUI();
        Debug.Log("Toggle uhuyyyyyyy");
    }

    private void UpdateEquippedWeaponUI()
    {
        if (meleeOrRanged && equippedCombat[0] != null)
        {
            equippedWeapon = equippedCombat[0];
            if (PlayerUI.Instance != null && PlayerUI.Instance.equippedUI != null)
            {
                PlayerUI.Instance.equippedUI.sprite = equippedWeapon.sprite;
                // print("uhuyyy kepakeee bangsatttt");
            }
        }
        else if (equippedCombat[1] != null)
        {
            equippedWeapon = equippedCombat[1];
            if (PlayerUI.Instance != null && PlayerUI.Instance.equippedUI != null)
            {
                PlayerUI.Instance.equippedUI.sprite = equippedWeapon.sprite;
            }
        }
    }
    private void UpdateItemUseUI()
    {
        if (itemUse1 && quickSlots[0] != null)
        {
            equippedItem = quickSlots[0];
            if (PlayerUI.Instance != null && PlayerUI.Instance.itemUseUI != null)
            {
                PlayerUI.Instance.itemUseUI.sprite = equippedItem.sprite;
            }
        }
        else if (quickSlots[1] != null)
        {
            equippedItem = quickSlots[1];
            if (PlayerUI.Instance != null && PlayerUI.Instance.itemUseUI != null)
            {
                PlayerUI.Instance.itemUseUI.sprite = equippedItem.sprite;
            }
        }
    }

}
