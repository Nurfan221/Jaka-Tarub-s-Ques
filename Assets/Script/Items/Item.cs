using UnityEngine;
using System.Linq;


public enum ItemType
{
    Melee_Combat,
    Ranged_Combat,
    Heal,
    Buff,
    Item,
    Quest,
    ItemPrefab,
    Animal
}

[System.Flags]
public enum ItemCategory
{
    None = 0,
    Fruit = 1,
    Meat = 2,
    Fuel = 4,
    Vegetable = 8,
    Food = 16,
    Drink = 32,
    Medicine = 64,
    Ammo = 128,
    Weapon = 256,
    Crafting_Material = 512,
    PlantSeed = 1024,
    TreeSeed = 2048,
    ItemPrefab = 4096
}

[CreateAssetMenu(menuName = "Make an Item")]
public class Item : ScriptableObject
{
    [Header("STATS")]
    public int itemID;
    public string itemName;
    public ItemType type;
    public ItemCategory categories;
    public Sprite sprite;
    [TextArea]
    public string itemDescription;
    public int QuantityFuel;
    public float health; //deklarasikan health untuk menentukan berapa kali item di gunakan

    // Combat Item
    [Header("COMBAT")]
    public int Level;
    public int MaxLevel;
    public int Damage;
    public int AreaOfEffect;
    public int SpecialAttackCD;
    public int SpecialAttackStamina;
    public int UpgradeCost;
    public GameObject RangedWeapon_ProjectilePrefab;

    // Regular Item
    [Header("REGULAR")]
    public bool isStackable = false;
    public int stackCount;
    public int BuyValue;
    public int SellValue;
    public int BurningTime;
    public int CookTime;
    public GameObject prefabItem;
    

    // Seed Properties (khusus untuk benih)
    [Header("SEED PROPERTIES")]
    public float growthTime; // Lama pertumbuhan dalam detik
    public Sprite[] growthImages; // Gambar untuk tiap tahap pertumbuhan
    public GameObject plantPrefab; // Prefab tanaman yang akan tumbuh dari benih
    public GameObject dropItem; //prefab untuk buah yang akan di hasilkan

    public bool IsInCategory(ItemCategory category)
    {
        return (categories & category) == category;
    }


}
