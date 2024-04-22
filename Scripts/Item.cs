using UnityEngine;
using UnityEngine.UI;

public enum ItemType
{
    Weapon,
    Armor,
    Potion
}

public abstract class Item : MonoBehaviour
{
    public string itemName;
    public int itemID;
    public ItemType itemType; // Add this property
    public int gold = 0;
    public bool legendary = false;

    // Common properties and methods for all items can go here
    public void DestroyItem()
    {
        Destroy(gameObject);
    }
}